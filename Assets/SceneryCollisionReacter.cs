using UnityEngine;
using System.Collections;

public class SceneryCollisionReacter : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTriggerEnter(Collider other)
	{
		if(other.tag.Equals("BasicExplosion"))
		{
			rigidbody.AddExplosionForce(other.GetComponent<BasicExplosion>().explosionForce, other.transform.position, other.GetComponent<BasicExplosion>().explosionRadius);
		}
	}
}
