using UnityEngine;
using System.Collections.Generic;

public class ClingyDanBehavior : BaseEnemy {
	public float firingDistance;
	public bool usesEnemyScriptColliders;
	public Transform turret;
	public float rotationDelta;
	private bool breakingDown;
	public int numBursts;
	public float fireRate;
	public float reloadTime;
	private int currentBurstNum;
	private float fireTimer;
	public Transform bulletEmitter;
	public float firingRandomness;
	public BasicBullet currentBullet;
	void Start () {
		currentBurstNum = numBursts;

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
		else
		{
			if(usesEnemyScriptColliders)
			{
				if(isAwake)
				{
					Vector3 distance = Util.player.transform.position - turret.position;
					distance += Util.player.rigidbody.velocity*(distance.magnitude/currentBullet.initialSpeed);
					turret.rotation = Quaternion.RotateTowards(turret.rotation, Quaternion.LookRotation(distance), rotationDelta*Time.deltaTime);
					GunUpdate();
				}
			}
			else
			{
				Vector3 distance = Util.player.transform.position - turret.position;
				Vector3 attackPoint = distance + Util.player.rigidbody.velocity*(distance.magnitude/currentBullet.initialSpeed);
				turret.rotation = Quaternion.RotateTowards(turret.rotation, Quaternion.LookRotation(attackPoint), rotationDelta*Time.deltaTime);
				if(distance.magnitude < firingDistance)
				{
					isAwake = true;
					GunUpdate();
				}
				else
				{
					isAwake = false;
				}
			}
		}
	}
	private void GunUpdate()
	{
		if(currentBurstNum < numBursts)
		{
			if(fireTimer > fireRate)
			{
				Vector3 randVector = Util.GenerateRandomVector3(bulletEmitter.forward, firingRandomness);
				Util.Fire(currentBullet, bulletEmitter.position, Quaternion.LookRotation(randVector), 
				          randVector*currentBullet.initialSpeed);
				currentBurstNum++;
				fireTimer -= fireRate;
			}
		}
		else
		{
			if(fireTimer > reloadTime)
			{
				fireTimer -= reloadTime;
				currentBurstNum = 0;
			}
		}
		fireTimer += Time.deltaTime;
	}
	public override void KillMe ()
	{
		if(!breakingDown)
		{
			deathTimeoutTimer += Time.deltaTime;
			if(attachedBarrier != null)
			{
				attachedBarrier.UnregisterEnemy();
			}
			FixedJoint[] joints = GetComponentsInChildren<FixedJoint>();
			for(int i = 0; i < joints.Length; i++)
			{
				joints[i].breakForce = joints[i].breakTorque = 0;
			}
		}
	}
	void OnTriggerEnter(Collider other)
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
		}
	}
	public void OnJointBreak()
	{
		KillMe();
	}
	/*private Vector3 IterativeSolver(Vector3 firerPos, float firerMagnitude, Vector3 fireePos, Vector3 fireeVel)
	{
		float t = 0;
		Vector3 firerVel = (fireePos - firerPos).normalized*firerMagnitude;
		while(((fireeVel*t+fireePos)-(firerVel*t+firerPos)).magnitude > aimingError)
		{
			
		}
		return firerVel;
	}*/
}
