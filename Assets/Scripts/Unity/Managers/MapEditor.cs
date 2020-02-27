using System;
using System.Collections.Generic;
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

        private static SessionSettings S => SessionSettings.Instance;
        private HexCell.TileType _currentBrushType = HexCell.TileType.Transparent;
        private HexMap _map;

        private void Awake()
        {
            CreateMap();
            InitBrushSelectionPanel();
            SaveButton.AddTrigger(EventTriggerType.PointerClick, () =>
            {

            });
        }

        private void InitBrushSelectionPanel()
        {
            foreach (HexCell.TileType t in SystemGeneric.EnumValues<HexCell.TileType>())
            {
                GameObject go = Instantiate( Stuff.Prefabs.Single( g => g.name == "Brush Select"), BrushSelectionPanel.transform);
                go.GetComponent<RawImage>().color = HexMapDrawer.FromHexType(t);
                go.AddTrigger(EventTriggerType.PointerClick, () => _currentBrushType = t);
            }
        }

        private HexMap MapFromDimensions(int x, int y)
        {
            HexMap map = new HexMap("temp", new List<HexCell>(), new List<HexCell.TileType>());

            for (int i = 0; i < y; ++i)
            {
                for (int j = 0; j < x; ++j)
                {
                    map.Cells.Add(new HexCell(_map, new HexCoordinates(j, -x-i), HexCell.TileType.Transparent));
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
                HexMapDrawer.Instance.RepaintCell(HexMapDrawer.Instance.SelectDrawnCell(cell));
                HexMapDrawer.Instance.TriangulateCells();
            };
            MainCameraController.Instance.Init();
        }
    }
}