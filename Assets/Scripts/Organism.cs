using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DelaunayTriangulator;

public class Organism : MonoBehaviour
{

	public Shader s;
	public GameObject g;
	
	private List<Triad> tris;
	private List<Vertex> vertexes;
	private Triangulator trimaker;
	private Material m;
	
	bool drawme;
	int[] sortedIndices;
	int drawthis = 0;
	private ParticleSystem.Particle[] ps;
	// Use this for initialization
	void Start ()
	{
		vertexes = new List<Vertex>();
		tris = new List<Triad>();
		trimaker = new Triangulator();
		m = new Material(s);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	void OnPostRender() {
		if(tris.Count > 0)
		{
			ps = GameObject.Find("City").GetComponent<City>().pps.particles.ToArray();
			m.SetPass(0);
			GL.PushMatrix();
			GL.MultMatrix(g.transform.transform.localToWorldMatrix);
			
			//GL.Color( new Color(1,1,1,.4f) );
			
			
			GL.Begin( GL.LINES );
			float alpha = .3f;
			for(int i = 0; i < tris.Count; i++) {
				Triad t = tris[i];
				
				
				Color newColor = ps[t.a].color;
				newColor.a = alpha;
				GL.Color( newColor );
				GL.Vertex3(ps[t.a].position.x, ps[t.a].position.y, ps[t.a].position.z);
				newColor = ps[t.b].color;
				newColor.a = alpha;
				GL.Color( newColor );
				GL.Vertex3(ps[t.b].position.x, ps[t.b].position.y, ps[t.b].position.z);
				

				GL.Vertex3(ps[t.b].position.x, ps[t.b].position.y, ps[t.b].position.z);
				newColor = ps[t.c].color;
				newColor.a = alpha;
				GL.Color( newColor );
				GL.Vertex3(ps[t.c].position.x, ps[t.c].position.y, ps[t.c].position.z);
				
				
				GL.Vertex3(ps[t.c].position.x, ps[t.c].position.y, ps[t.c].position.z);
				newColor = ps[t.a].color;
				newColor.a = alpha;
				GL.Color( newColor );
				GL.Vertex3(ps[t.a].position.x, ps[t.a].position.y, ps[t.a].position.z);
				
				
				
			}
			GL.End();
			
			
			GL.PopMatrix();
		}
		
		
		if(drawme)
		{
			m.SetPass(0);
			GL.PushMatrix();
			//GL.MultMatrix(g.transform.transform.localToWorldMatrix);
			GL.Begin( GL.LINES );
			GL.Color( new Color(1,1,1,.4f) );
			
			for(int i = 1; i < drawthis - 1; i++) {
				
				//GL.Vertex3(vertexes[0].x, vertexes[0].y, 0);
				//GL.Vertex3(vertexes[sortedIndices[i]].x, vertexes[sortedIndices[i]].y, 0);
				GL.Vertex3(vertexes[i].x, vertexes[i].y, 0);
				GL.Vertex3(vertexes[i+1].x, vertexes[i+1].y, 0);
			}
			
			GL.End();
			drawthis++;
			drawthis = Mathf.Min(drawthis, vertexes.Count);
			GL.PopMatrix();
		}
	} 
	
	public void makeTris(Vector2[] points)
	{
		vertexes.Clear();
		vertexes.Capacity = points.Length;
		
		for(int i = 0; i < points.Length; i++)
		{
			vertexes.Add(new Vertex(points[i].x, points[i].y));
		}
		
		tris = trimaker.Triangulation(vertexes, true);
		
		/*
		int nump = vertexes.Count;

        float[] distance2ToCentre = new float[nump];
        sortedIndices = new int[nump];
		
		for (int k = 0; k < nump; k++)
        {
            distance2ToCentre[k] = vertexes[0].distance2To(vertexes[k]);
            sortedIndices[k] = k;
        }

        // Sort by distance to seed point
        Array.Sort(distance2ToCentre, sortedIndices);
		
		drawme = true;
		drawthis = 0;
		
		vertexes = trimaker.ConvexHull(vertexes, true);
		*/
		
		
	}
}

