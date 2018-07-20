using System.Collections.Generic;
using Hex;
using UnityEngine;

public static class Stuff
{
	public static readonly List<HexMap> Maps;
	public static readonly List<GameObject> Particles;
	public static readonly AllSprites Sprites;
	public static readonly List<GameObject> Prefabs;

	static Stuff()
	{
		Maps = new List<HexMap>(Resources.LoadAll<HexMap>("Maps"));
		Particles = new List<GameObject>(Resources.LoadAll<GameObject>("Particles"));
		Prefabs = new List<GameObject>(Resources.LoadAll<GameObject>("Blender"));
		Prefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs"));
		Sprites = new AllSprites();
	}
}
public class AllSprites
{
	public readonly List<Sprite> CharacterHexagons;
	public readonly List<Sprite> HighlightHexagons;
	public readonly List<Sprite> Abilities;
	public readonly List<Sprite> Effects;
	public readonly List<Sprite> Icons;

	public AllSprites()
	{
		CharacterHexagons = new List<Sprite>(Resources.LoadAll<Sprite>("Sprites/CharacterHexagons"));
		HighlightHexagons = new List<Sprite>(Resources.LoadAll<Sprite>("Sprites/HighlightHexagons"));
		Abilities = new List<Sprite>(Resources.LoadAll<Sprite>("Sprites/Abilities"));
		Effects = new List<Sprite>(Resources.LoadAll<Sprite>("Sprites/Effects"));
		Icons = new List<Sprite>(Resources.LoadAll<Sprite>("Sprites/Icons"));
	}
}