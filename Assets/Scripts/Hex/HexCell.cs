using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Managers;
using NKMObjects.Templates;
using UnityEngine;

namespace Hex
{
	public class HexCell : MonoBehaviour
	{
		private static Game Game => GameStarter.Instance.Game;
		private static Active Active => Game.Active;
		private static Spawner Spawner => Spawner.Instance;
		
		public HexCoordinates Coordinates;

		private Character _characterOnCell;
		public Character CharacterOnCell
		{
			get { return _characterOnCell; }
			set
			{
				if (_characterOnCell != null)
				{
					_characterOnCell.IsLeaving = true;
					OnLeave?.Invoke(_characterOnCell);
					_characterOnCell.IsLeaving = false;
				}
				if(value!=null) OnEnter?.Invoke(value);
				_characterOnCell = value;
			}
		}

		public List<GameObject> Highlights;//{ get; set; }
//		public GameObject HelpHighlight;// { get; set; }
		public HexTileType Type;// { get; set; }
		public Color Color;
		public List<HexCellEffect> Effects = new List<HexCellEffect>();
		public bool IsFreeToStand => CharacterOnCell == null && Type != HexTileType.Wall;

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

		public void AddHighlight(string color) => Spawner.SpawnHighlightCellObject(this, color);
//		public void AddHighlight(HiglightColor color = HiglightColor.Black) => AddHighlight(ref Highlight, color);
//		public void AddHighlight(HiglightColor color = HiglightColor.Black) => AddHighlight(ref HelpHighlight, color);
		

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

			switch (direction)
			{
				case HexDirection.Ne:
					return Game.HexMapDrawer.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X && c.Coordinates.Y == Coordinates.Y - distance);
				case HexDirection.E:
					return Game.HexMapDrawer.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X + distance && c.Coordinates.Y == Coordinates.Y - distance);
				case HexDirection.Se:
					return Game.HexMapDrawer.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X + distance && c.Coordinates.Y == Coordinates.Y);
				case HexDirection.Sw:
					return Game.HexMapDrawer.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X && c.Coordinates.Y == Coordinates.Y + distance);
				case HexDirection.W:
					return Game.HexMapDrawer.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X - distance && c.Coordinates.Y == Coordinates.Y + distance);
				case HexDirection.Nw:
					return Game.HexMapDrawer.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X - distance && c.Coordinates.Y == Coordinates.Y);
				default:
					throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
			}
		}

		public int GetDistance(HexCell cell)
		{
			HexDirection direction = GetDirection(cell);
			HexCell lastCell = this;
			int distance = 0;
			while (lastCell!=cell)
			{
				distance++;
				lastCell = lastCell.GetNeighbor(direction);
				if (lastCell == null) return -1;
			}

			return distance;
		}
		public List<HexCell> GetLine(HexDirection direction, int depth)
		{
			List<HexCell> visited = new List<HexCell>();
			HexCell lastCell = this;
			for (int i = 0; i < depth; i++)
			{
				HexCell neighbor = lastCell.GetNeighbor(direction);
				if(neighbor==null) continue; //TODO: why not break?
				
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
						                 stopAtEnemyCharacters && neighbor.CharacterOnCell != null && neighbor.CharacterOnCell.Owner != Active.GamePlayer ||
						                 stopAtFriendlyCharacters && neighbor.CharacterOnCell != null && neighbor.CharacterOnCell.Owner == Active.GamePlayer;
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
						                 stopAtEnemyCharacters && neighbor.CharacterOnCell != null && neighbor.CharacterOnCell.Owner != Active.GamePlayer ||
						                 stopAtFriendlyCharacters && neighbor.CharacterOnCell != null && neighbor.CharacterOnCell.Owner == Active.GamePlayer;
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

		public delegate void CharacterDelegate(Character character);

		public event CharacterDelegate OnEnter;
		public event CharacterDelegate OnLeave;

		public bool IsSpawnFor(GamePlayer player) => Type == HexMapDrawer.Instance.HexMap.SpawnPoints[player.GetIndex()];
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