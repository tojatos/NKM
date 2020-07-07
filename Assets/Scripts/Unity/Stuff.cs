using System.Collections.Generic;
using System.IO;
using System.Linq;
using NKMCore.Hex;
using Unity.Managers;
using UnityEngine;

namespace Unity
{
    public static class Stuff
    {
        private static List<HexMap> _maps;
        public static List<HexMap> Maps
        {
            get
            {
                //lazy loading because paths are null before Unity's awake
                if(_maps == null) ReloadMaps();

                return _maps;
            }
        }

        private static void DownloadHexMapsFromJar(string targetPath)
        {
            //TODO: something overrides
            // //TODO: do not hardcode
            // var mapNames = new[] {"1v1v1", "Arena", "Linia", "Map1", "Shuriken", "TestMap"};
            // foreach (string mapName in mapNames)
            // {
            //     //TODO: async wait for all requests
            //     string url = $"jar:file://{Application.dataPath}!/assets/HexMaps/{mapName}.hexmap";
            //     UnityWebRequest dbDownloadRequest = UnityWebRequest.Get(url);
            //     dbDownloadRequest.SendWebRequest();
            //     while (dbDownloadRequest.isDone)
            //     {
            //         if (dbDownloadRequest.isHttpError || dbDownloadRequest.isNetworkError) throw new Exception($"Error while downloading {mapName} from jar");
            //         Thread.Sleep(100);
            //     }
            //     File.WriteAllBytes(Path.Combine(targetPath, $"{mapName}.hexmap"), dbDownloadRequest.downloadHandler.data);
            // }
        }

        private static void ReloadMaps()
        {
            var hexMapDirs = GetHexMapDirs();
            var hexMapFiles = hexMapDirs.SelectMany(dir => new DirectoryInfo(dir).GetFiles("*.hexmap"));
            var hexMapFileContents = hexMapFiles.Select(file => File.ReadAllText(file.FullName));
            hexMapFileContents = hexMapFileContents.Select(c => c.Replace("\r\n", "\n"));
            _maps = hexMapFileContents.Select(HexMapSerializer.Deserialize).ToList();
        }

        private static IEnumerable<string> GetHexMapDirs()
        {
            if (Directory.Exists(PathManager.HexMapsDirPath))
                return new[] {PathManager.HexMapsDirPath, PathManager.UserHexMapsDirPath};

            DownloadHexMapsFromJar(PathManager.AndroidHexMapsDirPath);
            return new[] {PathManager.AndroidHexMapsDirPath, PathManager.UserHexMapsDirPath};
        }

        public static readonly List<GameObject> Particles;
        public static readonly AllSprites Sprites;
        public static readonly List<GameObject> Prefabs;

        static Stuff()
        {
            Particles = new List<GameObject>(Resources.LoadAll<GameObject>("Particles"));
            Prefabs = new List<GameObject>(Resources.LoadAll<GameObject>("Blender"));
            Prefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs"));
            Sprites = new AllSprites();
        }
    }
    public class AllSprites
    {
        public readonly List<Sprite> CharacterHexagons;
        public readonly List<Sprite> HighlightHexagons;
        public readonly List<Sprite> Abilities;
        public readonly List<Sprite> Effects;
        public readonly List<Sprite> Icons;

        public AllSprites()
        {
            CharacterHexagons = new List<Sprite>(Resources.LoadAll<Sprite>("Sprites/CharacterHexagons"));
            HighlightHexagons = new List<Sprite>(Resources.LoadAll<Sprite>("Sprites/HighlightHexagons"));
            Abilities = new List<Sprite>(Resources.LoadAll<Sprite>("Sprites/Abilities"));
            Effects = new List<Sprite>(Resources.LoadAll<Sprite>("Sprites/Effects"));
            Icons = new List<Sprite>(Resources.LoadAll<Sprite>("Sprites/Icons"));
        }
    }
}