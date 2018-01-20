using System.Collections.Generic;
using System.Linq;
using Hex;
using MyGameObjects.MyGameObject_templates;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSelect : SingletonMonoBehaviour<SpriteSelect>
{
	private Active Active;
	private List<MyGameObject> _objectsToFill;
	public List<MyGameObject> SelectedObjects { get; private set; }
	public Button FinishSelectingButton;
	public Text Title;

	private void Awake()
	{
		Active = Active.Instance;
		_objectsToFill = new List<MyGameObject>();
		SelectedObjects = new List<MyGameObject>();
	}
	public GameObject SpriteObjectPrefab;
	public delegate void FinishSelectingButtonClick();

	public void Open(IEnumerable<MyGameObject> objectsToFill, FinishSelectingButtonClick finishSelectingButtonClick, string title, string finishButtonText)
	{
		Active.Instance.UI = new List<GameObject> { gameObject };
		SelectedObjects.Clear();
		gameObject.transform.Find("Sprites").transform.Clear(); //Careful! Removes probably on the next frame
		_objectsToFill = new List<MyGameObject>(objectsToFill);
		_objectsToFill.ForEach(SpawnSpriteObject);
		FinishSelectingButton.onClick.RemoveAllListeners();
		FinishSelectingButton.onClick.AddListener(() => finishSelectingButtonClick());

		Title.text = title;
		FinishSelectingButton.GetComponentInChildren<Text>().text = finishButtonText;

		//select item if is the only one
		var spritesTransform = gameObject.transform.Find("Sprites").transform;
		if (_objectsToFill.Count == 1) spritesTransform.GetComponentsInChildren<Button>()[spritesTransform.childCount-1].onClick.Invoke(); //get last button, because the others are not removed yet for some reason
	}

	public void FinishSelectingCharacters()
	{
		var charactersPerPlayer = PlayerPrefs.GetInt("NumberOfCharactersPerPlayer", HexMapDrawer.Instance.HexMap.MaxCharacters);
		if (SelectedObjects.Count != charactersPerPlayer) return;

		var classNames = SelectedObjects.GetClassNames();
		Active.Player.Characters.AddRange(Spawner.Create("Characters", classNames).Cast<Character>());
		Active.Player.HasSelectedCharacters = true;
	}

	public void FinishUseCharacter()
	{
		if (SelectedObjects.Count != 1) return;

		HexMapDrawer.RemoveAllHighlights();
		Active.Player.GetSpawnPoints().Where(sp => sp.CharacterOnCell == null).ToList().ForEach(c => c.ToggleHighlight(HiglightColor.Red));
		Active.MyGameObject = Active.Player.Characters.Single(c => c.Name == SelectedObjects[0].Name);
		Active.UI = null;
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

}