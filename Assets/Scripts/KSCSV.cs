using UnityEngine;
using System;
using System.Collections;

public class KSCSV : MonoBehaviour
{

	static public KSCSV me;
	
	// Use this for initialization
	void Awake ()
	{
		KSCSV.me = GetComponent<KSCSV>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	public void loadCSV(GameObject caller, string file)
	{
		loadCSV(caller, file, 0, -1);	
	}
	
	public void loadCSV(GameObject caller, string file, int start, int end)
	{
		TextAsset text = (TextAsset)Resources.Load(file);
		string[] lines = text.text.Split(new char[] { '\n' });
		if(end < 0) end = lines.Length;
		StartCoroutine(streamCSVtoProjects(caller, new ArraySegment<string>(lines, start, end - start).Array));

	}
	
	IEnumerator streamCSVtoProjects(GameObject caller, string[] lines)
	{
		int i = 0;
		int chunk = 1000;
		KSProject[] projects = new KSProject[chunk];
		KSProjectResponse response = new KSProjectResponse();
		response.meta = new KSMeta();
		while(i < lines.Length)
		{
			
			for(int j = 0; j < chunk && i < lines.Length; j++)
			{
				projects[j] = parseProjectLine(lines[i]);
				i++;
			}
			
			response.objects = projects;
			caller.GetComponent<City>().ProcessResponse(response);
			yield return null;
		}
	}
	
	KSProject parseProjectLine(string line)
	{
		/****
		Meta:
		Raised - float
		Goal - float
		backers - int
		comments - int
		parentCat - string
		lat - float
		lon - float
		*****/
		
		KSProject proj = new KSProject();
		
		string[] vals = line.Split(',');
		
		proj.raised = float.Parse(vals[1]);
		proj.goal = float.Parse(vals[0]);
		proj.backers = int.Parse(vals[2]);
		proj.comments = int.Parse(vals[3]);
		proj.parentCat = vals[4].Trim('"');
		proj.lat = float.Parse(vals[5]);
		proj.lon = float.Parse(vals[6]);
		
		return proj;
	}
}

