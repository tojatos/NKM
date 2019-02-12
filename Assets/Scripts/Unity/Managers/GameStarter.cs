using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NKMCore;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;
using Unity.Hex;
using Unity.UI;
using Unity.UI.CharacterUI;
using UnityEngine;

namespace Unity.Managers
{
	public class GameStarter : SingletonMonoBehaviour<GameStarter>
	{
		public bool IsTesting;
		public bool PlayReplay;
		public ISelectable Selectable = new SpriteSelectSelectable();
		private static SessionSettings S => SessionSettings.Instance;
		private static int GetCharactersPerPlayerNumber() => S.GetDropdownSetting(SettingType.NumberOfCharactersPerPlayer) + 1;
		private static int GetPlayersNumber() => S.GetDropdownSetting(SettingType.NumberOfPlayers) + 2;
		private static int GetBansNumber() => S.GetDropdownSetting(SettingType.BansNumber) + 1;
		private static bool BansAreEnabled => S.GetDropdownSetting(SettingType.AreBansEnabled) == 1;

		private void Awake() => PrepareAndStartGame();

		private async void PrepareAndStartGame()
		{
			GameOptions gameOptions = GetGameOptions();

			var game = new Game(gameOptions);

			await BindCharactersToPlayers(game);
			
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
		
			game.Start();
		}

		private async Task BindCharactersToPlayers(Game game)
		{
			if (IsTesting) BindTestingCharactersToPlayers(game);
			else
			{
                switch (S.GetDropdownSetting(SettingType.PickType))
                {
                    case 0:
//                        BlindPick(game);
                        break;
                    case 1:
	                    List<Character> charactersToPick = Game.GetMockCharacters();
                        if(BansAreEnabled) await Bans(game, charactersToPick);
                        await DraftPick(game, charactersToPick);
                        break;
                    case 2:
//                        AllRandom(game);
                        break;
                }
				
			}
		
		}

		private async Task DraftPick(Game game, List<Character> charactersToPick)
		{
			int numberOfCharactersPerPlayer = GetCharactersPerPlayerNumber();
			Func<bool> allCharactersPicked = () => game.Players.All(p => p.Characters.Count == numberOfCharactersPerPlayer);
			while (!allCharactersPicked())
			{
				foreach (GamePlayer player in game.Players) 
					await PickOneCharacter(charactersToPick, player, game);
				if(allCharactersPicked()) break;
				foreach (GamePlayer player in game.Players.AsEnumerable().Reverse()) 
					await PickOneCharacter(charactersToPick, player, game);
			}
		}
		private async Task Bans(Game game, List<Character> charactersToPick)
		{
			int bansLeft = GetBansNumber();
			while(bansLeft != 0)
			{
				foreach (GamePlayer player in game.Players) 
					await BanOneCharacter(charactersToPick, player);
				bansLeft--;
				if(bansLeft==0) break;
				foreach (GamePlayer player in game.Players.AsEnumerable().Reverse()) 
					await BanOneCharacter(charactersToPick, player);
				bansLeft--;
			}
		}

		private async Task PickOneCharacter(List<Character> charactersToPick, GamePlayer player, Game game) =>
            await SelectOneCharacter(new SelectableProperties<Character>
            {
	            ToSelect = charactersToPick,
	            ConstraintOfSelection = list => list.Count == 1,
	            OnSelectFinish = list =>
	            {
		            player.Characters.Add(CharacterFactory.Create(game, list[0].Name));
		            charactersToPick.Remove(list[0]);
	            },
	            SelectionTitle = $"Wybór postaci - {player.Name}",
            });
		
		private async Task BanOneCharacter(List<Character> charactersToPick, GamePlayer player) =>
            await SelectOneCharacter(new SelectableProperties<Character>
            {
	            ToSelect = charactersToPick,
	            ConstraintOfSelection = list => list.Count == 1,
	            OnSelectFinish = list =>
	            {
		            charactersToPick.Remove(list[0]);
	            },
	            SelectionTitle = $"Banowanie postaci - {player.Name}",
            });
		private async Task SelectOneCharacter(SelectableProperties<Character> props)
		{
			bool isSelected= false;
			props.OnSelectFinish += list => isSelected = true;
			Selectable.Select(props);
			Func<bool> picked = () => isSelected;
			await picked.WaitToBeTrue();
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
				HexMap = HexMapFactory.FromScriptable(Stuff.Maps.Single(m => m.Map.name == "TestMap")),
				Players = testingGamePlayers,
				LogFilePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Testing Game Logs" + Path.DirectorySeparatorChar + DateTime.Now.ToString("u") + ".txt",
				Type = GameType.Local,
				Selectable = Selectable,
				PlaceAllCharactersRandomlyAtStart = true,
			};
			return gameOptions;
		}
//
//		private GameOptions GetReplayGameOptions()
//		{
//			string replayFilePath = Directory.GetFiles(Application.persistentDataPath + Path.DirectorySeparatorChar + "Game Logs" + Path.DirectorySeparatorChar)[0];
//			// ReSharper disable once AssignNullToNotNullAttribute
//			Directory.CreateDirectory(Path.GetDirectoryName(replayFilePath));	
//			string[] replayFileLines = File.ReadAllLines(replayFilePath);
//			var gameLog = new GameLog(replayFileLines);
//			List<GamePlayer> gamePlayers = gameLog.GetPlayerNames().Select(p => new GamePlayer
//			{
//				Name = p,
//				Characters = gameLog.GetCharacterNames(p).Select(c => CharacterFactory.Create(Game, c)).ToList()
//
//			}).ToList();
//			var gameOptions =  new GameOptions
//			{
//				HexMap = HexMapFactory.FromScriptable(Stuff.Maps.Single(m => m.Name == gameLog.GetMapName())),
//				Players = gamePlayers,
//				Type = GameType.Replay,
//				GameLog = gameLog
//			};
//			gameOptions.Players.ForEach(p =>
//			{
////				p.Characters.ForEach(c => c.Owner = p);
//				p.HasSelectedCharacters = true;
//			});
//			return gameOptions;
//		}

		private GameOptions GetGameOptions()
		{
			if (IsTesting) return GetTestingGameOptions();
//			if (PlayReplay) return GetReplayGameOptions();

			return new GameOptions
			{
				HexMap = GetMap(),
				Players = GetPlayers(),
				LogFilePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Game Logs" + Path.DirectorySeparatorChar + DateTime.Now.ToString("u") + ".txt",
				Type = GameType.Local,
				Selectable = Selectable,
				PlaceAllCharactersRandomlyAtStart = SessionSettings.Instance.GetDropdownSetting(SettingType.PickType) == 2,
			};
		}

		private static List<GamePlayer> GetPlayers() =>
			Enumerable.Range(0, GetPlayersNumber()).Select(n => new GamePlayer {Name = GetPlayerName(n)}).ToList();

		private static HexMap GetMap()
		{
            int mapIndex = S.GetDropdownSetting(SettingType.SelectedMapIndex);
            HexMapScriptable mapScriptable = Stuff.Maps[mapIndex];
            return HexMapFactory.FromScriptable(mapScriptable);
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


//		private void AllRandom(List<GamePlayer> players)
//		{
//			int numberOfCharactersPerPlayer = GetCharactersPerPlayerNumber();
//
//			List<string> allCharacterNames = Sqlite.GetCharacterNames(GameData.Conn);//AllMyGameObjects.Characters.Select(c => c.Name).ToList();
//			
//			players.ForEach(p=>
//			{
//				while (p.Characters.Count != numberOfCharactersPerPlayer)
//				{
//                    string randomCharacterName = SystemGeneric.GetRandomNoLog(allCharacterNames);
//                    allCharacterNames.Remove(randomCharacterName);
//                    p.AddCharacter(Game, randomCharacterName);
//				}
//				
//				p.HasSelectedCharacters = true;
//			});
//			
//		}

//		private async Task BlindPick(IEnumerable<GamePlayer> players)
//		{
//			foreach (GamePlayer p in players)
//			{
//				List<Character> allCharacters = Game.GetMockCharacters();//= new List<Character>(GameData.Conn.GetCharacterNames().Select(c => CharacterFactory.CreateWithoutId(Game, c)));
//                SpriteSelect.Instance.Open(allCharacters, () => FinishSelectingCharacters(p), $"Wybór postaci - {p.Name}", "Zakończ wybieranie postaci");
//                Func<bool> hasSelectedCharecters = () => p.HasSelectedCharacters;
//                await Async.WaitToBeTrue(hasSelectedCharecters);
//			}
//		}
//
//		private static void FinishSelectingCharacters(GamePlayer p)
//		{
//			List<Character> selectedCharacters = SpriteSelect.Instance.SelectedObjects;
//			if (selectedCharacters.Count != GetCharactersPerPlayerNumber()) return;
//
//			p.AddCharacters(selectedCharacters.Select(c => c.Name).ToArray());
//			p.HasSelectedCharacters = true;
//			SpriteSelect.Instance.Close();
//		}
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
