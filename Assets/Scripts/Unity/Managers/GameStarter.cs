using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mono.Data.Sqlite;
using NKMCore;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.HexCellEffects;
using NKMCore.Templates;
using Unity.Animations;
using Unity.Hex;
using Unity.UI;
using Unity.UI.CharacterUI;
using Unity.UI.HexCellUI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Action = NKMCore.Action;
using Effects = Unity.UI.CharacterUI.Effects;
using Logger = NKMCore.Logger;

namespace Unity.Managers
{
    public class GameStarter : SingletonMonoBehaviour<GameStarter>
    {
        public bool IsTesting;
        public bool IsReplay => SessionSettings.Instance.SelectedReplayFilePath != null;

        private static readonly SelectableManager SpriteSelectSelectableManager = new SelectableManager();
        private static readonly ISelectable Selectable = new SpriteSelectSelectable(SpriteSelectSelectableManager);
        private static SelectableAction _selectableAction;
        public static Game Game;

        private static GamePreparerDependencies _gamePreparerDependencies;
        private static bool IsClientConnected => ClientManager.Instance.Client.IsConnected;

        private void Awake()
        {
            NKMData.Connection = new SqliteConnection($"Data source={PathManager.DbPath}");
            _gamePreparerDependencies = SessionSettings.Instance.Dependencies;
            var sel = Selectable as SpriteSelectSelectable;

            if (IsClientConnected)
            {
                _selectableAction = new SelectableAction(GameType.Multiplayer, Selectable);
                _selectableAction.MultiplayerAction += message =>
                    ClientManager.Instance.Client.SendMessage(message);
                sel?.Init(_selectableAction);
            }
            else
            {
                _selectableAction = new SelectableAction(GameType.Local, Selectable);
                sel?.Init(_selectableAction);
            }
            _gamePreparerDependencies.SelectableManager = SpriteSelectSelectableManager;
            _gamePreparerDependencies.Selectable = Selectable;
            _gamePreparerDependencies.SelectableAction = _selectableAction;
            _gamePreparerDependencies.Logger = new Logger(PathManager.GetLogFilePath());

            if (!IsClientConnected)
            {
                if(IsTesting)
                    PrepareAndStartTestingGame();
                else if (IsReplay)
                    PrepareAndStartReplay();
                else
                    PrepareAndStartGame();
            }
        }

        private static async Task InitOnlineGame()
        {
            _gamePreparerDependencies.GameType = GameType.Multiplayer;
            var preparer = new GamePreparer(_gamePreparerDependencies);
            if (!preparer.AreOptionsValid)
                throw new Exception("Options are invalid!");
            Game = preparer.CreateGame();
            await preparer.BindCharactersToPlayers();
        }
        public async void HandleMessageFromServer(string message)
        {
            string[] data = message.Split(new []{' '}, 2);
            string header = data[0];
            string content = string.Empty;
            if (data.Length > 1) content = data[1];
            switch (header)
            {
                case ClientManager.Messages.AllRandom:
                {
                    ClientManager.Instance.Client.SendMessage("GET_CHARACTERS");
                    _gamePreparerDependencies.PickType = PickType.AllRandom;
                } break;
                case ClientManager.Messages.Draft:
                case ClientManager.Messages.Blind:
                {
                    if(Game != null) return;
                    _gamePreparerDependencies.PickType = header == ClientManager.Messages.Draft ? PickType.Draft : PickType.Blind;
                    await InitOnlineGame();

                    await Task.Delay(1000).ContinueWith(t => AsyncCaller.Instance.Call(() => RunGame(Game)));

                } break;
                case "SET_CHARACTERS":
                {
                    if(Game != null) return;
                    await InitOnlineGame();
                    AttachCharactersFromServer(Game, content);
                    await Task.Delay(1000).ContinueWith(t => AsyncCaller.Instance.Call(() => RunGame(Game)));
                } break;
                case "ACTION":
                {
                    Act(Game, content);
                } break;
                case "NKMRANDOM":
                {
                    string[] d = content.Split(';');
                    Game.Random.Set(d[0], int.Parse(d[1]));
                } break;
                case "STOP":
                {
                    Popup.Create(UIManager.Instance.transform).Show("STOP", content, Quit);
                } break;
            }
        }

        public static void Act(Game game, string content)
        {
            string[] actionData = content.Split(';');
            string actionType = actionData[0];
            string[] args = actionData.Length > 1 ? actionData[1].Split(':') : Array.Empty<string>();
            if (new[] {Action.Types.OpenSelectable, Action.Types.CloseSelectable}.Contains(actionType))
                _selectableAction.Make(actionType, args);
            else
                game.Action.Make(actionType, args);
        }

        public static void Quit()
        {
            if(IsClientConnected)
                ClientManager.Instance.Client.Disconnect();
            SceneManager.LoadScene(Scenes.MainMenu);
        }

        private static void AttachCharactersFromServer(Game game, string content)
        {
            List<(string playerName, List<string> characterNames)> playerNamesWithCharacters = content.Split(';').Select(x =>
            {
                string[] data = x.Split(':');
                string playerName = data[0];
                List<string> characterNames = data.Skip(1).ToList();
                return (playerName, characterNames);
            }).ToList();
            playerNamesWithCharacters.ForEach(c =>
            {
                game.Players.Find(p => p.Name == c.playerName).Characters.AddRange(c.characterNames.Select(x => CharacterFactory.Create(game, x)));
            });
            game.Action.MultiplayerAction += message =>
                ClientManager.Instance.Client.SendMessage(message);

        }

        private void PrepareAndStartTestingGame()
        {
            Game = new Game(GetTestingGameOptions());

            BindTestingCharactersToPlayers(Game);

            RunGame(Game);
        }

        private void PrepareAndStartReplay()
        {
            string[] logFileData = File.ReadAllLines(SessionSettings.Instance.SelectedReplayFilePath);
            SessionSettings.Instance.SelectedReplayFilePath = null; // Do not start new game as a replay
            ReplayPreparer preparer;
            try
            {
                preparer = new ReplayPreparer(logFileData);
            }
            catch (ReplayException e)
            {
                Popup.Create(UIManager.Instance.transform).Show("Error", e.Message, () => SceneManager.LoadScene(Scenes.ReplaySelect));
                throw;
            }
            ReplayResults replayResults = preparer.CreateGame(_gamePreparerDependencies);
            Game = replayResults.Game;

            Replay.Instance.Actions = replayResults.Actions;
            Replay.Instance.Show();

            RunGame(Game);
        }

        private static async void PrepareAndStartGame()
        {
            _gamePreparerDependencies.PlayerNames = GetPlayerNames();
            _gamePreparerDependencies.GameType = GameType.Local;
            var preparer = new GamePreparer(_gamePreparerDependencies);

            Game = preparer.CreateGame();
            await preparer.BindCharactersToPlayers();
            RunGame(Game);
        }

        private static List<string> GetPlayerNames()
        {
            var names = new List<string>();
            for (int i = 0; i < _gamePreparerDependencies.NumberOfPlayers; ++i)
            {
                names.Add(SessionSettings.Instance.PlayerNames.ElementAtOrDefault(i) != null
                    ? SessionSettings.Instance.PlayerNames[i]
                    : $"Player {i + 1}");
            }
            return names;
        }

        private static void RunGame(Game game)
        {
            InitUI(game);

            game.Start();
        }

        private static void AddTriggersToEvents(IGame game, HexMapDrawer d)
        {
            game.HexMap.AfterMove += (character, cell) =>
            {
                if (d.GetCharacterObject(character) == null) return;
                d.GetCharacterObject(character).transform.parent = d.SelectDrawnCell(cell).transform;
                AnimationPlayer.Add(new Destroy(d.SelectDrawnCell(cell).gameObject.GetComponent<LineRenderer>())); //Remove the line
                AnimationPlayer.Add(new MoveTo(d.GetCharacterObject(character).transform,
                    d.GetCharacterObject(character).transform.parent.transform.TransformPoint(0, 10, 0), 0.60f / character.Speed.Value));

            };
            game.HexMap.AfterCharacterPlace += (character, cell) => Spawner.Instance.Spawn(d.SelectDrawnCell(cell), character);
            game.Active.BeforeMoveCellsRemoved += cells => d.SelectDrawnCells(cells).ForEach(c => Destroy(c.gameObject.GetComponent<LineRenderer>()));
            game.Active.AfterMoveCellAdded += hexcell =>
            {
                //Draw a line between two hexcell centres

                //Check for component in case of Zoro's Lack of Orientation
                DrawnHexCell cell = d.SelectDrawnCell(hexcell);
                LineRenderer lRend = cell.gameObject.GetComponent<LineRenderer>() != null
                    ? cell.gameObject.GetComponent<LineRenderer>()
                    : cell.gameObject.AddComponent<LineRenderer>();
                lRend.SetPositions(new[]
                {
                    d.SelectDrawnCell(game.Active.MoveCells.SecondLast()).transform.position + Vector3.up * 20,
                    cell.transform.position + Vector3.up * 20
                });
                lRend.material = new Material(Shader.Find("Standard")) {color = Color.black};
                lRend.startColor = Color.black;
                lRend.endColor = Color.black;
                lRend.widthMultiplier = 2;
            };
            game.Active.AirSelection.AfterEnable += set => d.SelectDrawnCells(set).ForEach(c => c.AddHighlight(Highlights.BlueTransparent));
            game.Active.AirSelection.AfterCellsSet += set =>
            {
                d.RemoveHighlights();
                if (game.Active.HexCells != null && set != null)
                {
                    game.Active.HexCells.ToList().ForEach(c =>
                    {
                        if (set.All(ac => ac != c))
                        {
                            d.SelectDrawnCell(c).AddHighlight(Highlights.BlueTransparent);
                        }
                    });
                }

                set?.ToList().ForEach(c => d.SelectDrawnCell(c).AddHighlight(Highlights.RedTransparent));
            };
            game.Active.AfterAbilityPrepare += (ability, list) =>
            {
                d.RemoveHighlights();
                d.SelectDrawnCells(list).ForEach(c => c.AddHighlight(Highlights.RedTransparent));
            };
            game.Active.AfterCharacterSelectPrepare += (character, list) =>
            {
                d.SelectDrawnCells(list.Distinct()).ForEach(c =>
                    c.AddHighlight(!c.HexCell.IsEmpty && character.CanBasicAttack(c.HexCell.FirstCharacter)
                        ? Highlights.RedTransparent
                        : Highlights.GreenTransparent));
            };
            game.Active.AfterCharacterPlacePrepare += set => d.SelectDrawnCells(set).ForEach(c => c.AddHighlight(Highlights.RedTransparent));
            game.Active.AfterCancelPlacingCharacter += () => d.RemoveHighlights();
            game.Active.AfterClean += () => d.RemoveHighlights();

            d.AfterCellSelect += touchedCell =>
            {
                // AfterCellSelect?.Invoke(touchedCell);
                if (game.Active.SelectedCharacterToPlace != null)
                {
                    if(!game.Active.CanPlace(game.Active.SelectedCharacterToPlace, touchedCell)) return;
                    game.Action.PlaceCharacter(game.Active.SelectedCharacterToPlace, touchedCell);
                    if (game.Active.Phase.Number != 0) return;
                    game.Action.FinishTurn();
                }
                else if (game.Active.HexCells?.Contains(touchedCell) == true)
                {
                    if (game.Active.AbilityToUse != null)
                    {
                        //It is important to check in that order, in case ability uses multiple interfaces!
                        if(game.Active.AbilityToUse is IUseableCharacter && !touchedCell.IsEmpty)
                            game.Action.UseAbility((IUseableCharacter) game.Active.AbilityToUse, touchedCell.FirstCharacter);
                        else if(game.Active.AbilityToUse is IUseableCell)
                            game.Action.UseAbility((IUseableCell) game.Active.AbilityToUse, touchedCell);
                        else if(game.Active.AbilityToUse is IUseableCellList)
                            game.Action.UseAbility((IUseableCellList) game.Active.AbilityToUse, game.Active.AirSelection.IsEnabled ? game.Active.AirSelection.HexCells : game.Active.HexCells);
                    }
                    else if (game.Active.Character != null)
                    {
                        if(!touchedCell.IsEmpty && game.Active.Character.CanBasicAttack(touchedCell.FirstCharacter))
                            game.Action.BasicAttack(game.Active.Character, touchedCell.FirstCharacter);
                        else if(touchedCell.IsFreeToStand && game.Active.Character.CanBasicMove(touchedCell) && game.Active.MoveCells.Last() == touchedCell)
                            game.Action.BasicMove(game.Active.Character, game.Active.MoveCells);
                    }
                }
                else
                {
                    if (!touchedCell.IsEmpty)
                        game.Action.Select(touchedCell.FirstCharacter);
                    else if (game.Active.Character != null)
                        game.Action.Deselect();
                    else
                    {
                        //possibility of highlighting with control pressed
                        if (!Input.GetKey(KeyCode.LeftControl)) d.RemoveHighlights();
                        if (touchedCell.IsEmpty) d.SelectDrawnCell(touchedCell).AddHighlight(Highlights.BlackTransparent);
                    }
                }
            };
            game.HexMap.AfterCellEffectCreate += effect =>
            {
                if (effect is Conflagration)
                    d.SelectDrawnCell(effect.ParentCell).AddEffectHighlight(effect.Name);
                Unity.UI.HexCellUI.Effects.Instance.Refresh();
            };
            game.HexMap.AfterCellEffectRemove += effect =>
            {
                if (effect is Conflagration)
                    d.SelectDrawnCell(effect.ParentCell).RemoveEffectHighlight(effect.Name);
                Unity.UI.HexCellUI.Effects.Instance.Refresh();
            };
        }

        public void Update()
        {
            if(Game == null) return;

            if (Game.Active.AirSelection.IsEnabled)
            {
                HexCell cellPointed = HexMapDrawer.CellPointed();
                if (cellPointed != null && Game.Active.HexCells.Contains(cellPointed))
                {
                    Game.Active.AirSelection.HexCells = new HashSet<HexCell> { cellPointed };
                }
            }

            if(Game.Active.Character!=null && Game.Active.Character.CanUseBasicMove && Game.Active.HexCells != null)
            {
                HexCell cellPointed = HexMapDrawer.CellPointed();
                if (cellPointed != null && (Game.Active.HexCells.Contains(cellPointed)||cellPointed==Game.Active.Character.ParentCell))
                {
                    HexCell lastMoveCell = Game.Active.MoveCells.LastOrDefault();
                    if(lastMoveCell==null) throw new Exception("Move cell is null!");
                    if (cellPointed != lastMoveCell)
                    {
                        if (Game.Active.MoveCells.Contains(cellPointed))
                        {
                            //remove all cells to pointed
                            for (int i = Game.Active.MoveCells.Count - 1; i >= 0; i--)
                            {
                                if (Game.Active.MoveCells[i] == cellPointed) break;

                                //Remove the line
                                Destroy(HexMapDrawer.Instance.SelectDrawnCell(Game.Active.MoveCells[i]).gameObject
                                    .GetComponent<LineRenderer>());

                                Game.Active.MoveCells.RemoveAt(i);
                            }
                        }
                        else if (Game.Active.Character.Speed.Value >= Game.Active.MoveCells.Count &&
                                 lastMoveCell.GetNeighbors(Game.Active.GamePlayer, 1).Contains(cellPointed) && (cellPointed.CharactersOnCell.Count == 0||!cellPointed.CharactersOnCell.Any(c => c.IsEnemyFor(Game.Active.Character.Owner))))
                        {
                            Game.Active.AddMoveCell(cellPointed);

                        }
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                Game.Action.Cancel();
            }
        }

        private static void InitUI(Game game)
        {
            UIManager.Instance.Init(game);
            UIManager.Instance.UpdateActivePhaseText();
            AddTriggersToEvents(game, HexMapDrawer.Instance);
            // HexMapDrawer.Instance.Init(game);
            HexMapDrawer.Instance.CreateMap(game.HexMap);
            MainCameraController.Instance.Init();
            ConsoleDrawer.Instance.Init(game.Console);
            UI.CharacterUI.Abilities.Instance.Init(game);
            Effects.Instance.Init(game);
            Face.Instance.Init(game);
            UI.HexCellUI.Effects.Instance.Init(game);
            HexImage.Instance.Init();
            Spawner.Instance.Init(game);
            game.OnFinish += () => ShowFinishGamePopup(game);
            game.Active.Turn.TurnStarted += player =>
            {
                AnimationPlayer.Add(new ShowVanishablePopup($"{player.Name}", 2));
            };
            game.Active.Turn.TurnFinished += character =>
            {
                if(character == null) return;
                AnimationPlayer.Add(new Dim(character));
            };
            game.AfterAbilityInit += ability =>
            {
                AnimationPlayer.AddTriggers(ability);
                MusicManager.AddTriggers(ability);
                HexMapDrawer.Instance.AddTriggers(ability);
            };
            game.AfterCharacterInit += character =>
            {
                AnimationPlayer.AddTriggers(character);
                UIManager.Instance.AddUITriggers(character);
            };
        }

        private static void ShowFinishGamePopup(Game game)
        {
            GamePlayer victor = game.Players.SingleOrDefault(p => !p.IsEliminated);
            Popup.Create(UIManager.Instance.transform).Show("Gra zakończona", victor == null ? "Nikt nie wygrał" : $"{victor.Name} wygrał!", () =>
            {
                SceneManager.LoadScene(Scenes.MainMenu);
                if(IsClientConnected)
                    ClientManager.Instance.Client.Disconnect();
            });

        }
        private static void BindTestingCharactersToPlayers(Game game)
        {
            string testingCharactersFile = File.ReadAllText(PathManager.TestingCharactersFilePath).TrimEnd();
            string[][] characterNamesGrouped = testingCharactersFile.Split(new[] {"\n\n"}, StringSplitOptions.None).Select(s => s.Split('\n')).ToArray();
            for (int i = 0; i < characterNamesGrouped.Length; ++i)
            {
                game.Players[i].Characters.AddRange(characterNamesGrouped[i]
                    .Select(x => x.Trim())
                    .Select(c => CharacterFactory.Create(game, c)));
            }
        }
        private GameDependencies GetTestingGameOptions()
        {
            string testingCharactersFile = File.ReadAllText(PathManager.TestingCharactersFilePath).TrimEnd();
            string[][] charactersGrouped = testingCharactersFile.Split(new[] {"\n\n"}, StringSplitOptions.None).Select(s => s.Split('\n')).ToArray();
            string[] playerNames = {"Ryszard", "Maciej", "Zygfryd", "Bożydar"};
            List<GamePlayer> testingGamePlayers = charactersGrouped.Select((t, i) => new GamePlayer
                {
                    Name = playerNames[i % playerNames.Length],
                })
                .ToList();
            var gameOptions = new GameDependencies
            {
                HexMap = Stuff.Maps.Single(m => m.Name == "TestMap").Clone(),
                Players = testingGamePlayers,
                Type = GameType.Local,
                Selectable = _gamePreparerDependencies.Selectable,
                SelectableManager = _gamePreparerDependencies.SelectableManager,
                SelectableAction = _gamePreparerDependencies.SelectableAction,
                Logger = _gamePreparerDependencies.Logger,
                PlaceAllCharactersRandomlyAtStart = true,
            };
            return gameOptions;
        }
    }
}
