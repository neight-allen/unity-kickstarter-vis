using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpikeBall : MonoBehaviour
{

	private Mesh mesh;
	private Vector3[] originalVerts;
	public string type;
	private Dictionary<string,Color> colors = new Dictionary<string, Color>{
		{"Dance", Color.yellow},
		{"Technology", Color.green},
		{"Music", Color.red}
	};
	
	// Use this for initialization
	void Start ()
	{
		mesh = GetComponent<MeshFilter>().mesh;
		originalVerts = mesh.vertices;
		
		renderer.material.color = colors[type];
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(Input.GetButtonDown("Jump"))
			StartCoroutine(BuildSpikes());// SetSpikes();
		
		transform.Rotate(Vector3.up * 1 * Time.deltaTime);
	}
	
	void SetSpikes()
	{
		mesh.vertices = GetSpikes();
		mesh.RecalculateNormals();
			
	}
	
	Vector3[] GetSpikes()
	{
		Vector3[] verts = mesh.vertices;
		float[] values = new float[0];
		
		/*
		if(type == "Dance") values = KckData.me.danceGoals;
		if(type == "Music") values = KckData.me.musicGoals;
		if(type == "Technology") values = KckData.me.techGoals;
		*/
		
		if(type == "Dance") values = KckData.me.danceRaised;
		if(type == "Music") values = KckData.me.musicRaised;
		if(type == "Technology") values = KckData.me.techRaised;
		
		
		for(int i = 0; i < verts.Length; i++)
		{
			verts[i] = Vector3.Lerp(originalVerts[i], originalVerts[i] * values[i], 1f);
		}
		
		return verts;
	}
	
	IEnumerator BuildSpikes()
	{
		Vector3[] verts = GetSpikes();
		int num = verts.Length;
		
		Easing[] easers = new Easing[num];
		
		for(int i = 0; i < num; i++)
		{
			easers[i] = new Easing(Easing.EaseType.Back, originalVerts[i], verts[i], 10);
		}
		
		yield return null;
		
		while(!easers[0].finished)
		{
			Vector3[] temp = new Vector3[num];
			
			for(int i = 0; i < num; i++)
			{
				temp[i] = easers[i].Vector3;
			}
			
			mesh.vertices = temp;
			mesh.RecalculateNormals();
			yield return null;
		}
		yield return null;
	}
}

