using UnityEngine;
using System.Collections.Generic;

public class RiggedyAnnBehavior : BaseEnemy {
	public List<RiggedyAnnBehavior> friends;
	public GameObject DeflectiveSurface;
	public Transform graphics;
	public float yGraphicsOffset;
	private NavMeshAgent navAgent;
	public float graphicsRotationSpeed;
	private int updateCounter;
	public int framesToSkip;
	
	public float fireRate;
	public float fireTimer, deflectTimer;
	public Transform bulletEmitter;
	public BasicBullet currentBullet;
	private float shieldPct = 100;
	public float timeSinceLastHit;
	public Transform shield;
	public Material shieldMat;
	public float shieldRechargeDelay;
	public float shieldRechargeRate;
	public float shieldMaterialRate;
	public float standoffDistance;
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
	private float detectionTimeoutTimer, dodgeDist, dodgeTimer;
	public float dodgeDuration;
	public float healthRechargeRate, deflectCooldownDuration;
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
					dodgeTimer += Time.deltaTime;
					if(dodgeTimer < dodgeDuration)
					{
						transform.Translate(dodgeDist*Time.deltaTime*transform.right);
					}
					else
					{
						dodgeTimer = 0;
						danger = false;
					}
				}
				Vector3 vectorToPlayer = Util.player.transform.position - transform.position;
				if(vectorToPlayer.magnitude < standoffDistance)
				{
					
				}
				else
				{
					MoveTowardsPlayer(Util.player.transform.position);
				}
				UpdateShield();
			}
		}
	}
	private void UpdateShield()
	{
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
	private void MoveTowardsPlayer(Vector3 vectorToPlayer)
	{
		if(updateCounter%framesToSkip == 0)
		{
			navAgent.SetDestination(vectorToPlayer);
		}
		updateCounter++;
	}
	public void Jump()
	{
		if(!danger)
		{
			RaycastHit hit;
			float rightDistance;
			bool rightHit;
			rightHit = Physics.SphereCast(transform.position, shield.localScale.x, transform.right, out hit);
			rightDistance = hit.distance;
			if(Physics.SphereCast(transform.position, shield.localScale.x, -transform.right, out hit))
			{
				if(rightHit)
				{
					if(hit.distance < rightDistance)
					{
						dodgeDist = dist;
					}
					else
					{
						//go left
					}
				}
				else
				{
					//go right
				}
			}
			else
			{
				if(rightHit)
				{
					//go left
				}
				else
				{
					//go random
				}
			}
			danger = true;
		}
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
