using UnityEngine;
using System.Collections;

public class ClingyBullet : BasicBullet{
	public float homingStrength;
	private Transform targetedTransform;
	public float rocketStrength;
	public float maxSpeed;
	public float timeOutTime;
	private float timeOutTimer;
	void Start () {
		rigidbody.velocity = speed;
		targetedTransform = Util.player.transform;
	}
	
	void Update () {
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
			DestroyMe(transform.position);
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
