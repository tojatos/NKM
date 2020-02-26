using System;
using System.Collections.Generic;
using System.Linq;
using NKMCore;
using NKMCore.Abilities.Ononoki_Yotsugi;
using NKMCore.Abilities.Roronoa_Zoro;
using NKMCore.Abilities.Ryuko_Matoi;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;
using Unity.Extensions;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Unity.Hex
{
    public class HexMapDrawer : SingletonMonoBehaviour<HexMapDrawer>
    {
        public DrawnHexCell CellPrefab;
        public List<DrawnHexCell> Cells;
        public bool Created = false;
        private readonly Dictionary<Character, GameObject> _characterObjects = new Dictionary<Character, GameObject>();
        public static readonly Dictionary<Character, GameObject> Dims = new Dictionary<Character, GameObject>();

        public GameObject GetCharacterObject(Character character) => _characterObjects.GetValueOrDefault(character);
        public void SetCharacterObject(Character character, GameObject cObject) => _characterObjects[character] = cObject;

        public HexMesh HexMesh { get; private set; }

        public event Delegates.Cell AfterCellSelect;

        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Utilities.IsPointerOverUiObject()) return; //Do not touch cells if mouse is over UI

                HexCell cellPointed = CellPointed();
                if (cellPointed != null) AfterCellSelect?.Invoke(cellPointed);
            }
        }

        public static HexCell CellPointed()
        {
            Debug.Assert(Camera.main != null, "Camera.main != null");
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(inputRay, out RaycastHit hit)) return null;

            Vector3 position = hit.point;
            return Instance.GetCellByPosition(ref position);
        }

        private HexCell GetCellByPosition(ref Vector3 position)
        {
            position = transform.InverseTransformPoint(position);
            HexCoordinates coordinates = position.ToCoordinates();
            return Cells.Select(c => c.HexCell).First(c => c.Coordinates == coordinates);
        }


        public void AddTriggers(Ability ability)
        {
            if (ability is UrbCrunch)
            {
                ((UrbCrunch) ability).AfterCrunch += list =>
                {
                    //repaint crushed cells
                    List<HexCell> normalCells = list.FindAll(c => c.Type == HexCell.TileType.Normal);
                    SelectDrawnCells(normalCells).ForEach(c => c.Color = Color.white); //TODO: set the color depending on maps normal cell color
                    TriangulateCells();
                };
            }

            if (ability is OniGiri)
            {
                ((OniGiri) ability).AfterOniGiriPrepare += list =>
                {
                    //Show highlights on move cells
                    list.ForEach(c =>
                    {
                        HexDirection direction = ability.ParentCharacter.ParentCell.GetDirection(c);
                        HexCell moveCell = c.GetCell(direction, 2);
                        SelectDrawnCell(moveCell).AddHighlight(Highlights.BlueTransparent);
                    });
                };
            }

            if (ability is FiberDecapitation)
            {
                var ab = (FiberDecapitation) ability;
                ab.AfterPrepare += list =>
                {
                    //Show highlights on move cells
                    //TODO: redundant code
                    list.ForEach(c =>
                    {
                        HexDirection direction = ability.ParentCharacter.ParentCell.GetDirection(c);
                        HexCell moveCell = c.GetCell(direction, FiberDecapitation.TargetCellOffset);
                        SelectDrawnCell(moveCell).AddHighlight(Highlights.BlueTransparent);
                    });
                };
            }
        }

        public void CreateMap(HexMap hexMap)
        {
            HexMesh = GetComponentInChildren<HexMesh>();
            HexMesh.Init();
            hexMap.Cells.ForEach(CreateCell);

            TriangulateCells();
            Created = true;
        }

        private void TriangulateCells()
        {
            HexMesh.Triangulate(Cells);
        }

        private void CreateCell(HexCell hexCell)
        {
            Vector3 position;
            position.x = (hexCell.Coordinates.X + hexCell.Coordinates.Z * 0.5f) * (HexMetrics.InnerRadius * 2f);
            position.y = 0f;
            position.z = hexCell.Coordinates.Z * (HexMetrics.OuterRadius * 1.5f);

            DrawnHexCell cell = Instantiate(CellPrefab, transform, false);
            Cells.Add(cell);
            cell.HexCell = hexCell;
            cell.transform.localPosition = position;

            switch (hexCell.Type)
            {
                case HexCell.TileType.Normal:
                    cell.Color = Color.white;
                    break;
                case HexCell.TileType.Wall:
                    cell.Color = Color.black;
                    break;
                case HexCell.TileType.SpawnPoint1:
                case HexCell.TileType.SpawnPoint2:
                case HexCell.TileType.SpawnPoint3:
                case HexCell.TileType.SpawnPoint4:
                    cell.Color = Color.green;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void RemoveHighlights(Predicate<GameObject> predicate = null)
        {
            foreach (DrawnHexCell hexCell in Cells)
            {
                if (predicate == null)
                {
                    hexCell.Highlights.ForEach(Destroy);
                    hexCell.Highlights.Clear();
                }
                else
                {
                    hexCell.Highlights.FindAll(predicate).ForEach(Destroy);
                    hexCell.Highlights.RemoveAll(predicate);
                }
            }
        }
        public void RemoveHighlightsOfColor(string colorName) => RemoveHighlights(h => h.GetComponent<SpriteRenderer>().sprite.name == colorName);

        public List<DrawnHexCell> SelectDrawnCells(IEnumerable<HexCell> cells) => cells.Select(SelectDrawnCell).ToList();
        public DrawnHexCell SelectDrawnCell(HexCell cell) =>
            Cells.FirstOrDefault(g => g.HexCell == cell);

        public static void ShowHelpHexCells(List<DrawnHexCell> cells) => cells.ForEach(c => c.AddHighlight(Highlights.BlueTransparent));
        public void HideHelpHexCells() => RemoveHighlightsOfColor(Highlights.BlueTransparent);
    }
}
