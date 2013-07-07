using UnityEngine;
using System.Collections;

public class Jellyfish : MonoBehaviour
{

	public GameObject TentacleFab;
	public int NumTentacles;
	
	private GameObject head;
	
	// Use this for initialization
	void Start ()
	{
		head = GetComponentInChildren<JellyAnim>().gameObject;
		
		for(int i = 0; i < NumTentacles; i++)
		{
			Vector2 rand = Random.insideUnitCircle * head.collider.bounds.extents.x;
			Vector3 pos = new Vector3(rand.x, 2 - head.collider.bounds.extents.z, rand.y);
			//Vector3 pos = new Vector3(rand.x, 0, rand.y);
			Quaternion rot = Quaternion.Euler(0, Random.Range(0,360), 0);
			
			GameObject temp = Instantiate(TentacleFab, pos + head.collider.bounds.center, rot) as GameObject;
			
			temp.transform.parent = head.transform;
			//temp.transform.localPosition = pos;
			temp.GetComponent<InteractiveCloth>().AttachToCollider(head.collider);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

