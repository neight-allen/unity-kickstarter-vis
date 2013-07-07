using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ProjectParticleSet
{
	public List<ParticleSystem.Particle> particles;
	public List<KSProject> projects;
	
	public ProjectParticleSet()
	{
		//todo: Dynamically increment capacity to support more projects in the future
		particles = new List<ParticleSystem.Particle>(77000);
		projects = new List<KSProject>(77000);
	}
	
	public void ToParticleSystem(ParticleSystem ps)
	{
		ps.SetParticles(this.particles.ToArray(), this.particles.Count);
	}
	
}

public class City : MonoBehaviour
{
	public GameObject BuildingFab;
	public GameObject WindowFab;
	public AudioClip plugIn;
	
	private int limit;
	private Dictionary<string, GameObject> buildings = new Dictionary<string, GameObject>();
	private Queue projectQueue = new Queue(20000);
	
	private float buildrate;
	private float timer = 0;
	
	private string[] projFiles = {"kickscrape_7_fields_1k", "kickscrape_7_fields_15k", "kickscrape_7_fields_40k", "kickscrape_7_fields_75k"};
	private bool[] projFileLoaded;
	private float[] chunkBarriers = {-.6f, -.2f, .2f, .6f};
	private float[] buildRates = {1/200f, 1/2000f, 1/3000f, 1/4000f};
	
	private bool firstFrame = true;
	private GraphAxis GA;
	
	private string rotating = "no";
	
	private KSMetric ks_zero = new KSMetric("Zero", float.MaxValue, "");
	
	public ProjectParticleSet pps;
	
	// Use this for initialization
	void Start ()
	{
		buildrate = 1f/10000f;
		
		GA = GraphAxis.me;
		
		GA.city = gameObject;
		
		pps = new ProjectParticleSet();

		string url = "http://127.0.0.1:8000/api/project/?format=json&limit=2000";//&order_by=date_end";
		if(MasterSettings.me.offline)
		{
			//KSJSON.me.getOffline(gameObject, "ProcessResponse", "15k");
			//KSJSON.me.getOffline(gameObject, "ProcessResponse", "15k");
			if(GraphAxis.me.UItype == UIType.Rockband)
			{
				projFileLoaded = new bool[projFiles.Length];
				for(int i = 0; i < projFileLoaded.Length; i++)
					projFileLoaded[i] = false;
			}
			else
			{
				KSCSV.me.loadCSV(gameObject, projFiles[0]);
				KSCSV.me.loadCSV(gameObject, projFiles[2]);
				KSCSV.me.loadCSV(gameObject, projFiles[1]);
			}
			
		}	
		else
			KSJSON.me.getURL(gameObject, "ProcessResponse", url);
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		for(int i = chunkBarriers.Length - 1; i >= 0; i--)
		{
			if(Input.GetAxis("Pickups") != 0 && Input.GetAxis("Pickups") > chunkBarriers[i] && !projFileLoaded[i])
			{
				AudioSource.PlayClipAtPoint(plugIn, Camera.main.transform.position);
				buildrate = buildRates[i];
				projFileLoaded[i] = true;
				KSCSV.me.loadCSV(gameObject, projFiles[i]);
			}
		}
	}
	
	public void ProcessResponse(KSProjectResponse response)
	{
		
		if(!string.IsNullOrEmpty(response.meta.next))// && response.meta.offset < 10001)
		{
			string url = "http://127.0.0.1:8000" + response.meta.next;
			KSJSON.me.getURL(gameObject, "ProcessResponse", url);
		}
		
		if(projectQueue.Count > 0)
		{
			//projectQueue.AddRange(response.objects);
			foreach(KSProject proj in response.objects)
				projectQueue.Enqueue(proj);
		}
		else
		{
			//projectQueue.AddRange(response.objects);
			foreach(KSProject proj in response.objects)
				projectQueue.Enqueue(proj);
			StartCoroutine(BuildProjects());
		}
		//Debug.Log("Queue Size: " + projectQueue.Count);
	}
	
	public void setRotation(float progress)
	{
		float degrees = Mathf.Clamp(progress, 0f, 1f);
		if(rotating == "y")
		{
			transform.rotation = Quaternion.Euler(degrees * -90, 0, 0);
		}
		
		if(rotating == "x")
		{
			transform.rotation = Quaternion.Euler(0, degrees * 90, 0);
		}
			
	}

	public void ZoomIn(){ StartCoroutine(changeScale(.5f)); }
	
	public void ZoomOut(){ StartCoroutine(changeScale(2f)); }
	
	public void ZoomInIn(){ StartCoroutine(changeScale(.1f)); }
	
	public void ZoomOutOut(){ StartCoroutine(changeScale(10f)); }
	
	public void ZoomNone(){ StartCoroutine(changeScale(1f)); }
	
	public void ZoomInstant(float multiplier)
	{
		GA.zoom *= multiplier;
		int num = pps.projects.Count;
		for(int i = 0; i < num; i++)
			{
				ParticleSystem.Particle p = pps.particles[i];
				p.position = placeParticle(pps.projects[i], KSMetric.List[GA.xLabel], KSMetric.List[GA.yLabel]);
				pps.particles[i] = p;
			}
			pps.ToParticleSystem(particleSystem);
	}

	IEnumerator changeScale (float multiplier)
	{
		GA.zoom *= multiplier;
		
		Easing easer = new Easing(Easing.EaseType.EasyEase, 2f);
		int num = pps.projects.Count;
		Vector3[] oldLocations = new Vector3[num];
		Vector3[] newLocations = new Vector3[num];
		
		for(int i = 0; i < num; i++)
		{
			oldLocations[i] = pps.particles[i].position;
			newLocations[i] = placeParticle(
					pps.projects[i],
					KSMetric.List[GA.xLabel], 
					KSMetric.List[GA.yLabel]
				);
		}
		
		while(!easer.finished)
		{
			for(int i = 0; i < num; i++)
			{
				ParticleSystem.Particle p = pps.particles[i];
				p.position = Vector3.Lerp(oldLocations[i], newLocations[i], easer.Float);
				pps.particles[i] = p;
			}
			pps.ToParticleSystem(particleSystem);
			yield return null;
		}
		
		yield return null;
			
	}

	public void RotateX ()
	{
		if(rotating != "no")
			return;
		rotating = "x";
		//transform.RotateAround(Vector3.up, 90 * Mathf.Deg2Rad);
		for(int i = 0; i < pps.particles.Count; i++)
		{
			ParticleSystem.Particle p = pps.particles[i];
			p.position += Vector3.forward * zValue(pps.projects[i], KSMetric.List[GA.xLabelNew]);
			pps.particles[i] = p;
		}
		pps.ToParticleSystem(particleSystem);
		StartCoroutine(_rotatex());
		
		//ToDo:
			//Then draw axis lables
			//Then figure out how to rotate axis labels
			
	}
	
	IEnumerator _rotatex()
	{
		
		float start = transform.localRotation.eulerAngles.y;
		float stop = start + 90;
		Easing rotation = new Easing(Easing.EaseType.Linear, start, stop, 3); 
		
		while(!rotation.finished)
		{
			Vector3 currentRot = transform.localRotation.eulerAngles;
			if(Easing.TimeScale != 0f)
				transform.localRotation = Quaternion.Euler(currentRot.x, rotation.Float, currentRot.z);
			yield return null;
		}
		finishXRotation();
		yield return null;
	}
	
	void finishXRotation()
	{
		for(int i = 0; i < pps.particles.Count; i++)
		{
			ParticleSystem.Particle p = pps.particles[i];
			p.position = new Vector3(p.position.z, p.position.y, 0);
			pps.particles[i] = p;
		}
		
		pps.ToParticleSystem(particleSystem);
		
		
		
		transform.localRotation = Quaternion.identity;
		GA.xLabel = GA.xLabelNew;
		makeTris();
		rotating = "no";
	}
	
	public void RotateY ()
	{
		if(rotating != "no")
			return;
		rotating = "y";
		
		//transform.RotateAround(Vector3.up, 90 * Mathf.Deg2Rad);
		for(int i = 0; i < pps.particles.Count; i++)
		{
			ParticleSystem.Particle p = pps.particles[i];
			p.position += Vector3.forward * zValue(pps.projects[i], KSMetric.List[GA.yLabelNew]);
			pps.particles[i] = p;
		}
		pps.ToParticleSystem(particleSystem);
		StartCoroutine(_rotatey());
	}
	
	IEnumerator _rotatey()
	{
		
		float start = transform.localRotation.eulerAngles.x;
		float stop = start - 90;
		Easing rotation = new Easing(Easing.EaseType.Linear, start, stop, 3); 
		
		while(!rotation.finished)
		{
			Vector3 currentRot = transform.localRotation.eulerAngles;
			if(Easing.TimeScale != 0f)
				transform.localRotation = Quaternion.Euler(rotation.Float, currentRot.y, currentRot.z);
			yield return null;
		}
		finishYRotation();
		yield return null;
	}
	
	void finishYRotation()
	{
		for(int i = 0; i < pps.particles.Count; i++)
		{
			ParticleSystem.Particle p = pps.particles[i];
			p.position = new Vector3(p.position.x, p.position.z, 0);
			pps.particles[i] = p;
		}
		
		pps.ToParticleSystem(particleSystem);
		
		transform.localRotation = Quaternion.identity;
		GA.yLabel = GA.yLabelNew;
		makeTris();
		
		rotating = "no";
	}
	

	

	
	//todo: Create highlight category function
	//todo: plug it into the toolbar
	
	public void HighlightCategory(string category)
	{
		//set the new colors now so we don't have to do a test for every particle
		Dictionary<string, Color> newColors = new Dictionary<string, Color>(KSJSON.me.parentCatColors);
		if(category != "All")
			foreach(var key in newColors.Keys.ToArray())
				if(key != category)
					newColors[key] = Color.clear; //new Color(newColors[key].r, newColors[key].g, newColors[key].b, .2f);
					//newColors[key] = Color.Lerp(new Color(1,1,1,0), newColors[key], .2f);
		
		for(int i = 0; i < pps.projects.Count; i++)
		{
			ParticleSystem.Particle p = pps.particles[i];
			p.color = newColors[pps.projects[i].parentCat];
			pps.particles[i] = p;
		}
			
		pps.ToParticleSystem(particleSystem);
	}
	
	IEnumerator BuildProjects()
	{
		MasterSettings ms = MasterSettings.me;
		Debug.Log(projectQueue.Count);
		while(projectQueue.Count > 0)
		{
			timer += Time.deltaTime;
			//buildrate = 3f/(float)projectQueue.Count;
			while(timer > 0)
			{
				if(projectQueue.Count < 1)
					break;
				KSProject proj = (KSProject)projectQueue.Dequeue();
				//Debug.Log(proj.ToString());
				
				ParticleSystem.Particle temp = new ParticleSystem.Particle();
				
				temp.size = ms.WindowSize;
				temp.color = KSJSON.me.parentCatColors[proj.parentCat];
				temp.startLifetime = int.MaxValue;
				temp.lifetime = int.MaxValue;
				
				temp.position = placeParticle(
					proj,
					KSMetric.List[GA.xLabel], 
					KSMetric.List[GA.yLabel]
				);				
				
				pps.projects.Add(proj);
				pps.particles.Add(temp);
				
				timer -= buildrate;
				GA.ProjectCount++;
			}
			pps.ToParticleSystem(particleSystem);
			yield return null;
		}
		
		makeTris();
		
		//Debug.Log("Finished Queue");
	}
	
	IEnumerator BuildHistogram()
	{
		MasterSettings ms = MasterSettings.me;
		Debug.Log(projectQueue.Count);
		while(projectQueue.Count > 0)
		{
			timer += Time.deltaTime;
			//buildrate = 3f/(float)projectQueue.Count;
			while(timer > 0)
			{
				if(projectQueue.Count < 1)
					break;
				KSProject proj = (KSProject)projectQueue.Dequeue();
				//Debug.Log(proj.ToString());
				
				ParticleSystem.Particle temp = new ParticleSystem.Particle();
				
				temp.size = ms.WindowSize;
				temp.color = KSJSON.me.parentCatColors[proj.parentCat];
				temp.startLifetime = int.MaxValue;
				temp.lifetime = int.MaxValue;
				
				temp.position = placeParticle(
					proj,
					KSMetric.List[GA.xLabel], 
					KSMetric.List[GA.yLabel]
				);
				
				pps.projects.Add(proj);
				pps.particles.Add(temp);
				
				timer -= buildrate;
				GA.ProjectCount++;
			}
			pps.ToParticleSystem(particleSystem);
			yield return null;
		}
	}
	
	IEnumerator OldBuildProjects()
	{
		while(projectQueue.Count > 0)
		{
			timer += Time.deltaTime;
			//buildrate = 3f/(float)projectQueue.Count;
			while(timer > 0)
			{
				if(projectQueue.Count < 1)
					break;
				KSProject proj = (KSProject)projectQueue.Dequeue();
				//Debug.Log(proj.ToString());
				
				if(!buildings.ContainsKey(proj.parentCat))
				{
					Vector3 loc = new Vector3(buildings.Count * 1.5f, 0,0);
					loc = Vector3.zero;
					GameObject temp = (GameObject)Instantiate(BuildingFab);
					temp.transform.parent = transform;
					temp.transform.localPosition = loc;
					
					buildings.Add(proj.parentCat, temp);
				}
				
				//buildings[proj.parentCat].GetComponent<Building>().AddProject(proj);
				buildings[proj.parentCat].GetComponent<Building>().AddProject(proj,"scatter");
				
				timer -= buildrate;
			}
			yield return null;
		}
		//foreach(var building in buildings)
			//building.Value.GetComponent<Building>().StaticBatch();
		//Debug.Log("Finished Queue");
	}

	Vector3 placeParticle (KSProject proj, KSMetric xMetric, KSMetric yMetric)
	{
		if(GraphAxis.me.yAxisVisible)
			return new Vector3(metricToValue(proj, xMetric), metricToValue(proj, yMetric));
		else
			return new Vector3(metricToValue(proj, xMetric), 0);
	}
		
	float zValue (KSProject proj, KSMetric zMetric)
	{
		return metricToValue(proj,  zMetric);
	}

	void makeTris ()
	{
		try
		{
			Organism orgy = Camera.main.GetComponent<Organism>();
			if(!orgy.enabled) return;
			
			Debug.Log(orgy);
			int offset = 0;
			int limit = 5000;
			Vector2[] points = new Vector2[limit];
			
			float bigOne = 0;
			int bigIndex = 0;
			
			for(int i = 0; i < limit; i++)
			{
				//points[i] = new Vector2(pps.projects[i + offset].goal, pps.projects[i + offset].raised);
				points[i] = new Vector2(pps.particles[i + offset].position.x, pps.particles[i + offset].position.y);
				/*float temp = points[i].x + points[i].y;
				if(temp > bigOne){
					bigIndex = i;	
					bigOne = temp;
				}*/
			}
			
//			Vector2 v2 = points[bigIndex];
//			points[bigIndex] = points[0];
//			points[0] = v2;
			
			orgy.makeTris(points);
		}
		catch (MissingComponentException e){
			return;
		}
		
		
	}
		
	float metricToValue(KSProject proj, KSMetric metric)
	{
		return metricToValue(proj, metric, "linear");
	}
	
	float metricToValue(KSProject proj, KSMetric metric, string type)
	{
		string label = metric.Name;
		GeoBounds gb = GeoBounds.List[GA.Geographic];
		
		if(label == "Latitude")
			return (proj.GetMetric("Lat") - gb.Bottom) * 10 / gb.Scale;
		if(label == "Longitude")
			return (proj.GetMetric("Lon") - gb.Left) * 10 / gb.Scale;
		
		float zoom = GA.zoom;
		
		float s, logValue;
		
		if(type == "linear")
			return proj.GetMetric(label) / metric.Zoom / zoom;
		else
		{
			//s ^ 10 = modifier * zoom * 10
			//s ^ x = value
			s = Mathf.Log(metric.Zoom * zoom, 10);
			logValue = proj.GetMetric(label) > 0 ? Mathf.Log(proj.GetMetric(label), s) : 0;
			
			//Debug.Log(s + " ^ " + logValue + " = " + proj.GetMetric(label));
			return logValue;
		}
		
		return 0;
		/*
		switch(label.ToLower())
		//switch(label)
		{
		case "raised":
			if(type == "linear")
				return proj.raised / Modifiers["Raised"] / zoom;
			else
			{
//				s ^ 10 = modifier * zoom * 10
//				s ^ x = value
				s = Mathf.Log(Modifiers["Raised"] * zoom, 10);
				logValue = proj.raised > 0 ? Mathf.Log(proj.raised, s) : 0;
				
				//Debug.Log(s + " ^ " + logValue + " = " + proj.raised);
				return logValue;
			}
			break;
		
		case "goal":
			//return proj.goal / Modifiers["Goal"] / zoom;
			val = proj.goal / Modifiers["Goal"] / zoom;
			if(type == "linear")
				return val;
			else
			{
				s = Mathf.Log(Modifiers["Goal"] * zoom, 10);
				logValue = proj.goal > 0 ? Mathf.Log(proj.goal, s) : 0;
				
				//Debug.Log(proj.raised + " ^ " + s + " = " + logValue);
				return logValue;
			}
			break;
		
		case "backers":
			//return proj.backers / Modifiers["Backers"] / zoom;
			if(type == "linear")
				return proj.backers / Modifiers["Backers"] / zoom;
			else
			{
				s = Mathf.Log(Modifiers["Backers"] * zoom, 10);
				logValue = proj.backers > 0 ? Mathf.Log(proj.backers, s) : 0;
				return logValue;
			}
			break;
		case "comments":
			//return proj.comments / Modifiers["Comments"] / zoom;
			if(type == "linear")
				return proj.comments / Modifiers["Comments"] / zoom;
			else
			{
				s = Mathf.Log(Modifiers["Comments"] * zoom, 10);
				logValue = proj.comments > 0 ? Mathf.Log(proj.comments, s) : 0;
				return logValue;
			}
			break;
		case "dpb":
			//return proj.backers == 0 ? 0 : proj.raised / proj.backers / Modifiers["DpB"] / zoom;
			if(type == "linear")
				return Mathf.Min(proj.backers, proj.raised) == 0 ? 0 : proj.raised / proj.backers / Modifiers["DpB"] / zoom;
			else
			{
				s = Mathf.Log(Modifiers["DpB"] * zoom, 10);
				logValue = proj.backers > 0 ? Mathf.Log(proj.raised / proj.backers, s) : 0;
				return logValue;
			}
			break;
		case "% raised":
			//return proj.raised / proj.goal / Modifiers["% Raised"] / zoom;
			if(type == "linear")
				return proj.raised / proj.goal / Modifiers["% Raised"] / zoom;
			else
			{
				s = Mathf.Log(Modifiers["DpB"] * zoom, 10);
				logValue = proj.raised > 0 ? Mathf.Log((proj.raised / proj.goal) + 1, s) : 0;
				return logValue;
			}
			break;
		case "latitude":
			return (proj.lat - gb.Bottom) * 10 / gb.Scale;
			break;
		case "longitude":
			return (proj.lon - gb.Left) * 10 / gb.Scale;
			break;
		case "zero":
		default:
			return 0;
			break;
			
		}*/
	}
	
}

