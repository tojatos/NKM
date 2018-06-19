﻿using System;
using System.Collections.Generic;
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
		public Game Game = new Game();

		private async void Awake() => await PrepareAndStartGame();

		private async Task PrepareAndStartGame()
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
								new Character("Sinon"),
								new Character("Roronoa Zoro"),
								new Character("Hecate"),
								new Character("Itsuka Kotori"),
								new Character("Rem"),
								new Character("Asuna"),
							}
						},
						new GamePlayer
						{
							Name = "Maciej",
							Characters = new List<Character>
							{
								new Character("Bezimienni"),
								new Character("Shana"),
								new Character("Dekomori Sanae"),
								new Character("Gilgamesh"),
								new Character("Crona"),
								new Character("Hanekawa Tsubasa"),
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
					return await GetLocalPlayers();
		}
		private async Task<List<GamePlayer>> GetLocalPlayers()
		{
			var numberOfPlayers = SessionSettings.Instance.NumberOfPlayers;
			List<GamePlayer> players = new List<GamePlayer>();
			for (var i = 0; i < numberOfPlayers; i++)
				players.Add(new GamePlayer { Name = $"GamePlayer{i + 1}" });

			foreach (GamePlayer p in players)
			{
				await GetCharacters(p);
			}

			return players;
		}

		private async Task GetCharacters(GamePlayer p)
		{
			Debug.Log(p.Name);
			List<NKMObject> allCharacters = new List<NKMObject>(AllMyGameObjects.Characters);
			SpriteSelect.Instance.Open(allCharacters, () => FinishSelectingCharacters(p), $"Wybór postaci - {p.Name}", "Zakończ wybieranie postaci");
			Func<bool> hasSelectedCharecters = () => p.HasSelectedCharacters;
			await hasSelectedCharecters.WaitToBeTrue();
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
