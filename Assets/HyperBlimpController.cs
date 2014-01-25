using UnityEngine;
using System.Collections.Generic;

public class HyperBlimpController : BaseEnemy {
	private List<GameObject> colliders;
	public Transform[] rings;
	private int topPiecesLeft;
	private bool isDamageable = false;
	void Start () {
		colliders = new List<GameObject>();
	}
	
	void Update () {
		if(deathTimeoutTimer > 0)
		{
			if(deathTimeoutTimer > deathTimeout)
			{
				Destroy(gameObject);
			}
			deathTimeoutTimer += Time.deltaTime;
		}
		else
		{
			
		}
	}
	public override void HealthChange(float shieldDmg, float healthDmg)
	{
		if(health > 0)
		{
			health += healthDmg;
			if(health > maxHealth)
			{
				health = maxHealth;
			}
			if(health <= 0)
			{
				health = 0;
				KillMe();
			}
		}
	}
	public override void RealCollisionHandler(Collider other)
	{
		if(isDamageable)
		{
			if(other.tag.Equals("BasicExplosion"))
			{
				try
				{
					if(!colliders.Contains(other.gameObject))
					{
						colliders.Add(other.gameObject);
						BasicExplosion be = (BasicExplosion)other.GetComponent<BasicExplosion>();
						HealthChange(-be.shieldDamage, -be.healthDamage);
						rigidbody.AddExplosionForce(be.explosionForce, be.transform.position, be.explosionRadius);
					}
				}
				catch(System.InvalidCastException ie)
				{
					Debug.Log("Incorrect tag assignment for tag \"Basic Explosion\"");
				}
			}
		}
	}
	void OnTriggerEnter(Collider other)
	{
		RealCollisionHandler(other);
	}
	public override void KillMe()
	{
		deathTimeoutTimer += Time.deltaTime;
		rigidbody.isKinematic = false;
	}
	public void RemovePiece()
	{
		topPiecesLeft--;
		if(topPiecesLeft <= 0)
		{
			isDamageable = true;
		}
	}
	public void AddPiece()
	{
		topPiecesLeft++;
	}
}
