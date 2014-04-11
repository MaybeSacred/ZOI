//@Chris Tansey
//email: cmtansey@gatech.edu

using UnityEngine;
using System.Collections;

public class SlinkyEnemyBehavior : BaseEnemy {

	public float lookAheadTime, rotationDelta, firingDistance;
	public float fireTimer,fireRate, laserTimer, laserDuration;
	public NavMeshAgent navi;
	public bool miniSlinky, armedSlinky;
	public LaserBullet currentLaser;
	public Transform bulletEmitter;
	public LaserBullet currentBullet;
	public GameObject child, sphere;
	Rigidbody[] childrenBodies;
	public int squashHealthDamage, squashShieldDamage;
	public ParticleSystem explosion;
	//debugger for finding Vector3 positions
	//public GameObject debugObject;

	// Use this for initialization
	void Start () {

	
	}
	
	// Update is called once per frame
	void Update () {
		/*if (Util.isPaused)
						isAwake = false;
				else
						isAwake = true;*/
		if (deathTimeoutTimer > 0) {
			if(deathTimeoutTimer<0.5f)
				KillMe ();
			if (deathTimeoutTimer > deathTimeout) 
			{
				Destroy(gameObject);
			}
					deathTimeoutTimer += Time.deltaTime;
		} else 
		{
				#region ActivityHandler
				if (isAwake) {
						child.SendMessage ("SetMoving", true);
						navi.enabled = true;
						if (armedSlinky) {
								Vector3 distance = Util.player.transform.position - transform.position;
								if (distance.magnitude < firingDistance) 
								{
												FiringHandler ();



								}
						}
						//finds player for navMeshAgent
						navi.SetDestination (Util.player.transform.position);

						//handles rotating towards the player
						//transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Util.player.transform.position-transform.position+Util.player.rigidbody.velocity*lookAheadTime), rotationDelta*Time.deltaTime);

						//handles rotating towards destination
						transform.rotation = Quaternion.RotateTowards (transform.rotation, Quaternion.LookRotation (navi.steeringTarget - transform.position + Util.player.rigidbody.velocity * lookAheadTime), rotationDelta * Time.deltaTime);

						//debugging options:
						//debug distance to destination
						//print (navi.remainingDistance);
						//print (navi.steeringTarget);

						//GUI for location of steering target
						//debugObject.SendMessage ("sendPosition", navi.steeringTarget);
				} else {
						child.SendMessage ("SetMoving", false);
				}
				#endregion
		}
	}
	//handles moving the slinky forward relative to its animation cycle.
	void moveOffset()
	{
		if (isAwake)
		{
				if (miniSlinky)
						transform.Translate (Vector3.forward * 4);
				else
						transform.Translate (Vector3.forward * 8);
		}
						
	}
	#region FiringHandler
	//for armed slinky enemies only
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
	#endregion

	#region CollisionHandler
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
				}
			}
			catch(System.InvalidCastException ie)
			{
				Debug.Log("Incorrect tag assignment for tag \"Basic Explosion\"");
			}
		}
	}
	#endregion

	public override void KillMe ()
	{
		child.SendMessage ("SetMoving", false);
		navi.enabled = false;
		child.SendMessage ("DisableAnimation");
		sphere.renderer.enabled = false;
		for(int i = 0; i < childrenBodies.Length; i++)
		{
			childrenBodies[i].isKinematic = false;
			childrenBodies[i].useGravity = true;
			childrenBodies[i].mass = 2;
			childrenBodies[i].collider.tag = "Untagged";

		}

		ParticleSystem particles =(ParticleSystem) Instantiate (explosion, transform.position, transform.rotation);
		particles.enableEmission = true;

		for(int i = 0; i < childrenBodies.Length; i++)
		{
			childrenBodies[i].AddExplosionForce(50.0f, transform.position, -0.1f);
			
		}


		//gameObject.collider.enabled = false;
		//gameObject.rigidbody.AddExplosionForce (18.0f, bulletEmitter.transform.position,-0.1f);
		deathTimeoutTimer += Time.deltaTime;

	}
	public override void OnPlayerEnter()
	{
			isAwake = true;
		Destroy (rigidbody);
		childrenBodies = gameObject.GetComponentsInChildren<Rigidbody> ();
		collider.enabled = false;
	}
}
