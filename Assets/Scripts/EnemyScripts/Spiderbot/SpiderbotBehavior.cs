using UnityEngine;
using System.Collections;
//using System.Collections.Generic;

//@Chris Tansey
//contact: cmtansey@gatech.edu
public class SpiderbotBehavior : BaseEnemy {
	
	public float lookAheadTime, rotationDelta, legTimer, legSpeed, movementSpeed;
	private System.Collections.Generic.List<GameObject> colliders;
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
				if (legTimer<legSpeed)
				{
					legs[0].rigidbody.AddForce(1,5,0);
					print("leg");
					legs[1].rigidbody.AddForce(1,5,0);
				}
				else if (legTimer<legSpeed*2)
				{
					legs[4].rigidbody.AddForce(1,5,0);
					legs[5].rigidbody.AddForce(1,5,0);
				}
				else if (legTimer<legSpeed*3)
				{
					legs[2].rigidbody.AddForce(1,5,0);
					legs[3].rigidbody.AddForce(1,5,0);
				}
				else if (legTimer<legSpeed*4)
				{
					legs[6].rigidbody.AddForce(1,5,0);
					legs[7].rigidbody.AddForce(1,5,0);
				}else legTimer=0;

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
					ParticleSystem ps = Instantiate(explosionPS, transform.position - rigidbody.velocity*Time.fixedDeltaTime, transform.rotation) as ParticleSystem;
					speed = Vector3.zero;
					acceleration = Vector3.zero;
					timeOutCounter += Time.deltaTime;
					GetComponent<Collider>().enabled = false;
					if(GetComponent<MeshRenderer>() != null)
					{
						GetComponent<MeshRenderer>().enabled = false;
					}
					ps.GetComponent<BasicExplosion>().explosionDuration = explosionDuration;
					ps.GetComponent<BasicExplosion>().shieldDamage = shieldDamage;
					ps.GetComponent<BasicExplosion>().healthDamage = healthDamage;
					ps.GetComponent<BasicExplosion>().side = side;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(true)
		{
			print ("collision");
			if(other.tag.Equals("BasicExplosion"))
			{
				try
				{
					if(!colliders.Contains(other.gameObject))
					{
						colliders.Add(other.gameObject);
						BasicExplosion be = (BasicExplosion)other.GetComponent<BasicExplosion>();
						rigidbody.AddExplosionForce(be.explosionForce, other.transform.position, be.explosionRadius);
						HealthChange(-be.shieldDamage, -be.healthDamage);
					}
				}
				catch(System.InvalidCastException ie)
				{
					Debug.Log("Incorrect tag assignment for tag \"Basic Explosion\"");
				}
			}
			if(other.tag.Equals("Player"))
			{
				print("player collision");
				try
				{
					ParticleSystem ps = Instantiate(explosionPS, transform.position - rigidbody.velocity*Time.fixedDeltaTime, transform.rotation) as ParticleSystem;
					speed = Vector3.zero;
					acceleration = Vector3.zero;
					timeOutCounter += Time.deltaTime;
					GetComponent<Collider>().enabled = false;
					GetComponent<ParticleSystem>().Stop();
					if(GetComponent<MeshRenderer>() != null)
					{
						GetComponent<MeshRenderer>().enabled = false;
					}
					ps.GetComponent<BasicExplosion>().explosionDuration = explosionDuration;
					ps.GetComponent<BasicExplosion>().shieldDamage = shieldDamage;
					ps.GetComponent<BasicExplosion>().healthDamage = healthDamage;
					ps.GetComponent<BasicExplosion>().side = side;
				}
				catch(System.InvalidCastException ie)
				{
					Debug.Log ("incorrect tag assignment for tag \"Basic Explosion\"");
				}
			}
		}
	}
}
