using UnityEngine;
using System.Collections.Generic;

public class RiggedyAnnBehavior : BaseEnemy {
	public List<RiggedyAnnBehavior> friends;
	public Transform graphics;
	private NavMeshAgent navAgent;
	private int updateCounter;
	public int framesToSkip;
	
	public float fireRate;
	private float fireTimer, deflectTimer;
	public Transform cannonBulletEmitter;
	public Transform rocketBulletEmitter;
	public BasicBullet currentBullet;
	private float shieldPct = 100;
	private float timeSinceLastHit;
	public Transform shield;
	public Material shieldMat;
	public float shieldRechargeDelay;
	public float shieldRechargeRate;
	public float shieldMaterialRate;
	public float standoffDistance;
	/// <summary>
	/// Defines when the bot gives up and returns to patrolling
	/// </summary>
	public float detectionTimeout;
	public float maxDodgeDistance;
	private float detectionTimeoutTimer, dodgeDist, dodgeTimer;
	public float dodgeDuration;
	public float healthRechargeRate;
	private Vector3 medianPoint;
	public float firingRandomness;
	public float maxAcceptableDistFromPatrolMedian;
	private bool isUnderAttack;
	private bool danger;
	
	void Start()
	{
		HealthChange(100, maxHealth);
		navAgent = GetComponent<NavMeshAgent>();
		shieldMat = (Material)Instantiate(shieldMat);
		shield.renderer.material = shieldMat;
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
						navAgent.Move(dodgeDist*Time.deltaTime*transform.right);
						//transform.Translate();
					}
					else
					{
						dodgeTimer = 0;
						danger = false;
						//navAgent.enabled = true;
						navAgent.SetDestination(transform.position);
					}
				}
				else
				{
					Vector3 vectorToPlayer = Util.player.transform.position - transform.position;
					if(vectorToPlayer.magnitude < Fuzzify(standoffDistance))
					{
						if(fireTimer > fireRate)
						{
							Vector3 randomHemisphere = Util.GenerateRandomVector3(Util.player.transform.position - cannonBulletEmitter.position, firingRandomness);
							GetComponent<AudioSource>().Play();
							Util.Fire<BasicBullet>(currentBullet, cannonBulletEmitter.position, Quaternion.LookRotation(randomHemisphere), currentBullet.initialSpeed*randomHemisphere);
							fireTimer = 0;
						}
						fireTimer += Time.deltaTime;
						MoveTowardsPlayer(Util.player.transform.position - 15 * (Util.player.transform.position - transform.position).normalized);
					}
					else
					{
						MoveTowardsPlayer(Util.player.transform.position);
					}
				}
				graphics.LookAt(Util.player.transform.position);
				UpdateShield();
			}
		}
	}
	private float Fuzzify(float invalue){
		return invalue * Random.Range(.9f, 1.1f);
	}
	private void UpdateShield()
	{
		if(Time.timeSinceLevelLoad > timeSinceLastHit + shieldRechargeDelay)
		{
			HealthChange(shieldRechargeRate * Time.deltaTime, healthRechargeRate*Time.deltaTime);
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
		float chance = Random.Range (0, 1f);
		//half the time we jump
		if(chance > .5f)
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
						dodgeDist = Random.Range(maxDodgeDistance/2, maxDodgeDistance)/dodgeDuration;
					}
					else
					{
						//go left
						dodgeDist = -Random.Range(maxDodgeDistance/2, maxDodgeDistance)/dodgeDuration;
					}
				}
				else
				{
					//go right
					dodgeDist = Random.Range(maxDodgeDistance/2, maxDodgeDistance)/dodgeDuration;
				}
			}
			else
			{
				if(rightHit)
				{
					//go left
					dodgeDist = -Random.Range(maxDodgeDistance/2, maxDodgeDistance)/dodgeDuration;
				}
				else
				{
					//go random
					dodgeDist = (Random.Range(-1, 1)>0?1:-1) * Random.Range(maxDodgeDistance/2, maxDodgeDistance)/dodgeDuration;
				}
			}
			danger = true;
			//navAgent.enabled = false;
		}
	}
	public override void KillMe()
	{
		if(deathTimeoutTimer <= 0)
		{
			if(attachedBarrier != null)
			{
				attachedBarrier.UnregisterEnemy();
			}
			Util.theGUI.RemoveRadarObject(transform);
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
			if(healthDmg >= 0 && shieldPct >= 100)
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
