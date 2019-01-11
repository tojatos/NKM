﻿using System;
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
	private Console Console => _game.Console;
	public Active(Game game)
	{
		_game = game;
		Phase = new Phase();
		Turn = new Turn(game);
		AirSelection = new AirSelection(game);
	}

	public readonly Turn Turn;// { get; }
	public readonly Phase Phase;// { get; }
	public AirSelection AirSelection { get; }

	public GamePlayer GamePlayer;
	public ActionType ActionType;
	public IUseable AbilityToUse;
	public Character SelectedCharacterToPlace;
	public Character CharacterOnMap;
	public HexCell SelectedCell;
	public readonly List<HexCell> MoveCells = new List<HexCell>();
	
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
	private List<GameObject> _buttons;

	public bool IsActiveUse => !(AbilityToUse == null && (ActionType == ActionType.None || ActionType == ActionType.AttackAndMove) && SelectedCharacterToPlace == null);

	public void Reset()
	{
//		((Ability)AbilityToUse)?.Finish();
		if (IsActiveUse || Turn.IsDone)
		{
			CharacterOnMap?.Deselect();
		}
		AbilityToUse = null;
		HexCells = null;
		//NkmObject = null;
		SelectedCharacterToPlace = null;
		SelectedCell = null;
		ActionType = ActionType.None;
		if (AirSelection.IsEnabled) AirSelection.Disable();
	}
	public void Cancel()
	{
		if (AbilityToUse != null)
		{
			((Ability)AbilityToUse).Cancel();
			Console.GameLog($"ABILITY CANCEL");
		}
		else if (SelectedCharacterToPlace != null)
		{
			_game.HexMapDrawer.RemoveHighlights();
			SelectedCharacterToPlace = null;
		}
		else
		{
			CharacterOnMap?.Deselect();
			SelectedCell = null;
		}
	}

	public void Prepare(IUseable abilityToPrepare) => AbilityToUse = abilityToPrepare;
	public bool Prepare(ActionType actionTypeToPrepare, List<HexCell> cellRange, bool addToRange = false)
	{
		if (cellRange == null)
		{
			throw new Exception("Cell range cannot be null!");
		}

		if (cellRange.Count == 0 && !addToRange)
		{
			return false;
		}

		if (HexCells!=null&&addToRange)
		{
			HexCells.AddRange(cellRange);
		}
		else
		{
			HexCells = cellRange;
		}
		ActionType = actionTypeToPrepare;
		return true;
	}
	public void Prepare(IUseable abilityToPrepare, List<HexCell> cellRange, bool addToRange = false, bool toggleToRed = true)
	{
		Prepare(ActionType.UseAbility, cellRange, addToRange);

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
		ActionType = ActionType.None;
		HexCells = null;
		_game.HexMapDrawer.RemoveHighlights();
		_game.HexMapDrawer.RemoveHighlightsOfColor(Highlights.BlueTransparent);
	}
	public void CleanAndTrySelecting()
	{
        Clean();	
		CharacterOnMap?.Select();
	}
	private void MakeAction(Character characterThatMakesAction, ActionType actionType, List<HexCell> cells)
	{
		switch (actionType)
		{
			case ActionType.None:
				Console.DebugLog("Żadna akcja nie jest aktywna!");
				return;
			case ActionType.UseAbility:
                CharacterOnMap.TryToInvokeJustBeforeFirstAction();
//				Console.GameLog($"ABILITY USE: {((Ability)AbilityToUse).ID}: {string.Join("; ", cells.Select(p => p.Coordinates))}");
				AbilityToUse.Use(cells);
				Console.GameLog($"ABILITY USE: {string.Join("; ", cells.Select(p => p.Coordinates))}");
				break;
			case ActionType.AttackAndMove:
				if (cells.Count != 1)
				{
                    Console.DebugLog("Próbowano wykonać ruch lub atak na więcej niż jedno pole!");
					return;
				}

				bool isActionSuccessful = MakeAttackAndMoveAction(characterThatMakesAction, cells[0], MoveCells);
				if(!isActionSuccessful) return;
				
				HexCells = null;//TODO is this really needed?
				ActionType = ActionType.None;
				characterThatMakesAction.Select();
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}	
	}

	private bool MakeAttackAndMoveAction(Character character, HexCell cell, List<HexCell> moveCells)
	{
        if (cell.CharactersOnCell.Count > 0 && character.CanBasicAttack(cell.CharactersOnCell[0]))
        {
            character.MakeActionBasicAttack(cell.CharactersOnCell[0]);
        }
        else
        {
	        //try to move
            if (!cell.IsFreeToStand) return false;
            if (MoveCells.Last() != cell) return false;
            character.MakeActionBasicMove(moveCells);
        }

		return true;
	}

	public void MakeAction(Character characterThatMakesAction, ActionType actionType, HexCell cell) =>
		MakeAction(characterThatMakesAction, actionType, new List<HexCell>{cell});

	public void MakeAction(List<HexCell> cells) => MakeAction(!_game.IsReplay ? CharacterOnMap : Turn.CharacterThatTookActionInTurn, ActionType, cells);
	public void MakeAction(HexCell cell) => MakeAction(new List<HexCell> {cell});
	public void MakeAction() => (!_game.IsReplay ? CharacterOnMap : Turn.CharacterThatTookActionInTurn).TryToInvokeJustBeforeFirstAction();

	public static bool IsPointerOverUiObject()
	{
		var eventDataCurrentPosition =
			new PointerEventData(EventSystem.current) {position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)};
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}
}

