using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using Managers;
using NKMObjects.Templates;
using UnityEngine;
//using NKMObject = NKMObjects.Templates.NKMObject;

public class Spawner : SingletonMonoBehaviour<Spawner>
{
	public GameObject CharacterPrefab;
	public GameObject HighlightPrefab;
	private static Game Game => GameStarter.Instance.Game;
	private void SpawnCharacterObject(DrawnHexCell parentCell, Character characterToSpawn)
	{
		Sprite characterSprite = Stuff.Sprites.CharacterHexagons.SingleOrDefault(s => s.name == characterToSpawn.Name) ?? Stuff.Sprites.CharacterHexagons.Single(s => s.name == "Empty");
		GameObject characterObject = Instantiate(CharacterPrefab, parentCell.transform);
		characterObject.name = characterToSpawn.Name;
		characterObject.transform.Find("Character Sprite").GetComponent<SpriteRenderer>().sprite = characterSprite;
		characterObject.transform.Find("Border").GetComponent<SpriteRenderer>().color = characterToSpawn.Owner.GetColor();
		characterObject.transform.localPosition = new Vector3(0, 10, 0);
		//GameStarter.Instance.Game.HexMap.Place(characterToSpawn, parentCell.HexCell);
		HexMapDrawer.Instance.SetCharacterObject(characterToSpawn, characterObject);
	}
	public void SpawnHighlightCellObject(DrawnHexCell parentCell, string colorName)
	{
		GameObject highlightObject = Instantiate(HighlightPrefab, parentCell.transform);
		highlightObject.transform.localPosition = new Vector3(0, 11, 0);
		highlightObject.GetComponent<SpriteRenderer>().sprite = Stuff.Sprites.HighlightHexagons.Single(s => s.name == colorName); 
		parentCell.Highlights.Add(highlightObject);
	}
	public void SpawnEffectHighlightCellObject(DrawnHexCell parentCell, string effectName)
	{
		GameObject highlightObject = Instantiate(HighlightPrefab, parentCell.transform);
		highlightObject.transform.localPosition = new Vector3(0, 1, 0);
		var sr = highlightObject.GetComponent<SpriteRenderer>();
		sr.sprite = Stuff.Sprites.HighlightHexagons.Single(s => s.name == effectName); 
		parentCell.EffectHighlights.Add(highlightObject);
	}


	private static T Create<T>(string namespaceName, string className) where T : class 
	{
		string typeName = "NKMObjects." + namespaceName + "." + className;
		Type type = Type.GetType(typeName);
		if (type == null) throw new NullReferenceException();

		return Activator.CreateInstance(type, Game) as T;
	}

	public static IEnumerable<T> Create<T>(string namespaceName, IEnumerable<string> classNames) where T : class
	{
		return classNames.Select(className => Create<T>(namespaceName, className)).ToList();
	}

	public static bool CanSpawn(Character character, HexCell cell) => cell.IsFreeToStand && cell.IsSpawnFor(character.Owner);
	public void Spawn(DrawnHexCell cell, Character characterToSpawn)
	{
		SpawnCharacterObject(cell, characterToSpawn);
	}
}