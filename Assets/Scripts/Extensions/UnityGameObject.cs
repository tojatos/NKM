using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Extensions
{
	public static class UnityGameObject
	{
		private static void Toggle(this Selectable targetButton) => targetButton.interactable = !targetButton.interactable;
		private static void Toggle(this GameObject targetGameObject) => targetGameObject.SetActive(!targetGameObject.activeSelf);
		public static void Show(this GameObject targetGameObject) => targetGameObject.SetActive(true);
		public static void Show(this List<GameObject> targetGameObjects) => targetGameObjects.ForEach(Show);
		public static void Hide(this GameObject targetGameObject) => targetGameObject.SetActive(false);
		public static void Hide(this List<GameObject> targetGameObjects) => targetGameObjects.ForEach(Hide);
		public static void SetText(this Text gameObject, string text) => gameObject.GetComponent<Text>().text = text;

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

		public static void ChangeImageColor(this GameObject gameObject, Color targetColor)
		{
			gameObject.GetComponent<Image>().color = targetColor;
		}

		public static void AddTrigger(this GameObject gameObject, EventTriggerType eventTriggerType, UnityAction onTrigger) =>
			gameObject.AddTrigger(eventTriggerType, e => onTrigger());
		public static void AddTrigger(this GameObject gameObject, EventTriggerType eventTriggerType, UnityAction<BaseEventData> onTrigger)
		{
			EventTrigger trigger = gameObject.GetComponent<EventTrigger>() ?? gameObject.AddComponent<EventTrigger>();
			var entry = new EventTrigger.Entry {eventID = eventTriggerType};
			entry.callback.AddListener(onTrigger);
			trigger.triggers.Add(entry);
		}

		public static Dropdown AddDropdownGroup(this GameObject gameObject, DropdownSettings dropdownSettings)
		{
			GameObject dropdownGroup = Object.Instantiate(Stuff.Prefabs.Find(p => p.name == "DropdownGroup"), gameObject.transform);
			dropdownGroup.GetComponentInChildren<Text>().text = dropdownSettings.Description;

			var dropdown = dropdownGroup.GetComponentInChildren<Dropdown>();
			dropdown.name = dropdownSettings.Type;
			if(dropdownSettings.Options != null) dropdown.options = dropdownSettings.Options.Select(o => new Dropdown.OptionData(o)).ToList();
			dropdown.value = SessionSettings.Instance.GetDropdownSetting(dropdownSettings.Type);
			
			return dropdown;
		}

		public static Vector3 GetCharacterTransformPoint(this Transform transform) =>
			transform.TransformPoint(0, 10, 0);
	}

	public class DropdownSettings
	{
		public string Type;
		public string Description;
		public string[] Options;
	}
}