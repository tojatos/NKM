using System;
using JetBrains.Annotations;
using NKMCore.Hex;
using UnityEngine;

namespace Unity
{
    [Serializable]
    public class ColorToTileType
    {
        [UsedImplicitly] public Color Color;
        [UsedImplicitly] public HexCell.TileType TileType;
    }
}