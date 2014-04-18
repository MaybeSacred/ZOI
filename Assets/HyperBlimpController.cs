using UnityEngine;
using System.Collections.Generic;

public class HyperBlimpController : BaseEnemy, PlayerEvent {
	/*0-1 Determines how strongly blimp should rotate towards the player once in engagingDistance*/
	public float encirclingRotationAngle;
	private int randomTravelToSideOfPlayer;
	public float deathPullForce;
	public ParticleSystem enginePS;
	public Transform[] rings;
	public float forwardStopDistance;
	public float forwardSideStopDistance;
	private int topPiecesLeft;
	private int enginePiecesRemaining = 3;
	public float maxEngageDistance;
	public float rotationSpeed;
	public float movementSpeed;
	public float desiredAltitude;
	public float altitudeDeadZone;
	private PlayerController player;
	public float timeToEnginesStopAfterShot;
	private float timeIntoEnginesOff;
	public float correctionRotationSpeed;
	public float altitudeChangeSpeed;
	private float startEngineEmission;
	void Start () {
		player = Util.player;
		randomTravelToSideOfPlayer = Random.Range(-1, 1)>=0?1:-1;
		startEngineEmission = enginePS.emissionRate;
	}
	void Update () {
		if(deathTimeoutTimer > 0)
		{
			if(deathTimeoutTimer > deathTimeout)
			{
				Destroy(gameObject);
			}
			
			rigidbody.AddForce(transform.forward.normalized*deathPullForce);
			deathTimeoutTimer += Time.deltaTime;
			enginePS.emissionRate = Mathf.Lerp(startEngineEmission, 0, deathTimeoutTimer/deathTimeout);
		}
		else
		{
			if(isAwake)
			{
				RaycastHit hit;
				if(enginePiecesRemaining > 0)
				{
					Vector3 playerDistanceXZ = new Vector3(player.transform.position.x - transform.position.x, 0, player.transform.position.z - transform.position.z);
					Vector3 correctionVector = transform.forward;
					Physics.Raycast(transform.position, transform.forward, out hit, float.PositiveInfinity, Util.PLAYERWEAPONSIGNORELAYERS & ~(1<<10));
					if(hit.distance < forwardStopDistance)
					{
						correctionVector = -correctionVector;
					}
					Physics.Raycast(transform.position, transform.forward+transform.right, out hit, float.PositiveInfinity, Util.PLAYERWEAPONSIGNORELAYERS & ~(1<<10));
					if(hit.distance < forwardSideStopDistance)
					{
						correctionVector = Vector3.RotateTowards(correctionVector, -transform.right, rotationSpeed, 0);
					}
					Physics.Raycast(transform.position, transform.forward-transform.right, out hit, float.PositiveInfinity, Util.PLAYERWEAPONSIGNORELAYERS & ~(1<<10));
					if(hit.distance < forwardSideStopDistance)
					{
						correctionVector = Vector3.RotateTowards(correctionVector, transform.right, rotationSpeed, 0);
					}
					transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(correctionVector), Time.deltaTime*correctionRotationSpeed);
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
				Physics.Raycast(transform.position, -Vector3.up, out hit, float.PositiveInfinity, Util.PLAYERWEAPONSIGNORELAYERS & ~(1<<10));
				if(transform.position.y - player.transform.position.y < hit.distance)
				{
					if(transform.position.y - player.transform.position.y > desiredAltitude + altitudeDeadZone)
					{
						transform.position -= new Vector3(0, altitudeChangeSpeed * Time.deltaTime, 0);
					}
					else if(transform.position.y - player.transform.position.y < desiredAltitude - altitudeDeadZone)
					{
						transform.position += new Vector3(0, altitudeChangeSpeed * Time.deltaTime, 0);
					}
				}
				else
				{
					if(hit.distance > desiredAltitude + altitudeDeadZone)
					{
						transform.position -= new Vector3(0, altitudeChangeSpeed * Time.deltaTime, 0);
					}
					else if(hit.distance < desiredAltitude - altitudeDeadZone)
					{
						transform.position += new Vector3(0, altitudeChangeSpeed * Time.deltaTime, 0);
					}
				}
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
						HealthChange(-be.shieldDamage, -be.healthDamage);
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
		Util.theGUI.RemoveRadarObject(transform);
		foreach(Transform ring in rings)
		{
			BlimpRingPiece[] pieces = ring.GetComponentsInChildren<BlimpRingPiece>();
			foreach(BlimpRingPiece p in pieces)
			{
				p.HealthChange(0, float.NegativeInfinity);
			}
		}
		rigidbody.isKinematic = false;
		rigidbody.velocity = transform.forward*movementSpeed;
	}
	public override void OnPlayerEnter ()
	{
		base.OnPlayerEnter ();
		foreach(Transform ring in rings)
		{
			BlimpRingPiece[] pieces = ring.GetComponentsInChildren<BlimpRingPiece>();
			foreach(BlimpRingPiece p in pieces)
			{
				p.OnPlayerEnter();
			}
		}
	}
	public override void OnPlayerExit ()
	{
		base.OnPlayerExit ();
		foreach(Transform ring in rings)
		{
			BlimpRingPiece[] pieces = ring.GetComponentsInChildren<BlimpRingPiece>();
			foreach(BlimpRingPiece p in pieces)
			{
				p.OnPlayerExit();
			}
		}
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
		}
	}
	public void AddPiece()
	{
		topPiecesLeft++;
	}
}
