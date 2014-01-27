using UnityEngine;
using System.Collections.Generic;

public class HyperBlimpController : BaseEnemy {
	/*0-1 Determines how strongly blimp should rotate towards the player once in engagingDistance*/
	public float encirclingRotationAngle;
	private int randomTravelToSideOfPlayer;
	public float deathPullForce;
	private List<GameObject> colliders;
	public Transform[] rings;
	private int topPiecesLeft;
	private int enginePiecesRemaining = 3;
	private bool isDamageable = false;
	public float maxEngageDistance;
	public float rotationSpeed;
	public float movementSpeed;
	public float desiredAltitude;
	public float altitudeDeadZone;
	private PlayerController player;
	public float timeToEnginesStopAfterShot;
	private float timeIntoEnginesOff;
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
			rigidbody.AddForce((-Vector3.up + transform.forward).normalized*deathPullForce);
			deathTimeoutTimer += Time.deltaTime;
		}
		else
		{
			if(enginePiecesRemaining > 0)
			{
				Vector3 playerDistanceXZ = new Vector3(player.transform.position.x - transform.position.x, 0, player.transform.position.z - transform.position.z);
				if(playerDistanceXZ.magnitude < maxEngageDistance)
				{
					Vector3 encirclingVector = randomTravelToSideOfPlayer * Vector3.Cross(playerDistanceXZ, Vector3.up);
					encirclingVector = Vector3.Lerp(encirclingVector, playerDistanceXZ, encirclingRotationAngle);
					transform.position += transform.forward*movementSpeed*Time.deltaTime;
					transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(encirclingVector), Time.deltaTime*rotationSpeed);
				}
				else
				{
					transform.position += transform.forward*movementSpeed*Time.deltaTime;
					transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(playerDistanceXZ), Time.deltaTime*rotationSpeed);
				}
			}
			else
			{
				transform.position += transform.forward*Time.deltaTime*Mathf.Lerp(movementSpeed, 0, timeIntoEnginesOff/timeToEnginesStopAfterShot);
				timeIntoEnginesOff += Time.deltaTime;
			}
			RaycastHit hit;
			Physics.Raycast(transform.position, -Vector3.up, out hit, 2*desiredAltitude, Util.PLAYERWEAPONSIGNORELAYERS & ~(1<<10));
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
	public void RemovePiece(bool isEnginePiece)
	{
		if(isEnginePiece)
		{
			enginePiecesRemaining--;
		}
		else
		{
			topPiecesLeft--;
			if(topPiecesLeft <= 0)
			{
				isDamageable = true;
			}
		}
	}
	public void AddPiece()
	{
		topPiecesLeft++;
	}
}
