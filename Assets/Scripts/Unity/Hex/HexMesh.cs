using System.Collections.Generic;
using UnityEngine;

namespace Unity.Hex
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class HexMesh : MonoBehaviour
	{
		public Mesh Mesh { get; private set; }
		private MeshCollider _meshCollider;

		private List<Vector3> _vertices;
		private List<int> _triangles;
		private List<Color> _colors;

		public void Init()
		{
			GetComponent<MeshFilter>().mesh = Mesh = new Mesh();
			_meshCollider = gameObject.AddComponent<MeshCollider>();
			Mesh.name = "Hex Mesh";
			_vertices = new List<Vector3>();
			_triangles = new List<int>();
			_colors = new List<Color>();

		}
		public void Triangulate(List<DrawnHexCell> cells)
		{
			Mesh.Clear();
			_vertices.Clear();
			_triangles.Clear();
			_colors.Clear();
			//for (var i = 0; i < cells.Length; i++)
			//{
			//	Triangulate(cells[i]);
			//}
			cells.ForEach(Triangulate);
			Mesh.vertices = _vertices.ToArray();
			Mesh.colors = _colors.ToArray();
			Mesh.triangles = _triangles.ToArray();
			Mesh.RecalculateNormals();
			_meshCollider.sharedMesh = Mesh;
		}

		private void Triangulate(DrawnHexCell cell)
		{
			Vector3 center = cell.transform.localPosition;
			for (var i = 0; i < 6; i++)
			{
				AddTriangle(
					center,
					center + HexMetrics.Corners[i],
					center + HexMetrics.Corners[i + 1]
				);
				AddTriangleColor(cell.Color);
			}
		}

		private void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
		{
			var vertexIndex = _vertices.Count;
			_vertices.Add(v1);
			_vertices.Add(v2);
			_vertices.Add(v3);
			_triangles.Add(vertexIndex);
			_triangles.Add(vertexIndex + 1);
			_triangles.Add(vertexIndex + 2);
		}

		private void AddTriangleColor(Color color)
		{
			_colors.Add(color);
			_colors.Add(color);
			_colors.Add(color);
		}
	}
}