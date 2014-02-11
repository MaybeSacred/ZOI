using UnityEngine;
using System.Collections;

public class TurretController : MonoBehaviour {
	public Transform player;
	public Transform turret;
	public Transform cannonGO;
	public Transform fakeCannon;
	public Transform rocketLauncher;
	public Camera cam;
	public float turretFollowSensitivity;
	public float turretSmallAngleFollow;
	private Quaternion lookToQuat;
	private Vector3 initialCannonOffset;
	void Start () {
		initialCannonOffset = cannonGO.localPosition;
	}

	void Update ()
	{
		turret.localEulerAngles = new Vector3(0, Mathf.MoveTowardsAngle(turret.localEulerAngles.y, cam.transform.eulerAngles.y-player.eulerAngles.y, Time.deltaTime*turretFollowSensitivity), 0);
		cannonGO.rotation = Quaternion.RotateTowards(cannonGO.rotation, cam.transform.rotation, Time.deltaTime*turretFollowSensitivity);
		cannonGO.localEulerAngles = new Vector3(cannonGO.localEulerAngles.x, cannonGO.localEulerAngles.y, 0);
		transform.localPosition = Vector3.zero;
		cannonGO.position = fakeCannon.position;
		if(cam.transform.eulerAngles.x > 100 && cam.transform.eulerAngles.x < 345)
		{
			rocketLauncher.localEulerAngles = new Vector3(345, 0, 0);
		}
		else
		{
			rocketLauncher.localEulerAngles = new Vector3(cam.transform.eulerAngles.x, 0, 0);
		}
	}
}
