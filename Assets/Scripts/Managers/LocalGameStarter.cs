using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hex;
using MyGameObjects.MyGameObject_templates;
using NUnit.Framework;
using UIManagers;
using UnityEngine;

namespace Managers
{
	public class LocalGameStarter : SingletonMonoBehaviour<LocalGameStarter>
	{
		public Game Game = new Game();
		private async void Awake()
		{
			Task<List<Player>> players = GetPlayers();

			GameOptions gameOptions = new GameOptions
			{
				GameType = GameType.Local,
				Map = GetMap(),
				Players = await players,
				UIManager = UIManager.Instance
			};
			Debug.Log("Game options received");
			Game.Init(gameOptions);
			Game.StartGame();
		}

		private HexMap GetMap()
		{
			var mapIndex = PlayerPrefs.GetInt("SelectedMap", 0);
			var map = Stuff.Maps[mapIndex];
			return map;
		}
		private async Task<List<Player>> GetPlayers()
		{
			var NumberOfPlayers = PlayerPrefs.GetInt("NumberOfPlayers", 2);
			var Players = new List<Player>();
			for (var i = 0; i < NumberOfPlayers; i++)
				Players.Add(new Player {Name = $"Player{i + 1}"});

			foreach (var p in Players)
			{
				Debug.Log(p.Name);
				var allCharacters = new List<MyGameObject>(AllMyGameObjects.Instance.Characters);
				SpriteSelect.Instance.Open(allCharacters, () => FinishSelectingCharacters(p), "Wybór postaci", "Zakończ wybieranie postaci");

				Func<bool> hasSelectedCharecters = () => p.HasSelectedCharacters;
				await hasSelectedCharecters.WaitToBeTrue();
				Debug.Log(hasSelectedCharecters);
			}


			return Players;
		}
		private void FinishSelectingCharacters(Player p)
		{
			var charactersPerPlayer = PlayerPrefs.GetInt("NumberOfCharactersPerPlayer");
			if (SpriteSelect.Instance.SelectedObjects.Count != charactersPerPlayer) return;

			var classNames = SpriteSelect.Instance.SelectedObjects.GetClassNames();
			var characters = Spawner.Create("Characters", classNames).Cast<Character>().ToList();
			characters.ForEach(c=>c.Owner = p);
			p.Characters.AddRange(characters);
			p.HasSelectedCharacters = true;
			SpriteSelect.Instance.Close();
		}

	}
}