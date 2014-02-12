using UnityEngine;
using System.Collections.Generic;

public class Cubey : BaseEnemy {
	public Transform player;
	public float maxFuzzyEngageRadius;
	public float minFuzzyEngageRadius;
	/*Chance of responding to enemy when maxFuzzyEngageRadius > player distance > minFuzzyEngageRadius*/
	public float fuzzyEngageCoeff;
	public float fireRadius;
	public bool engagingEnemy;
	public float timeGranularity;
	public float timeCounter;
	private NavMeshAgent navi;
	public Vector3 startPos;
	private bool returnPathCalculated;
	public float firingRandomness;
	public BasicBullet currentBullet;
	public float firingRate;
	private float firingTimer;
	public Transform bulletEmitter;
	void Start () {
		navi = this.GetComponent<NavMeshAgent>();
		startPos = transform.position;
	}

	void Update () {
		Vector3 positionVector = player.position - this.transform.position;
		if(timeCounter > timeGranularity)
		{
			timeCounter -= timeGranularity;
			if(positionVector.magnitude > maxFuzzyEngageRadius)
			{
				engagingEnemy = false;
				if(!returnPathCalculated)
				{
					navi.SetDestination(startPos);
					returnPathCalculated = true;
				}
			}
			else if(!engagingEnemy)
			{
				returnPathCalculated = false;
				if(positionVector.magnitude <= maxFuzzyEngageRadius && positionVector.magnitude > minFuzzyEngageRadius)
				{
					float temp = (maxFuzzyEngageRadius - positionVector.magnitude)/(maxFuzzyEngageRadius-minFuzzyEngageRadius);
					if(Random.value < temp*fuzzyEngageCoeff)
					{
						engagingEnemy = true;
					}
				}
				else
				{
					engagingEnemy = true;
				}
			}
		}
		if(engagingEnemy)
		{
			if(positionVector.magnitude < fireRadius)
			{
				navi.Stop(false);
				transform.LookAt(player);
				if(firingTimer > firingRate)
				{
					Vector3 temp = Random.insideUnitSphere;
					Vector3 temp2 = (bulletEmitter.position-transform.position).normalized;
					temp = firingRandomness*Vector3.Cross(temp2, temp);
					Util.Fire(currentBullet, bulletEmitter.position, bulletEmitter.rotation, 
					               (bulletEmitter.position-transform.position-temp).normalized*currentBullet.initialSpeed);
					firingTimer -= firingRate;
				}
				firingTimer += Time.deltaTime;
			}
			else
			{
				navi.SetDestination(player.position);
			}
		}
		timeCounter += Time.deltaTime;
	}
	public void OnTriggerEnter(Collider other)
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
