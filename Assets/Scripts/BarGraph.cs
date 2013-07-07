using UnityEngine;
using System.Collections;

[System.Serializable]
public class BarGraphSettings
{
	public float BarSeperation = 0.7f;
	public float BarWidth = 1f;
	public float BarDepth = 1f;
}

public class BarGraph : MonoBehaviour
{

	public GameObject BarFab;
	public BarGraphSettings Settings;
	
	private GameObject[] goa_bars;
	private float f_graphHeight;
	
	public float unitHeight{
		get{return 1f / f_graphHeight;}
		set{}
	}
	
	// Use this for initialization
	void Start ()
	{
		goa_bars = new GameObject[0];
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	public void LoadSomeData()
	{
		KSJSON.me.getCountsByCategory(gameObject);
	}
	
	void ProcessGraphData(GraphData data)
	{
		float newHeight = float.NegativeInfinity;
		foreach(DataPoint dp in data.dataPoints)
			newHeight = Mathf.Max(newHeight, dp.value);
		f_graphHeight = newHeight;
		
		if(goa_bars.Length > 0)
		{
			Debug.Log("Setting values but not labels");
			foreach(DataPoint dp in data.dataPoints)
			{
				for(int i = 0; i < goa_bars.Length; i++)
				{
					if(goa_bars[i].GetComponent<Bar>().label == dp.label)
					{
						goa_bars[i].GetComponent<Bar>().setValue(dp.value);
						break;
					}
				}
			}
		
		}
		else
		{
			goa_bars = new GameObject[data.dataPoints.Length];
			for(int i = 0; i < goa_bars.Length; i++)
			{
				DataPoint dp = data.dataPoints[i];
				GameObject temp = (GameObject)Instantiate(BarFab);
				temp.transform.position = new Vector3((Settings.BarWidth + Settings.BarSeperation) * i, 0, 0);
				temp.GetComponent<Bar>().label = dp.label;
				temp.GetComponent<Bar>().setValue(dp.value);
				temp.transform.parent = transform;
				temp.renderer.material.color = KSJSON.me.parentCatColors[dp.label];
				goa_bars[i] = temp;
				
			}
		
		}
	}
}

