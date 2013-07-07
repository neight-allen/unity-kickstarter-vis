using UnityEngine;
using System.Collections;

public class SomeInput : MonoBehaviour
{

	public GameObject Target;
	
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(Input.GetButtonDown("Jump"))
			Target.GetComponent<BarGraph>().LoadSomeData();
		
		if(Input.GetKeyDown(KeyCode.G))
			KSJSON.me.getGoalByCategory(Target);
		
		if(Input.GetKeyDown(KeyCode.S))
			KSJSON.me.getSuccessByCategory(Target);
	}
}

