using UnityEngine;
using System.Collections;

public class ToothlessSpiderbotBehavior : BaseEnemy {

	public float lookAheadTime, rotationDelta, legSpeed, movementSpeed;
	public GameObject[] legs;
	
	public float movementForce, fireRate,numBursts, firingRandomness, reloadTime;
	public float shieldDamage;
	public float healthDamage;
	private float currentBurstNum,fireTimer, legTimer, hitDistance;
	public Transform bulletEmitter, cinematicAngle;
	public BasicBullet currentBullet;
	public float legMovementSpeed;

	public float firingAngle;

	void Start () {
		isAwake = true;
	}
	
	void Update () {
		if (deathTimeoutTimer > 0) {
			if (deathTimeoutTimer > deathTimeout) {
				Destroy (gameObject);
			}
			deathTimeoutTimer += Time.deltaTime;
		} else {
			if(isAwake)
			{
				legTimer+=Time.deltaTime;
				
				//handles looking at player
				transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Util.player.transform.position-transform.position+Util.player.rigidbody.velocity*lookAheadTime), rotationDelta*Time.deltaTime);
				
				#region legMovement
				//leg movement
				if (0.5*legSpeed<legTimer&&legTimer<legSpeed)
				{
					legs[0].transform.position = new Vector3(legs[0].transform.position.x,legs[0].transform.position.y+legMovementSpeed * Time.deltaTime,legs[0].transform.position.z);
					legs[1].transform.position = new Vector3(legs[1].transform.position.x,legs[1].transform.position.y+legMovementSpeed * Time.deltaTime,legs[1].transform.position.z);
				}
				else if (1.5*legSpeed<legTimer&&legTimer<legSpeed*2)
				{
					legs[4].transform.position = new Vector3(legs[4].transform.position.x,legs[4].transform.position.y+legMovementSpeed * Time.deltaTime,legs[4].transform.position.z);
					legs[5].transform.position = new Vector3(legs[5].transform.position.x,legs[5].transform.position.y+legMovementSpeed * Time.deltaTime,legs[5].transform.position.z);
				}
				else if (2.5*legSpeed<legTimer&&legTimer<legSpeed*3)
				{
					legs[2].transform.position = new Vector3(legs[2].transform.position.x,legs[2].transform.position.y+legMovementSpeed * Time.deltaTime,legs[2].transform.position.z);
					legs[3].transform.position = new Vector3(legs[3].transform.position.x,legs[3].transform.position.y+legMovementSpeed * Time.deltaTime,legs[3].transform.position.z);
				}
				else if (3.5*legSpeed<legTimer&&legTimer<legSpeed*4)
				{
					legs[6].transform.position = new Vector3(legs[6].transform.position.x,legs[6].transform.position.y+legMovementSpeed * Time.deltaTime,legs[6].transform.position.z);
					legs[7].transform.position = new Vector3(legs[7].transform.position.x,legs[7].transform.position.y+legMovementSpeed * Time.deltaTime,legs[7].transform.position.z);
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
				
				SteerTowardsRigidBody(Util.player.transform.position + Util.player.rigidbody.velocity*lookAheadTime - (transform.position + new Vector3(0,-hitDistance,0)));
				
				//bulletEmitter = new Vector3 (transform.position.x, transform.position.y, transform.position.z + Vector3.forward*9);
				
				#endregion
				
				#region fireProjectile
				if(currentBurstNum < numBursts)
				{
					if(fireTimer > fireRate)
					{
						//debug for spider eating player Event
						//Util.mainCamera.SendMessage("SpiderEating",cinematicAngle.transform);
						Vector3 fireVector = Util.GenerateRandomVector3(BallisticVel(Util.player.transform.position + Util.player.rigidbody.velocity - transform.position, firingAngle), firingRandomness);
						Util.FireVel (currentBullet,bulletEmitter.position, Quaternion.LookRotation(fireVector), fireVector * currentBullet.initialSpeed);
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
		if(other.collider.tag.Equals("Player")){
			
		}
	}
	//handles ballistic motion of the projectile 
	private Vector3 BallisticVel(Vector3 targetPoint,float angle) {
		float h = targetPoint.y;  // get height difference
		targetPoint.y = 0;  // retain only the horizontal direction
		float dist = targetPoint.magnitude ;  // get horizontal distance
		float a = angle * Mathf.Deg2Rad;  // convert angle to radians
		targetPoint.y = dist * Mathf.Tan(a);  // set dir to the elevation angle
		dist += h / Mathf.Tan(a);  // correct for small height differences
		// calculate the velocity magnitude
		float vel = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * a));
		return vel * targetPoint.normalized;
	}
}
