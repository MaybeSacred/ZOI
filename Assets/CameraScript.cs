using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {
	private float startZ;
	public Vector2 mousePos, oldMousePos;
	private Vector2 mouseSensitivity;
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
	public Vector2 mouseSensitivityRange;
	private int mouseSensitivityState;
	public int numberOfMouseSensitivityStates;
	public bool cameraIsActiveWhenPaused;
	void Start () {
		SetCurrentMouseSensitivity(numberOfMouseSensitivityStates/2);
		yAxisUpperAngleBound += 360;
		startFOV = camera.fieldOfView;
		startZ = cameraOffset.x;
		dayColor = camera.backgroundColor;
		mousePos = new Vector2();
		
	}

	void Update () {
		if(!Util.isPaused || cameraIsActiveWhenPaused)
		{
			Screen.lockCursor = true;
			if(Util.player.isAlive)
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
				Physics.Raycast(new Vector3(Util.player.transform.localPosition.x, Util.player.transform.localPosition.y + cameraOffset.y, Util.player.transform.localPosition.z), new Vector3(-transform.forward.x, 0, -transform.forward.z).normalized, out hit, float.PositiveInfinity, Util.PLAYERWEAPONSIGNORELAYERS);
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
				transform.localPosition = new Vector3(Util.player.transform.localPosition.x - cameraOffset.x*Mathf.Sin(Mathf.Deg2Rad*transform.eulerAngles.y), 
				                                      Util.player.transform.localPosition.y + cameraOffset.y, 
				                                      Util.player.transform.localPosition.z - cameraOffset.x*Mathf.Cos(Mathf.Deg2Rad*transform.eulerAngles.y));
			}
			else
			{
				transform.localPosition = new Vector3(Util.player.transform.localPosition.x - cameraOffset.x*Mathf.Sin(Mathf.Deg2Rad*transform.eulerAngles.y), 
				                                      Util.player.transform.localPosition.y + cameraOffset.y+2*Util.player.deathTimeoutTimer, 
				                                      Util.player.transform.localPosition.z - cameraOffset.x*Mathf.Cos(Mathf.Deg2Rad*transform.eulerAngles.y));
				transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Util.player.transform.position-transform.position), deathZoomoutSpeed*Time.deltaTime);
			}
		}
	}
	public int GetCurrentMouseSensitivity()
	{
		return mouseSensitivityState;
	}
	public void DecrementMouseSensitivity()
	{
		SetCurrentMouseSensitivity(mouseSensitivityState-1);
	}
	public void IncrementMouseSensitivity()
	{
		SetCurrentMouseSensitivity(mouseSensitivityState+1);
	}
	public void SetCurrentMouseSensitivity(int sensitivity)
	{
		mouseSensitivityState = Mathf.Clamp(sensitivity, 0, numberOfMouseSensitivityStates);
		mouseSensitivity = Vector2.Lerp(Vector2.one*mouseSensitivityRange.x, Vector2.one*mouseSensitivityRange.y, ((float)mouseSensitivityState)/numberOfMouseSensitivityStates);
		mouseSensitivity.y = -mouseSensitivity.y;
	}
}
