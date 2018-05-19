using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using MyGameObjects.MyGameObject_templates;
using UnityEngine;

namespace Hex
{
	public class HexCell : MonoBehaviour
	{
		private Active Active;
		private Spawner Spawner;
		public HexCoordinates Coordinates;
		public Character CharacterOnCell { get; set; }
		public GameObject Highlight { get; set; }
		public GameObject HelpHighlight { get; set; }
		public HexTileType Type { get; set; }
		public Color Color;
		public List<HexCellEffect> Effects = new List<HexCellEffect>();

		private Game Game;
		void Start()
		{
			Game = GameStarter.Instance.Game;
			Active = Game.Active;
		}


		private readonly HexCell[] _neighbors = new HexCell[6];

		private HexCell GetNeighbor(HexDirection direction)
		{
			return _neighbors[(int)direction];
		}
		//public HexCell[] GetNeighbors()
		//{
		//	return _neighbors;
		//}
		public void SetNeighbor(HexDirection direction, HexCell cell)
		{
			_neighbors[(int)direction] = cell;
			cell._neighbors[(int)direction.Opposite()] = this;
		}
		public void ToggleHighlight(HiglightColor color = HiglightColor.Black)
		{
			if (Highlight == null)
			{
				Spawner.SpawnHighlightCellObject(this, color);
			}
			else
			{
				Destroy(Highlight);
				Highlight = null;
			}
		}
		public void ToggleHelpHighlight(HiglightColor color = HiglightColor.Black)
		{
			if (HelpHighlight == null)
			{
				Spawner.SpawnHelpHighlightCellObject(this, color);
			}
			else
			{
				Destroy(HelpHighlight);
				HelpHighlight = null;
			}
		}

		private void Awake()
		{
			//GameStarter = GameObject.Find("GameStarter").GetComponent<GameStarter>();
			Spawner = Spawner.Instance;
		}
		public List<HexCell> GetNeighbors(int depth, bool stopAtWalls = false, bool stopAtEnemyCharacters = false, bool straightLine = false)
		{
			var neighborsList = new List<HexCell>();
			if (depth == 0) return neighborsList;

			foreach (HexDirection direction in Enum.GetValues(typeof(HexDirection)))
			{
				neighborsList.AddRange(GetNeighborsByDirection(depth, direction, stopAtWalls, stopAtEnemyCharacters, straightLine));
			}
			//remove duplicated and this hexcell
			neighborsList = neighborsList.Distinct().ToList();
			neighborsList.Remove(this);
			return neighborsList;

		}

		private IEnumerable<HexCell> GetNeighborsByDirection(int depth, HexDirection direction, bool stopAtWalls = false, bool stopAtEnemyCharacters = false, bool straightLine = false)
		{
			var neighborsList = new List<HexCell>();
			var neighbor = GetNeighbor(direction);
			if (neighbor == null || stopAtWalls && neighbor.Type == HexTileType.Wall || stopAtEnemyCharacters &&
			    neighbor.CharacterOnCell != null && neighbor.CharacterOnCell.Owner != Active.GamePlayer) return neighborsList;
			if (neighborsList.Any(listsNeighbor => listsNeighbor == neighbor)) return neighborsList;

			neighborsList.Add(neighbor);
			if (depth <= 1) return neighborsList;

			switch (direction)
			{
				case HexDirection.Ne:
					if(!straightLine)
					{
						neighborsList.AddRange(neighbor.GetNeighborsByDirection(depth - 1, HexDirection.Nw, stopAtWalls, stopAtEnemyCharacters));
						neighborsList.AddRange(neighbor.GetNeighborsByDirection(depth - 1, HexDirection.E, stopAtWalls, stopAtEnemyCharacters));
					}
					neighborsList.AddRange(neighbor.GetNeighborsByDirection(depth - 1, HexDirection.Ne, stopAtWalls, stopAtEnemyCharacters, straightLine));
					break;
				case HexDirection.E:
					if (!straightLine)
					{
						neighborsList.AddRange(neighbor.GetNeighborsByDirection(depth - 1, HexDirection.Ne, stopAtWalls, stopAtEnemyCharacters));
						neighborsList.AddRange(neighbor.GetNeighborsByDirection(depth - 1, HexDirection.Se, stopAtWalls, stopAtEnemyCharacters));
					}
					neighborsList.AddRange(neighbor.GetNeighborsByDirection(depth - 1, HexDirection.E, stopAtWalls, stopAtEnemyCharacters, straightLine));
					break;
				case HexDirection.Se:
					if (!straightLine)
					{
						neighborsList.AddRange(neighbor.GetNeighborsByDirection(depth - 1, HexDirection.E, stopAtWalls, stopAtEnemyCharacters));
						neighborsList.AddRange(neighbor.GetNeighborsByDirection(depth - 1, HexDirection.Sw, stopAtWalls, stopAtEnemyCharacters));
					}
					neighborsList.AddRange(neighbor.GetNeighborsByDirection(depth - 1, HexDirection.Se, stopAtWalls, stopAtEnemyCharacters, straightLine));
					break;
				case HexDirection.Sw:
					if (!straightLine)
					{
						neighborsList.AddRange(neighbor.GetNeighborsByDirection(depth - 1, HexDirection.Se, stopAtWalls, stopAtEnemyCharacters));
						neighborsList.AddRange(neighbor.GetNeighborsByDirection(depth - 1, HexDirection.W, stopAtWalls, stopAtEnemyCharacters));
					}
					neighborsList.AddRange(neighbor.GetNeighborsByDirection(depth - 1, HexDirection.Sw, stopAtWalls, stopAtEnemyCharacters, straightLine));
					break;
				case HexDirection.W:
					if (!straightLine)
					{
						neighborsList.AddRange(neighbor.GetNeighborsByDirection(depth - 1, HexDirection.Sw, stopAtWalls, stopAtEnemyCharacters));
						neighborsList.AddRange(neighbor.GetNeighborsByDirection(depth - 1, HexDirection.Nw, stopAtWalls, stopAtEnemyCharacters));
					}
					neighborsList.AddRange(neighbor.GetNeighborsByDirection(depth - 1, HexDirection.W, stopAtWalls, stopAtEnemyCharacters, straightLine));
					break;
				case HexDirection.Nw:
					if (!straightLine)
					{
						neighborsList.AddRange(neighbor.GetNeighborsByDirection(depth - 1, HexDirection.W, stopAtWalls, stopAtEnemyCharacters));
						neighborsList.AddRange(neighbor.GetNeighborsByDirection(depth - 1, HexDirection.Ne, stopAtWalls, stopAtEnemyCharacters));
					}
					neighborsList.AddRange(neighbor.GetNeighborsByDirection(depth - 1, HexDirection.Nw, stopAtWalls, stopAtEnemyCharacters, straightLine));
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
			}

			return neighborsList;

		}
		public HexDirection GetDirection(HexCell targetCell)
		{
			if (Coordinates.X == targetCell.Coordinates.X && Coordinates.Y == targetCell.Coordinates.Y && Coordinates.Z == targetCell.Coordinates.Z)
			{
				throw new Exception("cells are equal");
			}

			if (Coordinates.X == targetCell.Coordinates.X)
			{
				return Coordinates.Y < targetCell.Coordinates.Y ? HexDirection.Sw : HexDirection.Ne;
			}
			if (Coordinates.Y == targetCell.Coordinates.Y)
			{
				return Coordinates.X < targetCell.Coordinates.X ? HexDirection.Se : HexDirection.Nw;
			}
			if (Coordinates.Z == targetCell.Coordinates.Z)
			{
				return Coordinates.X < targetCell.Coordinates.X ? HexDirection.E : HexDirection.W;
			}

			throw new Exception("direction not found");
		}
		public HexCell GetCell(HexDirection direction, int value)
		{
			if (value <= 0)
				throw new ArgumentOutOfRangeException(nameof(value), value, null);

			HexCell cellToReturn;
			switch (direction)
			{
				case HexDirection.Ne:
					cellToReturn = Game.HexMapDrawer.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X && c.Coordinates.Y == Coordinates.Y - value);
					break;
				case HexDirection.E:
					cellToReturn = Game.HexMapDrawer.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X + value && c.Coordinates.Y == Coordinates.Y - value);
					break;
				case HexDirection.Se:
					cellToReturn = Game.HexMapDrawer.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X + value && c.Coordinates.Y == Coordinates.Y);
					break;
				case HexDirection.Sw:
					cellToReturn = Game.HexMapDrawer.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X && c.Coordinates.Y == Coordinates.Y + value);
					break;
				case HexDirection.W:
					cellToReturn = Game.HexMapDrawer.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X - value && c.Coordinates.Y == Coordinates.Y + value);
					break;
				case HexDirection.Nw:
					cellToReturn = Game.HexMapDrawer.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X - value && c.Coordinates.Y == Coordinates.Y);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
			}

			return cellToReturn;

		}
		public IEnumerable<HexCell> GetLine(HexDirection direction, int value)
		{
			return GetNeighborsByDirection(value, direction, false, false, true);
		}
	}
	public enum HexTileType
	{
		Normal,
		Wall,
		SpawnPoint1,
		SpawnPoint2,
		SpawnPoint3,
		SpawnPoint4

	}
}