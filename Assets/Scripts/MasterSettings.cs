using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class GeoBounds
{
	static public List<GeoBounds> List = new List<GeoBounds>();
	static public string[] ListNames{
		get{ 
			string[] names = new string[GeoBounds.List.Count];
			for(int i = 0; i < names.Length; i++)
				names[i] = GeoBounds.List[i].Name;
			return names;
		}
		set{}
	}
	
	private float f_minlat;
	private float f_maxlat;
	private float f_minlon;
	private float f_maxlon;
	
	public string Name;
	public float Scale;
	
	public float MinLat{
		get { return f_minlat; }
		set { f_minlat = value; setScale(); }
	}
	public float Bottom{
		get { return f_minlat; }
		set { f_minlat = value; setScale(); }
	}
	
	public float MaxLat{
		get { return f_maxlat; }
		set { f_maxlat = value; setScale();}
	}
	public float Top{
		get { return f_maxlat; }
		set { f_maxlat = value; setScale(); }
	}
	
	public float MinLon{
		get { return f_minlon; }
		set { f_minlon = value; setScale(); }
	}
	public float Left{
		get { return f_minlon; }
		set { f_minlon = value; setScale(); }
	}
	
	public float MaxLon{
		get { return f_maxlon; }
		set { f_maxlon = value; setScale(); }
	}
	public float Right{
		get { return f_maxlon; }
		set { f_maxlon = value; setScale(); }
	}
	
	public GeoBounds()
	{
		
	}
	
	public GeoBounds(string name, float minlat, float maxlat, float minlon, float maxlon)
	{
		this.Name = name;
		f_minlat = minlat;
		f_maxlat = maxlat;
		f_minlon = minlon;
		f_maxlon = maxlon;
		setScale();
	}
	
	private void setScale()
	{
		Scale = Mathf.Max(MaxLat-MinLat,MaxLon-MinLon);
	}
	
}

public class KSMetric
{
	public static List<KSMetric> List = new List<KSMetric>();
	static public string[] ListNames{
		get{ 
			string[] names = new string[KSMetric.List.Count];
			for(int i = 0; i < names.Length; i++)
				names[i] = KSMetric.List[i].Name;
			return names;
		}
		set{}
	}
	
	public string Name;
	public float Zoom;
	public string Format;
	
	public KSMetric()
	{
		
	}
	
	public KSMetric(string name, float zoom, string format)
	{
		this.Name = name;
		this.Zoom = zoom;
		this.Format = format;
	}
}

public class MasterSettings : MonoBehaviour
{

	static public MasterSettings me;
	
	public float polypHeight = .05f;
	public float polypAngle = 137.5f;
	public int polypsPerNode = 100;
	public float skinRadius = 1;
	public float nodeSpread = 1;
	public int circlePoints = 10;
	public int pointsPerNode = 10;
	public float tension = .5f;
	public int coneHeight = 5;
	public GameObject polypFab;
	
	public float WindowSpacing = .05f;
	public float WindowSize = .1f;
	
	public bool offline = false;
	
	// Use this for initialization
	void Awake ()
	{
		me = GetComponent<MasterSettings>();
/* What are the default values of a new particle.		
		ParticleSystem.Particle debugParticle = new ParticleSystem.Particle();
		PropertyInfo[] properties = debugParticle.GetType().GetProperties();
		foreach (PropertyInfo property in properties)
		    Debug.Log("Name: " + property.Name + ", Value: " + property.GetValue(debugParticle, null));
		    */
	}
	
	void Start()
	{
		GeoBounds.List.Add(new GeoBounds(
			"US",
			24.7433195f, 49.3457868f, //lat
			-124.7844079f, -66.9513812f)); //lon
		GeoBounds.List.Add(new GeoBounds(
			"World", -90f, 90, -180f, 180f));
		GeoBounds.List.Add(new GeoBounds(
			"New York",
			40.5f, 45f, //lat
			-71.85f, -79.7666666f));//lon
		GeoBounds.List.Add(new GeoBounds(
			"California",
			32.533333f, 42f, //lat
			-124.7844079f, -114.133333f)); //lon
		
		KSMetric.List.Add(new KSMetric("Pledged", 10000, "${0:0,0}"));
		KSMetric.List.Add(new KSMetric("Goal", 10000, "${0:0,0}"));
		KSMetric.List.Add(new KSMetric("Backers", 100, "{0:0,0}"));
		KSMetric.List.Add(new KSMetric("Comments", 50, "{0:0,0}"));
		KSMetric.List.Add(new KSMetric("DpB", 20, "${0:0,0}"));
		KSMetric.List.Add(new KSMetric("% Raised", 1, "{0:0%}"));
		KSMetric.List.Add(new KSMetric("Latitude", 1, ""));
		KSMetric.List.Add(new KSMetric("Longitude", 1, ""));
			
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

