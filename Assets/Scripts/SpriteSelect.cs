using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hex;
using Managers;
using MyGameObjects.MyGameObject_templates;
using UIManagers;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSelect : SingletonMonoBehaviour<SpriteSelect>
{
	private List<MyGameObject> _objectsToFill = new List<MyGameObject>();
	public List<MyGameObject> SelectedObjects { get; } = new List<MyGameObject>();
	public Button FinishSelectingButton;
	public Text Title;
	public GameObject SpriteObjectPrefab;

	public bool IsOpened => gameObject.transform.parent.gameObject.activeSelf;

	public void Open(IEnumerable<MyGameObject> objectsToFill, System.Action finishSelectingButtonClick, string title, string finishButtonText)
	{
		gameObject.transform.parent.gameObject.Show();
		SelectedObjects.Clear();
		gameObject.transform.Find("Sprites").transform.Clear(); //Careful! Removes probably on the next frame
		_objectsToFill = new List<MyGameObject>(objectsToFill);
		_objectsToFill.ForEach(SpawnSpriteObject);
		FinishSelectingButton.onClick.RemoveAllListeners();
		FinishSelectingButton.onClick.AddListener(()=>finishSelectingButtonClick());

		Title.text = title;
		FinishSelectingButton.GetComponentInChildren<Text>().text = finishButtonText;

		//select item if is the only one
		var spritesTransform = gameObject.transform.Find("Sprites").transform;
		if (_objectsToFill.Count == 1) spritesTransform.GetComponentsInChildren<Button>()[spritesTransform.childCount-1].onClick.Invoke(); //get last button, because the others are not removed yet for some reason
	}
	private void SpawnSpriteObject(MyGameObject o)
	{
		var spriteObject = Instantiate(SpriteObjectPrefab, gameObject.transform.Find("Sprites").transform);
		var button = spriteObject.GetComponent<Button>();
		button.onClick.AddListener(delegate
		{
			var isSelected = ToggleSelected(o);
			button.image.color = isSelected ? Color.white : Color.grey;
		});
		button.image.color = Color.grey;
		button.image.sprite = Stuff.Sprites.CharacterHexagons.SingleOrDefault(c => c.name == o.Name);
	}
	/// <summary>
	/// Adds object to selected list if is not on it already,
	/// otherwise removes it from that list.
	/// </summary>
	/// <param name="o">Object to toggle</param>
	/// <returns>Is selected</returns>
	private bool ToggleSelected(MyGameObject o)
	{
		if (SelectedObjects.Contains(o))
		{
			SelectedObjects.Remove(o);
			return false;
		}

		SelectedObjects.Add(o);
		return true;
	}

	public void Close() => gameObject.transform.parent.gameObject.Hide();
}