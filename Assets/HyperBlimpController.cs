using UnityEngine;
using System.Collections.Generic;

public class HyperBlimpController : BaseEnemy {
	private int randomTravelToSideOfPlayer;
	private List<GameObject> colliders;
	public Transform[] rings;
	private int topPiecesLeft;
	private bool isDamageable = false;
	public float maxEngageDistance;
	public float rotationSpeed;
	public float movementSpeed;
	public float desiredAltitude;
	public float altitudeDeadZone;
	private PlayerController player;
	void Start () {
		colliders = new List<GameObject>();
		player = Util.player;
		randomTravelToSideOfPlayer = Random.Range(-1, 1)>=0?1:-1;
	}
	
	void Update () {
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
			Vector3 playerDistanceXZ = new Vector3(player.transform.position.x - transform.position.x, 0, player.transform.position.z - transform.position.z);
			if(playerDistanceXZ.magnitude < maxEngageDistance)
			{
				Vector3 encirclingVector = randomTravelToSideOfPlayer * Vector3.Cross(playerDistanceXZ, Vector3.up);
				transform.position += transform.forward*movementSpeed*Time.deltaTime;
				transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(encirclingVector), Time.deltaTime*rotationSpeed);
			}
			else
			{
				transform.position += transform.forward*movementSpeed*Time.deltaTime;
				transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(playerDistanceXZ), Time.deltaTime*rotationSpeed);
			}
			RaycastHit hit;
			Physics.Raycast(transform.position, -Vector3.up, out hit, 2*desiredAltitude, Util.PLAYERWEAPONSIGNORELAYERS);
			if(hit.distance > desiredAltitude + altitudeDeadZone)
			{
				transform.position -= new Vector3(0, Time.deltaTime, 0);
			}
			else if(hit.distance < desiredAltitude - altitudeDeadZone)
			{
				transform.position += new Vector3(0, Time.deltaTime, 0);
			}
		}
	}
	public override void HealthChange(float shieldDmg, float healthDmg)
	{
		if(health > 0)
		{
			health += healthDmg;
			if(health > maxHealth)
			{
				health = maxHealth;
			}
			if(health <= 0)
			{
				health = 0;
				KillMe();
			}
		}
	}
	public override void RealCollisionHandler(Collider other)
	{
		if(isDamageable)
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
	}
	void OnTriggerEnter(Collider other)
	{
		RealCollisionHandler(other);
	}
	public override void KillMe()
	{
		deathTimeoutTimer += Time.deltaTime;
		rigidbody.isKinematic = false;
	}
	public void RemovePiece()
	{
		topPiecesLeft--;
		if(topPiecesLeft <= 0)
		{
			isDamageable = true;
		}
	}
	public void AddPiece()
	{
		topPiecesLeft++;
	}
}
