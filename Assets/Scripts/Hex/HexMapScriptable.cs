using System.Collections.Generic;
using UnityEngine;

namespace Hex
{
	[CreateAssetMenu(fileName = "New map", menuName = "Map")]
	public class HexMapScriptable : ScriptableObject
	{
		public string Name = "New map";
		public Texture2D Map;
		public int MaxCharacters;
		public List<HexTileType> SpawnPoints;
		public ColorToTileType[] ColorMappings;
		public int MaxPlayers => SpawnPoints.Count;
	}
}