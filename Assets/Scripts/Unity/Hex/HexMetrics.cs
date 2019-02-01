using UnityEngine;

namespace Unity.Hex
{
	public static class HexMetrics
	{

		public const float OuterRadius = 10f;
		public const float InnerRadius = OuterRadius * 0.866025404f;
		public static readonly Vector3[] Corners = {
			new Vector3(0f, 0f, OuterRadius),
			new Vector3(InnerRadius, 0f, 0.5f * OuterRadius),
			new Vector3(InnerRadius, 0f, -0.5f * OuterRadius),
			new Vector3(0f, 0f, -OuterRadius),
			new Vector3(-InnerRadius, 0f, -0.5f * OuterRadius),
			new Vector3(-InnerRadius, 0f, 0.5f * OuterRadius),
			new Vector3(0f, 0f, OuterRadius)
		};
	}
}