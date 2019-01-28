using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Animations;
using Extensions;
using Hex;
using Managers;
using NKMObjects;
using NKMObjects.Templates;
using UI;
using UI.CharacterUI;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

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
	public readonly Action Action;

	public bool IsInitialized;
	public bool IsReplay => Options.GameLog != null;
	public Game()
	{
		Active = new Active(this);
		Action = new Action(this);
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
		HexMap = HexMapFactory.FromScriptable(Options.MapScriptable);
		HexMap.AfterMove += (character, cell) =>
		{
			if (HexMapDrawer.GetCharacterObject(character) == null) return;
			HexMapDrawer.GetCharacterObject(character).transform.parent = Active.SelectDrawnCell(cell).transform;
            AnimationPlayer.Add(new Destroy(Active.SelectDrawnCell(cell).gameObject.GetComponent<LineRenderer>())); //Remove the line
			AnimationPlayer.Add(new MoveTo(HexMapDrawer.GetCharacterObject(character).transform,
				HexMapDrawer.GetCharacterObject(character).transform.parent.transform.TransformPoint(0, 10, 0), 0.13f));
			
		};
		HexMap.AfterCharacterPlace += (character, cell) => _spawner.Spawn(Active.SelectDrawnCell(cell), character);
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

		HexMapDrawer.CreateMap(HexMap);
		_uiManager.Init();
		MainCameraController.Instance.Init(Options.MapScriptable.Map.width, Options.MapScriptable.Map.height);
		UI.CharacterUI.Abilities.Instance.Init();
		_uiManager.UpdateActivePhaseText();
		TakeTurns();
		LogGameStart();
		if (GameStarter.Instance.IsTesting || SessionSettings.Instance.GetDropdownSetting(SettingType.PickType) == 2) PlaceAllCharactersOnSpawns(); //testing or all random
//		if (IsReplay)
//		{
//			MakeGameLogActions();
//		}
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
			Action.PlaceCharacter(Active.SelectedCharacterToPlace, touchedCell);
		}
		else if (Active.HexCells?.Contains(touchedCell) == true)
		{
			if (Active.AbilityToUse != null)
			{
				Action.UseAbility(Active.AbilityToUse, Active.AirSelection.IsEnabled ? Active.AirSelection.HexCells : Active.HexCells);
			}
			else if (Active.Character != null)
			{
				if(!touchedCell.IsEmpty && Active.Character.CanBasicAttack(touchedCell.FirstCharacter))
                    Action.BasicAttack(Active.Character, touchedCell.FirstCharacter);
				else if(touchedCell.IsFreeToStand && Active.Character.CanBasicMove(touchedCell) && Active.MoveCells.Last() == touchedCell)
					Action.BasicMove(Active.Character, Active.MoveCells);
			}
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
			if(!touchedCell.IsEmpty) Action.Select(touchedCell.FirstCharacter);
			else
			{
				Action.Deselect();
				Active.SelectDrawnCell(touchedCell).AddHighlight(Highlights.BlackTransparent);
			}
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

		//Spawner.Instance.Spawn(Active.SelectDrawnCell(spawnPoint), c);
		Action.PlaceCharacter(c, spawnPoint);
	}

	public void AddTriggersToEvents(Character character)
	{
        character.JustBeforeFirstAction += () => Active.Turn.CharacterThatTookActionInTurn = character;
        character.JustBeforeFirstAction += () => Console.GameLog($"ACTION TAKEN: {character}");
		character.OnDeath += () =>
		{
			HexMap.RemoveFromMap(character);
			AnimationPlayer.Add(new Destroy(HexMapDrawer.GetCharacterObject(character)));
			character.DeathTimer = 0;
			if (Active.Character == character) Active.Deselect();
		};
        character.HealthPoints.StatChanged += () =>
		{
			if (character.IsAlive) return;
			character.InvokeOnDeath();
		};
        character.AfterAttack += (targetCharacter, damage) =>
            Console.Log(
                $"{character.FormattedFirstName()} atakuje {targetCharacter.FormattedFirstName()}, zadając <color=red><b>{damage.Value}</b></color> obrażeń!");
        character.AfterAttack += (targetCharacter, damage) =>
            AnimationPlayer.Add(new Tilt(HexMapDrawer.GetCharacterObject(targetCharacter).transform));
        character.AfterAttack += (targetCharacter, damage) =>
            AnimationPlayer.Add(new ShowInfo(HexMapDrawer.GetCharacterObject(targetCharacter).transform, damage.Value.ToString(),
                Color.red));
        character.AfterHeal += (targetCharacter, valueHealed) =>
            AnimationPlayer.Add(new ShowInfo(HexMapDrawer.GetCharacterObject(targetCharacter).transform, valueHealed.ToString(),
                Color.blue));
        character.HealthPoints.StatChanged += () =>
        {
            if (Active.Character == character) MainHPBar.Instance.UpdateHPAmount(character);
        };
        character.OnDeath += () => character.Effects.Clear();
        character.OnDeath += () => Console.Log($"{character.FormattedFirstName()} umiera!");
        character.AfterHeal += (targetCharacter, value) =>
            Console.Log(targetCharacter != character
                ? $"{character.FormattedFirstName()} ulecza {targetCharacter.FormattedFirstName()} o <color=blue><b>{value}</b></color> punktów życia!"
                : $"{character.FormattedFirstName()} ulecza się o <color=blue><b>{value}</b></color> punktów życia!");
		Action.AfterAction += str =>
		{
			if (str == Action.Types.BasicAttack || str == Action.Types.BasicMove) Active.Select(Active.Character);
		};
		Active.AfterSelect += chara =>
		{
			Stats.Instance.UpdateCharacterStats(chara);
			MainHPBar.Instance.UpdateHPAmount(chara);
			UI.CharacterUI.Abilities.Instance.UpdateButtons();
			Effects.Instance.UpdateButtons();
		};
		Active.AfterDeselect += () =>
		{
			HexMapDrawer.RemoveHighlights();
			Stats.Instance.UpdateCharacterStats(null);
		};
		character.AfterBasicMove += moveCells => 
			Console.GameLog($"MOVE: {string.Join("; ", moveCells.Select(p => p.Coordinates))}"); //logging after action to make reading rng work
		
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
		Active.BeforeMoveCellsRemoved += cells => Active.SelectDrawnCells(cells).ForEach(c => Object.Destroy(c.gameObject.GetComponent<LineRenderer>()));
		Active.AfterMoveCellAdded += hexcell =>
		{
			//Draw a line between two hexcell centres

			//Check for component in case of Zoro's Lack of Orientation
			DrawnHexCell cell = Active.SelectDrawnCell(hexcell);
			LineRenderer lRend = cell.gameObject.GetComponent<LineRenderer>() != null
				? cell.gameObject.GetComponent<LineRenderer>()
				: cell.gameObject.AddComponent<LineRenderer>();
			lRend.SetPositions(new[]
			{
				Active.SelectDrawnCell(Active.MoveCells.Last()).transform.position + Vector3.up * 20,
				cell.transform.position + Vector3.up * 20
			});
			lRend.material = new Material(Shader.Find("Standard")) {color = Color.black};
			lRend.startColor = Color.black;
			lRend.endColor = Color.black;
			lRend.widthMultiplier = 2;
		};
	}
	
	public static bool IsPointerOverUiObject()
	{
		var eventDataCurrentPosition =
			new PointerEventData(EventSystem.current) {position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)};
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}

	public void ShowHelpHexCells(List<HexCell> cells) => Active.SelectDrawnCells(cells).ForEach(c => c.AddHighlight(Highlights.BlueTransparent));
	public void HideHelpHexCells() => HexMapDrawer.RemoveHighlightsOfColor(Highlights.BlueTransparent);
}
