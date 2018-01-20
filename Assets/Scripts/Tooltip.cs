using UnityEngine;
using UnityEngine.UI;

public class Tooltip : SingletonMonoBehaviour<Tooltip>
{
	public Text Text;
	public bool IsActive;

	private void Awake()
	{
		Text.text = "";
		IsActive = false;
	}

	public void Set(string text)
	{
		Text.text = text;
		IsActive = true;
	}

	public void Remove()
	{
		Text.text = "";
		IsActive = false;
	}

	private void LateUpdate()
	{
		if (!IsActive) return;

		var rectt = gameObject.GetComponent<RectTransform>().rect;
		gameObject.transform.position = Input.mousePosition + new Vector3(0, rectt.height/2+5, 0);
		if (Input.anyKey) IsActive = false;
	}
}