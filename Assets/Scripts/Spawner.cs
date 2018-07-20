using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Templates;
using UnityEngine;
using NKMObject = NKMObjects.Templates.NKMObject;

public class Spawner : SingletonMonoBehaviour<Spawner>
{
	public GameObject CharacterPrefab;
	public GameObject HighlightPrefab;
	private void SpawnCharacterObject(HexCell parentCell, Character characterToSpawn)
	{
		Sprite characterSprite = Stuff.Sprites.CharacterHexagons.SingleOrDefault(s => s.name == characterToSpawn.Name) ?? Stuff.Sprites.CharacterHexagons.Single(s => s.name == "Empty");
		GameObject characterObject = Instantiate(CharacterPrefab, parentCell.transform);
		characterObject.name = characterToSpawn.Name;
		characterObject.transform.Find("Character Sprite").GetComponent<SpriteRenderer>().sprite = characterSprite;
		characterObject.transform.Find("Border").GetComponent<SpriteRenderer>().color = characterToSpawn.Owner.GetColor();
		characterObject.transform.localPosition = new Vector3(0, 10, 0);
		parentCell.CharacterOnCell = characterToSpawn;
		characterToSpawn.ParentCell = parentCell;
		characterToSpawn.CharacterObject = characterObject;
		characterToSpawn.IsOnMap = true;
	}
	public void SpawnHighlightCellObject(HexCell parentCell, string colorName)
	{
		GameObject highlightObject = Instantiate(HighlightPrefab, parentCell.transform);
		highlightObject.transform.localPosition = new Vector3(0, 11, 0);
		highlightObject.GetComponent<SpriteRenderer>().sprite = Stuff.Sprites.HighlightHexagons.Single(s => s.name == colorName); 
		parentCell.Highlights.Add(highlightObject);
	}
	public void SpawnEffectHighlightCellObject(HexCell parentCell, string effectName)
	{
		GameObject highlightObject = Instantiate(HighlightPrefab, parentCell.transform);
		highlightObject.transform.localPosition = new Vector3(0, 1, 0);
		var sr = highlightObject.GetComponent<SpriteRenderer>();
		sr.sprite = Stuff.Sprites.HighlightHexagons.Single(s => s.name == effectName); 
		parentCell.EffectHighlights.Add(highlightObject);
	}


	private static NKMObject Create(string namespaceName, string className)
	{
		string typeName = "NKMObjects." + namespaceName + "." + className;
		Type type = Type.GetType(typeName);
		if (type == null) throw new NullReferenceException();

		var createdMyGameObject = Activator.CreateInstance(type) as NKMObject;
		return createdMyGameObject;
	}

	public static IEnumerable<NKMObject> Create(string namespaceName, IEnumerable<string> classNames)
	{
		return classNames.Select(className => Create(namespaceName, className)).ToList();
	}

	public static bool CanSpawn(Character character, HexCell cell) => cell.IsFreeToStand && cell.IsSpawnFor(character.Owner);
	public void Spawn(HexCell cell, Character characterToSpawn) => SpawnCharacterObject(cell, characterToSpawn);
}