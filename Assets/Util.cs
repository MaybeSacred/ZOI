using UnityEngine;
using System.Collections;

public class Util : MonoBehaviour {
	public static readonly int PLAYERSIDE = 0;
	public static readonly int ENEMYSIDE = 1;
	public static PlayerController player;
	public static CameraScript mainCamera;
	public static Terrain currentTerrain;
	public static float[] outsideBounds;
	public static readonly float GRAVITY = .333f;
	public static GameGUI theGUI;
	public static int PLAYERWEAPONSIGNORELAYERS;

	public static float currentTimeScale = 1;

	public static bool isPaused{get; private set;}
	void Start () {
		PLAYERWEAPONSIGNORELAYERS = ~(1<<LayerMask.NameToLayer("Player") | 1<<LayerMask.NameToLayer("Ignore Raycast") | 1<<LayerMask.NameToLayer("BulletLayer"));
		player = this.GetComponentInChildren<PlayerController>();
		theGUI = this.GetComponentInChildren<GameGUI>();
		mainCamera = this.GetComponentInChildren<CameraScript>();
		currentTerrain = GetComponentInChildren<Terrain>();
		if(currentTerrain == null)
		{
			Debug.Log("No Terrain was loaded");
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

	public static void FlipPausedState ()
	{
		isPaused = !isPaused;
		if(isPaused)
		{
			Time.timeScale = 0;
		}
		else
		{
			Time.timeScale = currentTimeScale;
		}
	}

	public static BasicBullet FireLaserType<T>(T t, Vector3 beginPosition, Vector3 endPosition, Quaternion inRotation) where T : BasicBullet
	{
		T bb = (T)Instantiate(t, (endPosition-beginPosition)/2+beginPosition, inRotation);
		return bb;
	}
 }
