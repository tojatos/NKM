using System;
using System.Collections.Generic;
using System.Linq;
using Hex;
using Managers;
using NKMObjects.Templates;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Extensions
{
	public static class NKMObject
	{
		public static List<HexCell> WhereEnemiesOf(this List<HexCell> cells, Character character) => cells.WhereEnemiesOf(character.Owner);
		public static List<HexCell> WhereEnemiesOf(this List<HexCell> cells, GamePlayer player) => 
			cells.FindAll(c => c.CharactersOnCell.Any(a => a.IsEnemyFor(player)));
		public static List<HexCell> WhereFriendsOf(this List<HexCell> cells, Character character) => cells.WhereFriendsOf(character.Owner);
		public static List<HexCell> WhereFriendsOf(this List<HexCell> cells, GamePlayer player) =>
			cells.FindAll(c => c.CharactersOnCell.Any(a => !a.IsEnemyFor(player)));
		public static List<HexCell> WhereCharacters(this List<HexCell> cells) => cells.FindAll(c => c.CharactersOnCell.Count > 0);

		private static string ToHex(this Color32 color) => $"#{color.r:X2}{color.g:X2}{color.b:X2}";
		public static void Clear(this Transform transform)
		{
			foreach (Transform child in transform)
			{
				Object.Destroy(child.gameObject);
			}
			//return transform;
		}
		public static Stat GetStat(this Character character, StatType type)
		{
			switch (type)
			{
				case StatType.HealthPoints:
					return character.HealthPoints;
				case StatType.AttackPoints:
					return character.AttackPoints;
				case StatType.BasicAttackRange:
					return character.BasicAttackRange;
				case StatType.Speed:
					return character.Speed;
				case StatType.PhysicalDefense:
					return character.PhysicalDefense;
				case StatType.MagicalDefense:
					return character.MagicalDefense;
				case StatType.Shield:
					return character.Shield;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}
		public static int GetIndex(this GamePlayer gamePlayer) => GameStarter.Instance.Game.Players.FindIndex(p => p == gamePlayer);
		public static Color GetColor(this GamePlayer gamePlayer)
		{
			switch (gamePlayer.GetIndex())
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
		public static FightType ToFightType(this string typeName)
		{
			switch (typeName)
			{
				case "Ranged":
					return FightType.Ranged;
				case "Melee":
					return FightType.Melee;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		private static HexTileType GetSpawnPointType(this GamePlayer gamePlayer, HexMap map) => map.SpawnPoints[gamePlayer.GetIndex()];
		public static List<HexCell> GetSpawnPoints(this GamePlayer gamePlayer, HexMap map) => map.Cells.FindAll(c => c.Type == gamePlayer.GetSpawnPointType(map));
		public static List<Character> GetCharacters(this IEnumerable<HexCell> cells) => cells.SelectMany(c => c.CharactersOnCell).ToList();

		public static bool ContainsType(this IEnumerable<object> objects, Type type) => objects.Any(o => o.GetType() == type);
		public static bool ContainsType<T>(this IEnumerable<object> objects) => objects.Any(o => o.GetType() == typeof(T));
		public static string FormattedFirstName(this Character character) => string.Format("<color={0}><</color><b>{1}</b><color={0}>></color>", ((Color32)character.Owner.GetColor()).ToHex(), character.Name.Split(' ').Last());
		public static string FirstName(this Character character) => character.Name.Split(' ').Last();
	}
}
