using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Extensions;
using Hex;
using Managers;
using NKMObjects.Templates;
using UI;
using UnityEditor.Callbacks;
using UnityEngine;

public class Game
{
	public GameOptions Options { get; private set; }

	public List<GamePlayer> Players;
	public List<Character> Characters => Players.SelectMany(p => p.Characters).ToList();
	public List<Ability> Abilities => Characters.SelectMany(p => p.Abilities).ToList();
	public readonly Active Active;
	private UIManager _uiManager;
	private Spawner _spawner;
	public HexMapDrawer HexMapDrawer;
	public HexMap HexMap;
	public Console Console => Console.Instance;

	public bool IsInitialized;
	public bool IsReplay => Options.GameLog != null;
	public Game()
	{
		Active = new Active(this);
	}

	public void Init(GameOptions gameOptions)
	{
		Options = gameOptions;

		Players = new List<GamePlayer>(gameOptions.Players);
		_uiManager = Options.UIManager;
		HexMapDrawer = HexMapDrawer.Instance;
		HexMapDrawer.Init(this);
		_spawner = Spawner.Instance;
		Players.ForEach(p => p.Characters.ForEach(c => c.Abilities.ForEach(a => a.Awake())));
		NKMRandom.OnValueGet += (name, value) => Console.GameLog($"RNG: {name}; {value}");
		IsInitialized = true;
	}

	/// <summary>
	/// Returns true if game is successfully started,
	/// otherwise returns false.
	/// </summary>
	/// <returns></returns>
	public bool StartGame()
	{
		if (!IsInitialized) return false;

		HexMap = HexMapFactory.FromScriptable(Options.MapScriptable);
		HexMapDrawer.CreateMap(HexMap);
//		HexMapDrawer.CreateMap(Options.MapScriptable);
		_uiManager.Init();
//		UIManager.VisibleUI = UIManager.GameUI;
//		Active.Buttons = UIManager.UseButtons;
		MainCameraController.Instance.Init(Options.MapScriptable.Map.width, Options.MapScriptable.Map.height);
		UI.CharacterUI.Abilities.Instance.Init();
		_uiManager.UpdateActivePhaseText();
		TakeTurns();
		LogGameStart();
		if (GameStarter.Instance.IsTesting || SessionSettings.Instance.GetDropdownSetting(SettingType.PickType) == 2) PlaceAllCharactersOnSpawns(); //testing or all random
		if (IsReplay)
		{
			MakeGameLogActions();
		}
		return true;
	}

	private void MakeGameLogActions()
	{
//		string[][] actions = Options.GameLog.Actions;
//		foreach (string[] action in actions)
//		{
//			MakeAction(action);
//		}
		Replay r = Replay.Instance;
		r.Actions = new Queue<string[]>(Options.GameLog.Actions);
		r.Show();
	}

	public void MakeAction(string[] action)
	{
			switch (action[0])
			{
				//TODO: Remove that all and work with clicks maybe, or not
                case "CHARACTER PLACED":
	                string[] data = action[1].SplitData();
	                Character character = Characters.First(c => c.ToString() == data[0]);
	                HexCell cell =  HexMap.Cells.First(c => c.ToString() == data[1]);
	                PlaceCharacter(character, cell);
	                break;
                case "TURN FINISHED": Active.Turn.Finish(); break;
                case "ACTION TAKEN": Characters.First(c => c.ToString() == action[1]).TryToInvokeJustBeforeFirstAction(); break;
                case "MOVE":
	                List<HexCell> moveCells = action[1].SplitData().ConvertToHexCellList(HexMap);
	                Active.Turn.CharacterThatTookActionInTurn.MakeActionBasicMove(moveCells);
	                break;
                case "BASIC ATTACK":
	                Character targetCharacter = Characters.First(c => c.ToString() == action[1]);
	                Active.Turn.CharacterThatTookActionInTurn.MakeActionBasicAttack(targetCharacter);
	                break;
                case "ABILITY CLICK":
	                ((IClickable) Abilities.First(a => a is IClickable && a.ID == int.Parse(action[1]))).Click();
	                break;
                case "ABILITY USE":
	                List<HexCell> targetCells = action[1].SplitData().ConvertToHexCellList(HexMap);
//	                ((IUseable) Abilities.First(a => a is IUseable && a.ID == abilityID)).Use(targetCells);
	                Active.AbilityToUse.Use(targetCells);
	                break;
                case "ABILITY CANCEL":
	                ((Ability)Active.AbilityToUse).Cancel();
	                break;
                case "RNG":
	                string[] rngData = action[1].SplitData();
	                NKMRandom.Set(rngData[0], int.Parse(rngData[1]));
	                break;
                default: 
	                Console.Instance.DebugLog("Unknown action in GameLog!");
	                break;
			}
	}

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

			if (Players.Count(p => !p.IsEliminated) == 1) FinishGame();

			if (!IsEveryCharacterPlacedInTheFirstPhase) continue;

//			if (NoCharacterOnMapCanTakeAction) Active.Phase.Finish();
			if (UIManager.CanClickEndTurnButton && NoCharacterOnMapCanTakeAction || Active.Phase.Number == 0 && NoCharacterOnMapCanTakeAction) Active.Phase.Finish();//TODO
		}
		// ReSharper disable once FunctionNeverReturns
	}

	private static void FinishGame() => Victory.Instance.Show();

	private bool NoCharacterOnMapCanTakeAction => Players.All(p => p.Characters.Where(c => c.IsOnMap).All(c => !c.CanTakeAction));
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

	public void TouchCell(HexCell touchedCell)
	{
		Active.SelectedCell = touchedCell;
		if (Active.SelectedCharacterToPlace != null)
		{
			UseMyGameObject(touchedCell);

		}
		else if (Active.HexCells?.Contains(touchedCell) == true)
		{
			Active.MakeAction(touchedCell);
		}
		else
		{
			if (Active.AbilityToUse != null)
			{
				return;
			}
			//possibility of highlighting with control pressed
			if (!Input.GetKey(KeyCode.LeftControl))
			{
				HexMapDrawer.RemoveHighlights();
			}
			
			if (touchedCell.CharactersOnCell.Count > 0)
			{
				touchedCell.CharactersOnCell[0].Select();
			}
			else
			{
				Active.CharacterOnMap?.Deselect();
				Active.SelectDrawnCell(touchedCell).AddHighlight(Highlights.BlackTransparent);
			}
		}
//		touchedCell.GetArea(HexDirection.Ne, 6, 7).ForEach(c => c.AddHighlight(Highlights.BlueTransparent));
	}

	private void UseMyGameObject(HexCell cell)
	{
		if (Active.Turn.WasCharacterPlaced) return;
		var activeCharacter = Active.SelectedCharacterToPlace;
		PlaceCharacter(activeCharacter, cell);
	}

	private void PlaceCharacter(Character characterToPlace, HexCell targetCell)
	{
		if(!Spawner.CanSpawn(characterToPlace, targetCell)) return;
			
		_spawner.Spawn(Active.SelectDrawnCell(targetCell), characterToPlace);

		Active.Turn.WasCharacterPlaced = true;
		if (Active.Phase.Number == 0)
		{
			_uiManager.ForcePlacingChampions = false;
			Active.Turn.Finish();
		}
		else
		{
			Active.SelectedCharacterToPlace = null;
			HexMapDrawer.RemoveHighlights();
			characterToPlace?.Select();
		}
		
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

		Spawner.Instance.Spawn(Active.SelectDrawnCell(spawnPoint), c);
	}
}
