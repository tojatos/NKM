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
		private Active _active;
		private Spawner _spawner;
		public HexCoordinates Coordinates;
		public Character CharacterOnCell { get; set; }
		public GameObject Highlight { get; set; }
		public GameObject HelpHighlight { get; set; }
		public HexTileType Type { get; set; }
		public Color Color;
		public List<HexCellEffect> Effects = new List<HexCellEffect>();

		private Game Game;

		private void Start()
		{
			Game = GameStarter.Instance.Game;
			_active = Game.Active;
		}

		private readonly HexCell[] _neighbors = new HexCell[6];

		private HexCell GetNeighbor(HexDirection direction)
		{
			return _neighbors[(int)direction];
		}
		public void SetNeighbor(HexDirection direction, HexCell cell)
		{
			_neighbors[(int)direction] = cell;
			cell._neighbors[(int)direction.Opposite()] = this;
		}
		public void ToggleHighlight(HiglightColor color = HiglightColor.Black)
		{
			if (Highlight == null)
			{
				_spawner.SpawnHighlightCellObject(this, color);
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
				_spawner.SpawnHelpHighlightCellObject(this, color);
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
			_spawner = Spawner.Instance;
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
		public HexCell GetCell(HexDirection direction, int distance)
		{
			if (distance <= 0)
				throw new ArgumentOutOfRangeException(nameof(distance), distance, null);

			HexCell cellToReturn;
			switch (direction)
			{
				case HexDirection.Ne:
					cellToReturn = Game.HexMapDrawer.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X && c.Coordinates.Y == Coordinates.Y - distance);
					break;
				case HexDirection.E:
					cellToReturn = Game.HexMapDrawer.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X + distance && c.Coordinates.Y == Coordinates.Y - distance);
					break;
				case HexDirection.Se:
					cellToReturn = Game.HexMapDrawer.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X + distance && c.Coordinates.Y == Coordinates.Y);
					break;
				case HexDirection.Sw:
					cellToReturn = Game.HexMapDrawer.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X && c.Coordinates.Y == Coordinates.Y + distance);
					break;
				case HexDirection.W:
					cellToReturn = Game.HexMapDrawer.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X - distance && c.Coordinates.Y == Coordinates.Y + distance);
					break;
				case HexDirection.Nw:
					cellToReturn = Game.HexMapDrawer.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X - distance && c.Coordinates.Y == Coordinates.Y);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
			}

			return cellToReturn;

		}
		public IEnumerable<HexCell> GetLine(HexDirection direction, int depth)
		{
			List<HexCell> visited = new List<HexCell>();
			HexCell lastCell = this;
			for (var i = 0; i < depth; i++)
			{
				HexCell neighbor = lastCell.GetNeighbor(direction);
				if(neighbor==null) continue;
				
				visited.Add(neighbor);
				lastCell = neighbor;
			}

			return visited;
		}
		public List<HexCell> GetNeighbors(int depth, SearchFlags searchFlags = SearchFlags.None)
		{
			bool stopAtWalls = searchFlags.HasFlag(SearchFlags.StopAtWalls);
			bool stopAtEnemyCharacters = searchFlags.HasFlag(SearchFlags.StopAtEnemyCharacters);
			bool stopAtFriendlyCharacters = searchFlags.HasFlag(SearchFlags.StopAtFriendlyCharacters);
			bool straightLine = searchFlags.HasFlag(SearchFlags.StraightLine);
			List<HexCell> visited = new List<HexCell>();
			if (depth == 0) return visited;

			if (straightLine)
			{
				foreach (HexDirection direction in Enum.GetValues(typeof(HexDirection)))
				{
					HexCell lastCell = this;
					for (var i = 0; i < depth; i++)
					{
                        HexCell neighbor = lastCell.GetNeighbor(direction);
						if(neighbor==null) continue;
						
						bool isBlocked = stopAtWalls && neighbor.Type == HexTileType.Wall ||
						                 stopAtEnemyCharacters && neighbor.CharacterOnCell != null && neighbor.CharacterOnCell.Owner != _active.GamePlayer ||
						                 stopAtFriendlyCharacters && neighbor.CharacterOnCell != null && neighbor.CharacterOnCell.Owner == _active.GamePlayer;
						if(isBlocked) continue;
						visited.Add(neighbor);
						lastCell = neighbor;
					}
				}
				return visited;
			}

			List<List<HexCell>> fringes = new List<List<HexCell>>();
			fringes.Add(new List<HexCell>{this});

			for (var i = 1; i <= depth; i++)
			{
				fringes.Add(new List<HexCell>());
				foreach (HexCell cell in fringes[i-1])
				{
					foreach (HexDirection direction in Enum.GetValues(typeof(HexDirection)))
					{
						HexCell neighbor = cell.GetNeighbor(direction);
						if(neighbor==null) continue;
						
						bool isBlocked = stopAtWalls && neighbor.Type == HexTileType.Wall ||
						                 stopAtEnemyCharacters && neighbor.CharacterOnCell != null && neighbor.CharacterOnCell.Owner != _active.GamePlayer ||
						                 stopAtFriendlyCharacters && neighbor.CharacterOnCell != null && neighbor.CharacterOnCell.Owner == _active.GamePlayer;
						bool isVisited = visited.Contains(neighbor);
						
						if(isBlocked || isVisited) continue;
						visited.Add(neighbor);
						fringes[i].Add(neighbor);
					}
				}
			}

			visited.Remove(this);
			return visited;
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

	[Flags]
	public enum SearchFlags
	{
		None = 0,
		StopAtWalls = 1,
		StopAtEnemyCharacters = 2,
		StopAtFriendlyCharacters = 4,
		StraightLine = 8,
		
	}
}