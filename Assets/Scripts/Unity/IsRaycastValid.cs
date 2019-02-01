using UnityEngine;

namespace Unity
{
	public class IsRaycastValid : MonoBehaviour, ICanvasRaycastFilter
	{
		public bool IsValid;
		public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
		{
			return IsValid;
		}
	}
}