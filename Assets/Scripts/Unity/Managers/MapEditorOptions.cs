using System.Linq;
using Unity.Extensions;
using UnityEngine;
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
        private static SessionSettings S => SessionSettings.Instance;

        private void Start()
        {
            CreateNew.onClick.AddListener(() =>
            {
                if (NewMapDimensionX.text == "" || NewMapDimensionY.text == "") {
                    Popup.Create(MainWindowHandle).Show("Błąd", "Podaj wymiary mapy");
                    return;
                }
                int x, y;
                try
                {
                    x = int.Parse(NewMapDimensionX.text);
                    y = int.Parse(NewMapDimensionY.text);
                    if (x < 1 || y < 1)
                    {
                        Popup.Create(MainWindowHandle).Show("Błąd", "Wymiary powinny być większe od 0");
                        return;
                    }
                }
                catch
                {
                    Popup.Create(MainWindowHandle).Show("Błąd", "Wymiary powinny być liczbą całkowitą większą od 0");
                    return;
                }

                S.NewMapDimX = x;
                S.NewMapDimY = x;
                S.SelectedMapCreationType = MapCreationTypes.New;
                SceneManager.LoadScene(Scenes.MapEditor);
            });
            CreateFromOther.onClick.AddListener(() =>
            {
                S.SelectedMapCreationType = MapCreationTypes.FromOld;
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