using UnityEngine;

public class WheelEnemy : BaseEnemy {
	public Transform graphicalObject, legs;
	public float lookAheadTime;
	public float movementForce;
	public float initialPopUpForce;
	public ParticleSystem explosionPS;
	
	public float shieldDamage;
	public float healthDamage;
	public float timeOutCounter;
	public float explosionDuration;
	private bool isAwakening = false;
	private Vector3 initialPosition;
	public float awakeningLeeWayTime;
	private float isAwakeningLeeWayTimer;
	// Use this for initialization
	void Start () {
		rigidbody.useGravity = false;
		initialPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(deathTimeoutTimer > 0)
		{
			if(deathTimeoutTimer > deathTimeout)
			{
				Destroy(gameObject);
			}
			deathTimeoutTimer += Time.deltaTime;
		}
		else
		{
			if(isAwake)
			{
				graphicalObject.rotation = Quaternion.LookRotation(rigidbody.velocity);
				legs.rotation *= new Quaternion(Mathf.Sin(rigidbody.velocity.magnitude*Time.deltaTime), 0, 0, Mathf.Cos(rigidbody.velocity.magnitude*Time.deltaTime));
			}
			else if(isAwakening)
			{
				collider.enabled = true;
				isAwakeningLeeWayTimer += Time.deltaTime;
			}
			else
			{
				collider.enabled = false;
				transform.position = initialPosition;
			}
		}
	}
	void FixedUpdate()
	{
		if(isAwake)
		{
			Vector3 distance = Util.player.transform.position - transform.position;
			SteerTowardsRigidBody(distance + Util.player.rigidbody.velocity*distance.magnitude/(rigidbody.velocity.magnitude < .05f? .05f: rigidbody.velocity.magnitude));
		}
	}
	private void SteerTowardsRigidBody(Vector3 direction)
	{
		direction.Normalize();
		direction.y += .1f;
		rigidbody.AddForce(direction*movementForce);
	}
	void OnCollisionEnter(Collision other)
	{
		if(other.collider.tag.Equals("Untagged"))
		{
			if(isAwakening && isAwakeningLeeWayTimer > awakeningLeeWayTime)
			{
				isAwake = true;
				isAwakening = false;
			}
		}
		else if(other.collider.tag.Equals("Player")){
			HealthChange(float.NegativeInfinity, float.NegativeInfinity);
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
			if(other.tag.Equals("Bullet"))
			{
				other.GetComponent<BasicBullet>().DestroyMe();
			}
		}
	}
	public override void KillMe()
	{
		ParticleSystem ps = Instantiate(explosionPS, transform.position, transform.rotation) as ParticleSystem;
		ps.GetComponent<BasicExplosion>().explosionDuration = explosionDuration;
		ps.GetComponent<BasicExplosion>().shieldDamage = shieldDamage;
		ps.GetComponent<BasicExplosion>().healthDamage = healthDamage;
		timeOutCounter += Time.deltaTime;
		Destroy(gameObject);
	}
	public override void OnPlayerEnter()
	{
		isAwakeningLeeWayTimer += Time.deltaTime;
		isAwakening = true;
		rigidbody.useGravity = true;
		rigidbody.AddForce(initialPopUpForce * transform.right);
	}
	public override void OnPlayerExit()
	{
		
	}
}
