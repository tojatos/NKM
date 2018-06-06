using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using Hex;
using MyGameObjects.MyGameObject_templates;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;
/// <summary>
/// Main utility class.
/// Contains information about active game state.
/// </summary>
public class Active
{
	private readonly Game Game;
	public Active(Game game)
	{
		Game = game;
		Phase = new Phase(game);
		Turn = new Turn(game);
		AirSelection = new AirSelection(game);
		IsDebug = true;
	}

	public Turn Turn { get; }
	public Phase Phase { get; }
	public AirSelection AirSelection { get; }

	public  GamePlayer GamePlayer;
	public  Action Action;
	public  Ability Ability;
	public  MyGameObject MyGameObject;
	public  Character CharacterOnMap;
	public readonly List<HexCell> MoveCells = new List<HexCell>();

	
	public List<HexCell> HexCells { get; set; }
	//TODO: refactor this
	public List<HexCell> HelpHexCells
	{
		private get { return _helpHexCells; }
		set
		{
			_helpHexCells = value;
			if (value == null)
			{
				Game.HexMapDrawer.RemoveAllHelpHighlights();
			}
			else
			{
				HelpHexCells.ForEach(c => c.ToggleHelpHighlight(HiglightColor.WhiteOrange));
			}
		}
	}

	private List<HexCell> _helpHexCells;
	private List<GameObject> _buttons;

//	public List<string> GetInfo => new List<string>{ ((int)Action).ToString(), CharacterOnMap.Guid.ToString(), Ability.Name, MyGameObject.Guid.ToString(), string.Join("*",HexCells.Select(c=>c.Coordinates))};
	public bool IsDebug { get; set; }

	//public bool IsActivePlayerCharacterSelected
	//{
	//	get { return CharacterOnMap != null && CharacterOnMap.Owner == GamePlayer; }
	//}
	public bool IsActiveUse => !(Ability == null && (Action == Action.None || Action == Action.AttackAndMove) && MyGameObject == null);


	/// <summary>
	/// On set: Hide previous Buttons and show new.
	/// </summary>
	public List<GameObject> Buttons
	{
		set
		{
			HelpHexCells = null;
			_buttons?.Hide();
			_buttons = value;
			_buttons?.Show();
		}
	}

	public void Reset()
	{
		Ability?.OnUseFinish();
		if (IsActiveUse || Turn.IsDone)
		{
			CharacterOnMap?.Deselect();
		}
		Ability = null;
		HexCells = null;
		MyGameObject = null;
		Action = Action.None;
		if (AirSelection.IsEnabled) AirSelection.Disable();
	}
	public void Cancel()
	{
		if (Ability != null)
		{
			Ability.Cancel();
		}
		else if(MyGameObject != null)
		{
			Game.HexMapDrawer.RemoveAllHighlights();
			MyGameObject = null;
		}
		else
		{
			CharacterOnMap?.Deselect();
		}
	}
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
	public bool Prepare(Ability abilityToPrepare, List<HexCell> cellRange, bool addToRange = false, bool toggleToRed = true)
	{
		var isPrepared = Prepare(Action.UseAbility, cellRange, addToRange);
		if (!isPrepared) return false;

		Ability = abilityToPrepare;
		if (toggleToRed)
		{
			Game.HexMapDrawer.RemoveAllHighlights();
			HexCells.ForEach(c => c.ToggleHighlight(HiglightColor.Red));
		}
		return true;
	}

	public void PlayAudio(string path, float volume = 0.8f)
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
			{MoveCells.Last().transform.position + Vector3.up, cell.transform.position + Vector3.up});
		lRend.material = new Material(Shader.Find("Standard"));
		lRend.startColor = Color.black;
		lRend.endColor = Color.black;
		lRend.widthMultiplier = 2;
		
		Game.Active.MoveCells.Add(cell);
		
	}

	public void Clean()
	{
		RemoveMoveCells();
		if(AirSelection.IsEnabled) AirSelection.Disable();
		Ability = null;
		Action = Action.None;
		HexCells = null;
		Game.HexMapDrawer.RemoveAllHighlights();
	}
	public void CleanAndTrySelecting()
	{
        Clean();	
		CharacterOnMap?.Select();
	}
	public void MakeAction(HexCell cell)
	{
		if (!HexCells.Contains(cell)) return;
//		if (Turn.CharacterThatTookActionInTurn == null) CharacterOnMap.InvokeJustBeforeFirstAction();
		if(CharacterOnMap!=null && !CharacterOnMap.TookActionInPhaseBefore) CharacterOnMap.InvokeJustBeforeFirstAction();
		switch (Action)
		{
			case Action.None:
				throw new Exception("Żadna akcja nie jest aktywna!");
			case Action.UseAbility:
				if (cell.CharacterOnCell != null)
				{
					Ability.Use(cell.CharacterOnCell);
				}
				else
				{
					Ability.Use(cell);
				}
				break;
			case Action.AttackAndMove:
				Character character = CharacterOnMap;
				if (cell.CharacterOnCell != null)
				{
					character.BasicAttack(cell.CharacterOnCell);
				}
				else
				{
					if (cell == MoveCells.LastOrDefault())
					{
                        if (character.Abilities.Any(a => a.OverridesMove))
                                                                              {
                                                                              if (character.Abilities.Count(a => a.OverridesMove) > 1)
                                                                              throw new Exception("Więcej niż jedna umiejętność próbuje nadpisać akcję ruchu!");
                                                                              
                                                                              character.Abilities.Single(a => a.OverridesMove).Move(MoveCells);
                                                                              }
                        else
                        {
                            character.BasicMove(MoveCells);
                        }
					}
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
				Ability.Use(cells);
//				List<Character> characters = cells.Where(c => c.CharacterOnCell != null).Select(c => c.CharacterOnCell).ToList();
//				Ability.Use(characters);
				//Turn.CharacterThatTookActionInTurn = CharacterOnMap;
				break;
			case Action.AttackAndMove:
				throw new Exception();
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public bool IsPointerOverUIObject()
	{
		var eventDataCurrentPosition =
			new PointerEventData(EventSystem.current) {position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)};
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}
}

public static class ActivePropertyName
{
	public const string GamePlayer = "GamePlayer";
	public const string Action = "Action";
	public const string Ability = "Ability";
	public const string MyGameObject = "MyGameObject";
	public const string CharacterOnMap = "CharacterOnMap";
}