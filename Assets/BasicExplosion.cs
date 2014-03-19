using UnityEngine;
using System.Collections;

public class BasicExplosion : MonoBehaviour {
	public int side;
	private float particleStopTime, currentTime;
	public float shieldDamage;
	public float healthDamage;
	public float explosionForce;
	public float explosionDuration;
	public float explosionRadius;
	void Start ()
	{
		particleStopTime = GetComponent<ParticleSystem>().duration + GetComponent<ParticleSystem>().startLifetime;
	}
	public void SetDamageVariables(float sd, float hd)
	{
		shieldDamage = sd;
		healthDamage = hd;
	}
	void Update () 
	{
		if(currentTime > particleStopTime)
		{
			Destroy(gameObject);
		}
		else if(currentTime > explosionDuration)
		{
			collider.enabled = false;
			if(rigidbody != null)
			{
				rigidbody.detectCollisions = false;
			}
		}
		currentTime += Time.deltaTime;
	}
}
