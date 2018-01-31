using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helpers;
using Hex;
using Multiplayer.Network;
using MyGameObjects.Characters;
using MyGameObjects.MyGameObject_templates;
using Newtonsoft.Json;
using UIManagers;
using UnityEngine;

namespace Managers
{
	public class GameStarter : SingletonMonoBehaviour<GameStarter>
	{
		public bool IsTesting = false;
		public Game Game = new Game();
		private GameType GameType;
		private Server ActiveServer;
		private Client ActiveClient;

		private async void Awake()
		{
			await PrepareAndStartGame();
		}

		private async Task PrepareAndStartGame()
		{
			GameOptions gameOptions;
			if (!IsTesting)
			{
				SetGameType();
				AssignClientOrServerIfNeeded();
				gameOptions = await GetGameOptions();
			}
			else
			{
				gameOptions = new GameOptions
				{
					GameType = GameType.Local,
					Map = Stuff.Maps[0],
					Players = new List<GamePlayer>
					{
						new GamePlayer
						{
							Name = "Ryszard",
							Characters = new List<Character>
							{
								new Sinon(),
								new Hecate()
							}
						},
						new GamePlayer
						{
							Name = "Maciej",
							Characters = new List<Character>
							{
								new Aqua(),
								new DekomoriSanae()
							}
						}
					},
					UIManager = UIManager.Instance
				};
				gameOptions.Players.ForEach(p =>
				{
					p.Characters.ForEach(c => c.Owner = p);
					p.HasSelectedCharacters = true;
				});
			}

			Game.Init(gameOptions);
			Game.StartGame();
			if (IsTesting)
			{
				Game.Players.ForEach(p=>p.Characters.ForEach(c =>
				{
					var spawnPoint = p.GetSpawnPoints().First(cell => cell.CharacterOnCell == null);
					Spawner.Instance.TrySpawning(spawnPoint, c);
				}));
				Game.Active.Phase.Finish();
			}

		}

		private async Task<GameOptions> GetGameOptions() => new GameOptions
		{
			GameType = GameType.Local,
			Map = GetMap(),
			Players = await GetPlayers(),
			UIManager = UIManager.Instance
		};

		private void AssignClientOrServerIfNeeded()
		{
			switch (GameType)
			{
				case GameType.Local:
					break;
				case GameType.MultiplayerServer:
					ActiveServer = FindObjectOfType<Server>();
					if (ActiveServer == null) throw new Exception("Server not found");

					break;
				case GameType.MultiplayerClient:
					ActiveClient = FindObjectOfType<Client>();
					if (ActiveClient == null) throw new Exception("Client not found");

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void SetGameType()
		{
			GameType = (GameType)PlayerPrefs.GetInt("GameType", (int)GameType.Local);
		}

		private HexMap GetMap()
		{
			switch (GameType)
			{
				case GameType.Local:
					var mapIndex = PlayerPrefs.GetInt("SelectedMapIndex", 0);
					var map = Stuff.Maps[mapIndex];
					return map;
				case GameType.MultiplayerServer:
					return Stuff.Maps[ActiveServer.SelectedMapIndex];
				case GameType.MultiplayerClient:
					return Stuff.Maps[ActiveClient.SelectedMapIndex];
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		private async Task<List<GamePlayer>> GetPlayers()
		{
			switch (GameType)
			{
				case GameType.Local:
					return await GetLocalPlayers();
				case GameType.MultiplayerServer:
					return await ActiveServer.GetCharactersFromClients();
				case GameType.MultiplayerClient:
					return await ActiveClient.GetPlayersFromServer();
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		private async Task<List<GamePlayer>> GetLocalPlayers()
		{
			var numberOfPlayers = PlayerPrefs.GetInt("NumberOfPlayers", 2);
			var players = new List<GamePlayer>();
			for (var i = 0; i < numberOfPlayers; i++)
				players.Add(new GamePlayer { Name = $"GamePlayer{i + 1}" });

			foreach (var p in players)
			{
				await GetCharacters(p);
			}

			return players;
		}

		private async Task GetCharacters(GamePlayer p)
		{
			Debug.Log(p.Name);
			var allCharacters = new List<MyGameObject>(AllMyGameObjects.Instance.Characters);
			SpriteSelect.Instance.Open(allCharacters, () => FinishSelectingCharacters(p), $"Wybór postaci - {p.Name}", "Zakończ wybieranie postaci");
			Func<bool> hasSelectedCharecters = () => p.HasSelectedCharacters;
			await hasSelectedCharecters.WaitToBeTrue();
		}
		private void FinishSelectingCharacters(GamePlayer p)
		{
			int charactersPerPlayer = GetCharactersPerPlayer();

			if (SpriteSelect.Instance.SelectedObjects.Count != charactersPerPlayer) return;

			var classNames = SpriteSelect.Instance.SelectedObjects.GetClassNames();
			p.AddCharacters(classNames);
			p.HasSelectedCharacters = true;
			SpriteSelect.Instance.Close();
		}

		private int GetCharactersPerPlayer()
		{
			int charactersPerPlayer;
			switch (GameType)
			{
				case GameType.Local:
					charactersPerPlayer = PlayerPrefs.GetInt("NumberOfCharactersPerPlayer");
					break;
				case GameType.MultiplayerServer:
					charactersPerPlayer = ActiveServer.PlayersPerCharacter;
					break;
				case GameType.MultiplayerClient:
					charactersPerPlayer = ActiveClient.PlayersPerCharacter;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return charactersPerPlayer;
		}

		public async Task<GamePlayer> GetGamePlayer()
		{
			var p = new GamePlayer {Name = ActiveClient.PlayerName };
			await GetCharacters(p);
			return p;
		}
		
	}
}