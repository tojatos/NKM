using System.Data;
//using System.Data.SQLite;
using Mono.Data.Sqlite;
using UnityEngine;

public static class GameData
{
	private static readonly string Path = "Data source=" + Application.streamingAssetsPath + "/database.db;";
	public static readonly IDbConnection Conn = new SqliteConnection(Path);

}