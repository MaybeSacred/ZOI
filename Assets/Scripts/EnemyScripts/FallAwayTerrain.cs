using UnityEngine;
using System.Collections;

public class FallAwayTerrain : BaseEnemy {

	private Vector3 initialPos;
	private Quaternion initialRot;
	private bool playerCollision;

	// Use this for initialization
	void Start () {
		initialPos = transform.position;
		initialRot = transform.rotation;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (!playerCollision) 
		{
			transform.position = initialPos;
			transform.rotation = initialRot;
		}
	
	}

	void OnCollisionEnter(Collision other)
	{
		/*if(other.collider.tag.Equals("Untagged"))
		{
			if(isAwakening && isAwakeningLeeWayTimer > awakeningLeeWayTime)
			{
				isAwake = true;
				isAwakening = false;
			}
		}
		else*/ if(other.collider.tag.Equals("Player")){
			playerCollision = true;
		}
	}
	void OnTriggerEnter(Collider other)
	{
		RealCollisionHandler(other);
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
						playerCollision = true;
					}
				}
				catch(System.InvalidCastException ie)
				{
					Debug.Log("Incorrect tag assignment for tag \"Basic Explosion\"");
				}
			}
			if(other.tag.Equals("Bullet"))
			{
				playerCollision = true;
			}
		}
	}
}
