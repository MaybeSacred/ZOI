using UnityEngine;
using System.Collections.Generic;

public class RiggedyAnnBehavior : BaseEnemy {
	public List<RiggedyAnnBehavior> friends;
	public Transform graphics;
	public float yGraphicsOffset;
	private NavMeshAgent navAgent;
	private float timer;
	public float graphicsRotationSpeed;
	private int updateCounter;
	public int framesToSkip;
	
	public float firingDistance;
	public float fireRate;
	private float fireTimer;
	public Transform bulletEmitter;
	public BasicBullet currentBullet;
	private float shieldPct = 100;
	public float timeSinceLastHit;
	public Transform shield;
	public Material shieldMat;
	public float shieldRechargeDelay;
	public float shieldRechargeRate;
	public float shieldMaterialRate;
	/// <summary>
	/// Together, detectionRange and detectionAngle define a sight cone in front of the riggedyAnne
	/// </summary>
	public float detectionRange;
	public float detectionAngle;
	public float foundDetectionRange;
	public float foundDetectionAngle;
	/// <summary>
	/// Defines when the bot gives up and returns to patrolling
	/// </summary>
	public float detectionTimeout;
	private float detectionTimeoutTimer, dodgeDist, dodgeTimer, dodgeDuration;
	public float healthRechargeRate;
	private Vector3 medianPoint;
	public float maxAcceptableDistFromPatrolMedian;
	private bool isUnderAttack;
	private bool danger;
	
	void Start()
	{
		HealthChange(100, maxHealth);
		navAgent = GetComponent<NavMeshAgent>();
		shieldMat = (Material)Instantiate(shieldMat);
		shield.renderer.material = shieldMat;
		isAwake = true;
		dodgeDuration = 2f;
	}
	void OnTriggerEnter(Collider other)
	{
		RealCollisionHandler(other);
	}
	void Update ()
	{
		if(deathTimeoutTimer > 0)
		{
			if(deathTimeoutTimer > deathTimeout)
			{
				Destroy(gameObject);
			}
			else
			{
				deathTimeoutTimer += Time.deltaTime;
			}
		}
		else
		{
			if(isAwake)
			{
				if(danger)
				{
					//this dodge is fast, but is instant to dodge the bullet
					
					//					transform.position = new Vector3(transform.position.x+dodgeDist,transform.position.y,transform.position.z);
					//					danger = false;
					//this dodge is too slow, but is relative to Time.deltaTime
					
					dodgeTimer+=Time.deltaTime;
					if(dodgeTimer<dodgeDuration)
						transform.Translate(dodgeDist*Time.deltaTime,0,0);
					else
					{
						dodgeTimer = 0;
						danger = false;
					}
				}
				MoveTowardsPlayer(Util.player.transform.position);
				if(Time.timeSinceLevelLoad > timeSinceLastHit + shieldRechargeDelay)
				{

					HealthChange(shieldRechargeRate * Time.deltaTime, 0);
					shield.renderer.enabled = true;
					shield.collider.enabled = true;
				}
				else
				{
					if(shieldPct > 0)
					{
						shield.renderer.enabled = true;
						shield.collider.enabled = true;
						if(Time.timeSinceLevelLoad-timeSinceLastHit < 1)
						{
							shield.renderer.materials[1].SetFloat("_Cutoff", (Time.timeSinceLevelLoad-timeSinceLastHit));
							shield.renderer.materials[1].mainTextureOffset += Random.insideUnitCircle*shieldMaterialRate;
						}
					}
					else
					{
						shield.renderer.enabled = false;
						shield.collider.enabled = false;
					}
				}
			}
		}
	}
	private void MoveTowardsPlayer(Vector3 vectorToPlayer)
	{
		if(updateCounter%framesToSkip ==0)
		{
			navAgent.SetDestination(vectorToPlayer);
		}
		updateCounter++;
	}
	
	void Jump(float dist)
	{
		dodgeDist = dist;
		danger = true;
	}
	public override void KillMe()
	{
		if(deathTimeoutTimer <= 0)
		{
			rigidbody.isKinematic = false;
			graphics.gameObject.GetComponent<SpringJoint>().breakForce = 0;
			navAgent.enabled = false;
			deathTimeoutTimer = .0001f;
		}
	}
	public override void RealCollisionHandler(Collider other)
	{
		if(isAwake)
		{
			if(other.tag.Equals("BasicExplosion"))
			{
				try
				{
					if(!colliders.Contains(other.gameObject))
					{
						colliders.Add(other.gameObject);
						BasicExplosion be = (BasicExplosion)other.GetComponent<BasicExplosion>();
						rigidbody.AddExplosionForce(be.explosionForce, other.transform.position, be.explosionRadius);
						HealthChange(-be.shieldDamage, -be.healthDamage);
					}
				}
				catch(System.InvalidCastException ie)
				{
					Debug.Log("Incorrect tag assignment for tag \"Basic Explosion\"");
				}
			}
			if(other.tag.Equals("Bullet"))
			{
				BasicBullet bb = (BasicBullet) other.GetComponent<BasicBullet>();

				if(!bb.deflectable)
					other.GetComponent<BasicBullet>().DestroyMe();
			}
		}
	}
	public override void HealthChange(float shieldDmg, float healthDmg)
	{
		if(shieldDmg >= 0 || healthDmg >= 0)
		{
			if(shieldDmg >=0)
			{
				shieldPct += shieldDmg;
				if(shieldPct > 100)
					shieldPct = 100;
			}
			if(healthDmg >= 0)
			{
				health += healthDmg;
				if(health > maxHealth)
					health = maxHealth;
			}
		}
		else
		{
			if(shieldPct > 0)
			{
				shieldPct += shieldDmg;
				if(shieldPct < 0)
				{
					shieldPct = 0;
					shield.collider.enabled = false;
				}
			}
			else
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
			timeSinceLastHit = Time.timeSinceLevelLoad;
		}
	}
	public void AlertFriends()
	{
		AlertMe();
		foreach(RiggedyAnnBehavior hb in friends)
		{
			hb.AlertMe();
		}
	}
	public void AlertMe()
	{
		isUnderAttack = true;
	}
}
