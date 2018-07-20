using System.Collections.Generic;
using System.Linq;
using Extensions;
using UnityEngine;
using UnityEngine.UI;
using NKMObject = NKMObjects.Templates.NKMObject;

public class SpriteSelect : SingletonMonoBehaviour<SpriteSelect>
{
	private List<NKMObject> _objectsToFill = new List<NKMObject>();
	public List<NKMObject> SelectedObjects { get; } = new List<NKMObject>();
	public Button FinishSelectingButton;
	public Text Title;
	public GameObject SpriteObjectPrefab;
	public GameObject Sprites;

	public bool IsOpened => gameObject.transform.parent.gameObject.activeSelf;

	public void Open(IEnumerable<NKMObject> objectsToFill, System.Action finishSelectingButtonClick, string title, string finishButtonText)
	{
		gameObject.transform.parent.gameObject.Show();
		SelectedObjects.Clear();
		Sprites.transform.Clear(); //Careful! Removes probably on the next frame
		_objectsToFill = new List<NKMObject>(objectsToFill);
		_objectsToFill.ForEach(SpawnSpriteObject);
		FinishSelectingButton.onClick.RemoveAllListeners();
		FinishSelectingButton.onClick.AddListener(()=>finishSelectingButtonClick());

		Title.text = title;
		FinishSelectingButton.GetComponentInChildren<Text>().text = finishButtonText;

		//select item if is the only one
		Transform spritesTransform = Sprites.transform;
		if (_objectsToFill.Count == 1) spritesTransform.GetComponentsInChildren<Button>()[spritesTransform.childCount-1].onClick.Invoke(); //get last button, because the others are not removed yet for some reason
	}
	private void SpawnSpriteObject(NKMObject o)
	{
		GameObject spriteObject = Instantiate(SpriteObjectPrefab, Sprites.transform);
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
	private bool ToggleSelected(NKMObject o)
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