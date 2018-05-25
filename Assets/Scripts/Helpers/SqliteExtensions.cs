using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Helpers
{
	public static class SqliteExtensions
	{
		private static List<SqliteRow> Select(this IDbConnection conn, string query)
		{
			List<SqliteRow> rows = new List<SqliteRow>();

			conn.Open();
			IDbCommand dbcmd = conn.CreateCommand();
			dbcmd.CommandText = query;
			IDataReader reader = dbcmd.ExecuteReader();
			while (reader.Read())
			{
				var row = new SqliteRow();
				var fieldCount = reader.FieldCount;
				for (var i = 0; i < fieldCount; i++)
				{
					var columnName = reader.GetName(i);
					var value = reader.GetValue(i).ToString();
					row.Add(columnName, value);
				}

				rows.Add(row);

			}

			reader.Close();
			dbcmd.Dispose();
			conn.Close();
			return rows;
		}
		public static List<string> GetCharacterNames(this IDbConnection conn) => Select(conn, "SELECT Name FROM Character").SelectMany(row => row.Data.Values).ToList();
		public static IEnumerable<string> GetAbilityClassNames(this IDbConnection conn, string characterName) => Select(conn, $"SELECT Ability.ClassName AS AbilityName FROM Character INNER JOIN Character_Ability ON Character.ID = Character_Ability.CharacterID INNER JOIN Ability ON Ability.ID = Character_Ability.AbilityID WHERE Character.Name = '{characterName}';").SelectMany(row => row.Data.Values).ToList();
		public static SqliteRow GetCharacterData(this IDbConnection conn, string characterName) => Select(conn, $"SELECT AttackPoints, HealthPoints, BasicAttackRange, Speed, PhysicalDefense, MagicalDefense, FightType, Description, Quote, Author.Name FROM Character INNER JOIN Author ON Character.AuthorID = Author.ID WHERE Character.Name = '{characterName}';")[0];
	}

	public class SqliteRow
	{
		public readonly Dictionary<string, string> Data = new Dictionary<string, string>();

		public void Add(string columnName, string value) => Data.Add(columnName, value);
		public string GetValue(string columnName) => Data.FirstOrDefault(data => data.Key == columnName).Value;
	}
}