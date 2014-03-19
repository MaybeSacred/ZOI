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
	public bool deflectable;
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
		else if(lifetimeTimer > lifetime)
		{
			timeOutCounter += Time.deltaTime;
		}
		lifetimeTimer += Time.deltaTime;
	}

	public void OnCollisionEnter(Collision other)
	{
		if(other.gameObject.tag.Equals("Deflective")&&deflectable)
		{
			rigidbody.velocity = new Vector3(0f,initialSpeed/10f,(initialSpeed/5f));
			//knocks off the Deflective tag once deflective
			other.gameObject.tag = "Untagged";
		}
		else 
		{
			DestroyMe();
		}
	}
	public virtual void DestroyMe()
	{
		ParticleSystem ps = Instantiate(explosionPS, transform.position - rigidbody.velocity*Time.fixedDeltaTime, transform.rotation) as ParticleSystem;
		rigidbody.isKinematic = true;
		timeOutCounter += Time.deltaTime;
		GetComponent<Collider>().enabled = false;
		GetComponent<ParticleSystem>().Stop();
		if(GetComponent<MeshRenderer>() != null)
		{
			GetComponent<MeshRenderer>().enabled = false;
		}
		ps.GetComponent<BasicExplosion>().SetDamageVariables(shieldDamage, healthDamage);
	}
}
