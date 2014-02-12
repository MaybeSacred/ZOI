using UnityEngine;
using System.Collections;
//using System.Collections.Generic;

//@Chris Tansey
//contact: cmtansey@gatech.edu
public class SpiderbotBehavior : BaseEnemy {
	
	public float lookAheadTime, rotationDelta, legTimer, legSpeed, movementSpeed;
	private bool breakingDown;
	public GameObject[] legs;
	
	public int side;
	public float lifetime;
	protected float lifetimeTimer;
	public ParticleSystem explosionPS;
	public Vector3 acceleration;
	public Vector3 speed;
	public float shieldDamage;
	public float healthDamage;
	public float timeOutCounter;
	public float explosionDuration;
	protected float endTime;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (deathTimeoutTimer > 0) {
			if (deathTimeoutTimer > deathTimeout) {
				Destroy (gameObject);
			}
			deathTimeoutTimer += Time.deltaTime;
		} else {
			if(true)
			{
				legTimer+=Time.deltaTime;
				Vector3 distance = Util.player.transform.position + Util.player.rigidbody.velocity*lookAheadTime;
				transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(distance), rotationDelta*Time.deltaTime);

				//leg movement
				if (0.5*legSpeed<legTimer&&legTimer<legSpeed)
				{
					legs[0].transform.position = new Vector3(legs[0].transform.position.x,legs[0].transform.position.y+0.2f,legs[0].transform.position.z);
					legs[1].transform.position = new Vector3(legs[1].transform.position.x,legs[1].transform.position.y+0.2f,legs[1].transform.position.z);
				}
				else if (1.5*legSpeed<legTimer&&legTimer<legSpeed*2)
				{
					legs[4].transform.position = new Vector3(legs[4].transform.position.x,legs[4].transform.position.y+0.2f,legs[4].transform.position.z);
					legs[5].transform.position = new Vector3(legs[5].transform.position.x,legs[5].transform.position.y+0.2f,legs[5].transform.position.z);
				}
				else if (2.5*legSpeed<legTimer&&legTimer<legSpeed*3)
				{
					legs[2].transform.position = new Vector3(legs[2].transform.position.x,legs[2].transform.position.y+0.2f,legs[2].transform.position.z);
					legs[3].transform.position = new Vector3(legs[3].transform.position.x,legs[3].transform.position.y+0.2f,legs[3].transform.position.z);
				}
				else if (3.5*legSpeed<legTimer&&legTimer<legSpeed*4)
				{
					legs[6].transform.position = new Vector3(legs[6].transform.position.x,legs[6].transform.position.y+0.2f,legs[6].transform.position.z);
					legs[7].transform.position = new Vector3(legs[7].transform.position.x,legs[7].transform.position.y+0.2f,legs[7].transform.position.z);
				}else if(legTimer>legSpeed*4)
					legTimer = 0;

				transform.position = Vector3.Lerp(transform.position,Util.player.transform.position, .5f-.5f*Mathf.Cos(movementSpeed));
			}
		}
					

	
	}

	public override void KillMe ()
	{
		deathTimeoutTimer += Time.deltaTime;
		FixedJoint[] joints = GetComponentsInChildren<FixedJoint>();
		for(int i = 0; i < joints.Length; i++)
		{
			joints[i].breakForce = joints[i].breakTorque = 0;
		}
	}

	void OnCollisionEnter(Collision other)
	{
		print("collision1");
		if(other.collider.tag.Equals("Player")){

		}
	}


}
