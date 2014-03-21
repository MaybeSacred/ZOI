﻿//@Chris Tansey
//email: cmtansey@gatech.edu

using UnityEngine;
using System.Collections;

public class SlinkyEnemyBehavior : MonoBehaviour {

	public float lookAheadTime, rotationDelta, firingDistance;
	public float fireTimer,fireRate, laserTimer, laserDuration;
	public NavMeshAgent navi;
	public bool miniSlinky, armedSlinky;
	public LaserBullet currentLaser;
	public Transform bulletEmitter;
	public LaserBullet currentBullet;
	//debugger for finding Vector3 positions
	//public GameObject debugObject;

	// Use this for initialization
	void Start () {

	
	}
	
	// Update is called once per frame
	void Update () {

		if (armedSlinky) 
		{
			Vector3 distance = Util.player.transform.position - transform.position;
			if (distance.magnitude < firingDistance) {
					FiringHandler ();



			}
		}
		//finds player for navMeshAgent
		navi.SetDestination (Util.player.transform.position);

		//handles rotating towards the player
		//transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Util.player.transform.position-transform.position+Util.player.rigidbody.velocity*lookAheadTime), rotationDelta*Time.deltaTime);

		//handles rotating towards destination
		transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(navi.steeringTarget-transform.position+Util.player.rigidbody.velocity*lookAheadTime),rotationDelta*Time.deltaTime);

		//debugging options:
		//debug distance to destination
		//print (navi.remainingDistance);
		//print (navi.steeringTarget);

		//GUI for location of steering target
		//debugObject.SendMessage ("sendPosition", navi.steeringTarget);


	}

	void moveOffset()
	{
		if (miniSlinky)
						transform.Translate (Vector3.forward * 4);
				else
			transform.Translate (Vector3.forward*8);
						
	}

	private void FiringHandler()
	{
		if(currentLaser != null)
		{
			if(laserTimer < laserDuration)
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
			laserTimer += Time.deltaTime;
		}
		else
		{
			if(fireTimer > fireRate)
			{
				currentLaser = (LaserBullet)Util.FireLaserType(currentBullet, bulletEmitter.position, Util.player.transform.position, Quaternion.LookRotation(Util.player.transform.position - bulletEmitter.position));
				laserTimer = 0;
				fireTimer = 0;
			}
			fireTimer += Time.deltaTime;
		}
	}
}