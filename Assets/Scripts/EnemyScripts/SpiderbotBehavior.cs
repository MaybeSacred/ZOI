using System.Collections;
using UnityEngine;
using System.Collections.Generic;

//@Chris Tansey
//contact: cmtansey@gatech.edu
public class SpiderbotBehavior : BaseEnemy {
	
	public float lookAheadTime, rotationDelta, legSpeed, movementSpeed;
	public GameObject[] legs;

	public float movementForce, fireRate,numBursts, firingRandomness, reloadTime;
	public float shieldDamage, healthDamage, stunDuration;
	private float currentBurstNum,fireTimer, legTimer,hitDistance;
	public float legMovementHeight;
	public Transform bulletEmitter, cinematicAngle;
	public BasicBullet currentBullet;
	public NavMeshAgent navi;
	public GameObject Hitbox;
	public GameObject explosionPos;
	bool dying;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (deathTimeoutTimer > 0) {
			if(dying ==false)
			{
				Destroy(Hitbox);
				SphereCollider mycollider = (SphereCollider) GetComponent<SphereCollider>();
				mycollider.center = new Vector3(0f,0f,0f);
				mycollider.radius = 0.5f;
			}
			bool dead =true;
			KillMe();
			if (deathTimeoutTimer > deathTimeout) {
				Destroy(gameObject);
			}
			deathTimeoutTimer += Time.deltaTime;
		} else {
			if(isAwake)
			{
				if(health <= 0)
				{
					KillMe();
				}

				//handles looking at player
				if(navi.remainingDistance>10f)
				{
					transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Util.player.transform.position-transform.position+Util.player.rigidbody.velocity*lookAheadTime), rotationDelta*Time.deltaTime);
					legTimer+=Time.deltaTime;
				}else legTimer = 0;
				#region legMovement
				//leg movement
				if (0.5*legSpeed<legTimer&&legTimer<legSpeed)
				{
					legs[0].transform.position = new Vector3(legs[0].transform.position.x,legs[0].transform.position.y+Time.deltaTime*legMovementHeight,legs[0].transform.position.z);
					legs[1].transform.position = new Vector3(legs[1].transform.position.x,legs[1].transform.position.y+Time.deltaTime*legMovementHeight,legs[1].transform.position.z);
				}
				else if (1.5*legSpeed<legTimer&&legTimer<legSpeed*2)
				{
					legs[4].transform.position = new Vector3(legs[4].transform.position.x,legs[4].transform.position.y+Time.deltaTime*legMovementHeight,legs[4].transform.position.z);
					legs[5].transform.position = new Vector3(legs[5].transform.position.x,legs[5].transform.position.y+Time.deltaTime*legMovementHeight,legs[5].transform.position.z);
				}
				else if (2.5*legSpeed<legTimer&&legTimer<legSpeed*3)
				{
					legs[2].transform.position = new Vector3(legs[2].transform.position.x,legs[2].transform.position.y+Time.deltaTime*legMovementHeight,legs[2].transform.position.z);
					legs[3].transform.position = new Vector3(legs[3].transform.position.x,legs[3].transform.position.y+Time.deltaTime*legMovementHeight,legs[3].transform.position.z);
				}
				else if (3.5*legSpeed<legTimer&&legTimer<legSpeed*4)
				{
					legs[6].transform.position = new Vector3(legs[6].transform.position.x,legs[6].transform.position.y+Time.deltaTime*legMovementHeight,legs[6].transform.position.z);
					legs[7].transform.position = new Vector3(legs[7].transform.position.x,legs[7].transform.position.y+Time.deltaTime*legMovementHeight,legs[7].transform.position.z);
				}else if(legTimer>legSpeed*4)
					legTimer = 0;

				#endregion


				#region movementHandling
				//raycast offsets spider to touch ground
				RaycastHit hit;
				if(Physics.Raycast(transform.position,Vector3.down,out hit))
				{
					hitDistance = hit.distance;
					//Debug.DrawRay(transform.position,Vector3.down,Color.green);

				}

				//alternate movement option
				//transform.position = Vector3.Lerp(transform.position,Util.player.transform.position, .5f-.5f*Mathf.Cos(movementSpeed));
				
				navi.SetDestination(Util.player.transform.position);
				//SteerTowardsRigidBody(Util.player.transform.position - (transform.position/*-new Vector3(0,-hitDistance,0)*/) + Util.player.rigidbody.velocity*lookAheadTime);

				//bulletEmitter = new Vector3 (transform.position.x, transform.position.y, transform.position.z + Vector3.forward*9);

				#endregion

				#region fireProjectile
				if(currentBurstNum < numBursts)
				{
					if(fireTimer > fireRate)
					{
						//default projectile settings for ZoI
						Vector3 temp = Random.insideUnitSphere;
						Vector3 temp2 = (bulletEmitter.position-transform.position).normalized;
						temp = firingRandomness*Vector3.Cross(temp2, temp);

						//debug for spider eating player Event
						//Util.mainCamera.SendMessage("SpiderEating",cinematicAngle.transform);


						Util.FireVel (currentBullet,bulletEmitter.position, Quaternion.LookRotation(bulletEmitter.position-transform.position),
						           BallisticVel(Util.player.transform,60f));

						//non-ballistic Projectile
						/*Util.Fire(currentBullet, bulletEmitter.position, Quaternion.LookRotation(bulletEmitter.position-transform.position-temp), 
						          (bulletEmitter.position-transform.position-temp).normalized*currentBullet.initialSpeed);*/


						currentBurstNum++;
						fireTimer -= fireRate;
					}
				}else
				{
					if(fireTimer > reloadTime)
					{
						fireTimer -= reloadTime;
						currentBurstNum = 0;
					}
				}
				fireTimer += Time.deltaTime;
				#endregion

			}
		}
		
		
		
	}
	
	private void SteerTowardsRigidBody(Vector3 direction)
	{
		direction.Normalize();
		rigidbody.AddForce(direction*movementForce);
		//print(rigidbody.velocity.magnitude);
	}
	public override void KillMe ()
	{
		if(attachedBarrier != null&&calledBarrier ==false)
		{
			calledBarrier =true;
			attachedBarrier.UnregisterEnemy();
		}
		Util.theGUI.RemoveRadarObject(transform);
		/*Hitbox.rigidbody.useGravity = true;
		Hitbox.rigidbody.isKinematic = false;
		Hitbox.rigidbody.constraints = RigidbodyConstraints.None;*/
		rigidbody.isKinematic = false;
		rigidbody.useGravity = true;
		gameObject.collider.isTrigger = false;
		gameObject.rigidbody.mass = 2;
		gameObject.collider.tag = "Untagged";

		navi.enabled = false;
		//gameObject.collider.enabled = false;
		gameObject.rigidbody.AddExplosionForce (18.0f, bulletEmitter.transform.position,-0.1f);
		deathTimeoutTimer += Time.deltaTime;
		HingeJoint[] joints = GetComponentsInChildren<HingeJoint>();
		float x = 3f;
		for(int i = 0; i < joints.Length; i++)
		{
			joints[i].collider.enabled = true;
			if(x<0.02f)
			{

					joints[i].breakForce = joints[i].breakTorque = 0;

			}
			x = Random.Range(0f,3f);
		}
	}
	
	void OnCollisionEnter(Collision other)
	{
		if(other.collider.tag.Equals("Player")){
			
		}
	}

	public void RealCollisionHandler(Collider other)
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
	
	//handles ballistic motion of the projectile 
	private Vector3 BallisticVel(Transform target,float angle) {
		Vector3 dir = target.position+ target.rigidbody.velocity- bulletEmitter.transform.position;  // get target direction
		float h = dir.y;  // get height difference
		dir.y = 0;  // retain only the horizontal direction
		float dist = dir.magnitude ;  // get horizontal distance
		float a = angle * Mathf.Deg2Rad;  // convert angle to radians
		dir.y = dist * Mathf.Tan(a);  // set dir to the elevation angle
		dist += h / Mathf.Tan(a);  // correct for small height differences
		// calculate the velocity magnitude
		float vel = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * a));
		return vel * dir.normalized;
	}
	public override void OnPlayerExit()
	{
		
	}
}