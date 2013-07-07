using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MeshHelper
{
	private List<Vector3> l_verticies;
	private List<int> l_triangles;
	private List<Vector2> l_uvs;
	
	public Mesh mesh{
		get{
			Mesh m = new Mesh();
			
			m.Clear();
			m.vertices = l_verticies.ToArray();
			m.triangles = l_triangles.ToArray();
			m.uv = l_uvs.ToArray();
			m.uv1 = m.uv;
			m.uv2 = m.uv;
			
			m.RecalculateNormals();
			m.RecalculateBounds();
			m.Optimize();
			
			return m;
		}
	}
	
	private void init()
	{
		l_verticies = new List<Vector3>();
		l_triangles = new List<int>();
		l_uvs = new List<Vector2>();
	}
	
	public MeshHelper()
	{
		init();

	}
	
	public MeshHelper(Mesh m)
	{
		init();
		
		addMesh(m);

	}
	
	public MeshHelper(Vector3[] verts)
	{
		init();
		l_verticies.AddRange(verts);

	}
	
	public MeshHelper(Vector3[] verts, Vector2[] uvs)
	{
		init();
		l_verticies.AddRange(verts);
		l_uvs.AddRange(uvs);
	}
	
	public MeshHelper (Mesh[] meshes)
	{

		l_verticies = new List<Vector3>();
		l_triangles = new List<int>();
		l_uvs = new List<Vector2>();
		
		foreach(Mesh m in meshes)
		{
			addMesh(m);
		}
			
	}
	
	public void addMesh(Mesh mesh)
	{
		int vOff = l_verticies.Count;
		
		l_verticies.AddRange(mesh.vertices);
		l_uvs.AddRange(mesh.uv);
		l_triangles.AddRange(mesh.triangles.Select(x => x + vOff).ToArray());
		
	}
	
	public void addTri(int[] ia)
	{
		l_triangles.AddRange(ia);
	}
	
	public void addTri(int a, int b, int c)
	{
		addTri(new int[]{a, b, c});
	}
	
	//Counter Clockwise as facing the quad
	public void addQuad(int[] ia)
	{
		addTri(new int[]{ ia[0], ia[1], ia[2] });
		addTri(new int[]{ ia[2], ia[3], ia[0] });
	}
	
	public void addQuad(int a, int b, int c, int d)
	{
		addQuad(new int[]{a, b, c, d});
	}
	
	public void addQuad(Vector3[] points, Vector2[] uvs)
	{
		l_verticies.AddRange(points);
		l_uvs.AddRange(uvs);
		int i = l_verticies.Count;
		
		addQuad(i-4, i-3, i-2, i-1);
	}
}


