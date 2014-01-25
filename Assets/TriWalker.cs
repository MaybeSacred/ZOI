using UnityEngine;
using System.Collections.Generic;

public class TriWalker : BaseEnemy {
	public Transform frontFootLeft;
	public Transform frontFootRight;
	public Transform backFoot;
	public float timeStep;
	private float currentTime;
	private bool backLegMoving;
	public Vector3 forwardForce;
	public Vector3 upForce;
	public float minFootCollisionDist;
	void Start () {
	
	}

	void Update ()
	{
		if(backLegMoving)
		{
			RaycastHit rh;
			Physics.Raycast(new Ray(backFoot.position, Vector3.down), out rh, 50f);
			if(rh.distance < minFootCollisionDist)
			{
				backFoot.rigidbody.AddForceAtPosition(forwardForce, new Vector3(0, 1, 0));
			}
			Physics.Raycast(new Ray(frontFootLeft.position, Vector3.down), out rh, 50f);
			if(rh.distance < minFootCollisionDist)
			{
				frontFootLeft.rigidbody.AddForce(upForce);
			}
			Physics.Raycast(new Ray(frontFootRight.position, Vector3.down), out rh, 50f);
			if(rh.distance < minFootCollisionDist)
			{
				frontFootRight.rigidbody.AddForce(upForce);
			}
			if(currentTime > timeStep)
			{
				backLegMoving = false;
				currentTime -= timeStep;
			}
		}
		else
		{
			RaycastHit rh;
			Physics.Raycast(new Ray(backFoot.position, Vector3.down), out rh, 50f);
			if(rh.distance < minFootCollisionDist)
			{
				backFoot.rigidbody.AddForce(upForce);
			}
			Physics.Raycast(new Ray(frontFootLeft.position, Vector3.down), out rh, 50f);
			if(rh.distance < minFootCollisionDist)
			{
				frontFootLeft.rigidbody.AddForceAtPosition(forwardForce, new Vector3(0, 1, 0));
			}
			Physics.Raycast(new Ray(frontFootRight.position, Vector3.down), out rh, 50f);
			if(rh.distance < minFootCollisionDist)
			{
				frontFootRight.rigidbody.AddForceAtPosition(forwardForce, new Vector3(0, 1, 0));
			}
			if(currentTime > timeStep)
			{
				backLegMoving = true;
				currentTime -= timeStep;
			}
		}
		currentTime += Time.deltaTime;
	}
	public void OnTriggerEnter(Collider other)
	{
		if(other.tag.Equals("BasicExplosion"))
		{
			try
			{
				BasicExplosion be = (BasicExplosion)other.GetComponent<BasicExplosion>();
				rigidbody.AddExplosionForce(be.explosionForce, other.transform.position, be.explosionRadius);
				HealthChange(-be.shieldDamage, -be.healthDamage);
			}
			catch
			{
				Debug.Log("Incorrect tag assignment for tag \"Basic Explosion\"");
			}
		}
	}

}
