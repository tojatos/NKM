using System;
using System.Collections.Generic;
using System.Linq;
using Hex;
using Managers;
using NKMObjects.Templates;
using UnityEngine;
using UnityEngine.EventSystems;
using NKMObject = NKMObjects.Templates.NKMObject;
using Object = UnityEngine.Object;
/// <summary>
/// Main utility class.
/// Contains information about active game state.
/// </summary>
public class Active
{
	private static Game Game => GameStarter.Instance.Game;
	public Active()
	{
		Phase = new Phase();
		Turn = new Turn();
		AirSelection = new AirSelection();
	}

	public Turn Turn { get; }
	public Phase Phase { get; }
	public AirSelection AirSelection { get; }

	public GamePlayer GamePlayer;
	public Action Action;
	public IUseable AbilityToUse;
	public NKMObject NkmObject;
	public NKMCharacter CharacterOnMap;
	public HexCell SelectedCell;
	public readonly List<HexCell> MoveCells = new List<HexCell>();

//	public delegate void UseableDelegate(IUseable ability);
//    public event UseableDelegate AfterAbilityUse;
	
	public List<HexCell> HexCells { get; set; }
	//TODO: refactor this
	public List<HexCell> HelpHexCells
	{
		private get { return _helpHexCells; }
		set
		{
			_helpHexCells = value;
			if (value == null) Game.HexMapDrawer.RemoveHighlightsOfColor(Highlights.BlueTransparent);
			else HelpHexCells.ForEach(c => c.AddHighlight(Highlights.BlueTransparent));
		}
	}

	private List<HexCell> _helpHexCells;
	private List<GameObject> _buttons;

	public bool IsActiveUse => !(AbilityToUse == null && (Action == Action.None || Action == Action.AttackAndMove) && NkmObject == null);

	public void Reset()
	{
//		((Ability)AbilityToUse)?.Finish();
		if (IsActiveUse || Turn.IsDone)
		{
			CharacterOnMap?.Deselect();
		}
		AbilityToUse = null;
		HexCells = null;
		NkmObject = null;
		SelectedCell = null;
		Action = Action.None;
		if (AirSelection.IsEnabled) AirSelection.Disable();
	}
	public void Cancel()
	{
		if (AbilityToUse != null)
		{
			((Ability)AbilityToUse).Cancel();
			Console.GameLog($"ABILITY CANCEL");
		}
		else if (NkmObject != null)
		{
			Game.HexMapDrawer.RemoveHighlights();
			NkmObject = null;
		}
		else
		{
			CharacterOnMap?.Deselect();
			SelectedCell = null;
		}
	}

	public void Prepare(IUseable abilityToPrepare) => AbilityToUse = abilityToPrepare;
	public bool Prepare(Action actionToPrepare, List<HexCell> cellRange, bool addToRange = false)
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
		Action = actionToPrepare;
		return true;
	}
	public void Prepare(IUseable abilityToPrepare, List<HexCell> cellRange, bool addToRange = false, bool toggleToRed = true)
	{
//		var isPrepared = Prepare(Action.UseAbility, cellRange, addToRange);
		Prepare(Action.UseAbility, cellRange, addToRange);
//		if (!isPrepared) return false;

		AbilityToUse = abilityToPrepare;
		if (!toggleToRed) return;
		Game.HexMapDrawer.RemoveHighlights();
		HexCells.ForEach(c => c.AddHighlight(Highlights.RedTransparent));
//		return true;
	}

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
		
		for (var i = MoveCells.Count - 1; i >= 0; i--)
		{
			//Remove the line
			Object.Destroy(MoveCells[i].gameObject.GetComponent<LineRenderer>());
			
			MoveCells.RemoveAt(i);
		}
	}

	public void AddMoveCell(HexCell cell)
	{
		if(MoveCells==null||MoveCells.Count<1) throw new Exception("Unable to add move cell!");
		//Draw a line between two hexcell centres
		
		//Check for component in case of Zoro's Lack of Orientation
		LineRenderer lRend = cell.gameObject.GetComponent<LineRenderer>() != null ? cell.gameObject.GetComponent<LineRenderer>() : cell.gameObject.AddComponent<LineRenderer>();
		lRend.SetPositions(new[]
			{MoveCells.Last().transform.position + Vector3.up * 20, cell.transform.position + Vector3.up * 20});
		lRend.material = new Material(Shader.Find("Standard")) {color = Color.black};
		lRend.startColor = Color.black;
		lRend.endColor = Color.black;
		lRend.widthMultiplier = 2;
		
		Game.Active.MoveCells.Add(cell);
		
	}
	
	public void Clean()
	{
		RemoveMoveCells();
		if(AirSelection.IsEnabled) AirSelection.Disable();
		AbilityToUse = null;
		Action = Action.None;
		HexCells = null;
		Game.HexMapDrawer.RemoveHighlights();
		Game.HexMapDrawer.RemoveHighlightsOfColor(Highlights.BlueTransparent);
	}
	public void CleanAndTrySelecting()
	{
        Clean();	
		CharacterOnMap?.Select();
	}
	private void MakeAction(NKMCharacter characterThatMakesAction, Action action, List<HexCell> cells)
	{
		switch (action)
		{
			case Action.None:
				Console.Instance.DebugLog("Żadna akcja nie jest aktywna!");
				return;
			case Action.UseAbility:
                CharacterOnMap.TryToInvokeJustBeforeFirstAction();
//				Console.GameLog($"ABILITY USE: {((Ability)AbilityToUse).ID}: {string.Join("; ", cells.Select(p => p.Coordinates))}");
				AbilityToUse.Use(cells);
				Console.GameLog($"ABILITY USE: {string.Join("; ", cells.Select(p => p.Coordinates))}");
				break;
			case Action.AttackAndMove:
				if (cells.Count != 1)
				{
                    Console.Instance.DebugLog("Próbowano wykonać ruch lub atak na więcej niż jedno pole!");
					return;
				}

				bool isActionSuccessful = MakeAttackAndMoveAction(characterThatMakesAction, cells[0], MoveCells);
				if(!isActionSuccessful) return;
				
				HexCells = null;//TODO is this really needed?
				Action = Action.None;
				characterThatMakesAction.Select();
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}	
	}

	private bool MakeAttackAndMoveAction(NKMCharacter character, HexCell cell, List<HexCell> moveCells)
	{
        if (cell.CharacterOnCell != null && character.CanBasicAttack(cell.CharacterOnCell))
        {
            character.MakeActionBasicAttack(cell.CharacterOnCell);
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

	public void MakeAction(NKMCharacter characterThatMakesAction, Action action, HexCell cell) =>
		MakeAction(characterThatMakesAction, action, new List<HexCell>{cell});

	public void MakeAction(List<HexCell> cells) => MakeAction(!Game.IsReplay ? CharacterOnMap : Turn.CharacterThatTookActionInTurn, Action, cells);
	public void MakeAction(HexCell cell) => MakeAction(new List<HexCell> {cell});
	public void MakeAction() => (!Game.IsReplay ? CharacterOnMap : Turn.CharacterThatTookActionInTurn).TryToInvokeJustBeforeFirstAction();

	public static bool IsPointerOverUiObject()
	{
		var eventDataCurrentPosition =
			new PointerEventData(EventSystem.current) {position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)};
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}
}

