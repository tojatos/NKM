﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Extensions;
using Hex;
using NKMObjects.Templates;
using UI;
using UnityEngine;
using NKMObject = NKMObjects.Templates.NKMObject;

namespace Managers
{
	public class GameStarter : SingletonMonoBehaviour<GameStarter>
	{
		public bool IsTesting;
		public string ReplayFilePath;
		public Game Game = new Game();
		private static SessionSettings S => SessionSettings.Instance;
		private static int GetCharactersPerPlayerNumber() => S.GetDropdownSetting(SettingType.NumberOfCharactersPerPlayer) + 1;
		private static int GetPlayersNumber() => S.GetDropdownSetting(SettingType.NumberOfPlayers) + 2;
		private static int GetBansNumber() => S.GetDropdownSetting(SettingType.BansNumber) + 1;

		private void Awake() => PrepareAndStartGame();

		private async void PrepareAndStartGame()
		{
			GameOptions gameOptions = await GetGameOptions();

			Game.Init(gameOptions);
			bool isGameStarted = Game.StartGame();
			if(!isGameStarted) throw new Exception("Game has not started!");
		}

		private static GameOptions GetTestingGameOptions()
		{
            string testingCharactersFile = File.ReadAllText(Application.dataPath + Path.DirectorySeparatorChar + "testing_characters.txt").TrimEnd();
			string[][] charactersGrouped = testingCharactersFile.Split(new[] {"\n\n"}, StringSplitOptions.None).Select(s => s.Split('\n')).ToArray();
			string[] playerNames = {"Ryszard", "Maciej", "Zygfryd", "Bożydar"};
			List<GamePlayer> testingGamePlayers = charactersGrouped.Select((t, i) => new GamePlayer
				{
					Name = playerNames[i % (playerNames.Length)],
					Characters = t.Select(x => new Character(x.Trim())).ToList()
				})
				.ToList();
			var gameOptions = new GameOptions
			{
				Map = Stuff.Maps.Single(m => m.Map.name == "TestMap"),
				Players = testingGamePlayers,
				UIManager = UIManager.Instance,
				LogFilePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Testing Game Logs" + Path.DirectorySeparatorChar + DateTime.Now.ToString("u") + ".txt",
				Type = GameType.Local
			};
			gameOptions.Players.ForEach(p =>
			{
				p.Characters.ForEach(c => c.Owner = p);
				p.HasSelectedCharacters = true;
			});
			return gameOptions;
		}

		private GameOptions GetReplayGameOptions()
		{
			string[][] logData = File.ReadAllLines(ReplayFilePath).Select(f => f.Split(new []{": "}, 2, StringSplitOptions.RemoveEmptyEntries)).ToArray();
			string mapName = logData.First(x => x[0] == "MAP")[1];
			string[] playerNames = logData.First(x => x[0] == "PLAYERS")[1].Split(new []{"; "}, StringSplitOptions.RemoveEmptyEntries).ToArray();
			List<GamePlayer> gamePlayers = playerNames.Select(p => new GamePlayer
			{
				Name = p,
				Characters = logData.First(x => x[0] == p)[1]
					.Split(new[] {"; "}, StringSplitOptions.RemoveEmptyEntries)
					.Select(characterName => new Character(characterName.Split(' ')[0])).ToList()

			}).ToList();
			return new GameOptions
			{
				Map = Stuff.Maps.Single(m => m.Map.name == mapName),
				Players = gamePlayers,
				UIManager = UIManager.Instance,
				Type = GameType.Replay,
				Actions = logData.SkipWhile(x => x[0] != "GAME STARTED").Skip(1).ToArray()
			};
		}

		private async Task<GameOptions> GetGameOptions()
		{
			if (IsTesting) return GetTestingGameOptions();
//			if (ReplayFilePath != null) return GetReplayGameOptions();

			return new GameOptions
			{
				Map = GetMap(),
				Players = await GetPlayers(),
				UIManager = UIManager.Instance,
				LogFilePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Game Logs" + Path.DirectorySeparatorChar + DateTime.Now.ToString("u") + ".txt",
				Type = GameType.Local
			};
		}

		private static HexMap GetMap()
		{
//					var mapIndex = SessionSettings.Instance.SelectedMapIndex;
            int mapIndex = S.GetDropdownSetting(SettingType.SelectedMapIndex);
            HexMap map = Stuff.Maps[mapIndex];
            return map;
		}
		
		private static async Task<List<GamePlayer>> GetPlayers()
		{
//			var numberOfPlayers = SessionSettings.Instance.NumberOfPlayers;
			int numberOfPlayers = GetPlayersNumber();
			List<GamePlayer> players = new List<GamePlayer>();
			for (int i = 0; i < numberOfPlayers; i++)
				players.Add(new GamePlayer {Name = GetPlayerName(i)});
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

		private static async Task GetCharacters(List<GamePlayer> players)
		{
			switch (S.GetDropdownSetting(SettingType.PickType))
			{
				case 0:
					await BlindPick(players);
					break;
				case 1:
                    List<Character> charactersToPick = new List<Character>(AllMyGameObjects.Characters.Select(c => new Character(c.Name)));
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

		private static void AllRandom(List<GamePlayer> players)
		{
			int numberOfCharactersPerPlayer = GetCharactersPerPlayerNumber();

			List<string> allCharacterNames = AllMyGameObjects.Characters.Select(c => c.Name).ToList();
			
			players.ForEach(p=>
			{
				while (p.Characters.Count != numberOfCharactersPerPlayer)
				{
                    string randomCharacterName = allCharacterNames.GetRandom();
                    allCharacterNames.Remove(randomCharacterName);
                    p.AddCharacter(randomCharacterName);
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
			await wait.WaitToBeTrue();
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
			await wait.WaitToBeTrue();
		}
		private static async Task BlindPick(IEnumerable<GamePlayer> players)
		{
//			players.ForEach(p => Debug.Log(p.Name));
			foreach (GamePlayer p in players)
			{
                List<NKMObject> allCharacters = new List<NKMObject>(AllMyGameObjects.Characters);
                SpriteSelect.Instance.Open(allCharacters, () => FinishSelectingCharacters(p), $"Wybór postaci - {p.Name}", "Zakończ wybieranie postaci");
                Func<bool> hasSelectedCharecters = () => p.HasSelectedCharacters;
                await hasSelectedCharecters.WaitToBeTrue();
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
}
