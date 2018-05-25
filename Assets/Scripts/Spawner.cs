using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using Hex;
using MyGameObjects.MyGameObject_templates;
using UnityEngine;

public class Spawner : SingletonMonoBehaviour<Spawner>
{
	public GameObject CharacterPrefab;
	public ColorToHighlight[] HighlightColorMappings;

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
	public void SpawnHighlightCellObject(HexCell parentCell, HiglightColor highlightColor)
	{
		foreach (ColorToHighlight highlightColorMapping in HighlightColorMappings)
		{
			if (highlightColorMapping.HiglightColor != highlightColor) continue;

			GameObject highlightObject = Instantiate(highlightColorMapping.HiglightPrefab, parentCell.transform);
			highlightObject.transform.localPosition = new Vector3(0, 11, 0);
			parentCell.Highlight = highlightObject;
		}
	}
	public void SpawnHelpHighlightCellObject(HexCell parentCell, HiglightColor highlightColor)
	{
		foreach (ColorToHighlight highlightColorMapping in HighlightColorMappings)
		{
			if (highlightColorMapping.HiglightColor != highlightColor) continue;

			GameObject highlightObject = Instantiate(highlightColorMapping.HiglightPrefab, parentCell.transform);
			highlightObject.transform.localPosition = new Vector3(0, 12, 0);
			parentCell.HelpHighlight = highlightObject;
		}
	}

	private static MyGameObject Create(string namespaceName, string className)
	{
		var typeName = "MyGameObjects." + namespaceName + "." + className;
		Type type = Type.GetType(typeName);
		if (type == null) 	throw new ArgumentNullException();

		var createdMyGameObject = Activator.CreateInstance(type) as MyGameObject;
		return createdMyGameObject;
	}

	public static IEnumerable<MyGameObject> Create(string namespaceName, IEnumerable<string> classNames)
	{
		return classNames.Select(className => Create(namespaceName, className)).ToList();
	}
	public void TrySpawning(HexCell cell, Character characterToSpawn)
	{
		HexTileType playerSpawnpointType = HexMapDrawer.Instance.HexMap.SpawnPoints[characterToSpawn.Owner.GetIndex()];
		if (cell.Type != playerSpawnpointType)
		{
			throw new Exception("To nie twój spawn!");
		}
		if (cell.CharacterOnCell != null)
		{
			throw new Exception("Tu już stoi postać zwana " + cell.CharacterOnCell.Name + "!");
		}

		SpawnCharacterObject(cell, characterToSpawn);
	}
}