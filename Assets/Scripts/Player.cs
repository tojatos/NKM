using System;
using System.Collections.Generic;
using MyGameObjects.MyGameObject_templates;
using Hex;
using Managers;
using UnityEngine;

public class Player
{
	public string Name { get; set; }
	public bool HasSelectedCharacters { get; set; }
	public List<Character> Characters { get; }
	//public List<Character> StartingCharacters { get; private set; }
	public bool HasFinishedSelecting => HasSelectedCharacters;

	public Player()
	{
		HasSelectedCharacters = false;
		Characters = new List<Character>();
	}

	private HexTileType GetSpawnPointType()
	{
		return HexMapDrawer.Instance.HexMap.SpawnPoints[GameManager.GetIndex(this)];
	}
	public IEnumerable<HexCell> GetSpawnPoints()
	{
		return HexMapDrawer.Cells.FindAll(c => c.Type == GetSpawnPointType());
	}
	public Color GetColor()
	{
		switch (GameManager.GetIndex(this))
		{
			case 0:
				return Color.red;
			case 1:
				return Color.green;
			case 2:
				return Color.blue;
			case 3:
				return Color.cyan;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
}