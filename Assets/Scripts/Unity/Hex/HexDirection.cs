using System;
using System.Linq;

namespace Unity.Hex
{
	public enum HexDirection 
	{
		Ne, E, Se, Sw, W, Nw
	}

	public static class HexDirectionExtensions {

		public static HexDirection Opposite (this HexDirection direction) {
			return (int)direction < 3 ? (direction + 3) : (direction - 3);
		}
		public static HexDirection[] NearbyDirections (this HexDirection direction)
		{
			int[] nearbyNumbers =
			{
				(int) direction - 1,
				(int) direction + 1,
			};
			int maxEnumValue = Enum.GetValues(typeof(HexDirection)).Cast<int>().Max();
			if (nearbyNumbers[0] < 0) nearbyNumbers[0] = maxEnumValue;
			if (nearbyNumbers[1] > maxEnumValue) nearbyNumbers[1] = 0;
			return Array.ConvertAll(nearbyNumbers, x => (HexDirection) x);
		}
	}
}