using System;
using System.Data;
//using System.Data.SQLite;
using Mono.Data.Sqlite;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
//#if UNITY_EDITOR
//using UnityEditor;
//#endif

//[InitializeOnLoad]
public static class GameData
{
//	[DllImport("SQLite.Interop")]
	private static string conn = "Data source=" + Application.streamingAssetsPath + "/database.db;";
	public static IDbConnection Conn = new SqliteConnection(conn);
//	static GameData() // static Constructor
//	{
//		var currentPath = Environment.GetEnvironmentVariable("PATH",
//			EnvironmentVariableTarget.Process);
//#if UNITY_EDITOR_32
//    var dllPath = Application.dataPath
//        + Path.DirectorySeparatorChar + "Plugins"
//        + Path.DirectorySeparatorChar + "x86";
//#elif UNITY_EDITOR_64
//		var dllPath = Application.dataPath
//		              + Path.DirectorySeparatorChar + "Plugins"
//		              + Path.DirectorySeparatorChar + "x86_64";
//#else // Player
//    var dllPath = Application.dataPath
//        + Path.DirectorySeparatorChar + "Plugins";
//
//#endif
//		if (currentPath != null && currentPath.Contains(dllPath) == false)
//			Environment.SetEnvironmentVariable("PATH", currentPath + Path.PathSeparator
//			                                           + dllPath, EnvironmentVariableTarget.Process);
//	}
}