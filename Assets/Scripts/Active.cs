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
		IsDebug = true;
	}

	public Turn Turn { get; }
	public Phase Phase { get; }
	public AirSelection AirSelection { get; }

	public GamePlayer GamePlayer;
	public Action Action;
	public IUseable AbilityToUse;
	public NKMObject NkmObject;
	public Character CharacterOnMap;
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

	public bool IsDebug { get; set; }

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
		if (AbilityToUse != null) ((Ability)AbilityToUse).Cancel();
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
	//TODO: Merge MakeActions
	public void MakeAction(HexCell cell)
	{
		if (!HexCells.Contains(cell)) return;
//		if (CharacterOnMap!= null && Turn.CharacterThatTookActionInTurn == null) CharacterOnMap.InvokeJustBeforeFirstAction();
		switch (Action)
		{
			case Action.None:
				throw new Exception("Żadna akcja nie jest aktywna!");
			case Action.UseAbility:
                if (Turn.CharacterThatTookActionInTurn == null) CharacterOnMap.InvokeJustBeforeFirstAction();
				AbilityToUse.Use(new List<HexCell> {cell});
				Console.Instance.GameLog($"ABILITY USE: {((Ability)AbilityToUse).ID}: {cell.Coordinates}");
				break;
			case Action.AttackAndMove:
				Character character = CharacterOnMap;
				if(character==null) throw new NullReferenceException();
				if (cell.CharacterOnCell != null && (cell.CharacterOnCell.IsEnemyFor(character.Owner)||character.CanAttackAllies))
				{
					if(!character.GetBasicAttackCells().Contains(cell)) return;
					
                    if (Turn.CharacterThatTookActionInTurn == null) CharacterOnMap.InvokeJustBeforeFirstAction();
					character.BasicAttack(cell.CharacterOnCell);
				}
				else
				{
					if (!cell.IsFreeToStand) return;
					if (MoveCells.Last() != cell) return;
					if (Turn.CharacterThatTookActionInTurn == null) CharacterOnMap.InvokeJustBeforeFirstAction();
					character.BasicMove(MoveCells);
					character.InvokeAfterBasicMove();
				}

				HexCells = null;//TODO is this really needed?
				Action = Action.None;
				character.Select();
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
	public void MakeAction(List<HexCell> cells)
	{
		if (Turn.CharacterThatTookActionInTurn == null) CharacterOnMap.InvokeJustBeforeFirstAction();
		switch (Action)
		{
			case Action.None:
				throw new Exception("Żadna akcja nie jest aktywna!");
			case Action.UseAbility:
				AbilityToUse.Use(cells);
//				AfterAbilityUse?.Invoke(AbilityToUse);
//				List<Character> characters = cells.Where(c => c.CharacterOnCell != null).Select(c => c.CharacterOnCell).ToList();
//				Ability.Use(characters);
				//Turn.CharacterThatTookActionInTurn = CharacterOnMap;
				Console.Instance.GameLog($"ABILITY USE: {((Ability)AbilityToUse).ID}: {string.Join("; ", cells.Select(p => p.Coordinates))}");
				break;
			case Action.AttackAndMove:
				throw new Exception();
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public void MakeAction()
	{
		if (Turn.CharacterThatTookActionInTurn == null) CharacterOnMap.InvokeJustBeforeFirstAction();
	}

	public static bool IsPointerOverUiObject()
	{
		var eventDataCurrentPosition =
			new PointerEventData(EventSystem.current) {position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)};
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}
    public void TakeActionWithCharacter()
    {
        CharacterOnMap.InvokeJustBeforeFirstAction();
        Turn.Finish();
    }
}

