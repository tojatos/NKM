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

        private static SessionSettings S => SessionSettings.Instance;
        private HexCell.TileType _currentBrushType = HexCell.TileType.Transparent;

        private void Awake()
        {
            CreateMap();
            InitBrushSelectionPanel();
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

        private void CreateMap()
        {
            switch (S.SelectedMapCreationType)
            {
                case MapCreationTypes.New:
                {
                    int x = S.NewMapDimX;
                    int y = S.NewMapDimY;
                    // HexMapDrawer.Instance.CreateMap(); //TODO
                }
                    break;
                case MapCreationTypes.FromOld:
                {
                    var oldMap = Stuff.Maps[S.GetDropdownSetting(SettingType.SelectedMapToEditIndex)].Clone();
                    HexMapDrawer.Instance.CreateMap(oldMap);
                    HexMapDrawer.Instance.AfterCellDrag += cell =>
                    {
                        cell.Type = _currentBrushType;
                        HexMapDrawer.Instance.RepaintCell(HexMapDrawer.Instance.SelectDrawnCell(cell));
                        HexMapDrawer.Instance.TriangulateCells();
                    };
                    MainCameraController.Instance.Init();
                }
                    break;
                default:
                {
                    Debug.LogError("Selected Map Cretion Type not found");
                    //TODO: handle exception
                }
                    break;
            }
        }
    }
}