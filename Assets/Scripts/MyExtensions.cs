using System;
using System.Collections.Generic;
using System.Linq;
using Hex;
using MyGameObjects.MyGameObject_templates;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public static class MyExtensions
{
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
		cellRange.RemoveAll(cell => cell.CharacterOnCell == null || cell.CharacterOnCell.Owner == Active.Instance.Player);
	}
	/// <summary>
	/// Leaves only cells with friendly characters
	/// </summary>
	public static void RemoveNonFriends(this List<HexCell> cellRange)
	{
		cellRange.RemoveAll(cell => cell.CharacterOnCell == null || cell.CharacterOnCell.Owner != Active.Instance.Player);
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
		entry.callback.AddListener((eventData) => { Tooltip.Instance.Set(tooltipText); });
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
		entry.callback.AddListener((eventData) => { Tooltip.Instance.Remove(); });
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
}