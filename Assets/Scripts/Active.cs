using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helpers;
using Hex;
using Managers;
using Multiplayer.Network;
using MyGameObjects.MyGameObject_templates;
using UIManagers;
using UnityEngine;
using UnityEngine.EventSystems;

public class Active
{
	//#region Singleton
	//private static Active _instance;
	//public static Active Instance => _instance ?? (_instance = new Active());

	//#endregion
	private Game Game;
	public Active(Game game)
	{
		Game = game;
		Phase = new Phase(game);
		Turn = new Turn(game);
		AirSelection = new AirSelection(game);
		IsDebug = false;
	}
//	private Active()
//	{
//		Phase = new Phase();
//		Turn = new Turn { Active = this };
//		AirSelection = new AirSelection { Active = this };
//		IsDebug = false;
//	}
//	public UIManager UIManager;

	public Turn Turn { get; }
	public Phase Phase { get; }
	public AirSelection AirSelection { get; }

//	private GamePlayer _gamePlayer;
//	public GamePlayer GamePlayer
//	{
//		get
//		{
//			if (Game.Type == GameType.MultiplayerClient) return Game.Client.SendGetActiveMessage("GamePlayer");
//
//			return _gamePlayer;
//		}
//		set
//		{
//			if (Game.Type == GameType.MultiplayerClient) Game.Client.SendSetActiveMessage("GamePlayer", value.Name);
//			else _gamePlayer = value;
//		}
//	}
	private readonly Synchronizable<GamePlayer> _gamePlayer = new Synchronizable<GamePlayer>(ActivePropertyName.GamePlayer);
	public GamePlayer GamePlayer
	{
		get { return _gamePlayer.Get(); }
		set { _gamePlayer.Set(value); }
	}

	public Action Action { private get; set; }
	public Ability Ability { get; set; }
	public MyGameObject MyGameObject { get; set; }
	public Character CharacterOnMap { get; set; }
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
			if (_buttons != null)
			{
				//UIManager.Hide(_buttons);
				_buttons.Hide();
			}
			_buttons = value;
			//UIManager.Show(_buttons);
			_buttons.Show();
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
		var ac = Resources.Load("Audio/"+path) as AudioClip;
		AudioSource.PlayClipAtPoint(ac, Camera.main.transform.position, volume);
	}

	public void Clean()
	{
		if(AirSelection.IsEnabled) AirSelection.Disable();
		Ability = null;
		Action = Action.None;
		HexCells = null;
		Game.HexMapDrawer.RemoveAllHighlights();
		CharacterOnMap?.Select();
	}
	public void MakeAction(HexCell cell)
	{
		if (!HexCells.Contains(cell)) return;

		if (Game.Type == GameType.MultiplayerClient)
		{
			//			Game.Client.SendMakeActionMessage(Action, Ability, CharacterOnMap, cell.Coordinates.ToString());
						Game.Client.SendMakeActionMessage(cell);

		}

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

public class Synchronizable<T>
{
//	private Game Game;
	public Synchronizable(string name)
	{
		_name = name;
	}
	private string _name;
	private T _value;

	public T Get()
	{
		if (GameStarter.Instance.Game.Type == GameType.MultiplayerClient) return GameStarter.Instance.Game.Client.TryToGetActiveVariable<T>(_name).Result;

		return _value;
	}
	public void Set(T value)
	{
		if (GameStarter.Instance.Game.Type == GameType.MultiplayerClient) GameStarter.Instance.Game.Client.TryToSetActiveVariable<T>(_name, value);
		else _value = value;
	}
}

public static class ActivePropertyName
{
	public static readonly string GamePlayer = "GamePlayer";
}