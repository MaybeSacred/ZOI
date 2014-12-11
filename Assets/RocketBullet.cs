using UnityEngine;
using System.Collections;

public class RocketBullet : BasicBullet {
	public float homingStrength;
	private Transform targetedTransform;
	public float timeBetweenChecks;
	public float checkRadius;
	private float checkTimer;
	public float rocketStrength;
	public float maxSpeed;
	public float timeOutTime;
	private float timeOutTimer;
	void Start () {
		rigidbody.velocity = speed;
	}
	
	void Update () {
		if(targetedTransform == null)
		{
			if(checkTimer > timeBetweenChecks)
			{
				if(Physics.CheckSphere(transform.position, checkRadius, 1<<10))
				{
					Collider[] colliderz = Physics.OverlapSphere(transform.position, checkRadius, 1<<10);
					if(colliderz.Length > 0)
					{
						int temp = Mathf.FloorToInt(Random.Range(0, (float)colliderz.Length));
						BaseEnemy bemp = colliderz[temp].gameObject.GetComponent<BaseEnemy>();
						if(bemp != null && bemp.health > 0)
						{
							lifetimeTimer = 0;
							targetedTransform = colliderz[temp].transform;
						}
						else
						{
							bemp = colliderz[temp].transform.parent.GetComponent<BaseEnemy>();
							if(bemp != null && bemp.health > 0)
							{
								lifetimeTimer = 0;
								targetedTransform = colliderz[temp].transform;
							}
						}
					}
				}
				collider.enabled = true;
				checkTimer-=timeBetweenChecks;
			}
			checkTimer += Time.deltaTime;
		}
		else
		{
			timeOutTimer += Time.deltaTime;
			if(timeOutTimer > timeOutTime)
			{
				rigidbody.useGravity = true;
			}
			else
			{
				Vector3 distance = targetedTransform.position - transform.position;
				rigidbody.AddTorque(Time.deltaTime * homingStrength * Vector3.Cross(transform.forward, distance.normalized));
			}
		}
		if(timeOutCounter > 0)
		{
			if(timeOutCounter > endTime)
			{
				Destroy(gameObject);
			}
			timeOutCounter += Time.deltaTime;
		}
		else if(lifetimeTimer > lifetime)
		{
			timeOutCounter += Time.deltaTime;
			DestroyMe();
		}
		lifetimeTimer += Time.deltaTime;
	}
	void FixedUpdate()
	{
		rigidbody.AddForce(transform.forward * rocketStrength * Time.deltaTime);
		if(rigidbody.velocity.magnitude > maxSpeed)
		{
			rigidbody.AddForce(-transform.forward * .25f * Time.deltaTime);
		}
	}
}
