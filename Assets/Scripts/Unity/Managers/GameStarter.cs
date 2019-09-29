﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Mono.Data.Sqlite;
using NKMCore;
using Unity.Animations;
using Unity.Hex;
using Unity.UI;
using Unity.UI.CharacterUI;
using Unity.UI.HexCellUI;
using UnityEngine.SceneManagement;
using Action = NKMCore.Action;
using Effects = Unity.UI.CharacterUI.Effects;
using Logger = NKMCore.Logger;

namespace Unity.Managers
{
    public class GameStarter : SingletonMonoBehaviour<GameStarter>
    {
        public bool IsTesting;
        private static readonly SelectableManager SelectableManager = new SelectableManager();
        [CanBeNull] private static readonly ISelectable Selectable = new SpriteSelectSelectable(SelectableManager);
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
            _gamePreparerDependencies.SelectableManager = SelectableManager;
            _gamePreparerDependencies.Selectable = Selectable;
            _gamePreparerDependencies.SelectableAction = _selectableAction;
            _gamePreparerDependencies.Logger = new Logger(PathManager.GetLogFilePath());

            if (!IsClientConnected)
            {
                if(IsTesting)
                    PrepareAndStartTestingGame();
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
            Game = await preparer.CreateGame();
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
                    string[] actionData = content.Split(';');
                    string actionType = actionData[0];
                    string[] args = actionData.Length > 1 ? actionData[1].Split(':') : Array.Empty<string>();
                    if (new[] {Action.Types.OpenSelectable, Action.Types.CloseSelectable}.Contains(actionType))
                        _selectableAction.Make(actionType, args);
                    else
                        Game.Action.Make(actionType, args);
                } break;
                case "NKMRANDOM":
                {
                    string[] d = content.Split(';');
                    Game.Random.Set(d[0], int.Parse(d[1]));
                } break;
                case "STOP":
                {
                    Popup.Instance.Show("STOP", content, Quit);
                } break;
            }
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
        private static async void PrepareAndStartGame()
        {
            _gamePreparerDependencies.PlayerNames = GetPlayerNames();
            _gamePreparerDependencies.GameType = GameType.Local;
            var preparer = new GamePreparer(_gamePreparerDependencies);

            Game = await preparer.CreateGame();
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

        private static void InitUI(Game game)
        {
            UIManager.Instance.Init(game);
            UIManager.Instance.UpdateActivePhaseText();
            HexMapDrawer.Instance.Init(game);
            HexMapDrawer.Instance.CreateMap(game.HexMap);
            MainCameraController.Instance.Init(game);
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
            Popup.Instance.Show("Gra zakończona", victor == null ? "Nikt nie wygrał" : $"{victor.Name} wygrał!", () =>
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
                HexMap = HexMapFactory.FromScriptable(Stuff.Maps.Single(m => m.Map.name == "TestMap")),
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
