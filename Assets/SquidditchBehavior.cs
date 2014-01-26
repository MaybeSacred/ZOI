﻿using UnityEngine;
using System.Collections.Generic;

public class SquidditchBehavior : BaseEnemy, PlayerEvent {
	public Transform bulletEmitterGO;
	public Light bulletEmitterLight;
	public Transform graphics;
	private Quaternion nextRotation;
	public float minAlpha;
	public float maxAlpha;
	public float rotationSpeed;
	public float minAngle;
	public float minAcceptableAngle;
	public float minFlitDistance;
	public float maxFlitDistance;
	public float movementSpeed;
	public float movementError;
	public float waitTime;
	private float waitTimer;
	private Vector3 moveToFlitPosition;
	public float rotationDelta;
	private bool breakingDown;
	public float firingDistance;
	public float attachedFiringDistance;
	public float standOffDistance;
	public float fireRate;
	private float fireTimer;
	public Transform bulletEmitter;
	public BasicBullet currentBullet;
	public float deathGlowConstant;
	private List<GameObject> colliders;
	public LaserBullet currentLaser;
	private float laserDurationTimer;
	public float laserDuration;
	public float laserGroundRate;
	public float laserRandomness;
	private float timeIntoMovement;
	public bool isAttachedToGameObject;
	void Start () {
		moveToFlitPosition = transform.position;
		nextRotation = Random.rotationUniform;
		colliders = new List<GameObject>();
	}
	void Update () 
	{
		if(deathTimeoutTimer > 0)
		{
			bulletEmitterLight.range = deathGlowConstant*(deathTimeout-deathTimeoutTimer);
			if(deathTimeoutTimer > deathTimeout)
			{
				Destroy(gameObject);
			}
			deathTimeoutTimer += Time.deltaTime;
		}
		else
		{
			if(isAwake)
			{
				Vector3 distance = Util.player.transform.position - transform.position;
				if(distance.magnitude < firingDistance)
				{
					FiringHandler();
					if((moveToFlitPosition - transform.position).magnitude < movementError)
					{
						if(waitTimer >= waitTime)
						{
							RaycastHit info;
							Physics.Raycast(transform.position, distance.normalized, out info);
							if(info.collider.tag.Equals("Player"))
							{
								if(distance.magnitude < standOffDistance)
								{
									GenerateNextMovementPoint(Mathf.Sign(Random.Range(-1, 0))*Vector3.Cross(distance, Vector3.up));
								}
								else
								{
									GenerateNextMovementPoint(distance);
								}
							}
							else
							{
								GenerateNextMovementPoint(Vector3.up);
							}
							waitTimer = 0;
							timeIntoMovement = 0;
						}
						waitTimer += Time.deltaTime;
					}
					else
					{
						transform.position = Vector3.Slerp(transform.position, moveToFlitPosition, .5f-.5f*Mathf.Cos(timeIntoMovement*movementSpeed));
					}
				}
				else
				{
					if((moveToFlitPosition - transform.position).magnitude < movementError)
					{
						RaycastHit info;
						Physics.Raycast(transform.position, distance.normalized, out info);
						if(info.collider.tag.Equals("Player"))
						{
							GenerateNextMovementPoint(distance);
						}
						else
						{
							GenerateNextMovementPoint(Vector3.up);
						}
						timeIntoMovement = 0;
					}
					else
					{
						transform.position = Vector3.Slerp(transform.position, moveToFlitPosition, .5f-.5f*Mathf.Cos(timeIntoMovement*movementSpeed));
					}
				}
				if(Quaternion.Angle(graphics.rotation, nextRotation) < minAngle)
				{
					Quaternion possibleAngle = Random.rotationUniform;
					while(Quaternion.Angle(nextRotation, possibleAngle) < minAcceptableAngle)
					{
						possibleAngle = Random.rotationUniform;
					}
					nextRotation = possibleAngle;
				}
				timeIntoMovement += Time.deltaTime;
				bulletEmitterGO.rotation = Quaternion.LookRotation(distance);
				graphics.rotation = Quaternion.Lerp(graphics.rotation, nextRotation, Time.deltaTime*rotationSpeed);
				graphics.renderer.materials[0].SetFloat("_Cutoff", (maxAlpha-minAlpha)*Mathf.PerlinNoise(0, Time.timeSinceLevelLoad)+minAlpha);
			}
			else if(isAttachedToGameObject)
			{
				Vector3 distance = Util.player.transform.position - transform.position;
				if(distance.magnitude < attachedFiringDistance)
				{
					if(currentLaser != null)
					{
						FiringHandler();
					}
					else
					{
						RaycastHit hit;
						if(Physics.Raycast(bulletEmitter.position, distance, out hit, attachedFiringDistance))
						{
							if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
							{
								FiringHandler();
							}
						}
					}
				}
				bulletEmitterGO.rotation = Quaternion.LookRotation(distance);
				graphics.renderer.materials[0].SetFloat("_Cutoff", (maxAlpha-minAlpha)*Mathf.PerlinNoise(0, Time.timeSinceLevelLoad)+minAlpha);
			}
		}
	}
	public void DetachFromGameObject()
	{
		isAwake = true;
		isAttachedToGameObject = false;
		moveToFlitPosition = transform.position;
		transform.parent = null;
	}
	private void FiringHandler()
	{
		if(currentLaser != null)
		{
			if(laserDurationTimer < laserDuration)
			{
				currentLaser.transform.rotation = Quaternion.LookRotation(Util.player.transform.position - bulletEmitter.position);
				RaycastHit hit;
				Physics.Raycast(bulletEmitter.position, currentLaser.transform.forward, out hit, float.PositiveInfinity, ~(1<<8 | 1<<2));
				if(hit.distance != 0)
				{
					currentLaser.transform.localScale =  new Vector3(currentLaser.transform.localScale.x, currentLaser.transform.localScale.y, hit.distance/2f);
					currentLaser.transform.position = currentLaser.transform.forward*hit.distance/2f+bulletEmitter.position;
				}
			}
			else
			{
				currentLaser = null;
			}
			laserDurationTimer += Time.deltaTime;
		}
		else
		{
			if(fireTimer > fireRate)
			{
				currentLaser = (LaserBullet)Util.FireLaserType(currentBullet, bulletEmitter.position, Util.player.transform.position, Quaternion.LookRotation(Util.player.transform.position - bulletEmitter.position));
				laserDurationTimer = 0;
				fireTimer = 0;
			}
			fireTimer += Time.deltaTime;
		}
	}
	private Vector3 GenerateBiasedVector(Vector3 bias, int numberOfVectorsToCheck)
	{
		Vector3 temp1;
		Vector3 output = Random.onUnitSphere;
		int count = 1;
		while(count < numberOfVectorsToCheck)
		{
			temp1 = Random.onUnitSphere;
			if(Vector3.Angle(temp1, bias) < Vector3.Angle(output, bias))
			{
				output = temp1;
			}
			count++;
		}
		return output;
	}
	private void GenerateNextMovementPoint(Vector3 distance)
	{
		distance = distance.normalized;
		RaycastHit info;
		Vector3 possible = GenerateBiasedVector(distance, 3);
		Vector3 bestSoFar = Vector3.zero;
		float bestDistanceSoFar = 0;
		float bestAngleSoFar = 360;
		int i = 0;
		while(i < 5)
		{
			Physics.Raycast(transform.position, possible, out info);
			if(bestDistanceSoFar < info.distance && bestAngleSoFar > Vector3.Angle(possible, distance))
			{
				bestSoFar = possible;
				bestAngleSoFar = Vector3.Angle(possible, distance);
				bestDistanceSoFar = info.distance;
			}
			possible = GenerateBiasedVector(distance, 3);
			i++;
		}
		if(bestDistanceSoFar < maxFlitDistance)
		{
			moveToFlitPosition = transform.position + bestSoFar*bestDistanceSoFar/2;
		}
		else 
		{
			moveToFlitPosition = transform.position + bestSoFar*Random.Range(minFlitDistance, maxFlitDistance);
		}
	}
	public override void RealCollisionHandler(Collider other)
	{
		if(!isAttachedToGameObject)
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
	public override void OnPlayerEnter()
	{
		if(!isAttachedToGameObject)
		{
			isAwake = true;
		}
	}
	void OnTriggerEnter(Collider other)
	{
		RealCollisionHandler(other);
	}
	public override void KillMe()
	{
		deathTimeoutTimer += Time.deltaTime;
		rigidbody.useGravity = true;
		rigidbody.isKinematic = false;
		currentLaser = null;
		gameObject.layer = LayerMask.NameToLayer("Default");
	}
	public override void OnPlayerExit()
	{
		
	}
}