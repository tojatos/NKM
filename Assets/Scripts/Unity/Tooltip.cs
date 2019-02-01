using UnityEngine;
using UnityEngine.UI;

namespace Unity
{
	public class Tooltip : SingletonMonoBehaviour<Tooltip>
	{
		public Text Text;
		public bool IsActive;
		private Vector3? _position;
		public static Vector3 CharacterPosition => new Vector3(-400, -350);


		public void Init()
		{
			Text.text = "";
			IsActive = false;
		}
		public void Set(string text, Vector3? position = null)
		{
			Text.text = text;
			IsActive = true;
			_position = position;
		}
		public void Remove()
		{
			Text.text = "";
			IsActive = false;
			_position = null;
		}
		private void LateUpdate()
		{
			if (!IsActive) return;

			Rect rectt = gameObject.GetComponent<RectTransform>().rect;
			var vlg = gameObject.GetComponent<VerticalLayoutGroup>();
			if (_position != null)
			{
				var pos = (Vector3) _position;
				gameObject.transform.localPosition = pos;
				vlg.childControlWidth = false;
				gameObject.transform.GetChild(0).GetComponent<RectTransform>().
					SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 475);
				gameObject.transform.position += new Vector3(0, rectt.height/2);
			}
			else
			{
				gameObject.transform.position = Input.mousePosition + new Vector3(0, rectt.height/2+5, 0);
				vlg.childControlWidth = true;
			}
			if (Input.anyKey) IsActive = false;
		}
	}
}