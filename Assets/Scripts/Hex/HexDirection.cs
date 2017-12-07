namespace Hex
{
	public enum HexDirection {
		Ne, E, Se, Sw, W, Nw
	}

	public static class HexDirectionExtensions {

		public static HexDirection Opposite (this HexDirection direction) {
			return (int)direction < 3 ? (direction + 3) : (direction - 3);
		}
	}
}