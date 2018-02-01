using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using Hex;
using Managers;
using MyGameObjects.MyGameObject_templates;
using UnityEngine;

public class Spawner : SingletonMonoBehaviour<Spawner>
{
	public GameObject CharacterPrefab;
	public ColorToHighlight[] HighlightColorMappings;
	private Game Game;
	private void Awake()
	{
		Game = GameStarter.Instance.Game;
	}

	public void SpawnCharacterObject(HexCell parentCell, Character characterToSpawn)
	{
		var characterSprite = Stuff.Sprites.CharacterHexagons.SingleOrDefault(s => s.name == characterToSpawn.Name) ?? Stuff.Sprites.CharacterHexagons.Single(s => s.name == "Empty");
		var characterObject = Instantiate(CharacterPrefab, parentCell.transform);
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
		foreach (var highlightColorMapping in HighlightColorMappings)
		{
			if (highlightColorMapping.HiglightColor != highlightColor) continue;

			var highlightObject = Instantiate(highlightColorMapping.HiglightPrefab, parentCell.transform);
			highlightObject.transform.localPosition = new Vector3(0, 11, 0);
			parentCell.Highlight = highlightObject;
		}
	}
	public void SpawnHelpHighlightCellObject(HexCell parentCell, HiglightColor highlightColor)
	{
		foreach (var highlightColorMapping in HighlightColorMappings)
		{
			if (highlightColorMapping.HiglightColor != highlightColor) continue;

			var highlightObject = Instantiate(highlightColorMapping.HiglightPrefab, parentCell.transform);
			highlightObject.transform.localPosition = new Vector3(0, 12, 0);
			parentCell.HelpHighlight = highlightObject;
		}
	}

	public static MyGameObject Create(string namespaceName, string className)
	{
		var type = Type.GetType("MyGameObjects." + namespaceName + "." + className);
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
		var playerSpawnpointType = HexMapDrawer.Instance.HexMap.SpawnPoints[characterToSpawn.Owner.GetIndex()];
		if (cell.Type != playerSpawnpointType)
		{
			throw new Exception("To nie twój spawn!");
		}
		if (cell.CharacterOnCell != null)
		{
			throw new Exception("Tu już stoi postać zwana " + cell.CharacterOnCell.Name + "!");
		}

		SpawnCharacterObject(cell, characterToSpawn);
		if (Game.Type == GameType.MultiplayerServer) Game.Server.SendSpawnCharacterMessege(cell, characterToSpawn);

	}
}