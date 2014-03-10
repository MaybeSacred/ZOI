using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SixthSense : MonoBehaviour {
	private List<GameObject> colliders;
	public GameObject SpideySenseOwner;

	// Use this for initialization
	void Start () {
		colliders = new List<GameObject> ();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void RealCollisionHandler(Collider other)
	{
		if(other.gameObject.tag.Equals ("Bullet"))
		{
			print (other.gameObject.tag);
			//try
			//{

			if(!colliders.Contains(other.gameObject))
				{
					print ("dodge?");
					float jump = Random.Range(-20f, 20f);
					if(Mathf.Abs(jump)<10f)
					{
						print ("yes, dodge");
						SpideySenseOwner.SendMessage("Jump",jump);
					}
				}

			//}
//			catch
//			{
//				Debug.Log("Incorrect tag assignment for tag \"Bullet\"");
//			}
		}
	}
	public void OnTriggerEnter(Collider other)
	{
		RealCollisionHandler(other);
	}
	public void OnTriggerStay(Collider other)
	{
		//only consistent triggers
		RealCollisionHandler(other);
	}
}
