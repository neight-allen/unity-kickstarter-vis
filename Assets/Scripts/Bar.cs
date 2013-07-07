using UnityEngine;
using System.Collections;

public class Bar : MonoBehaviour
{
	public string label = "";
	private float f_value = 0f;
	
	private BarGraph bg_parent;
	
	// Use this for initialization
	void Start ()
	{
		bg_parent = transform.parent.GetComponent<BarGraph>();		
		transform.localScale = new Vector3(bg_parent.Settings.BarWidth, 0, bg_parent.Settings.BarDepth);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	public void setValue(float value)
	{
		f_value = value;
		StartCoroutine(setHeight());
	}
	
	IEnumerator setHeight()
	{
		yield return null;
		
		transform.localScale = new Vector3(transform.localScale.x, f_value * bg_parent.unitHeight, transform.localScale.z);	
		transform.localPosition = new Vector3(transform.position.x, transform.localScale.y / 2f, transform.position.z);
		
		yield return null;
		
	}
}

