using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using JetBrains.Annotations;
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

		public BetterHexCell BetterHexCell;
		
		public HexCoordinates Coordinates => BetterHexCell.Coordinates;

//		private NKMCharacter _characterOnCell;
//		public NKMCharacter CharacterOnCell
//		{
//			get { return _characterOnCell; }
//			set
//			{
//				if (_characterOnCell != null)
//				{
//					_characterOnCell.IsLeaving = true;
//					OnLeave?.Invoke(_characterOnCell);
//					_characterOnCell.IsLeaving = false;
//				}
//				if(value!=null) OnEnter?.Invoke(value);
//				_characterOnCell = value;
//			}
//		}
		public NKMCharacter CharacterOnCell
		{
			get { return !BetterHexCell.IsFreeToStand(Game.HexMap) ? BetterHexCell.CharactersOnCell(Game.HexMap)[0] : null; }
			set {
				if (value == null)
				{
					Game.HexMap.RemoveFromMap(BetterHexCell.CharactersOnCell(Game.HexMap)[0]);
				}
				else
				{
					Game.HexMap.Move(value, BetterHexCell);
				}
				
			}
		}

		public List<GameObject> Highlights;
		public List<GameObject> EffectHighlights;
		public HexTileType Type
		{
			get { return BetterHexCell.Type; }
			set { BetterHexCell.Type = value; }
		}

		public Color Color;
		public List<HexCellEffect> Effects = new List<HexCellEffect>();
		public bool IsFreeToStand => CharacterOnCell == null && Type != HexTileType.Wall;

//		private readonly HexCell[] _neighbors = new HexCell[6];
//		public List<HexCell> ClosestFreeToStand(SearchFlags searchFlags = SearchFlags.None)
//		{
//			int i = 1;
//			while (i < 500) //TODO: Change 500 to a map's maximum value, good enough for now.
//			{
//                List<HexCell> neighboursFreeToStand = GetNeighbors(i, searchFlags).FindAll(c => c.IsFreeToStand);
//                if (neighboursFreeToStand.Count == 0) return neighboursFreeToStand;
//				i++;
//			}
//			return new List<HexCell>(); // There are no cells that are free to stand
//		}
//
//        private HexCell GetNeighbor(HexDirection direction) => _neighbors[(int)direction];
//        public void SetNeighbor(HexDirection direction, HexCell cell)
//		{
//			_neighbors[(int)direction] = cell;
//			cell._neighbors[(int)direction.Opposite()] = this;
//		}

		public void AddHighlight(string color) => Spawner.SpawnHighlightCellObject(this, color);
		
		public void AddEffectHighlight(string effectName) => Spawner.SpawnEffectHighlightCellObject(this, effectName);
		public void RemoveEffectHighlight(string effectName)
		{
			GameObject highlight = EffectHighlights.FirstOrDefault(h => h.GetComponent<SpriteRenderer>().sprite.name == effectName);
			if(highlight==null) return;
//			AnimationPlayer.Add(new Destroy(highlight));
			Destroy(highlight);
            EffectHighlights.Remove(highlight);
			
		}
			
		public HexDirection GetDirection(HexCell targetCell) => BetterHexCell.GetDirection(targetCell.BetterHexCell);

		public HexCell GetCell(HexDirection direction, int distance) => HexMapDrawer.Instance.Cells.First(c => c.BetterHexCell ==
			BetterHexCell.GetCell(Game.HexMap, direction, distance));

		public List<HexCell> GetNeighbors(int depth, SearchFlags searchFlags = SearchFlags.None,
			Predicate<BetterHexCell> stopAt = null)
			=> BetterHexCell.GetNeighbors(Game.HexMap, Active.GamePlayer, depth, searchFlags, stopAt)
				.Select(c => HexMapDrawer.Instance.Cells.FirstOrDefault(g => g.BetterHexCell == c)).ToList();
		
		public List<HexCell> GetLine(HexDirection direction, int depth)
			=> BetterHexCell.GetLine(direction, depth)
				.Select(c => HexMapDrawer.Instance.Cells.FirstOrDefault(g => g.BetterHexCell == c)).ToList();

		public int GetDistance(HexCell cell) => BetterHexCell.GetDistance(cell.BetterHexCell);
		
		public List<HexCell> GetArea(HexCell targetCell, int width) => GetArea(GetDirection(targetCell), GetDistance(targetCell), width);
		public List<HexCell> GetArea(HexDirection direction, int height, int width)
			=> BetterHexCell.GetArea(direction, height, width)
				.Select(c => HexMapDrawer.Instance.Cells.FirstOrDefault(g => g.BetterHexCell == c)).ToList();
		
//		public HexDirection GetDirection(HexCell targetCell)
//		{
//			if (Coordinates.X == targetCell.Coordinates.X && Coordinates.Y == targetCell.Coordinates.Y && Coordinates.Z == targetCell.Coordinates.Z)
//			{
//				throw new Exception("cells are equal");
//			}
//
//			if (Coordinates.X == targetCell.Coordinates.X)
//			{
//				return Coordinates.Y < targetCell.Coordinates.Y ? HexDirection.Sw : HexDirection.Ne;
//			}
//			if (Coordinates.Y == targetCell.Coordinates.Y)
//			{
//				return Coordinates.X < targetCell.Coordinates.X ? HexDirection.Se : HexDirection.Nw;
//			}
//			if (Coordinates.Z == targetCell.Coordinates.Z)
//			{
//				return Coordinates.X < targetCell.Coordinates.X ? HexDirection.E : HexDirection.W;
//			}
//
//			throw new Exception("direction not found");
//		}
//		public HexCell GetCell(HexDirection direction, int distance)
//		{
//			if (distance <= 0)
//				throw new ArgumentOutOfRangeException(nameof(distance), distance, null);
//
//			switch (direction)
//			{
//				case HexDirection.Ne:
//					return Game.HexMapDrawer.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X && c.Coordinates.Y == Coordinates.Y - distance);
//				case HexDirection.E:
//					return Game.HexMapDrawer.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X + distance && c.Coordinates.Y == Coordinates.Y - distance);
//				case HexDirection.Se:
//					return Game.HexMapDrawer.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X + distance && c.Coordinates.Y == Coordinates.Y);
//				case HexDirection.Sw:
//					return Game.HexMapDrawer.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X && c.Coordinates.Y == Coordinates.Y + distance);
//				case HexDirection.W:
//					return Game.HexMapDrawer.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X - distance && c.Coordinates.Y == Coordinates.Y + distance);
//				case HexDirection.Nw:
//					return Game.HexMapDrawer.Cells.SingleOrDefault(c => c.Coordinates.X == Coordinates.X - distance && c.Coordinates.Y == Coordinates.Y);
//				default:
//					throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
//			}
//		}
//
//		public int GetDistance(HexCell cell)
//		{
//			HexDirection direction = GetDirection(cell);
//			HexCell lastCell = this;
//			int distance = 0;
//			while (lastCell!=cell)
//			{
//				distance++;
//				lastCell = lastCell.GetNeighbor(direction);
//				if (lastCell == null) return -1;
//			}
//
//			return distance;
//		}
//		public List<HexCell> GetLine(HexDirection direction, int depth)
//		{
//			List<HexCell> visited = new List<HexCell>();
//			HexCell lastCell = this;
//			for (int i = 0; i < depth; i++)
//			{
//				HexCell neighbor = lastCell.GetNeighbor(direction);
//				if(neighbor==null) continue;
//				
//				visited.Add(neighbor);
//				lastCell = neighbor;
//			}
//
//			return visited;
//		}
//		public List<HexCell> GetNeighbors(int depth, SearchFlags searchFlags = SearchFlags.None, Func<HexCell, bool> stopAt = null)
//		{
//			bool stopAtWalls = searchFlags.HasFlag(SearchFlags.StopAtWalls);
//			bool stopAtEnemyCharacters = searchFlags.HasFlag(SearchFlags.StopAtEnemyCharacters);
//			bool stopAtFriendlyCharacters = searchFlags.HasFlag(SearchFlags.StopAtFriendlyCharacters);
//			bool straightLine = searchFlags.HasFlag(SearchFlags.StraightLine);
//			List<HexCell> visited = new List<HexCell>();
//			if (depth == 0) return visited;
//
//			if (straightLine)
//			{
//				foreach (HexDirection direction in Enum.GetValues(typeof(HexDirection)))
//				{
//					HexCell lastCell = this;
//					for (var i = 0; i < depth; i++)
//					{
//                        HexCell neighbor = lastCell.GetNeighbor(direction);
//						if(neighbor==null) continue;
//						
//						if(stopAt != null && stopAt(neighbor)) continue;
//						bool isBlocked = stopAtWalls && neighbor.Type == HexTileType.Wall ||
//						                 stopAtEnemyCharacters && neighbor.CharacterOnCell != null && neighbor.CharacterOnCell.Owner != Active.GamePlayer ||
//						                 stopAtFriendlyCharacters && neighbor.CharacterOnCell != null && neighbor.CharacterOnCell.Owner == Active.GamePlayer;
//						if(isBlocked) continue;
//						visited.Add(neighbor);
//						lastCell = neighbor;
//					}
//				}
//				return visited;
//			}
//
//			List<List<HexCell>> fringes = new List<List<HexCell>>();
//			fringes.Add(new List<HexCell>{this});
//
//			for (var i = 1; i <= depth; i++)
//			{
//				fringes.Add(new List<HexCell>());
//				foreach (HexCell cell in fringes[i-1])
//				{
//					foreach (HexDirection direction in Enum.GetValues(typeof(HexDirection)))
//					{
//						HexCell neighbor = cell.GetNeighbor(direction);
//						if(neighbor==null) continue;
//
//						if(stopAt != null && stopAt(neighbor)) continue;
//						
//						bool isBlocked = stopAtWalls && neighbor.Type == HexTileType.Wall ||
//						                 stopAtEnemyCharacters && neighbor.CharacterOnCell != null && neighbor.CharacterOnCell.Owner != Active.GamePlayer ||
//						                 stopAtFriendlyCharacters && neighbor.CharacterOnCell != null && neighbor.CharacterOnCell.Owner == Active.GamePlayer;
//						bool isVisited = visited.Contains(neighbor);
//						
//						if(isBlocked || isVisited) continue;
//						visited.Add(neighbor);
//						fringes[i].Add(neighbor);
//					}
//				}
//			}
//
//			visited.Remove(this);
//			return visited;
//		}
//
//		public List<HexCell> GetArea(HexCell targetCell, int width) => GetArea(GetDirection(targetCell), GetDistance(targetCell), width);
//		public List<HexCell> GetArea(HexDirection direction, int height, int width)
//		{
//			if (height < 1 || width < 1 || width % 2 == 0) throw new ArgumentOutOfRangeException();
//			
//			List<HexCell> areaCells = new List<HexCell>();
//			
//			HexDirection[] nearbyDirections = direction.NearbyDirections();
//			List<HexCell> firstCells = new List<HexCell>();
//			firstCells.Add(GetNeighbor(direction));
//			foreach (HexDirection d in nearbyDirections)
//			{
//				HexCell lastCell = this;
//				for (int i = width; i > 1; i-=2)
//				{
//					lastCell = lastCell.GetNeighbor(d);
//					if(lastCell==null) break;
//					firstCells.Add(lastCell);
//				}
//			}
//			areaCells.AddRange(firstCells);
//			firstCells.ForEach(c => areaCells.AddRange(c.GetLine(direction, height - 1)));
//
//			return areaCells;
//
//		}
//		public delegate void CharacterDelegate(NKMCharacter character);

//		public event CharacterDelegate OnEnter;
//		public event CharacterDelegate OnLeave;

		public bool IsSpawnFor([NotNull] GamePlayer player) => Type == Game.Options.MapScriptable.SpawnPoints[player.GetIndex()];

		public override string ToString() => Coordinates.ToString();
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
