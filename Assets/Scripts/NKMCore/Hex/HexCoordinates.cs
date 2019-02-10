namespace NKMCore.Hex
{
	public struct HexCoordinates
	{
		private readonly int _x;

		private readonly int _z;

		public int X => _x;

		public int Z => _z;

		public int Y => -X - Z;

		public HexCoordinates(int x, int z)
		{
			_x = x;
			_z = z;
		}

		public static HexCoordinates FromOffsetCoordinates(int x, int z)
		{
			return new HexCoordinates(x - z / 2, z);
		}


		public override string ToString() => $"({X}, {Y}, {Z})";
		public static bool operator ==(HexCoordinates c1, HexCoordinates c2) => c1.Equals(c2);
		public static bool operator !=(HexCoordinates c1, HexCoordinates c2) => !(c1 == c2);
		private bool Equals(HexCoordinates c) => _x == c._x && _z == c._z;
		public override bool Equals(object obj) 
		{
			if (obj == null || GetType() != obj.GetType()) return false;
			return Equals((HexCoordinates)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (_x * 397) ^ _z;
			}
		}

	}
}