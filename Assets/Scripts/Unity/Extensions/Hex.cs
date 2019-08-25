using NKMCore.Hex;
using Unity.Hex;
using UnityEngine;

namespace Unity.Extensions
{
    public static class Hex
    {
        public static HexCoordinates ToCoordinates(this Vector3 position)
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
    }
}