using UnityEngine;
using System.Collections.Generic;

public class StaticShieldTopPieceController : MonoBehaviour {
	private List<GameObject> colliders;
	public float health;
	private float deathTimeoutTimer;
	public float deathTimeout;
	public float textureSpeed;
	public Material baseMaterial;
	public Material laserMaterial;
	public float laserBeamRate;
	public Transform laserBeam;
	void Start () {
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
		laserMaterial.mainTextureOffset += new Vector2(0, laserBeamRate*Time.deltaTime);
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
					rigidbody.AddExplosionForce(be.explosionForce, other.transform.position, be.explosionRadius);
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
		}
		if(health <= 0)
		{
			KillMe();
		}
	}
	public void KillMe()
	{
		if(deathTimeoutTimer == 0)
		{
			Destroy(laserBeam.gameObject);
			deathTimeoutTimer += Time.deltaTime;
			transform.parent.GetComponent<StaticShieldController>().RemovePiece();
			rigidbody.constraints = RigidbodyConstraints.None;
		}
	}
}
