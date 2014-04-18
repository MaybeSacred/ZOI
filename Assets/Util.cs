using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class Util : MonoBehaviour {
	public static readonly int PLAYERSIDE = 0;
	public static readonly int ENEMYSIDE = 1;
	public static PlayerController player;
	public static CameraScript mainCamera;
	public static float[] outsideBounds;
	public static readonly float GRAVITY = .333f;
	public static GameGUI theGUI;
	public static int PLAYERWEAPONSIGNORELAYERS;
	public static MusicSystem ms;
	public static float currentTimeScale = 1;
	public CheckpointBehaviour firstCheckpoint;

	public static bool isPaused{get; private set;}
	void Start () {
		//DontDestroyOnLoad(this);
		PLAYERWEAPONSIGNORELAYERS = ~(1<<LayerMask.NameToLayer("Player") | 1<<LayerMask.NameToLayer("Ignore Raycast") | 1<<LayerMask.NameToLayer("BulletLayer"));
		player = this.GetComponentInChildren<PlayerController>();
		theGUI = this.GetComponentInChildren<GameGUI>();
		mainCamera = this.GetComponentInChildren<CameraScript>();
		ms = this.GetComponentInChildren<MusicSystem>();
		theGUI.SetNextCheckpoint(firstCheckpoint);
	}
	public void LoadLevel(int levelNum)
	{
		Application.LoadLevel(levelNum);
		player = this.GetComponentInChildren<PlayerController>();
		theGUI = this.GetComponentInChildren<GameGUI>();
		mainCamera = this.GetComponentInChildren<CameraScript>();
		ms = this.GetComponentInChildren<MusicSystem>();
		CheckpointBehaviour[] checkpoints = this.GetComponentsInChildren<CheckpointBehaviour>();
		for(int i = 0; i < checkpoints.Length; i++)
		{
			if(checkpoints[i].isFirstCheckpoint)
			{
				theGUI.SetNextCheckpoint(checkpoints[i]);
				break;
			}
		}
	}
	/*Handles details of firing a BasicBullet
	 *Should be used by all objects attempting to fire a bullet
	 */
	public static void Fire<T>(T t, Vector3 inPosition, Quaternion inRotation, Vector3 inSpeed) where T : BasicBullet
	{
		T bb = (T)Instantiate(t, inPosition, inRotation);
		bb.speed = inSpeed;
	}

	public static void FireVel<T>(T t, Vector3 inPosition, Quaternion inRotation, Vector3 inSpeed) where T: BasicBullet
	{
			T bb = (T)Instantiate (t, inPosition, inRotation);
			bb.rigidbody.velocity = inSpeed;
	}
	public static void Pause()
	{
		isPaused = true;
		Time.timeScale = 0;
		ms.pauseMusic(true);
	}
	public static void UnPause()
	{
		isPaused = false;
		Time.timeScale = 1;
		ms.pauseMusic(false);
	}

	public static BasicBullet FireLaserType<T>(T t, Vector3 beginPosition, Vector3 endPosition, Quaternion inRotation) where T : BasicBullet
	{
		T bb = (T)Instantiate(t, (endPosition-beginPosition)/2+beginPosition, inRotation);
		return bb;
	}
	/// <summary>
	/// Generates a random normalized Vector3 at most maxTheta radians away from input vector
	/// </summary>
	/// <returns>The random vector3.</returns>
	/// <param name="inVec">In vec.</param>
	/// <param name="maxTheta">Max theta.</param>
	public static Vector3 GenerateRandomVector3(Vector3 inVec, float maxTheta)
	{
		Vector3 ranVec = Random.onUnitSphere;
		float ranFloat = Random.Range(0, maxTheta);
		ranVec = (Vector3.Cross(inVec, ranVec).normalized);
		if(ranFloat > Mathf.PI/2)
		{
			return Vector3.RotateTowards(-inVec, ranVec, Mathf.PI-ranFloat, 0);
		}
		else
		{
			return Vector3.RotateTowards(inVec, ranVec, ranFloat, 0);
		}
	}
 }
