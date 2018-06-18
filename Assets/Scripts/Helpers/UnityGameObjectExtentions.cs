using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Helpers
{
	public static class UnityGameObjectExtentions
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
		
		public static void AddTrigger(this GameObject gameObject, EventTriggerType eventTriggerType, UnityAction<BaseEventData> onTrigger)
		{
			EventTrigger trigger = gameObject.GetComponent<EventTrigger>() ?? gameObject.AddComponent<EventTrigger>();
			var entry = new EventTrigger.Entry {eventID = eventTriggerType};
			entry.callback.AddListener(onTrigger);
			trigger.triggers.Add(entry);
		}
	}
}