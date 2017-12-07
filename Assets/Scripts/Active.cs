using System;
using System.Collections.Generic;
using System.Linq;
using Hex;
using MyGameObjects.MyGameObject_templates;
using UIManagers;
using UnityEngine;
using UnityEngine.EventSystems;

public class Active
{
	#region Singleton
	private static Active _instance;
	public static Active Instance => _instance ?? (_instance = new Active());

	#endregion

	public UIManager UIManager;
	
	public Turn Turn { get; }
	public Phase Phase { get; }
	public AirSelection AirSelection { get; }
	public Player Player { get; set; }
	public Action Action { private get; set; }
	public Ability Ability { get; set; }
	public MyGameObject MyGameObject { get; set; }
	public Character CharacterOnMap { get; set; }
	public List<HexCell> HexCells { get; set; }
	public List<HexCell> HelpHexCells
	{
		private get { return _helpHexCells; }
		set
		{
			_helpHexCells = value;
			if (value == null)
			{
				HexMapDrawer.RemoveAllHelpHighlights();
			}
			else
			{
				HelpHexCells.ForEach(c => c.ToggleHelpHighlight(HiglightColor.WhiteOrange));
			}
		}
	}

	private List<HexCell> _helpHexCells;
	private List<GameObject> _ui;
	private List<GameObject> _buttons;


	public bool IsDebug { get; set; }

	//public bool IsActivePlayerCharacterSelected
	//{
	//	get { return CharacterOnMap != null && CharacterOnMap.Owner == Player; }
	//}
	public bool IsActiveUse => !(Ability == null && (Action == Action.None || Action == Action.AttackAndMove) && MyGameObject == null);

	/// <summary>
	/// On set: Hide previous UI and show new. If set to null show GameUI.
	/// </summary>
	public List<GameObject> UI
	{
		get { return _ui; }
		set
		{
			if (_ui != null)
			{
				UIManager.Hide(_ui);
			}
			_ui = value ?? UIManager.GameUI;
			UIManager.Show(_ui);
		}
	}
	/// <summary>
	/// On set: Hide previous Buttons and show new.
	/// </summary>
	public List<GameObject> Buttons
	{
		set
		{
			HelpHexCells = null;
			if (_buttons != null)
			{
				UIManager.Hide(_buttons);
			}
			_buttons = value;
			UIManager.Show(_buttons);
		}
	}



	private Active()
	{
		Phase = new Phase();
		Turn = new Turn {Active = this};
		AirSelection = new AirSelection {Active = this};
		IsDebug = false;
	}

	public void Reset()
	{
		Ability?.OnUseFinish();
		if (IsActiveUse || Turn.IsDone)
		{
			Character.Deselect();
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
			HexMapDrawer.RemoveAllHighlights();
			MyGameObject = null;
		}
		else if (CharacterOnMap != null)
		{
			Character.Deselect();
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
			HexMapDrawer.RemoveAllHighlights();
			HexCells.ForEach(c => c.ToggleHighlight(HiglightColor.Red));
		}
		return true;
	}

	public void PlayAudio(string path, float volume = 0.8f)
	{
		var ac = Resources.Load("Audio/"+path) as AudioClip;
		AudioSource.PlayClipAtPoint(ac, Camera.main.transform.position, volume);
	}

	public void Clean()
	{
		if(AirSelection.IsEnabled) AirSelection.Disable();
		Ability = null;
		Action = Action.None;
		HexCells = null;
		HexMapDrawer.RemoveAllHighlights();
		CharacterOnMap?.Select();
	}
	public void MakeAction(HexCell cell)
	{
		if (!HexCells.Contains(cell)) return;
		if (Turn.CharacterThatTookActionInTurn == null) CharacterOnMap.InvokeJustBeforeFirstAction();
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
				var character = CharacterOnMap;
				if (cell.CharacterOnCell != null)
				{
					character.BasicAttack(cell.CharacterOnCell);
				}
				else
				{
					if (character.Abilities.Any(a => a.OverridesMove))
					{
						if (character.Abilities.Count(a => a.OverridesMove) > 1)
							throw new Exception("Więcej niż jedna umiejętność próbuje nadpisać akcję ruchu!");
						character.Abilities.Single(a => a.OverridesMove).Move(cell);
					}
					else
					{
						character.BasicMove(cell);
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
	public void MakeAction(IEnumerable<HexCell> cells)
	{
		if (Turn.CharacterThatTookActionInTurn == null) CharacterOnMap.InvokeJustBeforeFirstAction();
		switch (Action)
		{
			case Action.None:
				throw new Exception("Żadna akcja nie jest aktywna!");
			case Action.UseAbility:
				var characters = cells.Where(c => c.CharacterOnCell != null).Select(c => c.CharacterOnCell).ToList();
				Ability.Use(characters);
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
		var results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}
}