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
//		private static Game Game => GameStarter.Instance.Game;
//		private static Active Active => Game.Active;

		public static IEnumerable<string> GetClassNames<T>(this List<T> list)
		{
			List<string> classNames = new List<string>();
			list.ForEach(l => classNames.Add(l.GetType().Name));
			return classNames;
		}

		/// <summary>
		/// Leaves only cells with enemy characters
		/// </summary>
//		public static void RemoveNonEnemies(this List<HexCell> cellRange)
//		{
//			cellRange.RemoveAll(cell => cell.CharacterOnCell == null || cell.CharacterOnCell.Owner == GameStarter.Instance.Game.Active.GamePlayer);
//		}
		/// <summary>
		/// Leaves only cells with friendly characters
		/// </summary>
//		public static void RemoveNonFriends(this List<HexCell> cellRange) //TODO:  there should be a Player parameter, to define who is a friend
//		{
//			cellRange.RemoveAll(cell => cell.CharacterOnCell == null || cell.CharacterOnCell.Owner != GameStarter.Instance.Game.Active.GamePlayer);
//		}
		/// <summary>
		/// Leaves only cells with characters
		/// </summary>
//		public static void RemoveNonCharacters(this List<HexCell> cellRange)
//		{
//			//var unavailableRange = new List<HexCell>(cellRange);
//			cellRange.RemoveAll(cell => cell.CharacterOnCell == null);
//			//cellRange.ForEach(c => unavailableRange.Remove(c));
//			//return;
//		}

		public static List<HexCell> WhereOnlyEnemiesOf(this List<HexCell> cells, GamePlayer player) => 
			cells.FindAll(c => c.CharacterOnCell!= null && c.CharacterOnCell.Owner != player);
		public static List<HexCell> WhereOnlyFriendsOf(this List<HexCell> cells, GamePlayer player) =>
			cells.FindAll(c => c.CharacterOnCell != null && c.CharacterOnCell.Owner == player);
		public static List<HexCell> WhereOnlyCharacters(this List<HexCell> cells) => cells.FindAll(c => c.CharacterOnCell != null);

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
			Stat stat;
			switch (type)
			{
				case StatType.HealthPoints:
					stat = character.HealthPoints;
					break;
				case StatType.AttackPoints:
					stat = character.AttackPoints;
					break;
				case StatType.BasicAttackRange:
					stat = character.BasicAttackRange;
					break;
				case StatType.Speed:
					stat = character.Speed;
					break;
				case StatType.PhysicalDefense:
					stat = character.PhysicalDefense;
					break;
				case StatType.MagicalDefense:
					stat = character.MagicalDefense;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}

			return stat;
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
		private static HexTileType GetSpawnPointType(this GamePlayer gamePlayer) => GameStarter.Instance.Game.HexMapDrawer.HexMap.SpawnPoints[gamePlayer.GetIndex()];
		public static IEnumerable<HexCell> GetSpawnPoints(this GamePlayer gamePlayer) => GameStarter.Instance.Game.HexMapDrawer.Cells.FindAll(c => c.Type == gamePlayer.GetSpawnPointType());
		public static List<Character> GetCharacters(this IEnumerable<HexCell> cells) => cells.Where(c => c.CharacterOnCell != null).Select(c => c.CharacterOnCell).ToList();

		public static bool ContainsType(this IEnumerable<object> objects, Type type) => objects.Any(o => o.GetType() == type);
		public static bool ContainsType<T>(this IEnumerable<object> objects) => objects.Any(o => o.GetType() == typeof(T));
		public static string FormattedFirstName(this Character character) => string.Format("<color={0}><</color><b>{1}</b><color={0}>></color>", ((Color32)character.Owner.GetColor()).ToHex(), character.Name.Split(' ').Last());
	}
}
