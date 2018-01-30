using System;
using Hex;
using JetBrains.Annotations;
using UnityEngine;
[Serializable]
public class ColorToTileType
{
	[UsedImplicitly] public Color Color;
	[UsedImplicitly] public HexTileType HexTileType;
}