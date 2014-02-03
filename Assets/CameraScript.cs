using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {
	private float startZ;
	public Vector2 mousePos, oldMousePos;
	public Vector2 mouseSensitivity;
	public PlayerController player;
	public float yAxisUpperAngleBound, yAxisLowerAngleBound;
	public Vector2 cameraOffset;
	public float lerpthThpeed;
	public Transform sun;
	private Color dayColor;
	public Color nightColor;
	private float realYAngle;
	public float cameraRaycastOffset;
	/*Controls how much the camera moves up and down based on x angle*/
	public float cameraUpDownCoeff;
	private float startFOV;
	public float zoomToFOV;
	private bool isZoomed;
	public float deathZoomoutSpeed;
	void Start () {
		yAxisUpperAngleBound += 360;
		startFOV = camera.fieldOfView;
		startZ = cameraOffset.x;
		dayColor = camera.backgroundColor;
		mousePos = new Vector2();
	}

	void Update () {
		Screen.lockCursor = true;
		if(player.deathTimeoutTimer > 0)
		{
			transform.localPosition = new Vector3(player.transform.localPosition.x - cameraOffset.x*Mathf.Sin(Mathf.Deg2Rad*transform.eulerAngles.y), 
			                                      player.transform.localPosition.y + cameraOffset.y+2*player.deathTimeoutTimer, 
			                                      player.transform.localPosition.z - cameraOffset.x*Mathf.Cos(Mathf.Deg2Rad*transform.eulerAngles.y));
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(player.transform.position-transform.position), deathZoomoutSpeed*Time.deltaTime);
		}
		else
		{
			mousePos.x = Input.GetAxis("Mouse X");
			mousePos.y = Input.GetAxis("Mouse Y");
			Vector3 temp = transform.eulerAngles;
			temp.x += mousePos.y*mouseSensitivity.y;
			if(temp.x > yAxisLowerAngleBound && temp.x < yAxisUpperAngleBound)
			{
				if(temp.x > 180)
				{
					transform.eulerAngles = new Vector3(yAxisUpperAngleBound, transform.eulerAngles.y + mousePos.x*mouseSensitivity.x, 0);
				}
				else
				{
					transform.eulerAngles = new Vector3(yAxisLowerAngleBound, transform.eulerAngles.y + mousePos.x*mouseSensitivity.x, 0);
				}
			}
			else
			{
				transform.eulerAngles += new Vector3(mousePos.y*mouseSensitivity.y, mousePos.x*mouseSensitivity.x, 0);
			}
			RaycastHit hit;
			Physics.Raycast(new Vector3(player.transform.localPosition.x, player.transform.localPosition.y + cameraOffset.y, player.transform.localPosition.z), new Vector3(-transform.forward.x, 0, -transform.forward.z).normalized, out hit, float.PositiveInfinity, Util.PLAYERWEAPONSIGNORELAYERS);
			if(hit.distance < startZ)
			{
				cameraOffset.x = Mathf.Lerp(cameraOffset.x, hit.distance-cameraRaycastOffset, lerpthThpeed*Time.deltaTime);
			}
			else
			{
				cameraOffset.x = Mathf.Lerp(cameraOffset.x, startZ, lerpthThpeed*Time.deltaTime);
			}
			if(Input.GetAxis("Mouse ScrollWheel") > 0)
			{
				isZoomed = true;
			}
			else if(Input.GetAxis("Mouse ScrollWheel") < 0)
			{
				isZoomed = false;
			}
			if(isZoomed)
			{
				camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, zoomToFOV, 10*Time.deltaTime);
			}
			else
			{
				camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, startFOV, 10*Time.deltaTime);
			}
			transform.localPosition = new Vector3(player.transform.localPosition.x - cameraOffset.x*Mathf.Sin(Mathf.Deg2Rad*transform.eulerAngles.y), 
												  player.transform.localPosition.y + cameraOffset.y, 
												  player.transform.localPosition.z - cameraOffset.x*Mathf.Cos(Mathf.Deg2Rad*transform.eulerAngles.y));
			}
	}
}
