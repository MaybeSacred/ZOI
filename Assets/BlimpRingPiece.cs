using UnityEngine;
using System.Collections.Generic;

public class BlimpRingPiece : MonoBehaviour {
	private List<GameObject> colliders;
	public float health;
	private float deathTimeoutTimer;
	public float deathTimeout;
	public SquidditchBehavior attachedSquiddie;
	public float textureSpeed;
	public HyperBlimpController blimp;
	public Material baseMaterial;
	void Start () {
		blimp.AddPiece();
		colliders = new List<GameObject>();
	}
	
	void Update () 
	{
		if(deathTimeoutTimer > 0)
		{
			if(deathTimeoutTimer > deathTimeout)
			{
				Destroy(gameObject);
			}
			deathTimeoutTimer += Time.deltaTime;
		}
		baseMaterial.mainTextureOffset += new Vector2(0, Time.deltaTime*textureSpeed);
	}
	void OnTriggerEnter(Collider other)
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
					if(health <= 0)
					{
						rigidbody.AddExplosionForce(be.explosionForce, other.transform.position, be.explosionRadius);
					}
				}
			}
			catch(System.InvalidCastException ie)
			{
				Debug.Log("Incorrect tag assignment for tag \"Basic Explosion\"");
			}
		}
		if(other.tag.Equals("Bullet"))
		{
			other.GetComponent<BasicBullet>().DestroyMe();
		}
	}
	public void HealthChange(float shieldDmg, float healthDmg)
	{
		health += healthDmg;
		if(health < 0)
		{
			health = 0;
			KillMe();
		}
	}
	public void KillMe()
	{
		if(deathTimeoutTimer == 0)
		{
			deathTimeoutTimer += Time.deltaTime;
			attachedSquiddie.DetachFromGameObject();
			blimp.RemovePiece();
			transform.parent = null;
			rigidbody.isKinematic = false;
		}
	}
}
