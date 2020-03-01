using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Unity.Managers
{
    public class PathManager : CreatableSingletonMonoBehaviour<PathManager>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void DDOL() => DontDestroyOnLoad(Instance);

        public static string DbPath;
        public static string AndroidDbPath;
        public static string SettingsDirPath;
        public static string HexMapsDirPath;
        public static string AndroidHexMapsDirPath;
        public static string UserHexMapsDirPath;
        public static string TestingCharactersFilePath;
        public static string ServerListFilePath;
        public static string LogDirPath;

        private static string MakePath(params string[] pathComponents) =>
            string.Join(Path.DirectorySeparatorChar.ToString(), pathComponents);

        public static string GetLogFilePath()
        {
            string[] pathSegments =
            {
               LogDirPath,
               DateTime.Now.ToString("yyyy-MM"),
               DateTime.Now.ToString("dd"),
               DateTime.Now.ToString("HH.mm.ss") + ".txt",
            };
            return MakePath(pathSegments);
        }

        private void Awake()
        {
            DbPath = MakePath(Application.streamingAssetsPath, "database.db");
            AndroidDbPath = MakePath(Application.persistentDataPath, "database.db");
            SettingsDirPath = MakePath(Application.persistentDataPath, "Settings");
            HexMapsDirPath = MakePath(Application.streamingAssetsPath, "HexMaps");
            AndroidHexMapsDirPath = MakePath(Application.persistentDataPath, "HexMaps");
            UserHexMapsDirPath = MakePath(Application.persistentDataPath, "HexMaps");
            TestingCharactersFilePath = MakePath(Application.dataPath, "testing_characters.txt");
            ServerListFilePath = MakePath(SettingsDirPath, "server_list.txt");
            LogDirPath = MakePath(Application.persistentDataPath, "game_logs");

            new List<string> {SettingsDirPath, UserHexMapsDirPath}.ForEach(s => Directory.CreateDirectory(s));
        }
    }
}