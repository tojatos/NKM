using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using NKMObjects.Templates;
using UnityEngine;

namespace Hex
{
    public class BetterHexCell
    {
		public readonly HexCoordinates Coordinates;
	    public HexTileType Type;
		public List<HexCellEffect> Effects = new List<HexCellEffect>();
		private readonly BetterHexCell[] _neighbors = new BetterHexCell[6];
        public BetterHexCell GetNeighbor(HexDirection direction) => _neighbors[(int)direction];
        public void SetNeighbor(HexDirection direction, BetterHexCell cell)
		{
			_neighbors[(int)direction] = cell;
			cell._neighbors[(int)direction.Opposite()] = this;
		}
	    
        public BetterHexCell(HexCoordinates coords, HexTileType type)
        {
            Coordinates = coords;
	        Type = type;
        }

	    public bool IsFreeToStand(HexMap map) => CharactersOnCell(map).Count == 0;
	    public List<NKMCharacter> CharactersOnCell(HexMap map) => map.GetCharacters(this);

	    public HexDirection GetDirection(BetterHexCell hexCell) => GetDirection(hexCell.Coordinates);
		public HexDirection GetDirection(HexCoordinates targetCoordinates)
		{
			if (Coordinates.X == targetCoordinates.X && Coordinates.Y == targetCoordinates.Y && Coordinates.Z == targetCoordinates.Z)
			{
				throw new Exception("cells are equal");
			}

			if (Coordinates.X == targetCoordinates.X)
			{
				return Coordinates.Y < targetCoordinates.Y ? HexDirection.Sw : HexDirection.Ne;
			}
			if (Coordinates.Y == targetCoordinates.Y)
			{
				return Coordinates.X < targetCoordinates.X ? HexDirection.Se : HexDirection.Nw;
			}
			if (Coordinates.Z == targetCoordinates.Z)
			{
				return Coordinates.X < targetCoordinates.X ? HexDirection.E : HexDirection.W;
			}

			throw new Exception("direction not found");
		}
		public BetterHexCell GetCell(HexMap hexMap, HexDirection direction, int distance)
		{
			if (distance <= 0)
				throw new ArgumentOutOfRangeException(nameof(distance), distance, null);

			switch (direction)
			{
				case HexDirection.Ne:
					return hexMap.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X && c.Coordinates.Y == Coordinates.Y - distance);
				case HexDirection.E:
					return hexMap.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X + distance && c.Coordinates.Y == Coordinates.Y - distance);
				case HexDirection.Se:
					return hexMap.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X + distance && c.Coordinates.Y == Coordinates.Y);
				case HexDirection.Sw:
					return hexMap.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X && c.Coordinates.Y == Coordinates.Y + distance);
				case HexDirection.W:
					return hexMap.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X - distance && c.Coordinates.Y == Coordinates.Y + distance);
				case HexDirection.Nw:
					return hexMap.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X - distance && c.Coordinates.Y == Coordinates.Y);
				default:
					throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
			}
		}

		public int GetDistance(BetterHexCell cell)
		{
			HexDirection direction = GetDirection(cell);
			BetterHexCell lastCell = this;
			int distance = 0;
			while (lastCell!=cell)
			{
				distance++;
				lastCell = lastCell.GetNeighbor(direction);
				if (lastCell == null) return -1;
			}

			return distance;
		}

	    public static Predicate<BetterHexCell> IsEnemyStanding(HexMap map, GamePlayer friendlyPlayer) => cell =>
		    cell.CharactersOnCell(map).Any(c => c.IsEnemyFor(friendlyPlayer));
	    public static Predicate<BetterHexCell> IsFriendStanding(HexMap map, GamePlayer friendlyPlayer) => cell =>
		    cell.CharactersOnCell(map).Any(c => !c.IsEnemyFor(friendlyPlayer));
	    public static Predicate<BetterHexCell> IsWall => cell => cell.Type == HexTileType.Wall;

	    public List<BetterHexCell> GetLine(HexDirection direction, int depth)
		{
			List<BetterHexCell> visited = new List<BetterHexCell>();
			BetterHexCell lastCell = this;
			for (int i = 0; i < depth; i++)
			{
				BetterHexCell neighbor = lastCell.GetNeighbor(direction);
				if(neighbor==null) continue;
				
				visited.Add(neighbor);
				lastCell = neighbor;
			}

			return visited;
		}
		public List<BetterHexCell> GetNeighbors(HexMap map, GamePlayer friendlyPlayer, int depth, SearchFlags searchFlags = SearchFlags.None, Predicate<BetterHexCell> stopAt = null)
		{
			bool stopAtWalls = searchFlags.HasFlag(SearchFlags.StopAtWalls);
			bool stopAtEnemyCharacters = searchFlags.HasFlag(SearchFlags.StopAtEnemyCharacters);
			bool stopAtFriendlyCharacters = searchFlags.HasFlag(SearchFlags.StopAtFriendlyCharacters);
			bool straightLine = searchFlags.HasFlag(SearchFlags.StraightLine);
			if (stopAtWalls) stopAt = stopAt.Or(IsWall); 
			if (stopAtEnemyCharacters) stopAt = stopAt.Or(IsEnemyStanding(map, friendlyPlayer));
			if (stopAtFriendlyCharacters) stopAt = stopAt.Or(IsFriendStanding(map, friendlyPlayer));
			List<BetterHexCell> visited = new List<BetterHexCell>();
			if (depth == 0) return visited;

			if (straightLine)
			{
				foreach (HexDirection direction in Enum.GetValues(typeof(HexDirection)))
				{
					BetterHexCell lastCell = this;
					for (int i = 0; i < depth; i++)
					{
                        BetterHexCell neighbor = lastCell.GetNeighbor(direction);
						if(neighbor==null) continue;
						
						if(stopAt != null && stopAt(neighbor)) continue;
						visited.Add(neighbor);
						lastCell = neighbor;
					}
				}
				return visited;
			}

			List<List<BetterHexCell>> fringes = new List<List<BetterHexCell>>();
			fringes.Add(new List<BetterHexCell>{this});

			for (int i = 1; i <= depth; i++)
			{
				fringes.Add(new List<BetterHexCell>());
				foreach (BetterHexCell cell in fringes[i-1])
				{
					foreach (HexDirection direction in Enum.GetValues(typeof(HexDirection)))
					{
						BetterHexCell neighbor = cell.GetNeighbor(direction);
						if(neighbor==null) continue;

						if(stopAt != null && stopAt(neighbor)) continue;
						
						bool isVisited = visited.Contains(neighbor);
						
						if(isVisited) continue;
						visited.Add(neighbor);
						fringes[i].Add(neighbor);
					}
				}
			}

			visited.Remove(this);
			return visited;
		}

		public List<BetterHexCell> GetArea(BetterHexCell targetCell, int width) => GetArea(GetDirection(targetCell), GetDistance(targetCell), width);
		public List<BetterHexCell> GetArea(HexDirection direction, int height, int width)
		{
			if (height < 1 || width < 1 || width % 2 == 0) throw new ArgumentOutOfRangeException();
			
			List<BetterHexCell> areaCells = new List<BetterHexCell>();
			
			HexDirection[] nearbyDirections = direction.NearbyDirections();
			List<BetterHexCell> firstCells = new List<BetterHexCell>();
			firstCells.Add(GetNeighbor(direction));
			foreach (HexDirection d in nearbyDirections)
			{
				BetterHexCell lastCell = this;
				for (int i = width; i > 1; i-=2)
				{
					lastCell = lastCell.GetNeighbor(d);
					if(lastCell==null) break;
					firstCells.Add(lastCell);
				}
			}
			areaCells.AddRange(firstCells);
			firstCells.ForEach(c => areaCells.AddRange(c.GetLine(direction, height - 1)));

			return areaCells;

		}
    }
}