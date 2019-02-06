﻿using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
//using System.Data.SQLite;

namespace NKMCore
{
	public static class GameData
	{
		private static readonly string Path = "Data source=" + Application.streamingAssetsPath + "/database.db;";
		public static readonly IDbConnection Conn = new SqliteConnection(Path);

	}
}