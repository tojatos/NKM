using UnityEngine;

namespace Hex
{
	[System.Serializable]
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
			if (iX + iY + iZ != 0)
			{
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
			}
			return new HexCoordinates(iX, iZ);
		}

		public override string ToString()
		{
			return "(" +
			       X + ", " + Y + ", " + Z + ")";
		}
		public int DistanceTo(HexCoordinates other)
		{
			return
			((X < other.X ? other.X - X : X - other.X) +
			 (Y < other.Y ? other.Y - Y : Y - other.Y) +
			 (Z < other.Z ? other.Z - Z : Z - other.Z)) / 2;
		}
		//public string ToStringOnSeparateLines()
		//{
		//	return X + "\n" + Y + "\n" + Z;
		//}

	}
}