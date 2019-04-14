using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Mono.Data.Sqlite;
using NKMCore;
using NKMCore.Hex;
using NKMCore.Templates;
using Unity.Hex;
using Unity.UI;
using Unity.UI.CharacterUI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Unity.Managers
{
	public class GameStarter : SingletonMonoBehaviour<GameStarter>
	{
		public bool IsTesting;
		public ISelectable Selectable = new SpriteSelectSelectable();
		private static IDbConnection _conn;
		private static Game _game;
		private static SessionSettings S => SessionSettings.Instance;
		private static int GetCharactersPerPlayerNumber() => S.GetDropdownSetting(SettingType.NumberOfCharactersPerPlayer) + 1;
		private static int GetPlayersNumber() => S.GetDropdownSetting(SettingType.NumberOfPlayers) + 2;
		private static int GetBansNumber() => S.GetDropdownSetting(SettingType.BansNumber) + 1;
		private static PickType GetPickType()
		{
			switch (S.GetDropdownSetting(SettingType.PickType))
			{
				case 0: return PickType.Blind;
				case 1: return PickType.Draft;
				case 2: return PickType.AllRandom;
				default: throw new ArgumentOutOfRangeException();
			}
		}
		private HexMap GetMap()
		{
			if (IsTesting)
				return HexMapFactory.FromScriptable(Stuff.Maps.Single(m => m.Map.name == "TestMap"));
			
            int mapIndex = S.GetDropdownSetting(SettingType.SelectedMapIndex);
            HexMapScriptable mapScriptable = Stuff.Maps[mapIndex];
			
            return HexMapFactory.FromScriptable(mapScriptable);
		}

		private static bool BansAreEnabled => S.GetDropdownSetting(SettingType.AreBansEnabled) == 1;

		private void Awake()
		{
			_conn = new SqliteConnection("Data source=" + Application.streamingAssetsPath + "/database.db");
			if (ClientManager.Instance.Client.IsConnected)
			{
				S.Options.Connection = _conn;
				_game = new Game(S.Options);
				return;
			}
			if(IsTesting)
                PrepareAndStartTestingGame();
			else
                PrepareAndStartGame();
		}

		private void Start()
		{
			if (ClientManager.Instance.Client != null)
			{
                InitializeMessageHandler();
			}
		}
		private void InitializeMessageHandler()
		{
			Client client = ClientManager.Instance.Client;
			client.OnMessage += HandleMessageFromServerInMainThread;
			UnityAction<Scene, LoadSceneMode> removeMessageHandler = null;
			removeMessageHandler = (scene, mode) =>
			{
				client.OnMessage -= HandleMessageFromServerInMainThread;
				SceneManager.sceneLoaded -= removeMessageHandler;
			};
			SceneManager.sceneLoaded += removeMessageHandler;
		}

		private void HandleMessageFromServerInMainThread(string message) 
			=> AsyncCaller.Instance.Call(() => HandleMessageFromServer(message));

		private void HandleMessageFromServer(string message)
		{
			string[] data = message.Split(' ');
			string header = data[0];
			string content = string.Empty;
			if (data.Length > 1) content = data[1];
			switch (header)
			{
				case "BLINDPICK":
					_game.Selectable.Select(new SelectableProperties<Character>
                    {
                        ToSelect = Game.GetMockCharacters(),
                        ConstraintOfSelection = list => list.Count == 4,
                        OnSelectFinish = list => ClientManager.Instance.Client.SendMessage("PICKED " + string.Join(";", list.Select(c => c.ID))),
                        SelectionTitle = $"Wybór postaci",
                    });
					
					break;
			}
		}

		private void PrepareAndStartTestingGame()
		{
            var game = new Game(GetTestingGameOptions());
			
            BindTestingCharactersToPlayers(game);
			
            InitUI(game);
			
			game.Start();
		}
		private async void PrepareAndStartGame()
		{
            var preparer = new GamePreparer(new GamePreparerOptions
            {
                NumberOfPlayers = GetPlayersNumber(),
                NumberOfCharactersPerPlayer = GetCharactersPerPlayerNumber(),
                BansEnabled = BansAreEnabled,
                NumberOfBans = GetBansNumber(),
                HexMap = GetMap(),
                PickType = GetPickType(),
                GameType = GameType.Local,
                Selectable = Selectable,
                Connection = _conn,
                LogFilePath = GetLogFilePath(),
            });

            var game = new Game(preparer.GameOptions);
            await preparer.BindCharactersToPlayers(game);
            RunGame(game);
		}

		private static void RunGame(Game game)
		{
            InitUI(game);

            game.Start();
		}

		private string GetLogFilePath()
		{
            return Application.persistentDataPath +
                   Path.DirectorySeparatorChar +
                   (IsTesting ? "Testing Game Logs" : "Game Logs") +
                   Path.DirectorySeparatorChar +
                   DateTime.Now.ToString("yyyy-MM-dd hh.mm.ss") +
                   ".txt";
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
			Spawner.Instance.Init(game);

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
		private static void BindTestingCharactersToPlayers(Game game)
		{
            string testingCharactersFile = File.ReadAllText(Application.dataPath + Path.DirectorySeparatorChar + "testing_characters.txt").TrimEnd();
            string[][] characterNamesGrouped = testingCharactersFile.Split(new[] {"\n\n"}, StringSplitOptions.None).Select(s => s.Split('\n')).ToArray();
            for (int i = 0; i < characterNamesGrouped.Length; ++i)
            {
                game.Players[i].Characters.AddRange(characterNamesGrouped[i]
                    .Select(x => x.Trim())
                    .Select(c => CharacterFactory.Create(game, c)));
            }
		}
		private GameOptions GetTestingGameOptions()
		{
            string testingCharactersFile = File.ReadAllText(Application.dataPath + Path.DirectorySeparatorChar + "testing_characters.txt").TrimEnd();
			string[][] charactersGrouped = testingCharactersFile.Split(new[] {"\n\n"}, StringSplitOptions.None).Select(s => s.Split('\n')).ToArray();
			string[] playerNames = {"Ryszard", "Maciej", "Zygfryd", "Bożydar"};
			List<GamePlayer> testingGamePlayers = charactersGrouped.Select((t, i) => new GamePlayer
				{
					Name = playerNames[i % playerNames.Length],
				})
				.ToList();
			var gameOptions = new GameOptions
			{
				HexMap = GetMap(),
				Players = testingGamePlayers,
				LogFilePath = GetLogFilePath(),
				Type = GameType.Local,
				Selectable = Selectable,
				Connection = _conn,
				PlaceAllCharactersRandomlyAtStart = true,
			};
			return gameOptions;
		}
	}
}
