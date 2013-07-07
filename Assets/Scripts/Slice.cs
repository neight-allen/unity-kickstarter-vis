using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Slice : MonoBehaviour
{

	Mesh m; //mesh
	
	public int resolution = 60; //resolution
	
	[System.SerializableAttribute]
	public class SliceProps{
		public float r; //Radius
		public float ir; //Inner Radius
		public float h; // height;
		public float bA; //Beginning Angle
		public float eA; //Ending Angle
		public float texColumn;
		
		public bool compare(SliceProps o)
		{
			return r == o.r &&
				ir == o.ir &&
				h  == o.h  &&
				bA == o.bA &&
				eA == o.eA &&
				texColumn == o.texColumn;
		}
		
		public void setEqual(SliceProps o)
		{
			r  = o.r;
			ir = o.ir;
			h  = o.h;
			bA = o.bA;
			eA = o.eA;
			texColumn = o.texColumn;
		}
	}
	
	public SliceProps props;
	private SliceProps lastProps;
	
	public float seperationDistance = .5f;
	public float seperationDuration = .5f;
	
	public bool mouseOverAble = false;
	
	void Awake()
	{
		props = new SliceProps();
		lastProps = new SliceProps();
	}
	
	// Use this for initialization
	void Start ()
	{
		m = new Mesh();
		
		//updateSlice();
		
		lastProps.setEqual(props);
		
	}

	// Update is called once per frame
	void Update ()
	{
		if(!props.compare(lastProps))
		{
			props.r = Mathf.Max(props.r, props.ir);
			
			if(props.ir == 0)
				updateSlice();
			else
				updateDSlice();
			lastProps.setEqual(props);
		}
		
		//if(Input.GetButtonDown("Fire1"))
		//	StartCoroutine(openSlice());
		
		//if(Input.GetButtonDown("Fire2"))
		//	StartCoroutine(closeSlice());
	}
	
	void OnMouseEnter()
	{
		if(mouseOverAble)
			StartCoroutine(openSlice());
	}
	
	void OnMouseExit()
	{
		if(mouseOverAble)
			StartCoroutine(closeSlice());
	}
	
	Vector3 angleToPoint(float a, float d)
	{
		return new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a)) * d;
	}
	
	Vector3 angleToPoint(float a)
	{
		return angleToPoint(a, props.r);
	}
	
	public void open()
	{
		StartCoroutine(openSlice());
	}
	
	public void close()
	{
		StartCoroutine(closeSlice());
	}
	
	IEnumerator openSlice()
	{
		float theAngle = (props.eA + props.bA) / 2;
		
		Easing easer = new Easing(
			Easing.EaseType.Bounce, 
		    transform.localPosition,
          	new Vector3(Mathf.Cos(theAngle), 0, Mathf.Sin(theAngle)) * seperationDistance,
          	seperationDuration
      	);
		
		while(!easer.finished)
		{
			transform.localPosition = easer.Vector3;
			//h = .5f + easer.Float;
			yield return null;
		}
		
	}
	
	IEnumerator closeSlice()
	{
		float theAngle = (props.eA + props.bA) / 2;
		
		Easing easer = new Easing(
			Easing.EaseType.Linear, 
		    transform.localPosition,
          	Vector3.zero,
          	seperationDuration
      	);
		
		while(!easer.finished)
		{
			transform.localPosition = easer.Vector3;
			//h = 1.5f - easer.Float;
			yield return null;
		}
		
	}
	
	void updateDSlice()
	{
		m.Clear();
		
		if(props.bA > props.eA)
		{
			float temp = props.bA;
			props.bA = props.eA;
			props.eA = temp;
		}
		
		float segmentAngle = 2 * Mathf.PI / resolution;
		
		List<float> angles = new List<float>();
	
		angles.Add(props.bA);
		
		for(int i = 0; i < resolution; i++)
		{
			float currentAngle = i * segmentAngle;
			if(currentAngle != Mathf.Clamp(currentAngle, props.bA, props.eA)) continue;
			
			angles.Add(currentAngle);
		}
	
		angles.Add(props.eA);
		
		int numPoints = angles.Count * 4;
		
		Vector3[] verts = new Vector3[numPoints];
		Vector2[] uvs = new Vector2[verts.Length];
		
		int a = 0;
		foreach(float angle in angles)
		{
			verts[a] = angleToPoint(angle, props.ir);
			verts[a+1] = verts[a] + new Vector3(0,props.h,0);
			verts[a+2] = angleToPoint(angle, props.r);
			verts[a+3] = verts[a+2] + new Vector3(0,props.h,0);
		
			uvs[a] = uvs[a+1] = new Vector2(props.texColumn, 1f - (props.ir/props.r));
			uvs[a+2] = uvs[a+3] = new Vector2(props.texColumn,0.01f);
			
			a += 4;
		}
		
		MeshHelper mh = new MeshHelper(verts, uvs);
		
		for(int i = 0; i < numPoints - 4; i += 4)
		{
			//bottom face
			mh.addQuad(i, i+2, i+6, i+4);			
			//top face
			mh.addQuad(i+1, i+5, i+7, i+3);			
			//back face
			mh.addQuad(i+2, i+3, i+7, i+6);
			//inner face
			mh.addQuad(i, i+4, i+5, i+1);
		}
		
		//Side1
		
		a = 0;		
		mh.addQuad(
		new Vector3[] {
			verts[a],
			verts[a+1],
			verts[a+3],
			verts[a+2]
		}, new Vector2[] {
			uvs[a],
			uvs[a+1],
			uvs[a+3],
			uvs[a+2]
		});		
		
		
		//Side2	
		a = numPoints - 4;
		mh.addQuad(
		new Vector3[] {
			verts[a],
			verts[a+2],
			verts[a+3],
			verts[a+1]
		}, new Vector2[] {
			uvs[a],
			uvs[a+2],
			uvs[a+3],
			uvs[a+1]
		});		
		 
		GetComponent<MeshFilter>().mesh = mh.mesh;
		if(mouseOverAble)
			GetComponent<MeshCollider>().sharedMesh = mh.mesh;
	}
	
	void updateSlice(){
		m.Clear();
		
		if(props.bA > props.eA)
		{
			float temp = props.bA;
			props.bA = props.eA;
			props.eA = temp;
		}
		
		float segmentAngle = 2 * Mathf.PI / resolution;
		
		List<float> angles = new List<float>();
	
		angles.Add(props.bA);
		
		for(int i = 0; i < resolution; i++)
		{
			float currentAngle = i * segmentAngle;
			if(currentAngle != Mathf.Clamp(currentAngle, props.bA, props.eA)) continue;
			
			angles.Add(currentAngle);
		}
	
		angles.Add(props.eA);
		
		int numPoints = angles.Count * 2 + 2;
		
		Vector3[] verts = new Vector3[numPoints];
		verts[0] = Vector3.zero; //Center point
	
		int a = 2;
		foreach(float angle in angles)
		{
			verts[a] = angleToPoint(angle);
			a += 2;
		}
		
		
		for(int i = 0; i < verts.Length - 1; i += 2)
		{
			verts[i+1] = verts[i] + new Vector3(0,props.h,0);
		}
		
		int[] tris = new int[(verts.Length) * 6];
		
		int ti = 0;
		
		for(int i = 2; i < numPoints - 2; i += 2)
		{
			//bottom face
			tris[ti++] = 0;
			tris[ti++] = i;
			tris[ti++] = i + 2;
			//back face 1
			tris[ti++] = i;
			tris[ti++] = i + 1;
			tris[ti++] = i + 2;
			
		}
		
		for(int i = 3; i < numPoints - 2; i += 2)
		{
			
			//top face
			tris[ti++] = i + 2;
			tris[ti++] = i;
			tris[ti++] = 1;
			//back face 2
			tris[ti++] = i + 2;
			tris[ti++] = i + 1;
			tris[ti++] = i;
		}

		
		Vector2[] uvs = new Vector2[verts.Length];
		
		uvs[0] = uvs[1] = new Vector2(props.texColumn,.99f);
		
		for(int i = 2; i < uvs.Length; i++)
			uvs[i] = new Vector2(props.texColumn,0.01f);
		
		
		m.vertices = verts;
		m.triangles = tris;
		m.uv = uvs;
		m.uv1 = uvs;
		m.uv2 = uvs;
		
		MeshHelper mh = new MeshHelper(m);
		
		Mesh side1 = new Mesh();
		Mesh side2 = new Mesh();
		
		//side 1
		Vector3[] s1Verts = new Vector3[4];
		s1Verts[0] = verts[0];
		s1Verts[1] = verts[1];
		s1Verts[2] = verts[2];
		s1Verts[3] = verts[3];
		
		mh.addQuad(0,1,2,3);
		
		
		Vector2[] s1uvs = new Vector2[4];
		s1uvs[0] = uvs[0];
		s1uvs[1] = uvs[1];
		s1uvs[2] = uvs[2];
		s1uvs[3] = uvs[3];
		
		
		int[] s1tris = new int[6];
		ti = 0;
		s1tris[ti++] = 0;
		s1tris[ti++] = 1;
		s1tris[ti++] = 2;
		
		s1tris[ti++] = 3;
		s1tris[ti++] = 2;
		s1tris[ti++] = 1;
		
		side1.vertices = s1Verts;
		side1.triangles = s1tris;
		side1.uv = s1uvs;
		
		//side 2
		Vector3[] s2Verts = new Vector3[4];
		s2Verts[0] = verts[0];
		s2Verts[1] = verts[1];
		s2Verts[2] = verts[verts.Length - 2];
		s2Verts[3] = verts[verts.Length - 1];
		
		mh.addQuad(verts.Length - 2, verts.Length - 1, 0, 1);
		
		Vector2[] s2uvs = new Vector2[4];
		s2uvs[0] = uvs[0];
		s2uvs[1] = uvs[1];
		s2uvs[2] = uvs[uvs.Length - 2];
		s2uvs[3] = uvs[uvs.Length - 1];
		
		int[] s2tris = new int[6];
		ti = 0;
		
		s2tris[ti++] = 2;
		s2tris[ti++] = 1;
		s2tris[ti++] = 0;
		
		s2tris[ti++] = 2;
		s2tris[ti++] = 3;
		s2tris[ti++] = 1;
		
		side2.vertices = s2Verts;
		side2.triangles = s2tris;
		side2.uv = s2uvs;
		
		MeshHelper mh2 = new MeshHelper(new Mesh[]{m, side1, side2});

		GetComponent<MeshFilter>().mesh = mh2.mesh;
		if(mouseOverAble)
			GetComponent<MeshCollider>().sharedMesh = mh.mesh;
		
	}
			
}

