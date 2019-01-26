using System;
using System.Collections.Generic;
using System.Linq;
using Hex;
using NKMObjects.Templates;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

/// <summary>
/// Main utility class.
/// Contains information about active game state.
/// </summary>
public class Active
{
	private readonly Game _game;
	public Active(Game game)
	{
		_game = game;
		Phase = new Phase();
		Turn = new Turn(game);
		AirSelection = new AirSelection(game);
	}

	public readonly Turn Turn;
	public readonly Phase Phase;
	public AirSelection AirSelection { get; }

	public GamePlayer GamePlayer;
	public IUseable AbilityToUse;
	public Character SelectedCharacterToPlace;
	public Character Character;
	public HexCell SelectedCell;
	public readonly List<HexCell> MoveCells = new List<HexCell>();
	public event Delegates.CharacterD AfterSelect;
	public event Delegates.Void AfterDeselect;
	
	public List<HexCell> HexCells { get; set; }
	//TODO: refactor this
	public List<HexCell> HelpHexCells
	{
		private get { return _helpHexCells; }
		set
		{
			_helpHexCells = value;
			if (value == null) _game.HexMapDrawer.RemoveHighlightsOfColor(Highlights.BlueTransparent);
			else SelectDrawnCells(HelpHexCells).ForEach(c => c.AddHighlight(Highlights.BlueTransparent));
		}
	}

	private List<HexCell> _helpHexCells;

	public bool IsActiveUse => !(AbilityToUse == null && SelectedCharacterToPlace == null);// (ActionType == ActionType.None || ActionType == ActionType.AttackAndMove) && SelectedCharacterToPlace == null);

	public void Reset()
	{
		if (IsActiveUse || Turn.IsDone) Deselect();
		AbilityToUse = null;
		HexCells = null;
		SelectedCharacterToPlace = null;
		SelectedCell = null;
		if (AirSelection.IsEnabled) AirSelection.Disable();
	}

	public void Select(Character character)
	{
        Clean();
        Character = character;
		AfterSelect?.Invoke(character);
		
		if (GamePlayer != character.Owner || !character.CanTakeAction) return;
		
		Prepare(character.GetPrepareBasicAttackCells());
		Prepare(character.GetPrepareBasicMoveCells(), true);

        HexCells.Distinct().ToList().ForEach(c =>
            SelectDrawnCell(c).AddHighlight(!c.IsEmpty && character.CanBasicAttack(c.FirstCharacter)
                    ? Highlights.RedTransparent : Highlights.GreenTransparent));
        RemoveMoveCells();
        MoveCells.Add(character.ParentCell);
	}
	public void Deselect()
	{
        Character = null;
        HexCells = null;
        RemoveMoveCells();
		_game.HexMapDrawer.RemoveHighlights();
	}
	public void Cancel()
	{
		if (AbilityToUse != null)
		{
			((Ability)AbilityToUse).Cancel();
			//Console.GameLog($"ABILITY CANCEL");
		}
		else if (SelectedCharacterToPlace != null)
		{
			_game.HexMapDrawer.RemoveHighlights();
			SelectedCharacterToPlace = null;
		}
		else
		{
			Deselect();
			SelectedCell = null;
		}
	}

	public void Prepare(IUseable abilityToPrepare) => AbilityToUse = abilityToPrepare;
	private void Prepare(List<HexCell> cellRange, bool addToRange = false)
	{
        if (cellRange == null) cellRange = new List<HexCell>();
		if (!addToRange)
			HexCells = cellRange;
		else
			HexCells.AddRange(cellRange);
	}
	public void Prepare(IUseable abilityToPrepare, List<HexCell> cellRange, bool addToRange = false, bool toggleToRed = true)
	{
		Prepare(cellRange, addToRange);

		AbilityToUse = abilityToPrepare;
		if (!toggleToRed) return;
		_game.HexMapDrawer.RemoveHighlights();
		SelectDrawnCells(HexCells).ForEach(c => c.AddHighlight(Highlights.RedTransparent));
	}
	public static List<DrawnHexCell> SelectDrawnCells(IEnumerable<HexCell> cells) => cells.Select(SelectDrawnCell).ToList();
	public static DrawnHexCell SelectDrawnCell(HexCell cell) =>
		HexMapDrawer.Instance.Cells.FirstOrDefault(g => g.HexCell == cell);

	public static void PlayAudio(string path, float volume = 0.8f)
	{
		try
		{
			var ac = Resources.Load("Audio/"+path) as AudioClip;
            AudioSource.PlayClipAtPoint(ac, Camera.main.transform.position, volume);
		}
		catch (Exception e)
		{
			Debug.LogWarning(e.Message);
		}
		
	}

	public void RemoveMoveCells()
	{
		if(MoveCells==null || MoveCells.Count==0) return;
		
		for (int i = MoveCells.Count - 1; i >= 0; i--)
		{
			//Remove the line
			Object.Destroy(SelectDrawnCells(MoveCells)[i].gameObject.GetComponent<LineRenderer>());
			
			MoveCells.RemoveAt(i);
		}
	}

	public void AddMoveCell(HexCell c)
	{
		if(MoveCells==null||MoveCells.Count<1) throw new Exception("Unable to add move cell!");
		//Draw a line between two hexcell centres
		
		//Check for component in case of Zoro's Lack of Orientation
		DrawnHexCell cell = SelectDrawnCell(c);
		LineRenderer lRend = cell.gameObject.GetComponent<LineRenderer>() != null ? cell.gameObject.GetComponent<LineRenderer>() : cell.gameObject.AddComponent<LineRenderer>();
		lRend.SetPositions(new[]
			{SelectDrawnCell(MoveCells.Last()).transform.position + Vector3.up * 20, cell.transform.position + Vector3.up * 20});
		lRend.material = new Material(Shader.Find("Standard")) {color = Color.black};
		lRend.startColor = Color.black;
		lRend.endColor = Color.black;
		lRend.widthMultiplier = 2;
		
		_game.Active.MoveCells.Add(c);
		
	}
	
	public void Clean()
	{
		RemoveMoveCells();
		if(AirSelection.IsEnabled) AirSelection.Disable();
		AbilityToUse = null;
//		ActionType = ActionType.None;
		HexCells = null;
		_game.HexMapDrawer.RemoveHighlights();
		_game.HexMapDrawer.RemoveHighlightsOfColor(Highlights.BlueTransparent);
	}
	public void CleanAndTrySelecting()
	{
        Clean();	
//		Character?.Select();
		Select(Character);
	}
//	private void MakeAction(Character characterThatMakesAction, ActionType actionType, List<HexCell> cells)
//	{
//		if(!characterThatMakesAction.CanTakeAction) return;
//		switch (actionType)
//		{
//			case ActionType.None:
//				Console.DebugLog("Żadna akcja nie jest aktywna!");
//				return;
//			case ActionType.UseAbility:
//                //CharacterOnMap.TryToInvokeJustBeforeFirstAction();
//				CharacterOnMap.TryToTakeTurn();
//				//AbilityToUse.Use(cells);
//				Action.UseAbility(AbilityToUse, cells);
//				//Console.GameLog($"ABILITY USE: {string.Join("; ", cells.Select(p => p.Coordinates))}");
//				break;
//			case ActionType.AttackAndMove:
//				bool isActionSuccessful = MakeAttackAndMoveAction(characterThatMakesAction, cells[0], MoveCells);
//				if(!isActionSuccessful) return;
//				
//				HexCells = null;//TODO is this really needed?
//				ActionType = ActionType.None;
//				characterThatMakesAction.Select();
//				break;
//			default:
//				throw new ArgumentOutOfRangeException();
//		}	
//	}

//	private bool MakeAttackAndMoveAction(Character character, HexCell cell, List<HexCell> moveCells)
//	{
//        if (cell.CharactersOnCell.Count > 0 && character.CanBasicAttack(cell.CharactersOnCell[0]))
//        {
//	        Action.BasicAttack(character, cell.CharactersOnCell[0]);
//            //character.MakeActionBasicAttack(cell.CharactersOnCell[0]);
//        }
//        else
//        {
//	        //try to move
//            if (!cell.IsFreeToStand) return false;
//            if (MoveCells.Last() != cell) return false;
//	        character.BasicMove(moveCells);
//	        //character.MakeActionBasicMove(moveCells);
//        }
//
//		return true;
//	}

//	public void MakeAction(Character characterThatMakesAction, ActionType actionType, HexCell cell) =>
//		MakeAction(characterThatMakesAction, actionType, new List<HexCell>{cell});

//	public void MakeAction(List<HexCell> cells) => MakeAction(!_game.IsReplay ? CharacterOnMap : Turn.CharacterThatTookActionInTurn, ActionType, cells);
//	public void MakeAction(HexCell cell) => MakeAction(new List<HexCell> {cell});
//	public void MakeAction() => (!_game.IsReplay ? CharacterOnMap : Turn.CharacterThatTookActionInTurn).TryToInvokeJustBeforeFirstAction();

	public static bool IsPointerOverUiObject()
	{
		var eventDataCurrentPosition =
			new PointerEventData(EventSystem.current) {position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)};
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}
}

