using UnityEngine;
using System.Collections.Generic;

public class KatneenBehavior : BaseEnemy {
	public Transform shield;
	public Material shieldMat;
	public float shieldRechargeDelay;
	public float shieldRechargeRate;
	public float shieldMaterialRate;
	public float rotationDelta;
	private bool breakingDown;
	public float firingDistance;
	public float fireRate;
	private float fireTimer;
	public Transform bulletEmitter;
	public BasicBullet currentBullet;
	private float shieldPct = 100;
	public float timeSinceLastHit;
	public float warmupTime;
	public Light warningLight;
	public float finalWarningLightRange;
	public float postShotLightDropoff;
	public float lookAheadTime;
	public float deathGlowConstant;
	private Vector3 lockedOnDirection;
	private Vector3 lockedOnPoint;
	public Transform turret;
	public Transform[] legs;
	private List<GameObject> colliders;
	public LaserBullet currentLaser;
	private float laserDurationTimer;
	public float laserDuration;
	public float laserGroundRate;
	public float laserRandomness;
	void Start () {
		colliders = new List<GameObject>();
		shieldMat = (Material)Instantiate(shieldMat);
		shield.renderer.materials[1] = shieldMat;
	}
	void Update () 
	{
		if(isAwake)
		{
			if(deathTimeoutTimer > 0)
			{
				currentLaser = null;
				warningLight.range = deathTimeoutTimer*deathGlowConstant;
				if(deathTimeoutTimer > deathTimeout)
				{
					Destroy(transform.parent.gameObject);
				}
				deathTimeoutTimer += Time.deltaTime;
			}
			else
			{
				Vector3 distance = Util.player.transform.position - turret.transform.position;
				if(currentLaser != null)
				{
					if(laserDurationTimer < laserDuration)
					{
						lockedOnPoint = Vector3.MoveTowards(lockedOnPoint, Util.player.transform.position + laserRandomness*Random.insideUnitSphere, Time.deltaTime*laserGroundRate);
						currentLaser.transform.rotation = Quaternion.LookRotation(lockedOnPoint - bulletEmitter.position);
						RaycastHit hit;
						Physics.Raycast(bulletEmitter.position, currentLaser.transform.forward, out hit, float.PositiveInfinity, ~(1<<8 | 1<<2));
						if(hit.distance != 0)
						{
							currentLaser.transform.localScale =  new Vector3(currentLaser.transform.localScale.x, currentLaser.transform.localScale.y, hit.distance/2f);
							currentLaser.transform.position = currentLaser.transform.forward*hit.distance/2f+bulletEmitter.position;
						}
						turret.rotation = Quaternion.RotateTowards(turret.rotation, currentLaser.transform.rotation, rotationDelta*Time.deltaTime);
					}
					else
					{
						currentLaser = null;
					}
					laserDurationTimer += Time.deltaTime;
				}
				else
				{
					if(fireTimer > warmupTime)
					{
						if(fireTimer > fireRate-lookAheadTime)
						{
							if(lockedOnDirection == Vector3.zero)
							{
								lockedOnDirection = distance + lookAheadTime*Util.player.rigidbody.velocity;
								lockedOnPoint = Util.player.transform.position + lookAheadTime*Util.player.rigidbody.velocity;
							}
							turret.rotation = Quaternion.RotateTowards(turret.rotation, Quaternion.LookRotation(lockedOnDirection), rotationDelta*Time.deltaTime);
							if(fireTimer > fireRate)
							{
								currentLaser = (LaserBullet)Util.FireLaserType(currentBullet, bulletEmitter.position, lockedOnDirection+turret.position, Quaternion.LookRotation(lockedOnDirection+turret.position - bulletEmitter.position));
								laserDurationTimer = 0;
								fireTimer -= fireRate;
								lockedOnDirection = Vector3.zero;
							}
						}
						else
						{
							Vector3 futureDistanceVector = distance + lookAheadTime*Util.player.rigidbody.velocity;
							turret.transform.rotation = Quaternion.RotateTowards(turret.transform.rotation, Quaternion.LookRotation(futureDistanceVector), rotationDelta*Time.deltaTime);
						}
						warningLight.range = finalWarningLightRange*(fireTimer-warmupTime)/(fireRate-warmupTime);
					}
					else
					{
						Vector3 futureDistanceVector = distance + lookAheadTime*Util.player.rigidbody.velocity;
						turret.transform.rotation = Quaternion.RotateTowards(turret.transform.rotation, Quaternion.LookRotation(futureDistanceVector), rotationDelta*Time.deltaTime);
						warningLight.range = postShotLightDropoff*(1-fireTimer);
					}
					fireTimer += Time.deltaTime;
				}
			}
			if(Time.timeSinceLevelLoad > timeSinceLastHit + shieldRechargeDelay)
			{
				if(shieldPct < 100)
				{
					HealthChange(shieldRechargeRate * Time.deltaTime, 0);
				}
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
	void OnTriggerEnter(Collider other)
	{
		RealCollisionHandler(other);
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
	public override void KillMe()
	{
		if(!breakingDown)
		{
			currentLaser = null;
			deathTimeoutTimer += Time.deltaTime;
			turret.gameObject.AddComponent<Rigidbody>();
			gameObject.GetComponent<SpringJoint>().breakForce = 0;
			gameObject.GetComponent<SpringJoint>().breakTorque = 0;
			rigidbody.constraints = RigidbodyConstraints.None;
			for(int i = 0; i < legs.Length; i++)
			{
				Transform[] childrens = legs[i].GetComponentsInChildren<Transform>();
				for(int j = 0; j < childrens.Length; j++)
				{
					childrens[j].gameObject.AddComponent<Rigidbody>();
				}
			}
			breakingDown = true;
		}
	}
	public override void OnPlayerExit()
	{
		if(deathTimeoutTimer <=0)
		{
			isAwake = false;
			warningLight.range = 0;
		}
	}
}