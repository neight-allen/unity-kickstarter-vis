using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoralArm : MonoBehaviour
{

	float f_polypHeight;
	float f_polypAngle;
	int i_polypsPerNode;
	float f_skinRadius;
	float f_nodeSpread;
	int i_circlePoints;
	int i_pointsPerNode;
	float f_tension;
	int i_coneHeight;
	Vector3[] v3a_nodes;
	Vector3[] v3a_spinePoints;
	Mesh m_skin;
	GameObject[] go_polyps;
	GameObject pf_polyp;
	KSProject[] ks_projects;
	
	
	public string parentCat;
	
	bool drawLine = false;
	
	
	// Use this for initialization
	void Start ()
	{
		go_polyps = new GameObject[1000];
		
		MasterSettings ms = MasterSettings.me;
		
		i_polypsPerNode = ms.polypsPerNode;
		f_polypHeight = ms.polypHeight;
		f_skinRadius = ms.skinRadius;
		f_polypAngle = ms.polypAngle;
		f_nodeSpread = ms.nodeSpread;
		f_tension = ms.tension;
		i_pointsPerNode = ms.pointsPerNode;
		i_circlePoints = ms.circlePoints;
		i_coneHeight = ms.coneHeight;
		pf_polyp = ms.polypFab;
		
		//drawLine = true;

		KSJSON.me.getProjects(gameObject, "parentCat=" + parentCat + "&limit=500");
	}

	// Update is called once per frame
	void Update ()
	{
		
	}
	
	public void setProjects(KSProject[] projects)
	{
		///Debug.Log(projects[0].name);
		
		ks_projects = projects;
		
		for(int i = 0; i < go_polyps.Length; i++)
			Destroy(go_polyps[i]);
		
		go_polyps = new GameObject[projects.Length];
		
		makeNodes();
		makeSpine();
		makeSkin();
		placePolyps();
		
		foreach(MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
			mr.material.color = Color.gray;
		
		
	}
	
	void makeNodes()
	{
		v3a_nodes = new Vector3[Mathf.CeilToInt((float)go_polyps.Length/(float)i_polypsPerNode) + 1];
		
		for(int i = 0; i < v3a_nodes.Length; i++)
		{
			Vector3 addition = new Vector3(
            	Random.Range(-f_nodeSpread, f_nodeSpread),
            	i * f_polypHeight * i_polypsPerNode,
            	Random.Range(-f_nodeSpread, f_nodeSpread)
           	);
			
			v3a_nodes[i] = addition;
			//Debug.Log(addition);
		}
	}
	
	void makeNodes(int seed)
	{
		Random.seed = seed;
		makeNodes();
	}
	
	void makeSpine()
	{
		
		List<Vector3> spinePointsList = new List<Vector3>();
		
		for(int i = 0; i < v3a_nodes.Length - 1; i++)
		{
			
			Vector3 P0 = (i == 0) ? v3a_nodes[i] : v3a_nodes[i-1];
			Vector3 P1 = v3a_nodes[i];
			Vector3 P2 = v3a_nodes[i+1];
			Vector3 P3 = (i == v3a_nodes.Length - 2) ? v3a_nodes[i+1] : v3a_nodes[i+2];
		 
			Vector3 T1 = f_tension * (P2 - P0);
			Vector3 T2 = f_tension * (P3 - P1);
			
			for(int step = 0; step < i_pointsPerNode; step++)
			{
				float t = (float)step / (float)i_pointsPerNode;
				float t2 = t*t; 
				float t3 = t2*t;
			 
				float Blend1 =  2*t3 - 3*t2 + 1;
				float Blend2 = -2*t3 + 3*t2;
				float Blend3 =    t3 - 2*t2 + t;
				float Blend4 =    t3 -   t2;
			
				spinePointsList.Add(Blend1*P1 + Blend2*P2 + Blend3*T1 + Blend4*T2);
			}
		}
		
		v3a_spinePoints = spinePointsList.ToArray();
		
		if(drawLine)
		{
			LineRenderer line = GetComponent<LineRenderer>();
			line.SetVertexCount(v3a_spinePoints.Length);
			for(int i = 0; i < v3a_spinePoints.Length; i++)
				line.SetPosition(i, v3a_spinePoints[i]);
		}
				
	}
	
	void placePolyps()
	{
		for(int i = 0; i < go_polyps.Length; i++)
		{
			Quaternion polypRot = Quaternion.Euler(0, (137.5f * i) % 360f, 0);
			Vector3 polypPos = Vector3.up * i * f_polypHeight;
			float alongArm = (float)i / (float)i_polypsPerNode;
			polypPos = getSplinePoint(Mathf.FloorToInt(alongArm), alongArm - Mathf.FloorToInt(alongArm));
			GameObject temp = (GameObject)Instantiate(pf_polyp);
			temp.transform.parent = transform;
			temp.transform.localPosition = polypPos;
			temp.transform.localRotation = polypRot;
			temp.transform.Translate(Vector3.forward * f_skinRadius);
			float f_polypJitter = 10;
			Vector3 randomV3 = new Vector3(
				Random.Range(-f_polypJitter, f_polypJitter),
				Random.Range(-f_polypJitter, f_polypJitter),
				Random.Range(-f_polypJitter, f_polypJitter)
			);
			//temp.transform.Rotate(randomV3);
			
			temp.GetComponent<CoralPolyp>().setProject(ks_projects[i]);
			go_polyps[i] = temp;
		}
	}
	
	void makeSkin()
	{
		Vector3[] verts = new Vector3[v3a_spinePoints.Length * (i_circlePoints + 1)];
		Vector2[] uvs = new Vector2[verts.Length];
		
		for(int sp = 0; sp < v3a_spinePoints.Length; sp++)
		//for(int sp = 0; sp < 2; sp++)
		{
			for(int cp = 0; cp <= i_circlePoints; cp++)
			{
				int i = sp * (i_circlePoints + 1) + cp;
				float angle = 360f * Mathf.Deg2Rad / (float)i_circlePoints * cp;
				float radiusModifier = (v3a_spinePoints.Length - sp > i_coneHeight) ? 1 : (float)(v3a_spinePoints.Length - sp - 1) / (float)i_coneHeight;
				radiusModifier = (Mathf.Sin((float)cp / 5f) + 1f) / 2f;
				float x = Mathf.Cos(angle) * f_skinRadius * radiusModifier + v3a_spinePoints[sp].x;
				float z = Mathf.Sin(angle) * f_skinRadius * radiusModifier + v3a_spinePoints[sp].z;
				verts[i] = new Vector3(x, v3a_spinePoints[sp].y, z);
				float uvx = (float)cp / (float)i_circlePoints;
				float uvy = (float)sp / (float)v3a_spinePoints.Length * (float)i_pointsPerNode;
				uvs[i] = new Vector2(uvx, uvy);

				//TODO: rotate circle around tangent of point
			}
		}
		
		MeshHelper mh = new MeshHelper(verts, uvs);
		
		for(int sp = 0; sp < v3a_spinePoints.Length - 1; sp++)
		{
			for(int cp = 0; cp < i_circlePoints; cp++)
			{
				int i = sp * (i_circlePoints + 1) + cp;
				if(cp < i_circlePoints )
					mh.addQuad(i, i+i_circlePoints+1, i+2+i_circlePoints, i+1);
					//mh.addQuad(i, i+1, i+1+i_circlePoints, i+i_circlePoints);
				else
					mh.addQuad(i, i+i_circlePoints, i+1, i-i_circlePoints + 1);
			}
		}
		
		m_skin = mh.mesh;
		
		GetComponent<MeshFilter>().mesh = m_skin;
		
		//Debug.Log(verts.Length);
		//Debug.Log(GetComponent<MeshFilter>().mesh.vertexCount);
	}
	
	Vector3 getSplinePoint(int i, float t)
	{
		if(i == v3a_nodes.Length - 1)
		{
			i = v3a_nodes.Length - 2;
			t = 1;
		}
		
		Vector3 P0 = (i == 0) ? v3a_nodes[i] : v3a_nodes[i-1];
		Vector3 P1 = v3a_nodes[i];
		Vector3 P2 = v3a_nodes[i+1];
		Vector3 P3 = (i == v3a_nodes.Length - 2) ? v3a_nodes[i+1] : v3a_nodes[i+2];
	 
		Vector3 T1 = f_tension * (P2 - P0);
		Vector3 T2 = f_tension * (P3 - P1);
			
		float t2 = t*t; 
		float t3 = t2*t;
	 
		float Blend1 =  2*t3 - 3*t2 + 1;
		float Blend2 = -2*t3 + 3*t2;
		float Blend3 =    t3 - 2*t2 + t;
		float Blend4 =    t3 -   t2;
	
		return(Blend1*P1 + Blend2*P2 + Blend3*T1 + Blend4*T2);
		
	}
}

