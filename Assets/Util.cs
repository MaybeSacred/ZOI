using UnityEngine;
using System.Collections;

public class Util : MonoBehaviour {
	public static readonly int PLAYERSIDE = 0;
	public static readonly int ENEMYSIDE = 1;
	public static PlayerController player;
	public static Terrain currentTerrain;
	public static float[] outsideBounds;
	public static readonly float GRAVITY = .333f;
	public static GUI theGUI;
	public static int PLAYERWEAPONSIGNORELAYERS;
	void Start () {
		PLAYERWEAPONSIGNORELAYERS = ~(1<<LayerMask.NameToLayer("Player") | 1<<LayerMask.NameToLayer("Ignore Raycast") | 1<<LayerMask.NameToLayer("BulletLayer"));
		Debug.Log("Hi");
		Debug.Log("hi");Debug.Log("Hi");
		Debug.Log("hi");Debug.Log("Hi");
		Debug.Log("Hi");Debug.Log("Hi");
		Debug.Log("hi");Debug.Log("Hi");
		Debug.Log("hi");Debug.Log("Hi");
		Debug.Log("hi");Debug.Log("Hi");
		Debug.Log("hi");Debug.Log("Hi");
		player = this.GetComponentInChildren<PlayerController>();
		theGUI = this.GetComponentInChildren<GUI>();
		currentTerrain = GetComponentInChildren<Terrain>();
		if(currentTerrain == null)
		{
			Debug.Log("No Terrain was loaded");
		}
	}
	/*Handles details of firing a BasicBullet
	 *Should be used by all objects attempting to fire a bullet
	 */
	public static void Fire<T>(T t, Vector3 inPosition, Quaternion inRotation, Vector3 inSpeed, bool useGravity) where T : BasicBullet
	{
		T bb = (T)Instantiate(t, inPosition, inRotation);
		bb.speed = inSpeed;
		if(useGravity)
		{
			bb.rigidbody.useGravity = true;
		}
	}
	public static BasicBullet FireLaserType<T>(T t, Vector3 beginPosition, Vector3 endPosition, Quaternion inRotation) where T : BasicBullet
	{
		T bb = (T)Instantiate(t, (endPosition-beginPosition)/2+beginPosition, inRotation);
		bb.transform.localScale =  new Vector3(bb.transform.localScale.x, bb.transform.localScale.y, (endPosition-beginPosition).magnitude/2f);
		return bb;
	}
 }
