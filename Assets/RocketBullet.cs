using UnityEngine;
using System.Collections;

public class RocketBullet : BasicBullet {
	public float homingStrength;
	public Transform targetedTransform;
	public float distanceBetweenChecks;
	public float checkRadius;
	public float secondCheckRadius;
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
			if(checkTimer > distanceBetweenChecks)
			{
				if(Physics.CheckCapsule(transform.position, transform.position + transform.forward*2*checkRadius, checkRadius, 1<<10))
				{
					Collider[] colliderz = Physics.OverlapSphere(transform.position, secondCheckRadius, 1<<10);
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
				checkTimer-=distanceBetweenChecks;
			}
			checkTimer += Time.deltaTime;
			rigidbody.AddForce(rigidbody.velocity.normalized*rocketStrength);
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
				rigidbody.AddForce(homingStrength*(targetedTransform.position - transform.position).normalized);
			}
		}
		if(rigidbody.velocity.magnitude > maxSpeed)
		{
			rigidbody.AddForce(-rigidbody.velocity.normalized*.5f);
		}
		transform.rotation = Quaternion.LookRotation(rigidbody.velocity);
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
