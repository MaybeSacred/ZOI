using UnityEngine;
using System.Collections;

public class ShieldReacter : EnemyColliderPasser {
	public Transform reactionSystem;
	void Start () {
	
	}
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.layer == 8)
		{
			if(!other.tag.Equals("Laser"))
			{
				if((-Time.deltaTime*2*other.rigidbody.velocity + other.transform.position - transform.position).magnitude > transform.localScale.x/2)
				{
					ParticleSystem temp = ((Transform)Instantiate(reactionSystem, (other.transform.position - transform.position).normalized*transform.localScale.x/2 + transform.position, Quaternion.LookRotation(other.transform.position - transform.position))).GetComponent<ParticleSystem>();
					temp.startSize = 1.5f*other.rigidbody.mass;
				}
			}
		}
		realParents.SendMessage("RealCollisionHandler",other);
	}
}
