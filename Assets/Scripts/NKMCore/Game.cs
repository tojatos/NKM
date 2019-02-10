using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;
using Unity;
using Unity.UI;

namespace NKMCore
{
	public class Game
	{
		public GameOptions Options { get; private set; }

		public List<GamePlayer> Players;
		public List<Character> Characters => Players.SelectMany(p => p.Characters).ToList();
		private List<Ability> Abilities => Characters.SelectMany(c => c.Abilities).ToList();
		public readonly Active Active;
		public HexMap HexMap;
		public readonly Action Action;
		public readonly Console Console;
		public ISelectable Selectable { get; private set; }

		public bool IsInitialized;
		public bool IsReplay => Options.GameLog != null;
		public Game()
		{
			Active = new Active(this);
			Action = new Action(this);
			Console = new Console(this);
		}

		public void Init(GameOptions gameOptions)
		{
			Options = gameOptions;
			Selectable = gameOptions.Selectable;

			Players = new List<GamePlayer>(gameOptions.Players);
			Abilities.ForEach(a => a.Awake());
			HexMap = gameOptions.HexMap;//HexMapFactory.FromScriptable(Options.MapScriptable);
			NKMRandom.OnValueGet += (name, value) => Console.GameLog($"RNG: {name}; {value}");
			Action.AfterAction += str =>
			{
				if (str == Action.Types.BasicAttack || str == Action.Types.BasicMove) Active.Select(Active.Character);
			};
			IsInitialized = true;
		
		
		}
		/// <summary>
		/// Get a copy of every character in the game
		/// </summary>
		public static List<Character> GetMockCharacters()
		{
			var mockGame = new Game();
			return GameData.Conn.GetCharacterNames().Select(n => CharacterFactory.Create(mockGame, n)).ToList();
		}

		/// <summary>
		/// Returns true if game is successfully started,
		/// otherwise returns false.
		/// </summary>
		public bool StartGame()
		{
			if (!IsInitialized) return false;

			TakeTurns();
			if(Options.PlaceAllCharactersRandomlyAtStart) PlaceAllCharactersOnSpawns();
			return true;
		}

		/// <summary>
		/// Infinite loop that manages Turns and Phases
		/// </summary>
		private async void TakeTurns()
		{
			while (true)
			{
				foreach (GamePlayer player in Players)
				{
					if(player.IsEliminated) continue;
					await TakeTurn(player);
				}

				if (Players.Count(p => !p.IsEliminated) <= 1)
				{
					FinishGame();
					return;
				}

				if (!IsEveryCharacterPlacedInTheFirstPhase) continue;

				if(EveryCharacterTookActionInPhase) Active.Phase.Finish();
			}
		}

		private static void FinishGame()
		{
			//TODO
		}

		private bool EveryCharacterTookActionInPhase => Players.All(p => p.Characters.Where(c => c.IsAlive).All(c => c.TookActionInPhaseBefore));
		private bool IsEveryCharacterPlacedInTheFirstPhase => !(Active.Phase.Number == 0 && Players.Any(p => p.Characters.Any(c => !c.IsOnMap)));

		/// <summary>
		/// Start a turn and wait for player to end it
		/// </summary>
		private async Task TakeTurn(GamePlayer player)
		{
			Active.Turn.Start(player);
			Func<bool> isTurnDone = () => Active.Turn.IsDone;
			await isTurnDone.WaitToBeTrue();
		}

		/// <summary>
		/// Try to place all characters from game on their spawns
		/// 
		/// Dependencies:
		/// - Players.Characters
		/// - Active.Phase
		/// </summary>
		private void PlaceAllCharactersOnSpawns()
		{
			Players.ForEach(p => p.Characters.ForEach(c => TrySpawningOnRandomCell(p, c)));
			if(Active.Phase.Number==0) Active.Phase.Finish();
		}

		private void TrySpawningOnRandomCell(GamePlayer p, Character c)
		{
			HexCell spawnPoint = p.GetSpawnPoints(HexMap).FindAll(cell => Spawner.CanSpawn(c, cell)).GetRandomNoLog();
			if (spawnPoint == null) return;

			//Spawner.Instance.Spawn(Active.SelectDrawnCell(spawnPoint), c);
			Action.PlaceCharacter(c, spawnPoint);
		}

		public void AddTriggersToEvents(Character character)
		{
			AnimationPlayer.Instance.AddAnimationTriggers(character);
			UIManager.Instance.AddUITriggers(character);
			Console.AddTriggersToEvents(character);
		
			character.JustBeforeFirstAction += () => Active.Turn.CharacterThatTookActionInTurn = character;
			character.OnDeath += () =>
			{
				HexMap.RemoveFromMap(character);
				character.DeathTimer = 0;
				if (Active.Character == character) Active.Deselect();
			};
			character.HealthPoints.StatChanged += () =>
			{
				if (character.IsAlive) return;
				character.InvokeOnDeath();
			};
			character.OnDeath += () => character.Effects.Clear();
		
			Active.Turn.TurnFinished += other =>
			{
				if (other != character) return;
				character.HasFreeAttackUntilEndOfTheTurn = false;
				character.HasFreeMoveUntilEndOfTheTurn = false;
				character.HasFreeUltimatumAbilityUseUntilEndOfTheTurn = false;
			};

			Active.Phase.PhaseFinished += () =>
			{
				if (character.IsOnMap)
				{
					character.HasUsedBasicAttackInPhaseBefore = false;
					character.HasUsedBasicMoveInPhaseBefore = false;
					character.HasUsedNormalAbilityInPhaseBefore = false;
					character.HasUsedUltimatumAbilityInPhaseBefore = false;
					character.TookActionInPhaseBefore = false;
				}

				if (!character.IsAlive)
				{
					character.DeathTimer++;
				}
			};
		}
	}

	public interface ISelectable
	{
		void Select<T>(SelectableProperties<T> props);
	}
}
