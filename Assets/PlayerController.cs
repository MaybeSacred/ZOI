/// <summary>
/// For debug. Deactivates death
/// </summary>
//#define DEBUGMODE
using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    #region variables
    public bool isAlive{get; private set;}
    public PlayerWepDefinition[] playerWeaps;
    public AudioClip primaryWeapClip;
    private int steeringDirection;
	public bool strafeSteeringEngaged;
	private float strafeSteeringTimer, disabledControlsTimer, disabledControlsDuration, hitsAgainTimer, hitsAgainOffsetDuration;
	public float returnToCameraFollowTime;
	private WheelFrictionCurve startWheelFriction;
	private int numFlameParticlesPlaying;
	public ParticleSystem gameOverExplosion;
	public ParticleSystem[] flameParticles;
	public float centerOfMassAdjustment;
	public float deathTimeout;
	public float deathTimeoutTimer{get; private set;}
	public Vector3 restartPosition, restartRotation;
	private CheckpointBehaviour lastCheckpoint;
	private float lastCheckpointHealth;
	public ParticleSystem[] wheelGroundEffects;
	public float minShieldFlicker, maxShieldFlicker, shieldMaterialRate;
    public Transform shield;
	private Material shieldMat;
	private float timeSinceLastHit;
	public float shieldPct, shieldRechargeDelay, shieldRechargeRate;
	public WheelCollider[] wheels;
	public float primaryCannonTimer{get; private set;}
	public float primaryCannonReloadTime;
	private float secondaryAutoFireTimer;
	public ParticleSystem cannonFlash, cannonRingFlash;
	public Transform primaryBulletEmitter;
	public Transform cannonGO, cannonGraphics;
	private Vector3 initialCannonPosition, initialFrozenPosition;
	public Vector3 cannonKickbackDistance;
	public float cannonKickbackRestoreRate;
	public BasicBullet primaryBullet;
	public int currentSecondaryWep{get; private set;}
	public Transform body;
	public Transform turret;
	public float maxSlope, slopeLinearDropOff, autotargetDeltaAngle;
	public float steeringRate, steeringCenteringCoeff, bodyRotationEpsilon;
	public Camera theCam;
	public float forwardAcceleration, brakeForce, maxSpeed;
	private bool keyDown, wasForwardKeyDown, controlsDisabled, frozenTransform;
	public float health, maxHealth;
	private int numWheelsGrounded;
	private List<GameObject> colliders;
	private GameObject hitsAgain;
	private List<GameObject> hitsAgainSansDisabled;
	private bool collidersBacklogged;
	private AudioSource aSource;
	public float maxSpeedRetardingForce;
    #endregion
   
    void Start (){
        //starting physics
        aSource = GetComponentInChildren<AudioSource>();
		startWheelFriction = wheels[0].sidewaysFriction;
		primaryCannonTimer = primaryCannonReloadTime;
		lastCheckpointHealth = maxHealth;
		HealthChange(100, maxHealth);
		rigidbody.centerOfMass -= new Vector3(0, centerOfMassAdjustment, 0);
		colliders = new List<GameObject>();
		shieldMat = shield.renderer.materials[0];
		initialCannonPosition = cannonGraphics.localPosition;
		restartPosition = transform.position;
		isAlive = true;
		hitsAgainOffsetDuration = 2f;
		hitsAgainSansDisabled = new List<GameObject> ();
	}

	void Update () 
	{
		//collects collider objects that need to hit multiple times and delays their next collision detection
		if (collidersBacklogged) 
		{
			hitsAgainTimer+=Time.deltaTime;
			if(hitsAgainTimer>(hitsAgainOffsetDuration))
			{
				foreach(GameObject x in hitsAgainSansDisabled)
				{
					if(colliders.Contains(x))
					{
						colliders.Remove(x);
					}
				}
				hitsAgainTimer = 0;
				collidersBacklogged = false;
			}
		}
		if(colliders.Contains(hitsAgain))
		{
			disabledControlsTimer+=Time.deltaTime;
			//change this to delay to disable 1-hit
			if(disabledControlsTimer>(disabledControlsDuration+1f))
			{	
				colliders.Remove(hitsAgain);
				disabledControlsTimer = 0;
			}
		}else if (disabledControlsTimer>disabledControlsDuration+0f){
			disabledControlsTimer = 0;
		}
        //handles when the player does not exist and is respawning
		if(!Util.isPaused&&!controlsDisabled)
		{
			if(deathTimeoutTimer > 0)
			{
				deathTimeoutTimer += Time.deltaTime;
				if(deathTimeoutTimer > deathTimeout)
				{
	                //spawns the player
					PlayerDeath();
				}
			}
			else
			{
				keyDown = false;
				HandleMovementInput();
	           	HandleOtherInput();
				PrimaryWeaponCheck();
	            SecondaryWeaponCheck();
	            //cannon reloading
				UpdateTimers();
				UpdatePhysics();
				//updates tread particles
				UpdateTreadParticles();
				UpdateGraphics();
			}
		}
		else if(controlsDisabled)
		{
			disabledControlsTimer+=Time.deltaTime;
			if(disabledControlsTimer>disabledControlsDuration)
			{
				controlsDisabled = false;
				frozenTransform = false;

			}
			if(frozenTransform)
			{
				transform.position = initialFrozenPosition;
			}
		}
	}
	private void UpdatePhysics()
	{		
		//handles slope physics
		float kemp = Vector3.Angle(transform.up, Vector3.up);
		if(kemp > maxSlope)
		{
			for(int i = 0; i < wheels.Length; i++)
			{
				wheels[i].motorTorque *=(kemp>maxSlope+slopeLinearDropOff)?0:(maxSlope + slopeLinearDropOff - kemp)/slopeLinearDropOff;
				if(!keyDown)
				{
					wheels[i].brakeTorque = 0;
				}
			}
		}
		if(kemp > 150)
		{
			GameOver();
		}
	}
	private void PrimaryWeaponCheck()
	{
		if(Input.GetMouseButtonDown(0))
		{
			if(primaryCannonTimer > primaryCannonReloadTime)
			{
				if(Quaternion.Angle(theCam.transform.rotation, cannonGO.rotation) < autotargetDeltaAngle)
				{
					RaycastHit hit;
					Debug.DrawRay(theCam.transform.position, theCam.transform.forward, Color.green);
					Quaternion tempQuat;
					if(Physics.Raycast(theCam.transform.position, theCam.transform.forward, out hit, float.PositiveInfinity, Util.PLAYERWEAPONSIGNORELAYERS))
					{
						tempQuat = Quaternion.LookRotation(theCam.transform.forward*hit.distance+theCam.transform.position-cannonGO.position);
					}
					else
					{
						tempQuat = Quaternion.LookRotation(primaryBulletEmitter.forward);
					}
					Util.Fire(primaryBullet, primaryBulletEmitter.position, tempQuat, 
					          primaryBullet.initialSpeed*
					          (tempQuat*Vector3.forward).normalized);
				}
				else
				{
					Util.Fire(primaryBullet, primaryBulletEmitter.position, cannonGO.rotation, primaryBullet.initialSpeed*cannonGO.forward);
				}
				cannonGraphics.localPosition = initialCannonPosition-cannonKickbackDistance;
				primaryCannonTimer = 0;
				aSource.PlayOneShot(primaryWeapClip);
				cannonFlash.Play();
				cannonRingFlash.Play();
			}
		}
	}
	private void SecondaryWeaponCheck()
	{
		if(Input.GetMouseButtonDown(1) || Input.GetKeyDown("space"))
		{
			secondaryAutoFireTimer = playerWeaps[currentSecondaryWep].autoFireTime;
		}
		if(Input.GetMouseButton(1) || Input.GetKey("space"))
		{
			if(playerWeaps[currentSecondaryWep].bulletsLeft > 0)
			{
				if(secondaryAutoFireTimer > playerWeaps[currentSecondaryWep].autoFireTime)
				{
					if(Quaternion.Angle(theCam.transform.rotation, cannonGO.rotation) < autotargetDeltaAngle)
					{
						RaycastHit hit;
						Quaternion tempQuat;
						if(Physics.Raycast(theCam.transform.position, theCam.transform.forward, out hit, float.PositiveInfinity, Util.PLAYERWEAPONSIGNORELAYERS))
						{
							tempQuat = Quaternion.LookRotation(theCam.transform.forward*hit.distance+theCam.transform.position-playerWeaps[currentSecondaryWep].bulletEmitter.position);
						}
						else
						{
							tempQuat = Quaternion.LookRotation(playerWeaps[currentSecondaryWep].bulletEmitter.forward);
						}
						Vector3 randomTemp = Util.GenerateRandomVector3(tempQuat*Vector3.forward, playerWeaps[currentSecondaryWep].firingRandomness);
						Util.Fire(playerWeaps[currentSecondaryWep].bullet, playerWeaps[currentSecondaryWep].bulletEmitter.position,
						          Quaternion.LookRotation(randomTemp), playerWeaps[currentSecondaryWep].bullet.initialSpeed*randomTemp.normalized);
					}
					else
					{
						Vector3 randomTemp = Util.GenerateRandomVector3(playerWeaps[currentSecondaryWep].bulletEmitter.forward, playerWeaps[currentSecondaryWep].firingRandomness);
						Util.Fire(playerWeaps[currentSecondaryWep].bullet, playerWeaps[currentSecondaryWep].bulletEmitter.position,
						          Quaternion.LookRotation(randomTemp), playerWeaps[currentSecondaryWep].bullet.initialSpeed * randomTemp.normalized);
					}
					if(playerWeaps[currentSecondaryWep].bulletEmitter == primaryBulletEmitter)
					{
						cannonGraphics.localPosition = initialCannonPosition-cannonKickbackDistance;
						cannonRingFlash.Play();
					}
					if(currentSecondaryWep == 1)
					{
						aSource.PlayOneShot(playerWeaps[currentSecondaryWep].soundEffect, .1f);
					}
					else
					{
						aSource.PlayOneShot(playerWeaps[currentSecondaryWep].soundEffect);
					}
					playerWeaps[currentSecondaryWep].UseBullet();
					secondaryAutoFireTimer -= playerWeaps[currentSecondaryWep].autoFireTime;
				}
				else
				{
					secondaryAutoFireTimer += Time.deltaTime;
				}
			}
		}
	}
	private void HandleMovementInput()
	{
		if(Input.GetKey("a") || Input.GetKey("left"))
		{
			strafeSteeringEngaged = true;
			strafeSteeringTimer = 0;
			steeringDirection = -1;
		}
		else if(Input.GetKey("d") || Input.GetKey("right"))
		{
			strafeSteeringEngaged = true;
			strafeSteeringTimer = 0;
			steeringDirection = 1;
		}
		else
		{
			steeringDirection = 0;
			strafeSteeringTimer += Time.deltaTime;
			if(strafeSteeringTimer > returnToCameraFollowTime)
			{
				strafeSteeringEngaged = false;
			}
		}
		if(Input.GetKey("w") || Input.GetKey("up"))
		{
			if(strafeSteeringEngaged)
			{
				MovePlayerControlled(true);
			}
			else

			{
				MoveFollowCamera(true, -Mathf.DeltaAngle(body.transform.eulerAngles.y,theCam.transform.eulerAngles.y));
			}
			keyDown = true;
		}
		if(Input.GetKey("s") || Input.GetKey("down"))
		{
			//MovePlayerControlled is false when moving backwards
			if(strafeSteeringEngaged)
			{
				MovePlayerControlled(false);
			}
			else
			{
				MoveFollowCamera(false, -Mathf.DeltaAngle(body.transform.eulerAngles.y,theCam.transform.eulerAngles.y));
			}
			keyDown = true;
		}
		if(!keyDown)
		{
			//torque and braking while the vehicle is not being moved by player
			strafeSteeringEngaged = false;
			Brake();
		}
	}
	private void HandleOtherInput()
	{
		//unstuck tool
		/*if(Input.GetKeyDown("r"))
		{
			transform.rotation = Quaternion.identity;
			transform.localPosition += new Vector3(0, 3, 0);
		}*/
		/*Weapon cycling*/
		if (Input.GetKeyDown("tab"))
		{
			currentSecondaryWep++;
			if(currentSecondaryWep >= playerWeaps.Length)
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
	}
	private void UpdateTimers()
	{
		primaryCannonTimer += Time.deltaTime;
		for(int i = 0; i < playerWeaps.Length; i++)
		{
			playerWeaps[i].UpdateReloadTimer();
		}
	}
	private void UpdateGraphics()
	{
		//cannon kickback
		cannonGraphics.localPosition = Vector3.Lerp(cannonGraphics.localPosition, initialCannonPosition, Time.deltaTime*cannonKickbackRestoreRate);
		
		//shield regeneration
		if(Time.timeSinceLevelLoad > timeSinceLastHit + shieldRechargeDelay)
		{
			if(shieldPct < 100)
			{
				HealthChange(shieldRechargeRate * Time.deltaTime, 0);
			}
			shield.renderer.enabled = true;
			if(Time.timeSinceLevelLoad > timeSinceLastHit + 2*shieldRechargeDelay)
			{
				Util.ms.playBattleMusic(false);
			}
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
				rigidbody.AddForceAtPosition(-4*transform.up, wheelGroundEffects[i].transform.position);
			}
		}
	}
	void FixedUpdate()
	{
		if(rigidbody.velocity.magnitude >= maxSpeed)
		{
			rigidbody.AddForce(-rigidbody.velocity*maxSpeedRetardingForce);
		}
	}

	public void OnCollisionEnter(Collision other)
	{
		if(other.gameObject.tag.Equals("Squash"))
		   {
			try
			{	
				if(!colliders.Contains (other.collider.gameObject))
				{
					Squash s = other.collider.gameObject.GetComponent<Squash>();
					HealthChange(-s.shieldDamage, -s.healthDamage);
					colliders.Add(other.collider.gameObject);
					hitsAgainSansDisabled.Add(s.gameObject);
					collidersBacklogged = true;
				}
			}
			catch
			{
				Debug.Log ("incorrect tag assignment for tag \"Squash\"");
			}
		}

		if(other.gameObject.tag.Equals("Spider"))
		{
			try
			{
				if(!colliders.Contains(other.collider.gameObject))
				{
					SpiderbotBehavior be = (SpiderbotBehavior)other.collider.gameObject.GetComponent<SpiderbotBehavior>();
					colliders.Add(other.collider.gameObject);
					Util.mainCamera.SendMessage("SpiderEating",be.cinematicAngle.transform);
					controlsDisabled = true;
					frozenTransform = true;
					initialFrozenPosition = transform.position;
					disabledControlsDuration = be.stunDuration;
					hitsAgain = other.collider.gameObject;
					HealthChange(-be.shieldDamage, -be.healthDamage);
				}
			}
			catch
			{
				Debug.Log("Incorrect tag assignment for tag \"Spider\"");
			}
		}
	}

	public void RealCollisionHandler(Collider other)
	{
        
        //handles explosions with tags
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
		if(other.gameObject.tag.Equals("Spider"))
		{
			try
			{
				if(!colliders.Contains(other.gameObject)||(disabledControlsTimer == 0 && deathTimeoutTimer == 0))
				{
					SpiderbotBehavior be = (SpiderbotBehavior)other.gameObject.GetComponent<SpiderbotBehavior>();
					colliders.Add(other.gameObject);
					Util.mainCamera.SendMessage("SpiderEating",be.cinematicAngle.transform);
					DisablePlayerControl(be.stunDuration);
					frozenTransform = true;
					initialFrozenPosition = transform.position;
					hitsAgain = other.gameObject;
					HealthChange(-be.shieldDamage, -be.healthDamage);
				}
			}
			catch
			{
				Debug.Log("Incorrect tag assignment for tag \"Basic Explosion\"");
			}
		}

		if(other.tag.Equals("Freeze"))
		{
			try
			{
				if(!colliders.Contains(other.gameObject))
				{
					WebActions wa = (WebActions)other.GetComponent<WebActions>();
					colliders.Add(other.gameObject);
					frozenTransform = true;
					initialFrozenPosition = transform.position;
					DisablePlayerControl(wa.freezeTime);
					hitsAgain= other.gameObject;
				}
			}
			catch
				{
					Debug.Log ("Incorrect tag assignment for tag \"Freeze\"");
				}
		}

		else if(other.tag.Equals("HealthPack"))
		{
			try
			{
				if(health < maxHealth)
				{
					if(!colliders.Contains(other.gameObject))
					{
						colliders.Add(other.gameObject);
						HealthChange(0, 100);
					}
				}
			}
			catch
			{
				Debug.Log("Incorrect tag assignment for tag \"Health Pack\"");
			}
		}
		else if(other.tag.Equals("Laser"))
		{
			try
			{
				BasicBullet be = (BasicBullet)(other.GetComponent<BasicBullet>());
				HealthChange(-be.shieldDamage * Time.fixedDeltaTime, -be.healthDamage * Time.fixedDeltaTime);
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
        //only consistent triggers
		RealCollisionHandler(other);
	}
	public void DisablePlayerControl(float disableTime){
		controlsDisabled = true;
		disabledControlsDuration = disableTime;
	}
	public bool SetLastCheckpoint(Vector3 restartPos, Vector3 restartEulers, CheckpointBehaviour checkpoint)
	{
		if(checkpoint != lastCheckpoint)
		{
			restartPosition = restartPos;
			restartRotation = transform.eulerAngles;
			lastCheckpoint = checkpoint;
			lastCheckpointHealth = 100;
			return true;
		}
		return false;
	}
	public void PlayerDeath()
	{
		deathTimeoutTimer = 0;
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
		isAlive = true;
	}
	public void GameOver()
	{
		#if !DEBUGMODE
		if(deathTimeoutTimer == 0)
		{
			deathTimeoutTimer += Time.deltaTime;
			gameOverExplosion.Play();

            //tank is forced to stop during gameover
			for(int i = 0; i < wheels.Length; i++)
			{
				wheels[i].motorTorque = 0;
				wheels[i].brakeTorque = brakeForce;
			}
			isAlive = false;
		}
		#endif
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
                //near death
				health += healthDmg;
				if(health/maxHealth < .1f)
				{
					for(int i = 0; i < flameParticles.Length; i++)
					{
						flameParticles[i].Play();
					}
				}
                //low health
				else if(health/maxHealth < .25f)
				{
					if(numFlameParticlesPlaying < 2)
					{
						flameParticles[Mathf.FloorToInt(Random.Range(0, flameParticles.Length))].Play();
						numFlameParticlesPlaying++;
					}
				}
                //half health
				else if(health/maxHealth < .5f)
				{
					if(numFlameParticlesPlaying < 1)
					{
						flameParticles[Mathf.FloorToInt(Random.Range(0, flameParticles.Length))].Play();
						numFlameParticlesPlaying++;
					}
				}//dead
				if(health <= 0)
				{
					health = 0;
					GameOver();
				}
			}
			Util.ms.playBattleMusic(true);
			//determines when to recharge shield
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
	public void MoveTowardsPosition(Vector3 point){
		Vector3 vecToPoint = (point - transform.position);
		vecToPoint.y = 0;
		//Debug.Log(Vector3.Angle(Vector3.forward, vecToPoint));
		float angle;
		if(vecToPoint.x > 0){
			angle = -Mathf.DeltaAngle(body.transform.eulerAngles.y, Vector3.Angle(Vector3.forward, vecToPoint));
		}
		else{
			angle = -Mathf.DeltaAngle(body.transform.eulerAngles.y, -Vector3.Angle(Vector3.forward, vecToPoint));
		}
		MoveFollowCamera(true, angle);
	}

	public void Brake ()
	{
		for(int i = 0; i < wheels.Length; i++)
		{
			wheels[i].motorTorque = 0;
			wheels[i].brakeTorque = brakeForce;
		}
	}
	
	private void MoveFollowCamera(bool forward, float angleDiff)
	{
		if(forward)
		{
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
	[System.Serializable]
	public class PlayerWepDefinition{
		
		public float cannonReloadTimers{get; private set;}
		public float cannonReloadTime;
		public float firingRandomness;
		public int bulletsLeft{get; private set;}
		public Transform bulletEmitter;
		public ParticleSystem cannonFlash;
		public BasicBullet bullet;
		public float autoFireTime;
		public int totalBullets;
		public AudioClip soundEffect;
		public PlayerWepDefinition() {
			bulletsLeft = totalBullets;
			cannonReloadTimers = cannonReloadTime;
		}
		public void UpdateReloadTimer()
		{
			if(bulletsLeft < totalBullets)
			{
				if(cannonReloadTimers > cannonReloadTime)
				{
					bulletsLeft++;
					if(bulletsLeft > totalBullets)
					{
						bulletsLeft = totalBullets;
					}
					else
					{
						cannonReloadTimers = 0;
					}
				}
				else
				{
					cannonReloadTimers += Time.deltaTime;
				}
			}
		}
		public bool HasBullet()
		{
			return bulletsLeft > 0;
		}
		public bool IsFullyLoaded()
		{
			return bulletsLeft == totalBullets;
		}
		public void UseBullet()
		{
			if(bulletsLeft > 0)
			{
				bulletsLeft--;
				cannonFlash.Play();
			}
		}
	}
	private float SpeedSteeringCoupling()
	{
		return (1+2*(maxSpeed-rigidbody.velocity.magnitude)/maxSpeed);
	}
}
