using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NKMCore;
using NKMCore.Templates;
using Unity.Hex;
using Unity.UI;
using UnityEngine;
using Async = NKMCore.Extensions.Async;
using Sqlite = NKMCore.Extensions.Sqlite;
using SystemGeneric = NKMCore.Extensions.SystemGeneric;

//using NKMObject = NKMObjects.Templates.NKMObject;

namespace Unity.Managers
{
	public class GameStarter : SingletonMonoBehaviour<GameStarter>
	{
		public bool IsTesting;
		public bool PlayReplay;
		public Game Game = new Game();
		public ISelectable Selectable = new SpriteSelectSelectable();
		private static SessionSettings S => SessionSettings.Instance;
		private static int GetCharactersPerPlayerNumber() => S.GetDropdownSetting(SettingType.NumberOfCharactersPerPlayer) + 1;
		private static int GetPlayersNumber() => S.GetDropdownSetting(SettingType.NumberOfPlayers) + 2;
		private static int GetBansNumber() => S.GetDropdownSetting(SettingType.BansNumber) + 1;

		private void Awake() => PrepareAndStartGame();

		private async void PrepareAndStartGame()
		{
			GameOptions gameOptions = await GetGameOptions();

			if (Instance.IsTesting || SessionSettings.Instance.GetDropdownSetting(SettingType.PickType) == 2)
				gameOptions.PlaceAllCharactersRandomlyAtStart = true;
			gameOptions.Selectable = Selectable;

			Game.Init(gameOptions);
			
			UIManager.Instance.Init();
            UIManager.Instance.UpdateActivePhaseText();
			HexMapDrawer.Instance.Init(Game);
            HexMapDrawer.Instance.CreateMap(Game.HexMap);
            MainCameraController.Instance.Init(gameOptions.MapScriptable.Map.width, gameOptions.MapScriptable.Map.height);
            UI.CharacterUI.Abilities.Instance.Init();
		
			bool isGameStarted = Game.StartGame();
			if(!isGameStarted) throw new Exception("Game has not started!");
		}

		private GameOptions GetTestingGameOptions()
		{
            string testingCharactersFile = File.ReadAllText(Application.dataPath + Path.DirectorySeparatorChar + "testing_characters.txt").TrimEnd();
			string[][] charactersGrouped = testingCharactersFile.Split(new[] {"\n\n"}, StringSplitOptions.None).Select(s => s.Split('\n')).ToArray();
			string[] playerNames = {"Ryszard", "Maciej", "Zygfryd", "Bożydar"};
			List<GamePlayer> testingGamePlayers = charactersGrouped.Select((t, i) => new GamePlayer(Game)
				{
					Name = playerNames[i % (playerNames.Length)],
					Characters = t.Select(x => x.Trim()).Select(c => CharacterFactory.Create(Game, c)).ToList()
				})
				.ToList();
			var gameOptions = new GameOptions
			{
				MapScriptable = Stuff.Maps.Single(m => m.Map.name == "TestMap"),
				Players = testingGamePlayers,
				LogFilePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Testing Game Logs" + Path.DirectorySeparatorChar + DateTime.Now.ToString("u") + ".txt",
				Type = GameType.Local
			};
			gameOptions.Players.ForEach(p =>
			{
//				p.Characters.ForEach(c => c.Owner = p);
				p.HasSelectedCharacters = true;
			});
			return gameOptions;
		}

		private GameOptions GetReplayGameOptions()
		{
			string replayFilePath = Directory.GetFiles(Application.persistentDataPath + Path.DirectorySeparatorChar + "Game Logs" + Path.DirectorySeparatorChar)[0];
			// ReSharper disable once AssignNullToNotNullAttribute
			Directory.CreateDirectory(Path.GetDirectoryName(replayFilePath));	
			string[] replayFileLines = File.ReadAllLines(replayFilePath);
			var gameLog = new GameLog(replayFileLines);
			List<GamePlayer> gamePlayers = gameLog.GetPlayerNames().Select(p => new GamePlayer(Game)
			{
				Name = p,
				Characters = gameLog.GetCharacterNames(p).Select(c => CharacterFactory.Create(Game, c)).ToList()

			}).ToList();
			var gameOptions =  new GameOptions
			{
				MapScriptable = Stuff.Maps.Single(m => m.Name == gameLog.GetMapName()),
				Players = gamePlayers,
				Type = GameType.Replay,
				GameLog = gameLog
			};
			gameOptions.Players.ForEach(p =>
			{
//				p.Characters.ForEach(c => c.Owner = p);
				p.HasSelectedCharacters = true;
			});
			return gameOptions;
		}

		private async Task<GameOptions> GetGameOptions()
		{
			if (IsTesting) return GetTestingGameOptions();
			if (PlayReplay) return GetReplayGameOptions();

			return new GameOptions
			{
				MapScriptable = GetMap(),
				Players = await GetPlayers(),
				LogFilePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Game Logs" + Path.DirectorySeparatorChar + DateTime.Now.ToString("u") + ".txt",
				Type = GameType.Local
			};
		}

		private static HexMapScriptable GetMap()
		{
//					var mapIndex = SessionSettings.Instance.SelectedMapIndex;
            int mapIndex = S.GetDropdownSetting(SettingType.SelectedMapIndex);
            HexMapScriptable mapScriptable = Stuff.Maps[mapIndex];
            return mapScriptable;
		}
		
		private async Task<List<GamePlayer>> GetPlayers()
		{
//			var numberOfPlayers = SessionSettings.Instance.NumberOfPlayers;
			int numberOfPlayers = GetPlayersNumber();
			List<GamePlayer> players = new List<GamePlayer>();
			for (int i = 0; i < numberOfPlayers; i++)
				players.Add(new GamePlayer(Game) {Name = GetPlayerName(i)});
			await GetCharacters(players);
			return players;
		}

		private static string GetPlayerName(int i)
		{
			switch (i)
			{
				case 0: return SessionSettings.Instance.PlayerName1;
				case 1: return SessionSettings.Instance.PlayerName2;
				case 2: return SessionSettings.Instance.PlayerName3;
				case 3: return SessionSettings.Instance.PlayerName4;
				default: return $"Player {i + 1}";
			}
		}

		private async Task GetCharacters(List<GamePlayer> players)
		{
			switch (S.GetDropdownSetting(SettingType.PickType))
			{
				case 0:
					await BlindPick(players);
					break;
				case 1:
					List<Character> charactersToPick =
						new List<Character>(Sqlite.GetCharacterNames(GameData.Conn).Select(c => CharacterFactory.Create(Game, c)));//AllMyGameObjects.Characters.Select(c => new Character(c.Name)));
					if(S.GetDropdownSetting(SettingType.AreBansEnabled)==1) await Bans(players, charactersToPick);
					await DraftPick(players, charactersToPick);
					break;
				case 2:
					AllRandom(players);
					break;
			}
		}

		private static async Task DraftPick(List<GamePlayer> players, ICollection<Character> charactersToPick)
		{
			int numberOfCharactersPerPlayer = GetCharactersPerPlayerNumber();
			while (players.Any(p => p.Characters.Count != numberOfCharactersPerPlayer))
			{
				foreach (GamePlayer player in players) 
					await SelectOneCharacter(charactersToPick, player);
				foreach (GamePlayer player in players.AsEnumerable().Reverse()) 
					await SelectOneCharacter(charactersToPick, player);
			}
			players.ForEach(p=>p.HasSelectedCharacters=true);
		}
		private static async Task Bans(List<GamePlayer> players, ICollection<Character> charactersToPick)
		{
			int bansNumber = GetBansNumber();
			while(bansNumber != 0)
			{
				foreach (GamePlayer player in players) 
					await BanOneCharacter(charactersToPick, player);
				bansNumber--;
				if(bansNumber==0) break;
				foreach (GamePlayer player in players.AsEnumerable().Reverse()) 
					await BanOneCharacter(charactersToPick, player);
				bansNumber--;
			}
			players.ForEach(p=>p.HasSelectedCharacters=true);
		}

		private void AllRandom(List<GamePlayer> players)
		{
			int numberOfCharactersPerPlayer = GetCharactersPerPlayerNumber();

			List<string> allCharacterNames = Sqlite.GetCharacterNames(GameData.Conn);//AllMyGameObjects.Characters.Select(c => c.Name).ToList();
			
			players.ForEach(p=>
			{
				while (p.Characters.Count != numberOfCharactersPerPlayer)
				{
                    string randomCharacterName = SystemGeneric.GetRandomNoLog(allCharacterNames);
                    allCharacterNames.Remove(randomCharacterName);
                    p.AddCharacter(Game, randomCharacterName);
				}
				
				p.HasSelectedCharacters = true;
			});
			
		}

		private static async Task SelectOneCharacter(ICollection<Character> charactersToPick, GamePlayer player)
		{
			int numberOfCharactersPerPlayer = GetCharactersPerPlayerNumber();
			if(player.Characters.Count == numberOfCharactersPerPlayer) return;
			bool hasSelected = false;
			Func<bool> wait = () => hasSelected;
			Action<GamePlayer> finishSelectingCharacter = p =>
			{
				if (SpriteSelect.Instance.SelectedObjects.Count != 1) return;

				IEnumerable<string> names = SpriteSelect.Instance.SelectedObjects.Select(o => o.Name);
				Character picked = charactersToPick.Single(c => c.Name == names.First());
				charactersToPick.Remove(picked);
				p.AddCharacter(picked);
				hasSelected = true;
				SpriteSelect.Instance.Close();
			};

			SpriteSelect.Instance.Open(charactersToPick, () => finishSelectingCharacter(player),
				$"Wybór postaci - {player.Name}", "Zakończ wybieranie postaci");
			await Async.WaitToBeTrue(wait);
		}
        private static async Task BanOneCharacter(ICollection<Character> charactersToPick, GamePlayer player)
		{
			bool hasSelected = false;
			Func<bool> wait = () => hasSelected;
			Action<GamePlayer> finishSelectingCharacter = p =>
			{
				if (SpriteSelect.Instance.SelectedObjects.Count != 1) return;

				IEnumerable<string> names = SpriteSelect.Instance.SelectedObjects.Select(o => o.Name);
				Character picked = charactersToPick.Single(c => c.Name == names.First());
				charactersToPick.Remove(picked);
				hasSelected = true;
				SpriteSelect.Instance.Close();
			};

			SpriteSelect.Instance.Open(charactersToPick, () => finishSelectingCharacter(player),
				$"Banowanie postaci - {player.Name}", "Zakończ banowanie postaci");
			await Async.WaitToBeTrue(wait);
		}
		private async Task BlindPick(IEnumerable<GamePlayer> players)
		{
//			players.ForEach(p => Debug.Log(p.Name));
			foreach (GamePlayer p in players)
			{
				List<Character> allCharacters = Game.GetMockCharacters();//= new List<Character>(GameData.Conn.GetCharacterNames().Select(c => CharacterFactory.CreateWithoutId(Game, c)));
                SpriteSelect.Instance.Open(allCharacters, () => FinishSelectingCharacters(p), $"Wybór postaci - {p.Name}", "Zakończ wybieranie postaci");
                Func<bool> hasSelectedCharecters = () => p.HasSelectedCharacters;
                await Async.WaitToBeTrue(hasSelectedCharecters);
//				yield return new WaitUntil(()=>p.HasSelectedCharacters);
			}
		}

		private static void FinishSelectingCharacters(GamePlayer p)
		{
//			int numberOfCharactersPerPlayer = 
//			var charactersPerPlayer = GetCharactersPerPlayer();

			if (SpriteSelect.Instance.SelectedObjects.Count != GetCharactersPerPlayerNumber()) return;

			IEnumerable<string> names = SpriteSelect.Instance.SelectedObjects.Select(o=>o.Name);
			p.AddCharacters(names);
			p.HasSelectedCharacters = true;
			SpriteSelect.Instance.Close();
		}




	}

	class SpriteSelectSelectable : ISelectable
	{
		public void Select<T>(SelectableProperties<T> props)
		{
//			bool isSelected = false;
			if(typeof(T) == typeof(Character)) SpriteSelect.Instance.Open(props.ToSelect as List<Character>, () =>
			{
                List<Character> selectedObj = SpriteSelect.Instance.SelectedObjects;
				if (!props.ConstraintOfSelection(selectedObj as List<T>)) return;
				props.OnSelectFinish(selectedObj as List<T>);
//				isSelected = true;

                SpriteSelect.Instance.Close();
			}, props.SelectionTitle, "Zakończ wybieranie" );
		}
	}
}
