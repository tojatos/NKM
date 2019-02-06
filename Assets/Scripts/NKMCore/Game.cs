﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;
using Unity;
using Unity.Hex;
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

			Players = new List<GamePlayer>(gameOptions.Players);
			Abilities.ForEach(a => a.Awake());
			HexMap = HexMapFactory.FromScriptable(Options.MapScriptable);
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
			LogGameStart();
			if(Options.PlaceAllCharactersRandomlyAtStart) PlaceAllCharactersOnSpawns();
			return true;
		}

//	private void MakeGameLogActions()
//	{
////		string[][] actions = Options.GameLog.Actions;
////		foreach (string[] action in actions)
////		{
////			MakeAction(action);
////		}
//		Replay r = Replay.Instance;
//		r.Actions = new Queue<string[]>(Options.GameLog.Actions);
//		r.Show();
//	}

//	public void MakeAction(string[] action)
//	{
//			switch (action[0])
//			{
//				//TODO: Remove that all and work with clicks maybe, or not
//                case "CHARACTER PLACED":
//	                string[] data = action[1].SplitData();
//	                Character character = Characters.First(c => c.ToString() == data[0]);
//	                HexCell cell =  HexMap.Cells.First(c => c.ToString() == data[1]);
//	                Action.PlaceCharacter(character, cell);
//	                break;
//                case "TURN FINISHED": Active.Turn.Finish(); break;
//                case "ACTION TAKEN": Characters.First(c => c.ToString() == action[1]).TryToInvokeJustBeforeFirstAction(); break;
//                case "MOVE":
//	                List<HexCell> moveCells = action[1].SplitData().ConvertToHexCellList(HexMap);
//	                Active.Turn.CharacterThatTookActionInTurn.MakeActionBasicMove(moveCells);
//	                break;
//                case "BASIC ATTACK":
//	                Character targetCharacter = Characters.First(c => c.ToString() == action[1]);
//	                Active.Turn.CharacterThatTookActionInTurn.MakeActionBasicAttack(targetCharacter);
//	                break;
//                case "ABILITY CLICK":
//	                ((IClickable) Abilities.First(a => a is IClickable && a.ID == int.Parse(action[1]))).Click();
//	                break;
//                case "ABILITY USE":
//	                List<HexCell> targetCells = action[1].SplitData().ConvertToHexCellList(HexMap);
////	                ((IUseable) Abilities.First(a => a is IUseable && a.ID == abilityID)).Use(targetCells);
//	                Active.AbilityToUse.Use(targetCells);
//	                break;
//                case "ABILITY CANCEL":
//	                ((Ability)Active.AbilityToUse).Cancel();
//	                break;
//                case "RNG":
//	                string[] rngData = action[1].SplitData();
//	                NKMRandom.Set(rngData[0], int.Parse(rngData[1]));
//	                break;
//                default: 
//	                Console.Instance.DebugLog("Unknown action in GameLog!");
//	                break;
//			}
//	}

		private void LogGameStart()
		{//TODO: make ; and : character disallowed in player names
			string logText =
				$@"MAP: {Options.MapScriptable.Name}
PLAYERS: {string.Join("; ", Players.Select(p => p.Name))}
CHARACTERS:
{string.Join("\n", Players.Select(p => p.Name + ": " + string.Join("; ", p.Characters)))}
GAME STARTED: true";
			Console.GameLog(logText);
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

				if (Players.Count(p => !p.IsEliminated) <= 1) FinishGame();

				if (!IsEveryCharacterPlacedInTheFirstPhase) continue;

				if(EveryCharacterTookActionInPhase) Active.Phase.Finish();
			}
			// ReSharper disable once FunctionNeverReturns
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
}