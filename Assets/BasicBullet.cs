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
	public float deflectionSpeed;
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
		Vector3 collisionPoint = Vector3.zero;
		foreach(ContactPoint c in other.contacts){
			collisionPoint += c.point;
		}
		collisionPoint = (other.contacts.Length > 0?collisionPoint/other.contacts.Length:transform.position);
		DestroyMe(collisionPoint);
	}
	public void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag.Equals("Deflective") && deflectable)
		{
			if(!rigidbody.isKinematic)
			{
				rigidbody.velocity = (transform.position - other.transform.position).normalized*deflectionSpeed;
			}
			rigidbody.useGravity = true;
		}
		else 
		{
			if(other.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast")){
				Vector3 collisionPoint = other.collider.ClosestPointOnBounds(transform.position);
				DestroyMe(collisionPoint);
			}
		}
	}
	public virtual void DestroyMe(Vector3 collisionPoint)
	{
		ParticleSystem ps = Instantiate(explosionPS, collisionPoint, transform.rotation) as ParticleSystem;
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
