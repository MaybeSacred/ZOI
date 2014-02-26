//@Chris Tansey
//email: cmtansey@gatech.edu

using UnityEngine;
using System.Collections;

public class SlinkyEnemyBehavior : MonoBehaviour {

	public float lookAheadTime, rotationDelta;
	public NavMeshAgent navi;

	//debugger for finding Vector3 positions
	//public GameObject debugObject;

	// Use this for initialization
	void Start () {

	
	}
	
	// Update is called once per frame
	void Update () {
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
		transform.Translate (Vector3.forward*8);
	}
}
