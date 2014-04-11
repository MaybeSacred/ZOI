using UnityEngine;
using System.Collections;

public class BasicExplosion : MonoBehaviour {
	public int side;
	private float particleStopTime, currentTime;
	[HideInInspector]
	public float shieldDamage;
	[HideInInspector]
	public float healthDamage;
	public float explosionForce;
	public float explosionDuration;
	public float explosionRadius;
	void Start ()
	{
		particleStopTime = GetComponent<ParticleSystem>().duration + GetComponent<ParticleSystem>().startLifetime;
		AudioSource source = GetComponent<AudioSource>();
		if(audio != null)
		{
			audio.Play();
		}
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
