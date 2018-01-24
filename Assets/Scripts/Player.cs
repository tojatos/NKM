using System;
using System.Collections.Generic;
using MyGameObjects.MyGameObject_templates;
using Hex;
using UnityEngine;

public class Player
{
	private Game Game;

	public string Name { get; set; }
	public bool HasSelectedCharacters { get; set; }
	public List<Character> Characters { get;} = new List<Character>();
	//public List<Character> StartingCharacters { get; private set; }
	public bool HasFinishedSelecting => HasSelectedCharacters;

	public int GetIndex() => Game.Players.FindIndex(p=>p==this); //TODO move to players or make generic


	public void Init(Game game)
	{
		Game = game;
	}

	private HexTileType GetSpawnPointType()
	{
		return Game.HexMapDrawer.HexMap.SpawnPoints[GetIndex()];
	}
	public IEnumerable<HexCell> GetSpawnPoints()
	{
		return Game.HexMapDrawer.Cells.FindAll(c => c.Type == GetSpawnPointType());
	}
	public Color GetColor()
	{
		switch (GetIndex())
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