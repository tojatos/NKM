using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using JetBrains.Annotations;
using NKMObjects.Templates;

namespace Hex
{
    public class HexCell
    {
		public readonly HexCoordinates Coordinates;
	    public readonly HexMap Map;
	    public HexTileType Type;
		public List<HexCellEffect> Effects = new List<HexCellEffect>();
		private readonly HexCell[] _neighbors = new HexCell[6];
        public HexCell GetNeighbor(HexDirection direction) => _neighbors[(int)direction];
        public void SetNeighbor(HexDirection direction, HexCell cell)
		{
			_neighbors[(int)direction] = cell;
			cell._neighbors[(int)direction.Opposite()] = this;
		}
	    
        public HexCell(HexMap map, HexCoordinates coords, HexTileType type)
        {
	        Map = map;
            Coordinates = coords;
	        Type = type;
        }

	    public bool IsFreeToStand => IsEmpty && Type != HexTileType.Wall;
	    public bool IsEmpty => CharactersOnCell.Count == 0;
	    public List<Character> CharactersOnCell => Map.GetCharacters(this);
	    
		public bool IsSpawnFor([NotNull] GamePlayer player) => Type == Map.SpawnPoints[player.GetIndex()];

	    public HexDirection GetDirection(HexCell hexCell) => GetDirection(hexCell.Coordinates);
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
		public HexCell GetCell(HexDirection direction, int distance)
		{
			if (distance <= 0)
				throw new ArgumentOutOfRangeException(nameof(distance), distance, null);

			switch (direction)
			{
				case HexDirection.Ne:
					return Map.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X && c.Coordinates.Y == Coordinates.Y - distance);
				case HexDirection.E:
					return Map.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X + distance && c.Coordinates.Y == Coordinates.Y - distance);
				case HexDirection.Se:
					return Map.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X + distance && c.Coordinates.Y == Coordinates.Y);
				case HexDirection.Sw:
					return Map.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X && c.Coordinates.Y == Coordinates.Y + distance);
				case HexDirection.W:
					return Map.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X - distance && c.Coordinates.Y == Coordinates.Y + distance);
				case HexDirection.Nw:
					return Map.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X - distance && c.Coordinates.Y == Coordinates.Y);
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

	    public static Predicate<HexCell> IsEnemyStanding(HexMap map, GamePlayer friendlyPlayer) => cell =>
		    cell.CharactersOnCell.Any(c => c.IsEnemyFor(friendlyPlayer));
	    public static Predicate<HexCell> IsFriendStanding(HexMap map, GamePlayer friendlyPlayer) => cell =>
		    cell.CharactersOnCell.Any(c => !c.IsEnemyFor(friendlyPlayer));
	    public static Predicate<HexCell> IsWall => cell => cell.Type == HexTileType.Wall;

	    public List<HexCell> GetLine(HexDirection direction, int depth)
		{
			List<HexCell> visited = new List<HexCell>();
			HexCell lastCell = this;
			for (int i = 0; i < depth; i++)
			{
				HexCell neighbor = lastCell.GetNeighbor(direction);
				if(neighbor==null) continue;
				
				visited.Add(neighbor);
				lastCell = neighbor;
			}

			return visited;
		}
		public List<HexCell> GetNeighbors(GamePlayer friendlyPlayer, int depth, SearchFlags searchFlags = SearchFlags.None, Predicate<HexCell> stopAt = null)
		{
			bool stopAtWalls = searchFlags.HasFlag(SearchFlags.StopAtWalls);
			bool stopAtEnemyCharacters = searchFlags.HasFlag(SearchFlags.StopAtEnemyCharacters);
			bool stopAtFriendlyCharacters = searchFlags.HasFlag(SearchFlags.StopAtFriendlyCharacters);
			bool straightLine = searchFlags.HasFlag(SearchFlags.StraightLine);
			if (stopAtWalls) stopAt = stopAt.Or(IsWall); 
			if (stopAtEnemyCharacters) stopAt = stopAt.Or(IsEnemyStanding(Map, friendlyPlayer));
			if (stopAtFriendlyCharacters) stopAt = stopAt.Or(IsFriendStanding(Map, friendlyPlayer));
			List<HexCell> visited = new List<HexCell>();
			if (depth == 0) return visited;

			if (straightLine)
			{
				foreach (HexDirection direction in Enum.GetValues(typeof(HexDirection)))
				{
					HexCell lastCell = this;
					for (int i = 0; i < depth; i++)
					{
                        HexCell neighbor = lastCell.GetNeighbor(direction);
						if(neighbor==null) continue;
						
						if(stopAt != null && stopAt(neighbor)) continue;
						visited.Add(neighbor);
						lastCell = neighbor;
					}
				}
				return visited;
			}

			List<List<HexCell>> fringes = new List<List<HexCell>>();
			fringes.Add(new List<HexCell>{this});

			for (int i = 1; i <= depth; i++)
			{
				fringes.Add(new List<HexCell>());
				foreach (HexCell cell in fringes[i-1])
				{
					foreach (HexDirection direction in Enum.GetValues(typeof(HexDirection)))
					{
						HexCell neighbor = cell.GetNeighbor(direction);
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

		public List<HexCell> GetArea(HexCell targetCell, int width) => GetArea(GetDirection(targetCell), GetDistance(targetCell), width);
		public List<HexCell> GetArea(HexDirection direction, int height, int width)
		{
			if (height < 1 || width < 1 || width % 2 == 0) throw new ArgumentOutOfRangeException();
			
			List<HexCell> areaCells = new List<HexCell>();
			
			HexDirection[] nearbyDirections = direction.NearbyDirections();
			List<HexCell> firstCells = new List<HexCell>();
			firstCells.Add(GetNeighbor(direction));
			foreach (HexDirection d in nearbyDirections)
			{
				HexCell lastCell = this;
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