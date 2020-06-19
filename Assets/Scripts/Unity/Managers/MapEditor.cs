using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NKMCore.Extensions;
using NKMCore.Hex;
using Unity.Extensions;
using Unity.Hex;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unity.Managers
{
    public class MapEditor : SingletonMonoBehaviour<MapEditor>
    {
        public GameObject BrushSelectionPanel;
        public GameObject SaveButton;
        public Transform MainWindowHandle;

        private static SessionSettings S => SessionSettings.Instance;
        private HexCell.TileType _currentBrushType = HexCell.TileType.Transparent;
        private int? _currentSpawnNumber;
        private HexMap _map;

        private void ShowErrorPopup(string message) => Popup.Create(MainWindowHandle).Show("Błąd", message);

        private void Awake()
        {
            CreateMap();
            InitBrushSelectionPanel();
            SaveButton.AddTrigger(EventTriggerType.PointerClick, () =>
            {
                var popup = InputPopup.Create(MainWindowHandle);
                popup.Show("Podaj nazwę swojej mapy", () =>
                {
                    string mapName = popup.Input.text.Trim();
                    if (mapName == "")
                    {
                        ShowErrorPopup("Nazwa nie może być pusta!");
                        return;
                    }

                    if (!mapName.IsValidFilename())
                    {
                        ShowErrorPopup("Nazwa zawiera nieprawidłowe znaki");
                        return;
                    }

                    if (Stuff.Maps.Select(m => m.Name).Contains(mapName))
                    {
                        ShowErrorPopup("Taka mapa już istnieje!");
                        return;
                    }
                    var mapToSave = new HexMap(mapName, _map.Cells);
                    string savePath = Path.Combine(PathManager.UserHexMapsDirPath, $"{mapName}.hexmap");
                    File.WriteAllText(savePath, HexMapSerializer.Serialize(mapToSave));
                    popup.ClosePopup();
                    Popup.Create(MainWindowHandle).Show("Sukces", "Pomyślnie zapisano mapę");
                });

            });
        }

        private void InitBrushSelectionPanel()
        {
            foreach (HexCell.TileType t in SystemGeneric.EnumValues<HexCell.TileType>())
            {
                if (t == HexCell.TileType.SpawnPoint)
                    for (int i = 0; i < 4; ++i) //TODO: a better way to create spawn brushes
                        CreateBrush(t, i);
                else CreateBrush(t);
            }
        }

        private void CreateBrush(HexCell.TileType t, int? spawnNumber = null)
        {
            GameObject go = Instantiate( Stuff.Prefabs.Single( g => g.name == "Brush Select"), BrushSelectionPanel.transform);
            go.GetComponent<RawImage>().color = HexMapDrawer.FromHexType(t);
            go.AddTrigger(EventTriggerType.PointerClick, () =>
            {
                _currentBrushType = t;
                _currentSpawnNumber = spawnNumber;
            });
        }

        private HexMap MapFromDimensions(int x, int y)
        {
            var map = new HexMap("temp", new List<HexCell>());

            for (int i = 0; i < y; ++i)
            {
                for (int j = 0; j < x; ++j)
                {
                    map.Cells.Add(new HexCell(_map, new HexCoordinates(j, -x-i), HexCell.TileType.Transparent, null));
                }
            }

            map.Cells.ForEach(c =>
            {
                map.Cells.FindAll(w =>
                        Math.Abs(w.Coordinates.X - c.Coordinates.X) <= 1 &&
                        Math.Abs(w.Coordinates.Y - c.Coordinates.Y) <= 1 &&
                        Math.Abs(w.Coordinates.Z - c.Coordinates.Z) <= 1 &&
                        w != c)
                    .ForEach(w => c.SetNeighbor(c.GetDirection(w), w));
            });

            // map.SpawnPoints.AddRange(new []{HexCell.TileType.SpawnPoint1, HexCell.TileType.SpawnPoint2});

            // Debug.LogWarning(HexMapSerializer.Serialize(map));

            return map;
        }

        private void CreateMap()
        {
            switch (S.SelectedMapCreationType)
            {
                case MapCreationTypes.New:
                {
                    int x = S.NewMapDimX;
                    int y = S.NewMapDimY;
                    _map = MapFromDimensions(x, y);
                    HexMapDrawer.Instance.CreateMap(_map);
                } break;
                case MapCreationTypes.FromOld:
                {
                    _map = Stuff.Maps[S.GetDropdownSetting(SettingType.SelectedMapToEditIndex)].Clone();
                    HexMapDrawer.Instance.CreateMap(_map);
                } break;
                default:
                {
                    Debug.LogError("Selected Map Creation Type not found");
                    //TODO: handle exception
                    return;
                }
            }

            HexMapDrawer.Instance.AfterCellDrag += cell =>
            {
                cell.Type = _currentBrushType;
                cell.SpawnNumber = _currentSpawnNumber;
                HexMapDrawer.Instance.RepaintCell(HexMapDrawer.Instance.SelectDrawnCell(cell));
                HexMapDrawer.Instance.TriangulateCells();
            };
            MainCameraController.Instance.Init();
        }
    }
}