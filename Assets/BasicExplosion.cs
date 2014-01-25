using UnityEngine;
using System.Collections;

public class BasicExplosion : MonoBehaviour {
	public int side;
	private float checkTime, currentTime;
	public float shieldDamage;
	public float healthDamage;
	public float explosionForce;
	public float explosionDuration;
	public float explosionRadius;
	void Start () 
	{
		checkTime = GetComponent<ParticleSystem>().duration + GetComponent<ParticleSystem>().startLifetime;
	}

	void Update () 
	{
		if(currentTime > checkTime)
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
