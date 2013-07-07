using UnityEngine;
using System.Collections;

public class JellyAnim : MonoBehaviour
{

	public AnimationCurve Hump;
	public AnimationCurve zMovement;
	public float PoofHeight;
	
	private Vector3[] v3a_origin;
	private float meshHeight;
	private float zMin = 0;
	// Use this for initialization
	void Start ()
	{
		v3a_origin = GetComponent<MeshFilter>().mesh.vertices;
		
		float zMax = 0;
		
		for(int i = 0; i < v3a_origin.Length; i++)
		{
			zMax = Mathf.Max(zMax, v3a_origin[i].z);
			zMin = Mathf.Min(zMin, v3a_origin[i].z);
		}
		
		meshHeight = zMax - zMin;

	}
	
	// Update is called once per frame
	void Update ()
	{
		
		if(Input.GetButtonDown("Jump"))
			StartCoroutine(pushCycle());
		
		Camera.main.transform.LookAt(transform);
		
	}
	
	Mesh copyMesh(Mesh mesh)
	{
		Mesh newmesh = new Mesh();
        newmesh.vertices = mesh.vertices;
        newmesh.triangles = mesh.triangles;
        newmesh.uv = mesh.uv;
        newmesh.normals = mesh.normals;
        newmesh.colors = mesh.colors;
        newmesh.tangents = mesh.tangents;
		
		return newmesh;
	}
	
	float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up) {
		Vector3 perp = Vector3.Cross(fwd, targetDir);
		float dir = Vector3.Dot(perp, up);
		
		if (dir > 0f) {
			return 1f;
		} else if (dir < 0f) {
			return -1f;
		} else {
			return 0f;
		}
	}
	
	IEnumerator pushCycle()
	{
		float duration = 1f;
		float counter = duration;
		
		Vector3[] vertices = new Vector3[v3a_origin.Length];
		
		Mesh newmesh;
		
		while(counter > 0)
		{
		
			System.Array.Copy(v3a_origin, vertices, v3a_origin.Length);
			
			float percentDone = counter / duration;
			
			for(int i = 0; i < v3a_origin.Length; i++)
			{
				vertices[i] = newVertexPosition(vertices[i], percentDone);
			}
			
			GetComponent<MeshFilter>().mesh.vertices = vertices;
			GetComponent<MeshFilter>().mesh.RecalculateNormals();
			
			rigidbody.AddRelativeForce(Vector3.forward * zMovement.Evaluate(1f - counter / duration));
			counter -= Time.deltaTime;
			//Debug.Log("Evaluating: " + ((vertices[0].z - zMin) / meshHeight + counter - 1));
			yield return null;
		}
		
		GetComponent<MeshFilter>().mesh.vertices = v3a_origin;
		GetComponent<MeshFilter>().mesh.RecalculateNormals();
		
		
		yield return null;
	}
	
	Vector3 newVertexPosition(Vector3 original, float percentDone)
	{
		Vector3 zNormalized = new Vector3(original.x, original.y, 0);
				
		float angle = Vector3.Angle(Vector3.right, zNormalized);
		if(AngleDir(Vector3.right, Vector3.forward, zNormalized) > 0)
			angle = 360 - angle;
		angle *=  Mathf.Deg2Rad;
		
		return original + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0 ) //Push Out 1
			* Hump.Evaluate(percentDone * 2f - (original.z - zMin) / meshHeight) //Add hump shape
			* PoofHeight; // multiply by arbitrary scale
		
		
	}
}

