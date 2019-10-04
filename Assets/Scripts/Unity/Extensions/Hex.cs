using NKMCore.Hex;
using Unity.Hex;
using UnityEngine;

namespace Unity.Extensions
{
    public static class Hex
    {
        public static HexCoordinates ToCoordinates(this Vector3 position)
        {
            float x = position.x / (HexMetrics.InnerRadius * 2f);
            float y = -x;
            float offset = position.z / (HexMetrics.OuterRadius * 3f);
            x -= offset;
            y -= offset;
            int iX = Mathf.RoundToInt(x);
            int iY = Mathf.RoundToInt(y);
            int iZ = Mathf.RoundToInt(-x - y);
            if (iX + iY + iZ == 0) return new HexCoordinates(iX, iZ);
            float dX = Mathf.Abs(x - iX);
            float dY = Mathf.Abs(y - iY);
            float dZ = Mathf.Abs(-x - y - iZ);

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