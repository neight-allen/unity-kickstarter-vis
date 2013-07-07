using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using JsonFx.Json;

public class KSProject
{
	public string name;
	public string url;
	public int backers;
	public int id;
	public float goal;
	public float raised;
	public int comments;
	public float lat;
	public float lon;
	public string parentCat;
	public string category;
	public int rewardCount;
	public KSReward[] rewards;
	public DateTime end_date;
	
	public override string ToString()
	{
		return id.ToString();
	}
	
	public float GetMetric(string metric)
	{
		switch(metric)
		{
		case "Pledged":
			return raised;
			break;
		case "Goal":
			return goal;
			break;
		case "Backers":
			return backers;
			break;
		case "Comments":
			return comments;
			break;
		case "DpB":
			return backers == 0 ? 0 : raised / backers;
			break;
		case "% Raised":
			return goal == 0 ? 0 : raised / goal;
			break;
		case "Raised":
			return goal <= raised ? raised : 0;
			break;
		case "Lat":
		case "Lattitude":
			return lat;
			break;
		case "Lon":
		case "Long":
		case "Longitude":
			return lon;
			break;
		default:
			return 0;		
		}
	}
}

public class KSReward
{
	public int backers;
	public float price;
}

public class KSProjectResponse
{
	public KSProject[] objects;
	public KSMeta meta;
}

public class DataPoint
{
	public float value;
	public string label;
}

public class GraphData
{
	public DataPoint[] dataPoints;
}

public class KSMeta
{
	public string next;
	public string prev;
	public int offset;
}

public class KSJSON : MonoBehaviour
{

	static public KSJSON me;
	public string apiURL;
	
	public Dictionary<string, Color> parentCatColors;
	public Dictionary<string, Color> parentCatHighlight;
	
	Color HexToColor(string hex)
	{
		byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
		return new Color32(r,g,b, 255);
	}
	
	// Use this for initialization
	void Start ()
	{
		parentCatColors = new Dictionary<string, Color>();
		parentCatColors.Add("Art", HexToColor("CBC617"));
		parentCatColors.Add("Comics", HexToColor("E0B819"));
		parentCatColors.Add("Dance", HexToColor("E4A319"));
		parentCatColors.Add("Design", HexToColor("E58919"));
		parentCatColors.Add("Fashion", HexToColor("E76F27"));
		parentCatColors.Add("Film & Video", HexToColor("EB595D"));
		parentCatColors.Add("Food", HexToColor("D44B92"));
		parentCatColors.Add("Games", HexToColor("A84EA1"));
		parentCatColors.Add("Music", HexToColor("7460A3"));
		parentCatColors.Add("Photography", HexToColor("42758A"));
		parentCatColors.Add("Publishing", HexToColor("358E5C"));
		parentCatColors.Add("Technology", HexToColor("55A931"));
		parentCatColors.Add("Theater", HexToColor("80BD1D"));
		
		KSJSON.me = this;
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	public void getProjects(GameObject caller, string query)
	{
		getProjects(caller, new string[] {query});
	}
	
	public void getProjects(GameObject caller, string[] queries)
	{
		StartCoroutine(wwwProjects(caller, queries));
	}
	
	public void getCountsByCategory(GameObject caller)
	{
		StartCoroutine(GetGraphData(caller, "?type=bar&group=parentCat&field=count"));
	}
	
	public void getGoalByCategory(GameObject caller)
	{
		StartCoroutine(GetGraphData(caller, "?type=bar&group=parentCat&field=goal"));
	}
	
	public void getSuccessByCategory(GameObject caller)
	{
		StartCoroutine(GetGraphData(caller, "?type=bar&group=parentCat&field=success"));
	}
	
	public void getURL(GameObject caller, string message, string url)
	{
		StartCoroutine(APIWWW(caller,message,url));
	}
	
	IEnumerator GetGraphData(GameObject caller, string querystring)
	{
		string url = apiURL + querystring;
		
		WWW www = new WWW(url);
		
		yield return www;
		
		Debug.Log(www.text);
		
		GraphData response = JsonReader.Deserialize<GraphData>(www.text);
		
		caller.SendMessage("ProcessGraphData", response);
	}
	
	IEnumerator wwwProjects(GameObject caller, string[] queries)
	{
		string url = apiURL + string.Join("&", queries);

		WWW www = new WWW(url);
		
		yield return www;
		//Debug.Log(www.error + ": " + url);
		
		float start = Time.realtimeSinceStartup;
		KSProjectResponse response = JsonReader.Deserialize<KSProjectResponse>(www.text);
		
		Debug.Log("Processed JSON in " + (Time.time - (start * 1000)).ToString() + " msec.");
		
		caller.SendMessage("setProjects", response.objects);
			
	}
	
	IEnumerator APIWWW(GameObject caller, string message, string url)
	{
		WWW www = new WWW(url);
			
		yield return www;
		//Debug.Log(www.error + ": " + url);
		
		//float start = Time.realtimeSinceStartup;
		KSProjectResponse response = JsonReader.Deserialize<KSProjectResponse>(www.text);
		
		//Debug.Log("Processed JSON in " + ((Time.realtimeSinceStartup - start) * 1000).ToString() + " msec.");
		
		caller.SendMessage(message, response);
		
	}
	
	public void getOffline(GameObject caller, string message, string resource)
	{
		TextAsset text = (TextAsset)Resources.Load(resource);
		//Debug.Log(text.text);
		
		KSProjectResponse response = JsonReader.Deserialize<KSProjectResponse>(text.text);
		
		caller.SendMessage(message, response);
	}
}

