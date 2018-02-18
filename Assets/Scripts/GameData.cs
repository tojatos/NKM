using System.Data.SQLite;
using UnityEngine;

public static class GameData
{
	private static string conn = "Data source=" + Application.dataPath + "/database.db;";
	public static SQLiteConnection Conn = new SQLiteConnection(conn);
}