using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {
	private int steeringDirection;
	public bool strafeSteeringEngaged;
	public bool isStrafeSteeringDefaultOption;
	private WheelFrictionCurve startWheelFriction;
	private int numFlameParticlesPlaying;
	public ParticleSystem gameOverExplosion;
	public ParticleSystem[] flameParticles;
	public float centerOfMassAdjustment;
	public float deathTimeout;
	private float deathTimeoutTimer;
	public Vector3 restartPosition;
	public Vector3 restartRotation;
	private int lastCheckpoint = 0;
	private float lastCheckpointHealth;
	public ParticleSystem[] wheelGroundEffects;
	public float minShieldFlicker;
	public float maxShieldFlicker;
	public float shieldMaterialRate;
	public Transform shield;
	private Material shieldMat;
	private float timeSinceLastHit;
	public float shieldPct;
	public float shieldRechargeDelay;
	public float shieldRechargeRate;
	public WheelCollider[] wheels;
	public float[] secondaryCannonReloadTimers{get; private set;}
	public float primaryCannonTimer;
	public float primaryCannonReloadTime;
	public float secondaryOfflineReload;
	public float[] secondaryCannonReloadTime;
	public float[] firingRandomness;
	public int[] secondaryBulletsLeft{get; private set;}
	public Transform[] secondaryBulletEmitters;
	public ParticleSystem[] secondaryCannonFlashes;
	private float secondaryAutoFireTimer;
	public int[] totalSecondaryBullets;
	public ParticleSystem cannonFlash;
	public ParticleSystem cannonRingFlash;
	public Transform primaryBulletEmitter;
	public Transform cannonGO;
	public Transform cannonGraphics;
	private Vector3 initialCannonPosition;
	public Vector3 cannonKickbackDistance;
	public float cannonKickbackRestoreRate;
	public BasicBullet primaryBullet;
	public BasicBullet[] possibleSecondaries;
	public float[] secondaryAutoFireTimes;
	public int currentSecondaryWep{get; private set;}
	public Transform body;
	public Transform turret;
	public float maxSlope;
	public float autotargetDeltaAngle;
	public float steeringRate;
	public float steeringCenteringCoeff;
	public float bodyRotationEpsilon;
	public Camera theCam;
	public float forwardAcceleration;
	public float brakeForce;
	public float maxSpeed;
	private bool keyDown;
	public bool wasForwardKeyDown;
	public float health;
	public float maxHealth;
	private int numWheelsGrounded;
	private List<GameObject> colliders;
	void Start () {
		startWheelFriction = wheels[0].sidewaysFriction;
		primaryCannonTimer = primaryCannonReloadTime;
		secondaryCannonReloadTimers = new float[possibleSecondaries.Length];
		secondaryBulletsLeft = new int[possibleSecondaries.Length];
		lastCheckpointHealth = maxHealth;
		HealthChange(100, maxHealth);
		rigidbody.centerOfMass -= new Vector3(0, centerOfMassAdjustment, 0);
		colliders = new List<GameObject>();
		shieldMat = shield.renderer.materials[0];
		for(int i = 0; i < possibleSecondaries.Length;i++)
		{
			secondaryCannonReloadTimers[i] = secondaryCannonReloadTime[i];
			secondaryBulletsLeft[i] = totalSecondaryBullets[i];
		}
		initialCannonPosition = cannonGraphics.localPosition;
		restartPosition = transform.position;
	}

	void Update () 
	{
		if(deathTimeoutTimer > 0)
		{
			deathTimeoutTimer += Time.deltaTime;
			if(deathTimeoutTimer > deathTimeout)
			{
				PlayerDeath();
			}
		}
		else
		{
			keyDown = false;
			if(Input.GetKey(KeyCode.LeftShift))
			{
				strafeSteeringEngaged = !isStrafeSteeringDefaultOption;
			}
			else
			{
				strafeSteeringEngaged = isStrafeSteeringDefaultOption;
			}
			if(Input.GetKey("a") || Input.GetKey("left"))
			{
				steeringDirection = -1;
			}
			else if(Input.GetKey("d") || Input.GetKey("right"))
			{
				steeringDirection = 1;
			}
			else
			{
				steeringDirection = 0;
			}
			if(Input.GetKey("w") || Input.GetKey("up"))
			{
				if(strafeSteeringEngaged)
				{
					MovePlayerControlled(true);
				}
				else
				{
					MoveFollowCamera(true);
				}
				keyDown = true;
			}
			if(Input.GetKey("s") || Input.GetKey("down"))
			{
				if(strafeSteeringEngaged)
				{
					MovePlayerControlled(false);
				}
				else
				{
					MoveFollowCamera(false);
				}
				keyDown = true;
			}
			if(!keyDown)
			{
				for(int i = 0; i < wheels.Length; i++)
				{
					wheels[i].motorTorque = 0;
					wheels[i].brakeTorque = brakeForce;
				}
			}
			if(Input.GetKeyDown("r"))
			{
				transform.rotation = Quaternion.identity;
				transform.localPosition += new Vector3(0, 5, 0);
			}
			if(Input.GetKeyDown("tab"))
			{
				currentSecondaryWep++;
				if(currentSecondaryWep >= possibleSecondaries.Length)
				{
					currentSecondaryWep = 0;
				}
			}
			if(Input.GetKeyDown("1"))
			{
				currentSecondaryWep = 0;
			}
			if(Input.GetKeyDown("2"))
			{
				currentSecondaryWep = 1;
			}
			if(Input.GetKeyDown("3"))
			{
				currentSecondaryWep = 2;
			}
			if(Input.GetMouseButtonDown(0))
			{
				if(primaryCannonTimer > primaryCannonReloadTime)
				{
					if(Quaternion.Angle(theCam.transform.rotation, cannonGO.rotation) < autotargetDeltaAngle)
					{
						RaycastHit hit;
						Physics.Raycast(theCam.transform.position, theCam.transform.forward, out hit, float.PositiveInfinity, Util.PLAYERWEAPONSIGNORELAYERS);
						Quaternion tempQuat;
						if(hit.distance < 10)
						{
							tempQuat = Quaternion.LookRotation(theCam.transform.forward*10+theCam.transform.position-cannonGO.position);
						}
						else
						{
							tempQuat = Quaternion.LookRotation(theCam.transform.forward*hit.distance+theCam.transform.position-cannonGO.position);
						}
						Util.Fire(primaryBullet, primaryBulletEmitter.position, tempQuat, 
						          primaryBullet.initialSpeed*
						          (new Vector3(Mathf.Sin(Mathf.Deg2Rad*tempQuat.eulerAngles.y)*Mathf.Cos(Mathf.Deg2Rad*tempQuat.eulerAngles.x),
						             -Mathf.Sin(Mathf.Deg2Rad*tempQuat.eulerAngles.x),
						             Mathf.Cos(Mathf.Deg2Rad*tempQuat.eulerAngles.y)*Mathf.Cos(Mathf.Deg2Rad*tempQuat.eulerAngles.x))), primaryBullet.useGravity);
					}
					else
					{
						Util.Fire(primaryBullet, primaryBulletEmitter.position, cannonGO.rotation, primaryBullet.initialSpeed*cannonGO.forward, primaryBullet.useGravity);
					}
					cannonGraphics.localPosition = initialCannonPosition-cannonKickbackDistance;
					primaryCannonTimer = 0;
					cannonFlash.Play();
					cannonRingFlash.Play();
				}
			}
			if(Input.GetMouseButtonDown(1) || Input.GetKeyDown("space"))
			{
				secondaryAutoFireTimer = secondaryAutoFireTimes[currentSecondaryWep];
			}
			if(Input.GetMouseButton(1) || Input.GetKey("space"))
			{
				if(secondaryBulletsLeft[currentSecondaryWep] > 0)
				{
					if(secondaryAutoFireTimer > secondaryAutoFireTimes[currentSecondaryWep])
					{
						Vector3 randomTemp = firingRandomness[currentSecondaryWep]*Vector3.Cross(primaryBulletEmitter.forward, Random.insideUnitSphere);
						if(Quaternion.Angle(theCam.transform.rotation, cannonGO.rotation) < autotargetDeltaAngle)
						{
							RaycastHit hit;
							Physics.Raycast(theCam.transform.position, theCam.transform.forward, out hit, float.PositiveInfinity, Util.PLAYERWEAPONSIGNORELAYERS);
							Quaternion tempQuat;
							if(hit.distance < 10)
							{
								tempQuat = Quaternion.LookRotation(theCam.transform.forward*10+theCam.transform.position-secondaryBulletEmitters[currentSecondaryWep].position);
							}
							else
							{
								tempQuat = Quaternion.LookRotation(theCam.transform.forward*hit.distance+theCam.transform.position-secondaryBulletEmitters[currentSecondaryWep].position);
							}
							Util.Fire(possibleSecondaries[currentSecondaryWep], secondaryBulletEmitters[currentSecondaryWep].position,
							          Quaternion.LookRotation(new Vector3(Mathf.Sin(Mathf.Deg2Rad*tempQuat.eulerAngles.y)*Mathf.Cos(Mathf.Deg2Rad*tempQuat.eulerAngles.x),
							                                    -Mathf.Sin(Mathf.Deg2Rad*tempQuat.eulerAngles.x), Mathf.Cos(Mathf.Deg2Rad*tempQuat.eulerAngles.y)*Mathf.Cos(Mathf.Deg2Rad*tempQuat.eulerAngles.x)) - randomTemp), possibleSecondaries[currentSecondaryWep].initialSpeed*
							          (new Vector3(Mathf.Sin(Mathf.Deg2Rad*tempQuat.eulerAngles.y)*Mathf.Cos(Mathf.Deg2Rad*tempQuat.eulerAngles.x),
							             -Mathf.Sin(Mathf.Deg2Rad*tempQuat.eulerAngles.x),
							             Mathf.Cos(Mathf.Deg2Rad*tempQuat.eulerAngles.y)*Mathf.Cos(Mathf.Deg2Rad*tempQuat.eulerAngles.x)) - randomTemp).normalized, possibleSecondaries[currentSecondaryWep].useGravity);
						}
						else
						{
							Util.Fire(possibleSecondaries[currentSecondaryWep], secondaryBulletEmitters[currentSecondaryWep].position,
							          Quaternion.LookRotation(new Vector3(Mathf.Sin(Mathf.Deg2Rad*secondaryBulletEmitters[currentSecondaryWep].rotation.eulerAngles.y)*Mathf.Cos(Mathf.Deg2Rad*secondaryBulletEmitters[currentSecondaryWep].rotation.eulerAngles.x),
							                                    -Mathf.Sin(Mathf.Deg2Rad*secondaryBulletEmitters[currentSecondaryWep].rotation.eulerAngles.x), Mathf.Cos(Mathf.Deg2Rad*secondaryBulletEmitters[currentSecondaryWep].rotation.eulerAngles.y)*Mathf.Cos(Mathf.Deg2Rad*secondaryBulletEmitters[currentSecondaryWep].rotation.eulerAngles.x)) - randomTemp), possibleSecondaries[currentSecondaryWep].initialSpeed*
							          (new Vector3(Mathf.Sin(Mathf.Deg2Rad*secondaryBulletEmitters[currentSecondaryWep].rotation.eulerAngles.y)*Mathf.Cos(Mathf.Deg2Rad*secondaryBulletEmitters[currentSecondaryWep].rotation.eulerAngles.x),
							             -Mathf.Sin(Mathf.Deg2Rad*secondaryBulletEmitters[currentSecondaryWep].rotation.eulerAngles.x),
							             Mathf.Cos(Mathf.Deg2Rad*secondaryBulletEmitters[currentSecondaryWep].rotation.eulerAngles.y)*Mathf.Cos(Mathf.Deg2Rad*secondaryBulletEmitters[currentSecondaryWep].rotation.eulerAngles.x)) - randomTemp).normalized, possibleSecondaries[currentSecondaryWep].useGravity);
						}
						secondaryCannonReloadTimers[currentSecondaryWep] = 0;
						secondaryCannonFlashes[currentSecondaryWep].Play();
						if(secondaryBulletEmitters[currentSecondaryWep] == primaryBulletEmitter)
						{
							cannonGraphics.localPosition = initialCannonPosition-cannonKickbackDistance;
							cannonRingFlash.Play();
						}
						secondaryBulletsLeft[currentSecondaryWep]--;
						secondaryAutoFireTimer -= secondaryAutoFireTimes[currentSecondaryWep];
					}
					else
					{
						secondaryAutoFireTimer += Time.deltaTime;
					}
				}
			}
			UpdateTreadParticles();
			primaryCannonTimer += Time.deltaTime;
			for(int i = 0; i < secondaryCannonReloadTimers.Length; i++)
			{
				if(i == currentSecondaryWep)
				{
					secondaryCannonReloadTimers[i] += Time.deltaTime;
				}
				else
				{
					secondaryCannonReloadTimers[i] += secondaryOfflineReload*Time.deltaTime;
				}
			}
			if(secondaryCannonReloadTimers[currentSecondaryWep] > secondaryCannonReloadTime[currentSecondaryWep])
			{
				secondaryBulletsLeft[currentSecondaryWep] = totalSecondaryBullets[currentSecondaryWep];
			}
			//Debug.Log(Mathf.Sqrt(rigidbody.velocity.x*rigidbody.velocity.x + rigidbody.velocity.z*rigidbody.velocity.z));
			Vector2 temp = new Vector2(rigidbody.velocity.x, rigidbody.velocity.z);
			if(temp.magnitude > maxSpeed)
			{
				temp = temp.normalized*maxSpeed;
				rigidbody.velocity = new Vector3(temp.x, rigidbody.velocity.y, temp.y);
			}
			float kemp = Vector3.Angle(transform.up, Vector3.up);
			if(kemp > maxSlope)
			{
				for(int i = 0; i < wheels.Length; i++)
				{
					wheels[i].motorTorque *=(kemp>maxSlope+10)?0:(10 - kemp + maxSlope)/10;
					if(!keyDown)
					{
						wheels[i].brakeTorque = 0;
					}
				}
			}
			cannonGraphics.localPosition = Vector3.Lerp(cannonGraphics.localPosition, initialCannonPosition, Time.deltaTime*cannonKickbackRestoreRate);
			if(Time.timeSinceLevelLoad > timeSinceLastHit + shieldRechargeDelay)
			{
				if(shieldPct < 100)
				{
					HealthChange(shieldRechargeRate * Time.deltaTime, 0);
				}
				shield.renderer.enabled = true;
			}
			else
			{
				if(shieldPct > 0)
				{
					if(Time.timeSinceLevelLoad-timeSinceLastHit < 1)
					{
						shieldMat.SetFloat("_Cutoff", (Time.timeSinceLevelLoad-timeSinceLastHit));
						shieldMat.mainTextureOffset += Random.insideUnitCircle*shieldMaterialRate;
						shield.renderer.enabled = true;
					}
				}
				else
				{
					shieldMat.SetFloat("_Cutoff", 1);
					shield.renderer.enabled = false;
				}
			}
		}
	}
	private void UpdateTreadParticles()
	{
		WheelHit wheelhit;
		for(int i = 0; i < wheelGroundEffects.Length; i++)
		{
			if(wheels[i].GetGroundHit(out wheelhit))
			{
				wheelGroundEffects[i].transform.position = wheelhit.point;
				wheelGroundEffects[i].enableEmission = true;
			}
			else
			{
				wheelGroundEffects[i].enableEmission = false;
			}
		}
	}
	void FixedUpdate()
	{
		Vector2 temp = new Vector2(rigidbody.velocity.x, rigidbody.velocity.z);
		if(temp.magnitude > maxSpeed)
		{
			temp = temp.normalized*maxSpeed;
			rigidbody.velocity = new Vector3(temp.x, rigidbody.velocity.y, temp.y);
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
					BasicExplosion be = (BasicExplosion)other.GetComponent<BasicExplosion>();
					colliders.Add(other.gameObject);
					rigidbody.AddExplosionForce(be.explosionForce, other.transform.position, be.explosionRadius);
					HealthChange(-be.shieldDamage, -be.healthDamage);
				}
			}
			catch
			{
				Debug.Log("Incorrect tag assignment for tag \"Basic Explosion\"");
			}
		}
		if(other.tag.Equals("HealthPack"))
		{
			try
			{
				if(health < maxHealth)
				{
					if(!colliders.Contains(other.gameObject))
					{
						colliders.Add(other.gameObject);
						HealthChange(0, other.GetComponent<HealthPack>().deltaHealth);
					}
				}
			}
			catch
			{
				Debug.Log("Incorrect tag assignment for tag \"Health Pack\"");
			}
		}
		if(other.tag.Equals("Laser"))
		{
			try
			{
				BasicBullet be = (BasicBullet)(other.GetComponent<BasicBullet>());
				HealthChange(-be.shieldDamage, -be.healthDamage);
			}
			catch(System.InvalidCastException ie)
			{
				Debug.Log("Incorrect tag assignment for tag \"Bullet\"");
			}
		}
	}
	public void OnTriggerEnter(Collider other)
	{
		RealCollisionHandler(other);
	}
	public void OnTriggerStay(Collider other)
	{
		RealCollisionHandler(other);
	}
	public bool SetLastCheckpoint(Vector3 restartPos, Vector3 restartEulers, int checkpointNumber)
	{
		if(checkpointNumber > lastCheckpoint)
		{
			restartPosition = restartPos;
			restartRotation = restartEulers;
			lastCheckpoint = checkpointNumber;
			lastCheckpointHealth = health;
			return true;
		}
		return false;
	}
	private void PlayerDeath()
	{
		/*PlayerController pc = ((GameObject)Instantiate(this.gameObject, restartPosition, Quaternion.Euler(restartRotation))).GetComponent<PlayerController>();
		theCam.GetComponent<CameraScript>().player = pc;
		Util.player = pc;
		Destroy(gameObject);*/
		deathTimeoutTimer = 0;
		float temp = wheelGroundEffects[0].emissionRate;
		for(int i = 0; i < wheelGroundEffects.Length; i++)
		{
			wheelGroundEffects[i].enableEmission = false;
			wheelGroundEffects[i].Stop();
		}
		for(int i = 0; i < flameParticles.Length; i++)
		{
			flameParticles[i].Stop();
		}
		numFlameParticlesPlaying = 0;
		transform.position = restartPosition;
		transform.eulerAngles = restartRotation;
		HealthChange(100, lastCheckpointHealth);
	}
	public void GameOver()
	{
		if(deathTimeoutTimer == 0)
		{
			deathTimeoutTimer += Time.deltaTime;
			gameOverExplosion.Play();
			for(int i = 0; i < wheels.Length; i++)
			{
				wheels[i].motorTorque = 0;
				wheels[i].brakeTorque = brakeForce;
			}
		}
	}
	public void HealthChange(float shieldDmg, float healthDmg)
	{
		if(shieldDmg > 0 || healthDmg > 0)
		{
			if(shieldDmg >0)
			{
				shieldPct += shieldDmg;
				if(shieldPct > 100)
					shieldPct = 100;
			}
			if(healthDmg > 0)
			{
				health += healthDmg;
				for(int i = 0; i < flameParticles.Length; i++)
				{
					flameParticles[i].Stop();
				}
				if(health > maxHealth)
					health = maxHealth;
			}
		}
		else
		{
			if(shieldPct > 0)
			{
				shieldPct += shieldDmg;
				if(shieldPct < 0)
					shieldPct = 0;
			}
			else
			{
				health += healthDmg;
				if(health/maxHealth < .1f)
				{
					for(int i = 0; i < flameParticles.Length; i++)
					{
						flameParticles[i].Play();
					}
				}
				else if(health/maxHealth < .25f)
				{
					if(numFlameParticlesPlaying < 2)
					{
						flameParticles[Mathf.FloorToInt(Random.Range(0, flameParticles.Length))].Play();
						numFlameParticlesPlaying++;
					}
				}
				else if(health/maxHealth < .5f)
				{
					if(numFlameParticlesPlaying < 1)
					{
						flameParticles[Mathf.FloorToInt(Random.Range(0, flameParticles.Length))].Play();
						numFlameParticlesPlaying++;
					}
				}
				if(health < 0)
				{
					health = 0;
					GameOver();
				}
			}
			timeSinceLastHit = Time.timeSinceLevelLoad;
		}
	}
	public void RemoveBulletCollider(GameObject go)
	{
		colliders.Remove(go);
	}
	private void MovePlayerControlled(bool forward)
	{
		if(forward)
		{
			for(int i = 0; i < wheels.Length; i++)
			{
				wheels[i].motorTorque = forwardAcceleration;
				wheels[i].brakeTorque = 0;
				if(i < 4)
				{
					wheels[i].steerAngle = -steeringRate*steeringDirection*SpeedSteeringCoupling();
				}
				else
				{
					wheels[i].steerAngle = steeringRate*steeringDirection*SpeedSteeringCoupling();
				}
			}
		}
		else
		{
			for(int i = 0; i < wheels.Length; i++)
			{
				wheels[i].motorTorque = -forwardAcceleration;
				wheels[i].brakeTorque = 0;
				if(i < 4)
				{
					wheels[i].steerAngle = -steeringRate*steeringDirection*SpeedSteeringCoupling();
				}
				else
				{
					wheels[i].steerAngle = steeringRate*steeringDirection*SpeedSteeringCoupling();
				}
			}
		}
	}
	private void MoveFollowCamera(bool forward)
	{
		if(forward)
		{
			float angleDiff = -Mathf.DeltaAngle(body.transform.eulerAngles.y,theCam.transform.eulerAngles.y);
			if(!wasForwardKeyDown)
			{
				if(Mathf.Abs(Vector2.Angle(new Vector2(rigidbody.velocity.x, rigidbody.velocity.z), new Vector2(body.transform.forward.x, body.transform.forward.z))) > 90)
				{
					for(int i = 0; i < wheels.Length; i++)
					{
						if(wheels[i].isGrounded)
						{
							numWheelsGrounded++;
						}
						wheels[i].motorTorque = forwardAcceleration;
						wheels[i].brakeTorque = 0;
						if(i < 4)
						{
							if(Mathf.Abs(angleDiff) < bodyRotationEpsilon)
							{
								wheels[i].steerAngle = -steeringCenteringCoeff*(angleDiff)*SpeedSteeringCoupling();
							}
							else
							{
								wheels[i].steerAngle = -steeringRate*Mathf.Sign(angleDiff)*SpeedSteeringCoupling();
							}
						}
						else
						{
							if(Mathf.Abs(angleDiff) < bodyRotationEpsilon)
							{
								wheels[i].steerAngle = steeringCenteringCoeff*(angleDiff)*SpeedSteeringCoupling();
							}
							else
							{
								wheels[i].steerAngle = steeringRate*Mathf.Sign(angleDiff)*SpeedSteeringCoupling();
							}
						}
					}
				}
				else
				{
					wasForwardKeyDown = true;
				}
			}
			else
			{
				for(int i = 0; i < wheels.Length; i++)
				{
					if(wheels[i].isGrounded)
					{
						numWheelsGrounded++;
					}
					wheels[i].motorTorque = forwardAcceleration;
					wheels[i].brakeTorque = 0;
					if(i < 4)
					{
						if(Mathf.Abs(angleDiff) < bodyRotationEpsilon)
						{
							wheels[i].steerAngle = steeringCenteringCoeff*(angleDiff)*SpeedSteeringCoupling();
						}
						else
						{
							wheels[i].steerAngle = steeringRate*Mathf.Sign(angleDiff)*SpeedSteeringCoupling();
						}
					}
					else
					{
						if(Mathf.Abs(angleDiff) < bodyRotationEpsilon)
						{
							wheels[i].steerAngle = -steeringCenteringCoeff*(angleDiff)*SpeedSteeringCoupling();
						}
						else
						{
							wheels[i].steerAngle = -steeringRate*Mathf.Sign(angleDiff)*SpeedSteeringCoupling();
						}
					}
				}
			}
		}
		else
		{
			float angleDiff = -Mathf.DeltaAngle(body.transform.eulerAngles.y,theCam.transform.eulerAngles.y);
			if(wasForwardKeyDown)
			{
				if(Mathf.Abs(Vector2.Angle(new Vector2(rigidbody.velocity.x, rigidbody.velocity.z), new Vector2(-body.transform.forward.x, -body.transform.forward.z))) > 90)
				{
					for(int i = 0; i < wheels.Length; i++)
					{
						if(wheels[i].isGrounded)
						{
							numWheelsGrounded++;
						}
						wheels[i].motorTorque = -forwardAcceleration;
						wheels[i].brakeTorque = 0;
						if(i < 4)
						{
							if(Mathf.Abs(angleDiff) < bodyRotationEpsilon)
							{
								wheels[i].steerAngle = steeringCenteringCoeff*(angleDiff)*SpeedSteeringCoupling();
							}
							else
							{
								wheels[i].steerAngle = steeringRate*Mathf.Sign(angleDiff)*SpeedSteeringCoupling();
							}
						}
						else
						{
							if(Mathf.Abs(angleDiff) < bodyRotationEpsilon)
							{
								wheels[i].steerAngle = -steeringCenteringCoeff*(angleDiff)*SpeedSteeringCoupling();
							}
							else
							{
								wheels[i].steerAngle = -steeringRate*Mathf.Sign(angleDiff)*SpeedSteeringCoupling();
							}
						}
					}
				}
				else
				{
					wasForwardKeyDown = false;
				}
			}
			else
			{
				for(int i = 0; i < wheels.Length; i++)
				{
					if(wheels[i].isGrounded)
					{
						numWheelsGrounded++;
					}
					wheels[i].motorTorque = -forwardAcceleration;
					wheels[i].brakeTorque = 0;
					if(i < 4)
					{
						if(Mathf.Abs(angleDiff) < bodyRotationEpsilon)
						{
							wheels[i].steerAngle = -steeringCenteringCoeff*(angleDiff)*SpeedSteeringCoupling();
						}
						else
						{
							wheels[i].steerAngle = -steeringRate*Mathf.Sign(angleDiff)*SpeedSteeringCoupling();
						}
					}
					else
					{
						if(Mathf.Abs(angleDiff) < bodyRotationEpsilon)
						{
							wheels[i].steerAngle = steeringCenteringCoeff*(angleDiff)*SpeedSteeringCoupling();
						}
						else
						{
							wheels[i].steerAngle = steeringRate*Mathf.Sign(angleDiff)*SpeedSteeringCoupling();
						}
					}
				}
			}
		}
	}
	private float SpeedSteeringCoupling()
	{
		return (1+2*(maxSpeed-rigidbody.velocity.magnitude)/maxSpeed);
	}
}
