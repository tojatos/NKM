using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hex;
using Managers;
using MyGameObjects.MyGameObject_templates;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public static class MyExtensions
{
//	private static Active Active = GameStarter.Instance.Game.Active;

	public static IEnumerable<string> GetClassNames<T>(this List<T> list)
	{
		var classNames = new List<string>();
		list.ForEach(l => classNames.Add(l.GetType().Name));
		return classNames;
	}
	public static List<string> GetNames<T>(this IEnumerable<T> myGameObjects) where T : MyGameObject
	{
		return myGameObjects.Select(m => m.Name).ToList();
	}
	/// <summary>
	/// Leaves only cells with enemy characters
	/// </summary>
	public static void RemoveNonEnemies(this List<HexCell> cellRange)
	{
		cellRange.RemoveAll(cell => cell.CharacterOnCell == null || cell.CharacterOnCell.Owner == GameStarter.Instance.Game.Active.GamePlayer);
	}
	/// <summary>
	/// Leaves only cells with friendly characters
	/// </summary>
	public static void RemoveNonFriends(this List<HexCell> cellRange)
	{
		cellRange.RemoveAll(cell => cell.CharacterOnCell == null || cell.CharacterOnCell.Owner != GameStarter.Instance.Game.Active.GamePlayer);
	}
	/// <summary>
	/// Leaves only cells with characters
	/// </summary>
	public static void RemoveNonCharacters(this List<HexCell> cellRange)
	{
		//var unavailableRange = new List<HexCell>(cellRange);
		cellRange.RemoveAll(cell => cell.CharacterOnCell == null);
		//cellRange.ForEach(c => unavailableRange.Remove(c));
		//return;
	}
	private static void AddSetTooltipEvent(this GameObject gameObject, EventTriggerType eventTriggerType, string tooltipText)
	{
		var trigger = gameObject.GetComponent<EventTrigger>() ?? gameObject.AddComponent<EventTrigger>();
		//if (trigger == null) trigger =  gameObject.AddComponent<EventTrigger>();
		var entry = new EventTrigger.Entry {eventID = eventTriggerType};
		entry.callback.AddListener((eventData) => Tooltip.Instance.Set(tooltipText));
		trigger.triggers.Add(entry);
	}
	/// <summary>
	/// Show tooltip on PointerEnter with text
	/// </summary>
	public static void AddSetTooltipEvent(this GameObject gameObject, string tooltipText)
	{
		gameObject.AddSetTooltipEvent(EventTriggerType.PointerEnter, tooltipText);
	}
	private static void AddRemoveTooltipEvent(this GameObject gameObject, EventTriggerType eventTriggerType)
	{
		var trigger = gameObject.GetComponent<EventTrigger>() ?? gameObject.AddComponent<EventTrigger>();
		var entry = new EventTrigger.Entry {eventID = eventTriggerType};
		entry.callback.AddListener((eventData) => Tooltip.Instance.Remove());
		trigger.triggers.Add(entry);
	}
	/// <summary>
	/// Remove tooltip on PointerExit
	/// </summary>
	public static void AddRemoveTooltipEvent(this GameObject gameObject)
	{
		gameObject.AddRemoveTooltipEvent(EventTriggerType.PointerExit);
	}
	public static void ChangeImageColor(this GameObject gameObject, Color targetColor)
	{
		gameObject.GetComponent<Image>().color = targetColor;
	}
	public static string ToHex(this Color32 color)
	{
		var hex = "#" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
		return hex;
	}
	public static void Clear(this Transform transform)
	{
		foreach (Transform child in transform)
		{
			Object.Destroy(child.gameObject);
		}
		//return transform;
	}
	public static Stat GetStat(this Character character, StatType type)
	{
		Stat stat;
		switch (type)
		{
			case StatType.HealthPoints:
				stat = character.HealthPoints;
				break;
			case StatType.AttackPoints:
				stat = character.AttackPoints;
				break;
			case StatType.BasicAttackRange:
				stat = character.BasicAttackRange;
				break;
			case StatType.Speed:
				stat = character.Speed;
				break;
			case StatType.PhysicalDefense:
				stat = character.PhysicalDefense;
				break;
			case StatType.MagicalDefense:
				stat = character.MagicalDefense;
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(type), type, null);
		}

		return stat;
	}

	public static void Toggle(this Selectable targetButton) => targetButton.interactable = !targetButton.interactable;
	public static void Toggle(this GameObject targetGameObject) => targetGameObject.SetActive(!targetGameObject.activeSelf);
	public static void Show(this GameObject targetGameObject) => targetGameObject.SetActive(true);
	public static void Show(this List<GameObject> targetGameObjects) => targetGameObjects.ForEach(Show);
	public static void Hide(this GameObject targetGameObject) => targetGameObject.SetActive(false);
	public static void Hide(this List<GameObject> targetGameObjects) => targetGameObjects.ForEach(Hide);

	/// <summary>
	/// Disables button if condition is true,
	/// enables button if condition is false.
	/// </summary>
	public static void ToggleIf(this Button buttonToToggle, bool condition)
	{
		if (condition && buttonToToggle.interactable || !condition && !buttonToToggle.interactable)
		{
			buttonToToggle.Toggle();
		}
	}
	/// <summary>
	/// Disables object if condition is true,
	/// enables object if condition is false.
	/// </summary>
	public static void ToggleIf(this GameObject objectToToggle, bool condition)
	{
		if (condition && objectToToggle.activeSelf || !condition && !objectToToggle.activeSelf)
		{
			objectToToggle.Toggle();
		}
	}
	public static async Task WaitToBeTrue(this Func<bool> predicate)
	{
		while (!predicate.Invoke())
		{
			await Task.Delay(1);
		}
	}
	public static int GetIndex(this GamePlayer gamePlayer) => GameStarter.Instance.Game.Players.FindIndex(p => p == gamePlayer);
	public static Color GetColor(this GamePlayer gamePlayer)
	{
		switch (gamePlayer.GetIndex())
		{
			case 0:
				return Color.red;
			case 1:
				return Color.green;
			case 2:
				return Color.blue;
			case 3:
				return Color.cyan;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
	private static HexTileType GetSpawnPointType(this GamePlayer gamePlayer) => GameStarter.Instance.Game.HexMapDrawer.HexMap.SpawnPoints[gamePlayer.GetIndex()];
	public static IEnumerable<HexCell> GetSpawnPoints(this GamePlayer gamePlayer) => GameStarter.Instance.Game.HexMapDrawer.Cells.FindAll(c => c.Type == gamePlayer.GetSpawnPointType());
	public static string FormattedFirstName(this Character character) => string.Format("<color={0}><</color><b>{1}</b><color={0}>></color>", ((Color32)character.Owner.GetColor()).ToHex(), character.Name.Split(' ').Last());
}