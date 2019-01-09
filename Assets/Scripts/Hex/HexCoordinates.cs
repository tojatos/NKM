using UnityEngine;

namespace Hex
{
	public struct HexCoordinates
	{
		[SerializeField]
		private readonly int _x;

		[SerializeField]
		private readonly int _z;

		public int X => _x;

		public int Z => _z;

		public int Y => -X - Z;

		private HexCoordinates(int x, int z)
		{
			_x = x;
			_z = z;
		}

		public static HexCoordinates FromOffsetCoordinates(int x, int z)
		{
			return new HexCoordinates(x - z / 2, z);
		}

		public static HexCoordinates FromPosition(Vector3 position)
		{
			var x = position.x / (HexMetrics.InnerRadius * 2f);
			var y = -x;
			var offset = position.z / (HexMetrics.OuterRadius * 3f);
			x -= offset;
			y -= offset;
			var iX = Mathf.RoundToInt(x);
			var iY = Mathf.RoundToInt(y);
			var iZ = Mathf.RoundToInt(-x - y);
			if (iX + iY + iZ == 0) return new HexCoordinates(iX, iZ);
			var dX = Mathf.Abs(x - iX);
			var dY = Mathf.Abs(y - iY);
			var dZ = Mathf.Abs(-x - y - iZ);

			if (dX > dY && dX > dZ)
			{
				iX = -iY - iZ;
			}
			else if (dZ > dY)
			{
				iZ = -iX - iY;
			}
			return new HexCoordinates(iX, iZ);
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