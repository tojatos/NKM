using System;
using System.IO;
using System.Linq;
using NKMCore;
using NKMCore.Hex;
using Unity.Extensions;
using Unity.Hex;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Unity.Managers
{
    public class MapEditor : SingletonMonoBehaviour<MapEditor>
    {
        private static SessionSettings S => SessionSettings.Instance;

        private void Awake()
        {
            switch (S.SelectedMapCreationType)
            {
                case MapCreationTypes.New:
                {
                    int x = S.NewMapDimX;
                    int y = S.NewMapDimY;
                    // HexMapDrawer.Instance.CreateMap(); //TODO
                } break;
                case MapCreationTypes.FromOld:
                {
                    var oldMap = Stuff.Maps[S.GetDropdownSetting(SettingType.SelectedMapToEditIndex)].Clone();
                    HexMapDrawer.Instance.CreateMap(oldMap);
                    MainCameraController.Instance.Init();
                } break;
                default:
                {
                    //TODO: handle exception
                } break;
            }
        }
    }
}