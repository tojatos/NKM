using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Extensions
{
	public static class Tooltip
	{
		private static void AddSetTooltipEvent(this GameObject gameObject, EventTriggerType eventTriggerType, string tooltipText)
		{
			UnityAction<BaseEventData> updateTooltipText = abstractEventData => global::Tooltip.Instance.Set(tooltipText);
			gameObject.AddTrigger(eventTriggerType, updateTooltipText);
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
			EventTrigger trigger = gameObject.GetComponent<EventTrigger>() ?? gameObject.AddComponent<EventTrigger>();
			var entry = new EventTrigger.Entry {eventID = eventTriggerType};
			entry.callback.AddListener((eventData) => global::Tooltip.Instance.Remove());
			trigger.triggers.Add(entry);
		}

		/// <summary>
		/// Remove tooltip on PointerExit
		/// </summary>
		public static void AddRemoveTooltipEvent(this GameObject gameObject)
		{
			gameObject.AddRemoveTooltipEvent(EventTriggerType.PointerExit);
		}
	}
}