using UnityEngine;
using System.Collections;

public class CoralPolyp : MonoBehaviour
{

	float f_contentAlpha;
	KSProject project;
	bool isOver;
	
	public GUISkin polypPopup;
	public GUIStyle GSHeader;
	public GUIStyle GSLabel;
	
	// Use this for initialization
	void Start ()
	{
		f_contentAlpha = 0;	
		isOver = false;
	}

	// Update is called once per frame
	void Update ()
	{
		//isOver = false;
	}
	
	void OnGUI()
	{
		
		if(f_contentAlpha > 0)
		{

			GUI.contentColor = Color.Lerp(Color.clear, Color.white, f_contentAlpha);
			GUI.backgroundColor = Color.Lerp(Color.clear, Color.white, f_contentAlpha);
			
			//Debug.Log("Content Color: " + f_contentAlpha + ". " + project.name + " is at " + transform.position);
			
			Vector3 anchorPoint = transform.position;// + Vector3.up * transform.localScale.z * 5f + Vector3.left * transform.localScale.x * 5f;
			Vector3 startPoint = Camera.main.WorldToScreenPoint(anchorPoint);
			
			Rect bounds = new Rect(startPoint.x + 30, Screen.height - startPoint.y - 100, 200, 200);
			
			GUILayout.BeginArea(bounds, polypPopup.box);
			{
				GUILayout.BeginVertical();
				{
					GUILayout.Label("Project:", GSHeader);
					GUILayout.Label(project.name);
					GUILayout.Label("Goal:", GSHeader);
					GUILayout.Label(project.goal.ToString("C2"));
					GUILayout.Label("Raised:", GSHeader);
					GUILayout.Label(project.raised.ToString("C2"));
					GUILayout.Label("Goal:", GSHeader);
					GUILayout.Label((project.goal / project.raised).ToString() + "%");
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndArea();
			//GUI.Label(new Rect(0,0,1000,1000), project.name);
			
		}
	}
	
	void OnMouseEnter()
	{
		isOver = true;
		//Debug.Log("Over " + project.name);
	}
	
	void OnMouseExit()
	{
		isOver = false;
	}
	
	void LateUpdate()
	{
		if(isOver)
			f_contentAlpha += 3f * Time.deltaTime;
		else
			f_contentAlpha -= 3f * Time.deltaTime;
		f_contentAlpha = Mathf.Clamp(f_contentAlpha, 0f, 1f);
	}
	
	public void setProject(KSProject project)
	{
		this.project = project;
		//StartCoroutine(wiggle());
		
	}
	
	IEnumerator wiggle()
	{
		float xOffset = Random.Range(0, Mathf.PI);
		float yOffset = Random.Range(0, Mathf.PI);
		float xPeriod = Random.Range(5f,10f);
		float yPeriod = Random.Range(5f,10f);
		
		float wiggleDegs = 10f;
		
		Vector3 originalRot = transform.localRotation.eulerAngles;
		
		while(true)
		{
			Vector3 deltaRot = new Vector3(Mathf.Cos(Time.time / xPeriod + xOffset) * wiggleDegs, 0, Mathf.Sin(Time.time / yPeriod + yOffset));
			transform.localRotation = Quaternion.Euler(originalRot + deltaRot);
			yield return null;
		}
		
		yield return null;
	}
}

