using UnityEngine;
using System.Collections;

public class RocketBullet : BasicBullet {
	public float homingStrength;
	public Transform targetedTransform;
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
						int temp = Mathf.FloorToInt(Random.Range(0, colliderz.Length));
						if(colliderz[temp].transform.tag.Equals("EnemyGenerator"))
						{
							if(colliderz.Length > 1)
							{
								temp = (temp >= colliderz.Length-1?0:temp+1);
								targetedTransform = colliderz[temp].transform;
								lifetimeTimer = 0;
							}
						}
						else
						{
							lifetimeTimer = 0;
							targetedTransform = colliderz[temp].transform;
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
				rigidbody.AddTorque(homingStrength*Vector3.Cross(transform.forward, distance.normalized));
			}
		}
		rigidbody.AddForce(transform.forward*rocketStrength);
		if(rigidbody.velocity.magnitude > maxSpeed)
		{
			rigidbody.AddForce(-transform.forward*.25f);
		}
		if(timeOutCounter > 0)
		{
			if(timeOutCounter > endTime)
			{
				Destroy(this.gameObject);
			}
			timeOutCounter += Time.deltaTime;
		}
		if(lifetimeTimer > lifetime)
		{
			timeOutCounter += Time.deltaTime;
		}
		lifetimeTimer += Time.deltaTime;
	}
}
