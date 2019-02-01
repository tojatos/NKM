using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Unity.Extensions
{
	public static class Tooltip
	{
		private static void AddSetTooltipEvent(this GameObject gameObject, EventTriggerType eventTriggerType, string tooltipText, Vector3? postion = null)
		{
			UnityAction<BaseEventData> updateTooltipText = abstractEventData => global::Unity.Tooltip.Instance.Set(tooltipText, postion);
			gameObject.AddTrigger(eventTriggerType, updateTooltipText);
		}


		/// <summary>
		/// Show tooltip on PointerEnter with text
		/// </summary>
		public static void AddSetTooltipEvent(this GameObject gameObject, string tooltipText, Vector3? position = null) => 
			gameObject.AddSetTooltipEvent(EventTriggerType.PointerEnter, tooltipText, position);

		private static void AddRemoveTooltipEvent(this GameObject gameObject, EventTriggerType eventTriggerType)
		{
			EventTrigger trigger = gameObject.GetComponent<EventTrigger>() ?? gameObject.AddComponent<EventTrigger>();
			var entry = new EventTrigger.Entry {eventID = eventTriggerType};
			entry.callback.AddListener((eventData) => global::Unity.Tooltip.Instance.Remove());
			trigger.triggers.Add(entry);
		}

		/// <summary>
		/// Remove tooltip on PointerExit
		/// </summary>
		public static void AddRemoveTooltipEvent(this GameObject gameObject)
		{
			gameObject.AddRemoveTooltipEvent(EventTriggerType.PointerExit);
		}

		public static void AddDefaultTooltip(this GameObject gameObject, string tooltipText, Vector3? position = null)
		{
			gameObject.AddSetTooltipEvent(tooltipText, position);
			gameObject.AddRemoveTooltipEvent();
		}
		
	}
}