using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public enum UIType{
	Rockband,
	IPad,
	Mouse_Key,
	XBox360
}

public class GraphAxis : MonoBehaviour
{
	//todo: Label Axis
	//done: Rotate X Axis
	//done: Rotate Y Axis
	//didn't like: Motion Blur
	
	
	static public GraphAxis me;
	
	
	//public string[] axisNames = new string[]{"Pledged", "Goal", "Backers", "Comments", "DpB", "% Raised", "Latitue", "Longitude"};
	//public string[] Geographics = new string[]{"World", "US", "California", "New York"};
	public AudioClip ac_updown;
	public AudioClip ac_select;
	public AudioClip ac_back;
	public AudioClip ac_zoom;
	public AudioClip[] metricSounds;
	
	public UIType UItype;
	public int Geographic = 0;
	
	public int xLabel;
	public int yLabel;
	public int xLabelNew;
	public int yLabelNew;
	
	public float zoom = .1f;
	
	
	public GameObject city;
	
	public string[] legendTitles;
	private GUIStyle[] legendStyles;
	public int selectedLegend = 0;
	private int newLegend = 0;
	private int lastLegend = 0;
	
	public int ProjectCount = 0;
	
	public GUIStyle defaultStyle;
	private GUIStyle bigButton;
	
	public Color[] KeyColors = new Color[] { Color.green, Color.red, Color.yellow, Color.blue, Color.Lerp(Color.red, Color.yellow, .5f) };
	private GUIStyle[] keyStyles;
	
	private bool xAxisClicked, yAxisClicked, zoomClicked, catsClicked, geosClicked;
	public bool xAxisVisible, yAxisVisible, zoomVisible, catsVisible, geosVisible;
	
	float AccelerometerUpdateInterval = 1.0f / 60.0f;
	float LowPassKernelWidthInSeconds = 0.2f;
	
	private float LowPassFilterFactor;
	private Vector3 lowPassValue = Vector3.zero;

	
	Vector3 LowPassFilterAccelerometer() {
		lowPassValue = Vector3.Lerp(lowPassValue, Input.acceleration, LowPassFilterFactor);
		return lowPassValue;
	}
	
	void Awake ()
	{
		me = GetComponent<GraphAxis>();

		xAxisClicked = yAxisClicked = zoomClicked = catsClicked = geosClicked = false;
	}
	
	void Start()
	{
		int num = KSJSON.me.parentCatColors.Count;
		legendTitles = new string[num + 1];
		legendStyles = new GUIStyle[num + 1];
		int i = 1;
		legendTitles[0] = "All";
		foreach(var kvp in KSJSON.me.parentCatColors)
		{
			legendTitles[i] = kvp.Key;
			
			//GUIStyle temp = new GUIStyle(GUI.skin.button);
			//temp.normal.textColor = kvp.Value;
			//legendStyles[i] = temp;
			
			i++;
		}
		lowPassValue = Input.acceleration;
		LowPassFilterFactor = AccelerometerUpdateInterval / LowPassKernelWidthInSeconds; // tweakable
		/*
		1. Create a new Texture2D(1,1)
		2. Fill it with the desired color ( SetPixel ) 
		3. Set it's wrap mode to repeat
		4. Use Texture2D.Apply() to apply the changes
		5. Create a GUIStyle
		6. Set it's .normal background texture to the texture you just created
		*/		
		//first 5 are full color, second 5 are faded
		keyStyles = new GUIStyle[KeyColors.Length * 2];
		for(int j = 0; j < KeyColors.Length; j++)
		{
			Texture2D t = new Texture2D(1,1);
			t.SetPixel(0,0,KeyColors[j]);
			t.Apply();
			GUIStyle gs = new GUIStyle();
			gs.normal.background = t;
			keyStyles[j] = gs;
			
			//faded out versions
			Texture2D t2 = new Texture2D(1,1);
			t2.SetPixel(0,0,new Color(KeyColors[j].r, KeyColors[j].g, KeyColors[j].b, .3f));
			t2.Apply();
			GUIStyle gs2 = new GUIStyle();
			gs2.normal.background = t2;
			keyStyles[j+KeyColors.Length] = gs2;
		}
		
		
		
	}
	
	// Update is called once per frame
	void Update ()
	{
#if UNITY_IPHONE
		if(Input.touchCount > 1)
		{
			Easing.TimeScale = 0;
			
			city.GetComponent<City>().setRotation(1+LowPassFilterAccelerometer().x);
			//city.transform.rotation = Quaternion.Euler(Mathf.Clamp((1+LowPassFilterAccelerometer().x) * -90f, -90f, 0f), 0, 0);
			//city.transform.RotateAround(Vector3.right, Input.acceleration.x);
			Debug.Log(Input.acceleration.x);
			
		}
		else
			Easing.TimeScale = 1;
#endif
		
		if(Input.GetButtonDown("Jump"))
		{
			Easing.TimeScale = Easing.TimeScale == 1 ? 0f : 1;
		}
		
		if(UItype == UIType.XBox360 && Input.GetKey(KeyCode.JoystickButton16))
		{
			Easing.TimeScale = (Input.GetAxis("RightTrigger") + Input.GetAxis("LeftTrigger") + 2) / -2 + 1;
			//Debug.Log(Easing.TimeScale);
		}
		
		if(UItype == UIType.Rockband)
		{
			/*if(Input.GetButton("AxisDown"))
			{
				Easing.TimeScale = 0;
				city.GetComponent<City>().setRotation(Input.GetAxis("RightTrigger") / -2f + 0.5f);
				Debug.Log(Input.GetAxis("RightTrigger"));
			}
			else*/
				Easing.TimeScale = Input.GetAxisRaw("RightTrigger") * -1f;
		}
		
		if(Input.GetButtonDown("AxisDown"))
		{
			if(xAxisClicked)
			{
				xLabelNew++;
				if(xLabelNew >= KSMetric.List.Count)
					xLabelNew -= KSMetric.List.Count;
			}
			else if(yAxisClicked)
			{
				yLabelNew++;
				if(yLabelNew >= KSMetric.List.Count)
					yLabelNew -= KSMetric.List.Count;
			}
			else if(catsClicked)
			{
				newLegend++;
				if(newLegend == legendTitles.Length)
					newLegend = 0;
			}
			if(xAxisClicked || yAxisClicked || catsClicked)
				AudioSource.PlayClipAtPoint(ac_updown,Camera.main.transform.position);
			if(zoomClicked)
				AudioSource.PlayClipAtPoint(ac_zoom, Camera.main.transform.position);
		}
		
		if(Input.GetButtonDown("AxisUp"))
		{
			if(xAxisClicked)
			{
				xLabelNew--;
				if(xLabelNew < 0)
					xLabelNew += KSMetric.List.Count;
			}
			else if(yAxisClicked)
			{
				yLabelNew--;
				if(yLabelNew < 0)
					yLabelNew += KSMetric.List.Count;
			}
			else if(catsClicked)
			{
				newLegend--;
				if(newLegend < 0)
					newLegend += legendTitles.Length;
			}
			if(xAxisClicked || yAxisClicked || catsClicked)
				AudioSource.PlayClipAtPoint(ac_updown,Camera.main.transform.position);
			if(zoomClicked)
				AudioSource.PlayClipAtPoint(ac_zoom, Camera.main.transform.position);
		}
		
		if(Input.GetButton("AxisDown"))
		{
			if(zoomClicked)
			{
				city.GetComponent<City>().ZoomInstant(1 - Time.deltaTime * (1.2f + Input.GetAxis("RightTrigger")));
			}
		}
		
		if(Input.GetButton("AxisUp"))
		{
			if(zoomClicked)
			{
				city.GetComponent<City>().ZoomInstant(1 + Time.deltaTime * (1.2f + Input.GetAxis("RightTrigger")));
			}
		}
		
		if(Input.GetButtonDown("AxisSelect"))
		{
			if(xAxisClicked)
			{
				xAxisClicked = false;
				city.GetComponent<City>().RotateX();
			}
			
			if(yAxisClicked)
			{
				yAxisClicked = false;
				city.GetComponent<City>().RotateY();
			}
			
			if(catsClicked)
			{
				city.GetComponent<City>().HighlightCategory(legendTitles[newLegend]);
				lastLegend = selectedLegend;
				selectedLegend = newLegend;
			}
				
		}
		
		if(Input.GetButtonDown("yAxisSelect"))
		{
			yAxisVisible = true;
			if(yAxisClicked)
			{
				yAxisClicked = false;
				if(yLabel != yLabelNew)
				{
					city.GetComponent<City>().RotateY();
					AudioSource.PlayClipAtPoint(metricSounds[yLabelNew], Camera.main.transform.position);
				}
				else
					AudioSource.PlayClipAtPoint(ac_select, Camera.main.transform.position);
			}
			else
			{
				xAxisClicked = false;
				yAxisClicked = true;
				zoomClicked = false;
				catsClicked = false;
				AudioSource.PlayClipAtPoint(ac_select, Camera.main.transform.position);
			}
		}
		
		if(Input.GetButtonDown("xAxisSelect"))
		{
			if(xAxisClicked)
			{
				xAxisClicked = false;
				if(xLabelNew != xLabel)
				{
					city.GetComponent<City>().RotateX();
					AudioSource.PlayClipAtPoint(metricSounds[xLabelNew], Camera.main.transform.position);
				}
				else
					AudioSource.PlayClipAtPoint(ac_select, Camera.main.transform.position);
			}
			else
			{
				xAxisClicked = true;
				yAxisClicked = false;
				zoomClicked = false;
				catsClicked = false;
				AudioSource.PlayClipAtPoint(ac_select, Camera.main.transform.position);
			}
		}
		
		if(Input.GetButtonDown("zoomSelect"))
		{
			zoomVisible = true;	
			xAxisClicked = false;
			yAxisClicked = false;
			catsClicked = false;
			zoomClicked = !zoomClicked;
			AudioSource.PlayClipAtPoint(ac_select, Camera.main.transform.position);
			
		}
		
		if(Input.GetButtonDown("catsSelect"))
		{
			AudioSource.PlayClipAtPoint(ac_select, Camera.main.transform.position);
			catsVisible = true;	
			xAxisClicked = false;
			yAxisClicked = false;
			zoomClicked = false;
			catsClicked = !catsClicked;
			
			city.GetComponent<City>().HighlightCategory(legendTitles[newLegend]);
			lastLegend = selectedLegend;
			selectedLegend = newLegend;
		}
		
		if(Input.GetButtonDown("Back"))
		{
			AudioSource.PlayClipAtPoint(ac_back, Camera.main.transform.position);
			newLegend = selectedLegend;
			selectedLegend = lastLegend;
			lastLegend = newLegend;
			city.GetComponent<City>().HighlightCategory(legendTitles[selectedLegend]);
		}
	}
	
	void OnGUI()
	{
		if(UItype == UIType.IPad || UItype == UIType.Mouse_Key)
		{
			bigButton = new GUIStyle(GUI.skin.button);
			bigButton.font = defaultStyle.font;
		}
		else
		{
			bigButton = new GUIStyle(defaultStyle);
			bigButton.alignment = TextAnchor.LowerCenter;
		}
		
		GUIStyle fadedBigButton = new GUIStyle(bigButton);
		fadedBigButton.normal.textColor = new Color(1,1,1,.5f);
		
		GUIStyle currentBigButton = new GUIStyle(bigButton);
		currentBigButton.normal.textColor = new Color(1,1,1,.75f);
		
		//Get Axis bounds
		Camera c = Camera.main;
		Transform t = city.transform;
		Vector2 origin = c.WorldToScreenPoint(t.position);
		Vector2 top = c.WorldToScreenPoint(t.position + Vector3.up * t.localScale.y);
		Vector2 right = c.WorldToScreenPoint(t.position + Vector3.right * t.localScale.x);
		bool inGraph = (UItype == UIType.IPad || UItype == UIType.Mouse_Key) && new Rect(origin.x, origin.y, Screen.width, top.y).Contains(Input.mousePosition);
		Vector2 poa = new Vector2(origin.x, Screen.height - Input.mousePosition.y); //Point On Axis
		int lineheight = 10;
		//Normalize because its dumb
		origin.y = Screen.height - origin.y;
		top.y = Screen.height - top.y;
		right.y = Screen.height - right.y;
		
		float labelHeight = 28f;
		float labelWidth = 100f;
		float keysOffset = 27f;
		float offset;
		Color faded = new Color(1,1,1,0.5f);

		GUIStyle yValueStyle = new GUIStyle(defaultStyle);
		yValueStyle.alignment = TextAnchor.UpperRight;
		
		GUIStyle plStyle = new GUIStyle(defaultStyle);
		plStyle.alignment = TextAnchor.UpperRight;
		plStyle.normal.textColor = new Color(1,1,1,0.05f);
		plStyle.fontSize = 300;
		
		//Draw Projects Loaded
		//GUI.Label(new Rect(Screen.width - 250, 0, 250, 150), "Projects Loaded:\n" + ProjectCount, yValueStyle);
		GUI.Label(new Rect(0,0,Screen.width,Screen.height), ProjectCount.ToString(), plStyle);
		
		GUIHelper.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
		//Draw Axis Numbers
		if(yAxisVisible)
		{
			KSMetric yMetric = KSMetric.List[yLabel];
			
			//New Draw Axis
			GUIHelper.DrawLine(top, origin, faded);
			GUIHelper.DrawLine(origin, right, faded);
			
			if(inGraph)
			{
				GUIHelper.DrawLine(
					new Vector2(origin.x - lineheight/2, Screen.height - Input.mousePosition.y),
					new Vector2(origin.x + lineheight/2, Screen.height - Input.mousePosition.y),
					Color.white
				);
				
				GUIStyle ypValueStyle = new GUIStyle(defaultStyle);
				ypValueStyle.alignment = TextAnchor.MiddleRight;
				GUI.Label(
					new Rect(poa.x - 150, poa.y - 50, 150 - lineheight / 2 - 20, 100),
					string.Format(yMetric.Format,
						yMetric.Zoom * zoom * 10 * (poa.y - origin.y) / (top.y - origin.y)),
					ypValueStyle
				);
			}
			
			//Draw tick value
			GUILayout.BeginArea(new Rect(top.x - labelWidth, top.y, labelWidth, 100));
			{
				GUILayout.BeginVertical();
				{
					GUILayout.Label(
						string.Format(yMetric.Format, yMetric.Zoom * zoom * 10),
						yValueStyle,
						GUILayout.Height(labelHeight)
					);
					
					
					if(GUILayout.Button(yMetric.Name.ToUpper(), yValueStyle))
					{
						xAxisClicked = false;
						zoomClicked = false;
						yAxisClicked = !yAxisClicked;
					}
					if(UItype == UIType.Rockband)
					{
						Rect temp = GUILayoutUtility.GetLastRect();
						drawKeys(new Vector2(temp.x, temp.y + keysOffset), 0, yAxisClicked);
					}
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndArea();
			//actual tick
			GUIHelper.DrawLine(top - Vector2.right * lineheight, top, faded);
			
			if(yAxisClicked)
			{
				string[] labels = KSMetric.ListNames;
				for(int i = 0; i < labels.Length; i++)
				{
					if(UItype == UIType.IPad)
						offset = labelHeight * (i + 1);
					else
						offset = labelHeight * (i + 1 - yLabelNew);

					if(GUI.Button(
						new Rect(top.x - labelWidth * 2, top.y + offset, labelWidth, labelHeight),
						labels[i].ToUpper(),
						yValueStyle
					))
					{
						yAxisClicked = false;
						yLabelNew = i;
						city.GetComponent<City>().RotateY();
					}
				}
			}
		}
		
		if(xAxisVisible)
		{
			KSMetric xMetric = KSMetric.List[xLabel];
			
			//Metric name and tick value
			GUILayout.BeginArea(new Rect(right.x - labelWidth, right.y + 10, labelWidth, 100));
			{
				GUILayout.BeginVertical();
				{
					GUILayout.Label(
						string.Format(xMetric.Format, xMetric.Zoom * zoom * 10),
						yValueStyle,
						GUILayout.Height(labelHeight)
					);
					
					if(GUILayout.Button(xMetric.Name.ToUpper(), yValueStyle))
					{
						yAxisClicked = false;
						zoomClicked = false;
						xAxisClicked = !xAxisClicked;
					}
					if(UItype == UIType.Rockband)
					{
						Rect temp = GUILayoutUtility.GetLastRect();
						drawKeys(new Vector2(temp.x, temp.y + keysOffset), 1, xAxisClicked);
					}
					
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndArea();
			//actual tick
			GUIHelper.DrawLine(right, right + Vector2.up * lineheight, faded);
			if(!yAxisVisible){
				GUI.Label(new Rect(origin.x - 100, origin.y, 100, 100),string.Format(xMetric.Format,"0"), yValueStyle);
				GUIHelper.DrawLine(origin, origin + Vector2.up * lineheight, faded);
			}
			//Draw precise numbers
			if(inGraph)
			{
				GUIStyle xpValueStyle = new GUIStyle(defaultStyle);
				xpValueStyle.alignment = TextAnchor.UpperCenter;
				poa = new Vector2(Input.mousePosition.x, origin.y);
				GUI.Label(
					new Rect(poa.x - 100, poa.y + lineheight / 2 + 2, 200, 50),
					string.Format(xMetric.Format, 
						xMetric.Zoom * zoom * 10 * (poa.x - origin.x) / (right.x - origin.x) ),
					xpValueStyle
				);
				
				GUIHelper.DrawLine(
					new Vector2(Input.mousePosition.x, origin.y - lineheight/2),
					new Vector2(Input.mousePosition.x, origin.y + lineheight/2),
					Color.white
				);
			}
			
			 
			//draw new metrics list		
			if(xAxisClicked)
			{
				
				string[] labels = KSMetric.ListNames;
				for(int i = 0; i < labels.Length; i++)
				{
					if(UItype == UIType.IPad)
						offset = labelHeight * (i + 2 - labels.Length);
					else
						offset = labelHeight * (i + 1 - xLabelNew);

					if(GUI.Button(
						new Rect(right.x, right.y + 10 + offset, labelWidth, labelHeight),
						labels[i].ToUpper(),
						defaultStyle
					))
					{
						xAxisClicked = false;
						xLabelNew = i;
						city.GetComponent<City>().RotateX();
					}
				}
					
			}
		}
		
		GUIHelper.EndGroup();
		
		Vector2 zoomOffset, catsOffset;
		zoomOffset = catsOffset = Vector2.zero;
		
		//draw options
		GUILayout.BeginArea(new Rect(20,0,Screen.width - 40,Screen.height - 20));
		{
			GUILayout.BeginHorizontal();
			{
				//draw categories
				if(catsVisible)
				{
					GUILayout.BeginVertical(GUILayout.Width(100));
					{
						GUILayout.FlexibleSpace();
						if(catsClicked)
						{
							for(int i = 0; i < legendTitles.Length; i++)
							{
								if(UItype == UIType.Mouse_Key || UItype == UIType.IPad)								
								{
									if(GUILayout.Button(legendTitles[i], bigButton))
									{
										city.GetComponent<City>().HighlightCategory(legendTitles[i]);
										selectedLegend = i;
									}
								}
								else
								{
									if(i == selectedLegend)
										GUILayout.Label(legendTitles[i], bigButton);
									else if(i == newLegend)
										GUILayout.Label(legendTitles[i], currentBigButton);
									else
										GUILayout.Label(legendTitles[i], fadedBigButton);
								}
							}
						}
						if(GUILayout.Button("CATEGORIES", defaultStyle))
							catsClicked = !catsClicked;
	
						Rect temp = GUILayoutUtility.GetLastRect();
						catsOffset = new Vector2(temp.x, temp.y + 25);
					}
					GUILayout.EndVertical();
				}
				if(UItype == UIType.Rockband)
					GUILayout.FlexibleSpace();
				
				//draw zoom
				if(zoomVisible)
				{
					GUILayout.BeginVertical(GUILayout.Width(75));
					{
						GUILayout.FlexibleSpace();
						
						if((UItype == UIType.IPad || UItype == UIType.Mouse_Key) && zoomClicked)
						{
							if(GUILayout.RepeatButton("+++", bigButton))
								city.GetComponent<City>().ZoomInstant(1 - Time.deltaTime * 1.5f);
							if(GUILayout.RepeatButton("+", bigButton))
								city.GetComponent<City>().ZoomInstant(1 - Time.deltaTime * .3f);
							if(GUILayout.RepeatButton("-", bigButton))
								city.GetComponent<City>().ZoomInstant(1 + Time.deltaTime * .3f);
							if(GUILayout.RepeatButton("---", bigButton))
								city.GetComponent<City>().ZoomInstant(1 + Time.deltaTime * 1.5f);
						}
						if(GUILayout.Button("ZOOM", defaultStyle))
							zoomClicked = !zoomClicked;
						
						Rect temp = GUILayoutUtility.GetLastRect();
						zoomOffset = new Vector2(temp.x, temp.y + 25);
						
					}
					GUILayout.EndVertical();
				}
				if(UItype != UIType.Rockband)
					GUILayout.FlexibleSpace();
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndArea();
		
		if(UItype == UIType.Rockband)
		{
			if(catsVisible)
				drawKeys(catsOffset, 4, catsClicked);
			if(zoomVisible)
				drawKeys(zoomOffset, 2, zoomClicked);
		}
			

//		GUI.Label(
//			new Rect(right.x - 150, right.y + 10, 150, 100),
//			string.Format(xMetric.Format, xMetric.Zoom * zoom * 10) + "\n" + xMetric.Name.ToUpper(),
//			yValueStyle
//		);
		
		//Draw Precise numbers
		
		/*
		//Draw Scrubbers
		float xRot = GUI.VerticalSlider(
			new Rect(top.x - 20, top.y, 20, origin.y - top.y), 
			city.transform.localEulerAngles.x - 360,
			0f,-90f
		);
		
		float yRot = GUI.HorizontalSlider(
			new Rect(origin.x + 10, origin.y, right.x - origin.x, 20), 
			city.transform.localEulerAngles.y,
			0f,90f
		);
		
		
		city.transform.eulerAngles = new Vector3(xRot, yRot, 0);
		*/
		 
		//string label = "X: " + axisNames[xLabel] + "\nY: " + axisNames[yLabel];
		
		/*
		GUILayout.BeginArea(new Rect(0,0,Screen.width, Screen.height));
		{
			GUILayout.BeginVertical();
			{
				//GUILayout.Label(label);
				
				GUILayout.FlexibleSpace();
				GUILayout.BeginHorizontal();
				{
					GUILayout.BeginVertical();
					{
						GUILayout.Label("Zoom");
						GUILayout.BeginHorizontal();
						{
							if(GUILayout.Button("+"))
								city.GetComponent<City>().ZoomIn();
							
							if(GUILayout.Button("-"))
								city.GetComponent<City>().ZoomOut();
						}
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal();
						{
							if(GUILayout.Button("++"))
								city.GetComponent<City>().ZoomInIn();
							
							if(GUILayout.Button("--"))
								city.GetComponent<City>().ZoomOutOut();
						}
						GUILayout.EndHorizontal();
						
						GUILayout.Label("\n\nHorizontal Axis");
						int newValue = GUILayout.SelectionGrid(xLabel, KSMetric.ListNames, 2);
						if(newValue != xLabel)
						{
							xLabelNew = newValue;
							city.GetComponent<City>().RotateX();
						}
						GUILayout.Label("\nVertical Axis");
						newValue = GUILayout.SelectionGrid(yLabel, KSMetric.ListNames, 2);
						if(newValue != yLabel)
						{
							yLabelNew = newValue;
							city.GetComponent<City>().RotateY();
						}
						
						GUILayout.Label("\n\nGeo Bounds");
						newValue = GUILayout.SelectionGrid(Geographic, GeoBounds.ListNames, 2);
						if(newValue != Geographic)
						{
							Geographic = newValue;
							city.GetComponent<City>().ZoomNone();
							
						}
						
						GUILayout.Label("\n\nHighlight Category");
						newValue = GUILayout.SelectionGrid(selectedLegend, legendTitles, 2);
						if(newValue != selectedLegend)
						{
							city.GetComponent<City>().HighlightCategory(legendTitles[newValue]);
							selectedLegend = newValue;
						}
					}
					GUILayout.EndVertical();
					GUILayout.FlexibleSpace();
					
				}
				GUILayout.EndHorizontal();
				GUILayout.FlexibleSpace();
			}
			GUILayout.EndVertical();
		}
		GUILayout.EndArea();*/
		
		
	}
	
	void drawKeys(Vector2 o, int highlight, bool selected)
	{
		o.x += 25;
		if(selected)
			GUI.Label(new Rect(o.x, o.y, 75, 3), " ", keyStyles[highlight]);
		else
			for(int i = 0; i < KeyColors.Length; i++)
			{
				int offset = i == highlight ? 0 : KeyColors.Length;
				GUI.Label(new Rect(o.x + (15 * i), o.y, 13, 3), " ", keyStyles[i + offset]);
			}
	}
	
	void resetClicks()
	{
		xAxisClicked = false;
		yAxisClicked = false;
		zoomClicked = false;
		catsClicked = false;
		geosClicked = false;
	}
}

