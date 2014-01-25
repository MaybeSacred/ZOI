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
	/*Controls how much the camera moves up and down based on x angle*/
	public float cameraUpDownCoeff;
	void Start () {
		startZ = cameraOffset.x;
		dayColor = camera.backgroundColor;
		mousePos = new Vector2();
	}

	void Update () {
		Screen.lockCursor = true;
		mousePos.x = Input.GetAxis("Mouse X");
		mousePos.y = Input.GetAxis("Mouse Y");
		Vector3 temp = transform.eulerAngles;
		if(temp.x > 110)
			temp.x -=360;
		if(temp.x - mousePos.y <= yAxisUpperAngleBound)
		{
			transform.eulerAngles = new Vector3(yAxisUpperAngleBound, transform.eulerAngles.y + mousePos.x*mouseSensitivity.x, 0);
		}
		else if(temp.x - mousePos.y >= yAxisLowerAngleBound)
		{
			transform.eulerAngles = new Vector3(yAxisLowerAngleBound, transform.eulerAngles.y + mousePos.x*mouseSensitivity.x, 0);
		}
		else
		{
			transform.eulerAngles += new Vector3(mousePos.y*mouseSensitivity.y, mousePos.x*mouseSensitivity.x, 0);
		}
		RaycastHit hit;
		Physics.Raycast(new Vector3(player.transform.localPosition.x, player.transform.localPosition.y + cameraOffset.y, player.transform.localPosition.z), new Vector3(-transform.forward.x, 0, -transform.forward.z).normalized, out hit, float.PositiveInfinity, Util.PLAYERWEAPONSIGNORELAYERS);
		if(hit.distance <=0)
		{

		}
		else if(hit.distance < startZ)
		{
			cameraOffset.x = Mathf.Lerp(cameraOffset.x, hit.distance, lerpthThpeed*Time.deltaTime);
		}
		else
		{
			cameraOffset.x = Mathf.Lerp(cameraOffset.x, startZ, lerpthThpeed*Time.deltaTime);
		}
		transform.localPosition = new Vector3(player.transform.localPosition.x - cameraOffset.x*Mathf.Sin(Mathf.Deg2Rad*transform.eulerAngles.y), 
											  player.transform.localPosition.y + cameraOffset.y, 
											  player.transform.localPosition.z - cameraOffset.x*Mathf.Cos(Mathf.Deg2Rad*transform.eulerAngles.y));
		if(sun.eulerAngles.z < 99)
		{
			this.camera.backgroundColor = new Color(nightColor.r + (dayColor.r-nightColor.r)*Mathf.Cos(Mathf.Deg2Rad*sun.eulerAngles.z/1.1f), nightColor.g + (dayColor.g-nightColor.g)*Mathf.Cos(Mathf.Deg2Rad*sun.eulerAngles.z/1.1f), nightColor.b + (dayColor.b-nightColor.b)*Mathf.Cos(Mathf.Deg2Rad*sun.eulerAngles.z/1.1f));
		}
		else if(sun.eulerAngles.z > 261)
		{
			this.camera.backgroundColor = new Color(nightColor.r + (dayColor.r-nightColor.r)*Mathf.Cos(Mathf.Deg2Rad*(360-sun.eulerAngles.z)/1.1f), nightColor.g + (dayColor.g-nightColor.g)*Mathf.Cos(Mathf.Deg2Rad*(360-sun.eulerAngles.z)/1.1f), nightColor.b + (dayColor.b-nightColor.b)*Mathf.Cos(Mathf.Deg2Rad*(360-sun.eulerAngles.z)/1.1f));
		}
		else
		{
			this.camera.backgroundColor = nightColor;
		}
	}
}
