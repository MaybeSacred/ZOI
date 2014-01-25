using UnityEngine;
using System.Collections;

public class BasicBullet : MonoBehaviour {
	public string prettyName;
	public int side;
	public float lifetime;
	protected float lifetimeTimer;
	public ParticleSystem explosionPS;
	public Vector3 acceleration;
	public Vector3 speed;
	public float initialSpeed;
	public float shieldDamage;
	public float healthDamage;
	public float timeOutCounter;
	public float explosionDuration;
	public bool useGravity;
	protected float endTime;
	void Start () {
		endTime = this.GetComponent<ParticleSystem>().startLifetime + this.GetComponent<ParticleSystem>().duration;
		rigidbody.velocity = speed;
	}

	void Update () {
		if(timeOutCounter > 0)
		{
			if(timeOutCounter > endTime)
			{
				Destroy(this.gameObject);
			}
			timeOutCounter += Time.deltaTime;
		}
		if(lifetimeTimer > lifetime)
		{
			timeOutCounter += Time.deltaTime;
		}
		lifetimeTimer += Time.deltaTime;
	}
	
	public void OnCollisionEnter()
	{
		DestroyMe();
	}
	public virtual void DestroyMe()
	{
		ParticleSystem ps = Instantiate(explosionPS, transform.position - rigidbody.velocity*Time.fixedDeltaTime, transform.rotation) as ParticleSystem;
		speed = Vector3.zero;
		acceleration = Vector3.zero;
		timeOutCounter += Time.deltaTime;
		GetComponent<Collider>().enabled = false;
		GetComponent<ParticleSystem>().Stop();
		if(GetComponent<MeshRenderer>() != null)
		{
			GetComponent<MeshRenderer>().enabled = false;
		}
		ps.GetComponent<BasicExplosion>().explosionDuration = explosionDuration;
		ps.GetComponent<BasicExplosion>().shieldDamage = shieldDamage;
		ps.GetComponent<BasicExplosion>().healthDamage = healthDamage;
		ps.GetComponent<BasicExplosion>().side = side;
	}
}
