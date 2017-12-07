//using System.Data;
//using Mono.Data.Sqlite;
//using UnityEngine;

//public static class GameData
//{
//	public static void DbTest()
//	{
//		string conn = "URI=file:" + Application.dataPath + "/database.db"; //Path to database.
//		var dbconn = (IDbConnection)new SqliteConnection(conn);
//		dbconn.Open(); //Open connection to the database.
//		var dbcmd = dbconn.CreateCommand();
//		var sqlQuery = "SELECT Name FROM Authors";
//		dbcmd.CommandText = sqlQuery;
//		var reader = dbcmd.ExecuteReader();
//		while (reader.Read())
//		{
//			var name = reader.GetString(0);
//			Debug.Log(name);
//		}
//		reader.Close();
//		dbcmd.Dispose();
//		dbconn.Close();
//	}

//	public static string GetAllAbilityDescriptions()
//	{
//		var text = "";
//		AllMyGameObjects.Instance.Characters.ForEach(c => c.Abilities.ForEach(a =>
//		{
//			text += a.Name + '\n';
//			text += a.GetDescription() + "\n\n";
//		}));
//		return text;
//	}
//}