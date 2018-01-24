using System.Collections.Generic;
using UnityEngine;

namespace Hex
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class HexMesh : MonoBehaviour
	{

		Mesh _hexMesh;
		MeshCollider _meshCollider;

		List<Vector3> _vertices;
		List<int> _triangles;
		List<Color> _colors;

		public void Init()
		{
			GetComponent<MeshFilter>().mesh = _hexMesh = new Mesh();
			_meshCollider = gameObject.AddComponent<MeshCollider>();
			_hexMesh.name = "Hex Mesh";
			_vertices = new List<Vector3>();
			_triangles = new List<int>();
			_colors = new List<Color>();

		}
		public void Triangulate(List<HexCell> cells)
		{
			_hexMesh.Clear();
			_vertices.Clear();
			_triangles.Clear();
			_colors.Clear();
			//for (var i = 0; i < cells.Length; i++)
			//{
			//	Triangulate(cells[i]);
			//}
			cells.ForEach(Triangulate);
			_hexMesh.vertices = _vertices.ToArray();
			_hexMesh.colors = _colors.ToArray();
			_hexMesh.triangles = _triangles.ToArray();
			_hexMesh.RecalculateNormals();
			_meshCollider.sharedMesh = _hexMesh;
		}

		void Triangulate(HexCell cell)
		{
			var center = cell.transform.localPosition;
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
		void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
		{
			var vertexIndex = _vertices.Count;
			_vertices.Add(v1);
			_vertices.Add(v2);
			_vertices.Add(v3);
			_triangles.Add(vertexIndex);
			_triangles.Add(vertexIndex + 1);
			_triangles.Add(vertexIndex + 2);
		}

		void AddTriangleColor(Color color)
		{
			_colors.Add(color);
			_colors.Add(color);
			_colors.Add(color);
		}
	}
}