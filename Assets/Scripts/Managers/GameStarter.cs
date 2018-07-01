using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Extensions;
using Hex;
using NKMObjects.Templates;
using UI;
using NKMObject = NKMObjects.Templates.NKMObject;

namespace Managers
{
	public class GameStarter : SingletonMonoBehaviour<GameStarter>
	{
		public bool IsTesting;
		public Game Game = new Game();

		private void Awake() => PrepareAndStartGame();

		private async void PrepareAndStartGame()
		{
			GameOptions gameOptions = await GetGameOptions();

			Game.Init(gameOptions);
			var isGameStarted = Game.StartGame();
			if(!isGameStarted) throw new Exception("Game has not started!");
		}

		private static GameOptions GetTestingGameOptions()
		{
			var gameOptions = new GameOptions
			{
				Map = Stuff.Maps.Single(m => m.Map.name == "TestMap"),
				Players = new List<GamePlayer>
					{
						new GamePlayer
						{
							Name = "Ryszard",
							Characters = new List<Character>
							{
//								new Character("Sinon"),
//								new Character("Roronoa Zoro"),
//								new Character("Hecate"),
//								new Character("Itsuka Kotori"),
//								new Character("Llenn"),
								new Character("Yoshino"),
//								new Character("Dekomori Sanae"),
//								new Character("Rem"),
//								new Character("Kirito"),
//								new Character("Nibutani Shinka"),
//								new Character("Kurogane Ikki"),
//								new Character("Sakai Yuuji"),
							}
						},
						new GamePlayer
						{
							Name = "Maciej",
							Characters = new List<Character>
							{
								new Character("Monkey D. Luffy"),
//								new Character("Hanekawa Tsubasa"),
//								new Character("Bezimienni"),
//								new Character("Shana"),
//								new Character("Yasaka Mahiro"),
//								new Character("Gilgamesh"),
//								new Character("Crona"),
//								new Character("Asuna"),
//								new Character("Aqua"),
							}
						},
					},
				UIManager = UIManager.Instance
			};
			gameOptions.Players.ForEach(p =>
			{
				p.Characters.ForEach(c => c.Owner = p);
				p.HasSelectedCharacters = true;
			});
			return gameOptions;
		}

		private async Task<GameOptions> GetGameOptions()
		{
			if (IsTesting) return GetTestingGameOptions();

			return new GameOptions
			{
				Map = GetMap(),
				Players = await GetPlayers(),
				UIManager = UIManager.Instance,
			};
		}



		private HexMap GetMap()
		{
					var mapIndex = SessionSettings.Instance.SelectedMapIndex;
					HexMap map = Stuff.Maps[mapIndex];
					return map;
		}
		
		private async Task<List<GamePlayer>> GetPlayers()
		{
			var numberOfPlayers = SessionSettings.Instance.NumberOfPlayers;
			List<GamePlayer> players = new List<GamePlayer>();
			for (var i = 0; i < numberOfPlayers; i++)
			{
				players.Add(new GamePlayer { Name = GetPlayerName(i) });
				
			}
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
			switch (SessionSettings.Instance.PickType)
			{
				case 0:
					await BlindPick(players);
					break;
				case 1:
					await DraftPick(players);
					break;
			}
		}

		private async Task DraftPick(List<GamePlayer> players)
		{
            List<Character> charactersToPick = new List<Character>(AllMyGameObjects.Characters);
			while (players.Any(p => p.Characters.Count != SessionSettings.Instance.NumberOfCharactersPerPlayer))
			{
				for (int i = 0; i < players.Count; i++)
				{
                    GamePlayer player = players[i];
					await SelectOneCharacter(charactersToPick, player);
				}
				
				for (int i = players.Count-1; i >= 0 ; i--)
				{
                    GamePlayer player = players[i];
					await SelectOneCharacter(charactersToPick, player);
				}
			}
			players.ForEach(p=>p.HasSelectedCharacters=true);
		}

		private static async Task SelectOneCharacter(List<Character> charactersToPick, GamePlayer player)
		{
			if(player.Characters.Count == SessionSettings.Instance.NumberOfCharactersPerPlayer) return;
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

		private async Task BlindPick(List<GamePlayer> players)
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

		private void FinishSelectingCharacters(GamePlayer p)
		{
			var charactersPerPlayer = GetCharactersPerPlayer();

			if (SpriteSelect.Instance.SelectedObjects.Count != charactersPerPlayer) return;

			IEnumerable<string> names = SpriteSelect.Instance.SelectedObjects.Select(o=>o.Name);
			p.AddCharacters(names);
			p.HasSelectedCharacters = true;
			SpriteSelect.Instance.Close();
		}

		private int GetCharactersPerPlayer()
		{
					return SessionSettings.Instance.NumberOfCharactersPerPlayer;
		}



	}
}
