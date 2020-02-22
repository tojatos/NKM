using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Unity
{
    public class Utilities
    {
        public static bool IsPointerOverUiObject()
        {
            var eventDataCurrentPosition =
                new PointerEventData(EventSystem.current) {position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)};
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

    }
}