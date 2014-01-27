using UnityEngine;
using System.Collections;

public class TurretController : MonoBehaviour {
	public Transform player;
	public Transform turret;
	public Transform cannon;
	public Transform rocketLauncher;
	public Camera cam;
	public float turretFollowSensitivity;
	public float turretSmallAngleFollow;
	private Quaternion lookToQuat;
	void Start () {
		
	}

	void Update ()
	{
		/*RaycastHit hit;
		Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, float.PositiveInfinity, Util.PLAYERWEAPONSIGNORELAYERS);
		if(hit.distance < 10)
		{
			if(hit.distance != 0)
			{
				lookToQuat = Quaternion.LookRotation(cam.transform.forward*10+cam.transform.position-cannon.position);
			}
		}
		else
		{
			lookToQuat = Quaternion.LookRotation(cam.transform.forward*hit.distance+cam.transform.position-cannon.position);
		}
		turret.localEulerAngles = new Vector3(0, Mathf.MoveTowardsAngle(turret.localEulerAngles.y, lookToQuat.eulerAngles.y-player.eulerAngles.y, Time.deltaTime*turretFollowSensitivity), 0);
		if(Quaternion.Angle(cannon.rotation, lookToQuat) < 1)
		{
			cannon.rotation = Quaternion.Lerp(cannon.rotation, lookToQuat, Time.deltaTime*turretSmallAngleFollow);
		}
		else
		{
			cannon.rotation = Quaternion.RotateTowards(cannon.rotation, lookToQuat, Time.deltaTime*turretFollowSensitivity);
		}
		cannon.localEulerAngles = new Vector3(cannon.localEulerAngles.x, cannon.localEulerAngles.y, 0);*/
		turret.localEulerAngles = new Vector3(0, Mathf.MoveTowardsAngle(turret.localEulerAngles.y, cam.transform.eulerAngles.y-player.eulerAngles.y, Time.deltaTime*turretFollowSensitivity), 0);
		cannon.rotation = Quaternion.RotateTowards(cannon.rotation, cam.transform.rotation, Time.deltaTime*turretFollowSensitivity);
		cannon.localEulerAngles = new Vector3(cannon.localEulerAngles.x, cannon.localEulerAngles.y, 0);
		transform.localPosition = Vector3.zero;
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
