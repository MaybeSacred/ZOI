using UnityEngine;
using System.Collections;

public class WebShot : BasicBullet {

	public GameObject web;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnCollisionEnter(Collision other)
	{
		/*if(other.gameObject.tag.Equals("Untagged"))
		{
			Instantiate(web, transform.position - rigidbody.velocity*Time.fixedDeltaTime, transform.rotation);

		}*/
		DestroyMe ();
	}

	public void DestroyMe()
	{
		//ParticleSystem ps = Instantiate(explosionPS, transform.position - rigidbody.velocity*Time.fixedDeltaTime, transform.rotation) as ParticleSystem;
		Instantiate(web, transform.position - rigidbody.velocity*Time.fixedDeltaTime, transform.rotation);
		speed = Vector3.zero;
		acceleration = Vector3.zero;
		timeOutCounter += Time.deltaTime;
		GetComponent<Collider>().enabled = false;
		if(GetComponent<MeshRenderer>() != null)
		{
			GetComponent<MeshRenderer>().enabled = false;
		}
		/*ps.GetComponent<BasicExplosion>().explosionDuration = explosionDuration;
		ps.GetComponent<BasicExplosion>().shieldDamage = shieldDamage;
		ps.GetComponent<BasicExplosion>().healthDamage = healthDamage;
		ps.GetComponent<BasicExplosion>().side = side;*/
	}
}
