using System;
using System.IO;
using System.Linq;
using Unity.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Unity.Managers
{
    public class MapEditorOptions : SingletonMonoBehaviour<MapEditorOptions>
    {
        public InputField NewMapDimensionX;
        public InputField NewMapDimensionY;
        // public Dropdown MapSelectDropdown;
        public GameObject MapSelectContainer;
        public Button CreateNew;
        public Button CreateFromOther;
        public Transform MainWindowHandle;

        private void Start()
        {
            CreateNew.onClick.AddListener(() =>
            {
                if (NewMapDimensionX.text == "" || NewMapDimensionY.text == "") return; // TODO: show popup
                int x, y;
                try
                {
                    x = int.Parse(NewMapDimensionX.text);
                    y = int.Parse(NewMapDimensionY.text);
                    //TODO
                    SceneManager.LoadScene(Scenes.MapEditor);
                }
                catch
                {
                    // TODO: show popup
                }
            });
            CreateFromOther.onClick.AddListener(() =>
            {
                //TODO
                SceneManager.LoadScene(Scenes.MapEditor);
            });

            var mapSelectSettings = new SingleDropdownSettings
            {
                Type = SettingType.SelectedMapToEditIndex,
                Options = Stuff.Maps.Select(map => map.Name).ToArray()
            };
            var dropdown = MapSelectContainer.AddSingleDropdown(mapSelectSettings);
            dropdown.transform.SetSiblingIndex(1);
        }
    }
}