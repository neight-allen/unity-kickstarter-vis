using UnityEngine;
using System.Collections;

public class CameraTarget : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.Translate(Input.GetAxis("UpDown") * Vector3.up);
	}
}

