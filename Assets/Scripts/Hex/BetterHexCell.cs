namespace Hex
{
    public class BetterHexCell
    {
		public readonly HexCoordinates Coordinates;
	    public readonly HexTileType Type;
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
    }
}