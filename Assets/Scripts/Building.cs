using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class Building : MonoBehaviour
{

	public GameObject WinFab;
	public Material BaseMaterial;
	
	private List<GameObject> windows = new List<GameObject>();
	private Material mat;
	private ParticleSystem.Particle[] particles;
	private ParticleSystem.Particle defaultParticle;
	
	// Use this for initialization
	void Awake ()
	{
		mat = new Material(BaseMaterial);
		particles = new ParticleSystem.Particle[0];
		
		MasterSettings ms = MasterSettings.me;
		defaultParticle = new ParticleSystem.Particle();
		defaultParticle.size = ms.WindowSize;
		defaultParticle.color = Color.white;
		defaultParticle.startLifetime = int.MaxValue;
		defaultParticle.lifetime = int.MaxValue;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.I))
			StartCoroutine(rePosition(new Vector3(.01f, .5f, 1)));
	}
	
	public void AddProject(KSProject project)
	{
		AddProject(project, "Bar");
	}
	
	public void AddProject(KSProject project, string positioning)
	{
		MasterSettings ms = MasterSettings.me;
		ParticleSystem.Particle temp = new ParticleSystem.Particle();
		int arraySize = particleSystem.GetParticles(particles);
		
		temp.size = ms.WindowSize;
		temp.color = KSJSON.me.parentCatColors[project.parentCat];
		temp.startLifetime = int.MaxValue;
		temp.lifetime = int.MaxValue;
		
		switch(positioning.ToLower())
		{
			case "scatter":
				temp.position = ScatterPosition(project);
				break;
			case "bar":	
			default:
				temp.position = BarPosition(arraySize);
				break;
		}
		
		Array.Resize<ParticleSystem.Particle>(ref particles, arraySize + 1);
		particles[arraySize] = temp;
		
		particleSystem.SetParticles(particles, arraySize + 1);
	}
	
	public void OldAddProject(KSProject project)
	{
		MasterSettings ms = MasterSettings.me;
		GameObject temp = (GameObject)Instantiate(WinFab);
		temp.GetComponent<Window>().Project = project;
		temp.transform.parent = transform;
		temp.transform.localScale = Vector3.one * ms.WindowSize;
		temp.transform.localPosition = new Vector3(0,0,(float)windows.Count * (ms.WindowSize + ms.WindowSpacing));
		mat.color = KSJSON.me.parentCatColors[project.parentCat];
		temp.renderer.material = mat;
		windows.Add(temp);
		if(windows.Count % 1000 == 0 && false)
		{
			StaticBatch();
		}
			
	}
	
	public void StaticBatch()
	{
		StaticBatchingUtility.Combine(gameObject);
	}
	
	Vector3 BarPosition(int i)
	{
		MasterSettings ms = MasterSettings.me;
		
		float scale = (ms.WindowSize + ms.WindowSpacing);
		float z = Mathf.Floor(i / 100);
		float x = Mathf.Floor((i - z * 100) / 10);
		float y = i - z * 100 - x * 10;
		
		return new Vector3(x,y,z) * scale;
		
	}

	Vector3 ScatterPosition (KSProject project)
	{
		float raisedMax = 10000f;
		float goalMax = 10000f;
		float backersMax = 1000f;
		
		float graphSize = 10f;
		
		float x = project.raised / raisedMax * graphSize;
		float y = project.backers / backersMax * graphSize;
		float z = project.goal / goalMax * graphSize;
		
		return new Vector3(x,y,z);

	}
	
	public IEnumerator rePosition(Vector3 scale)
	{
		int arraySize = particleSystem.GetParticles(particles);	
		
		Easing[] easers = new Easing[arraySize]; 
		
		for(int i = 0; i < arraySize; i++)
			easers[i] = new Easing(Easing.EaseType.Ease, particles[i].position, Vector3.Scale(particles[i].position, scale), 2f);
		
		yield return null;
		
		while(!easers[0].finished)
		{
			Debug.Log("here");
			
			for(int i = 0; i < arraySize; i++)
				particles[i].position = easers[i].Vector3;
			particleSystem.SetParticles(particles, arraySize);
			yield return null;
				
		}
		yield return null;
	}
}

