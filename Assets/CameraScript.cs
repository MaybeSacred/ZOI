﻿using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {
	private float startZ;
	public Vector2 mousePos, oldMousePos;
	private Vector2 mouseSensitivity;
	public float yAxisUpperAngleBound, yAxisLowerAngleBound;
	public Vector2 cameraOffset;
	public float lerpthThpeed;
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
	public float distanceToTarget{get; private set;}
	private Vector3 beforeSpiderPos, spiderPos;
	private bool spiderFeeding;
	public float feedTimer, feedDuration, feedTimeStart;
	void Start () {
		Screen.lockCursor = true;
		SetCurrentMouseSensitivity(numberOfMouseSensitivityStates/2);
		yAxisUpperAngleBound += 360;
		startFOV = camera.fieldOfView;
		startZ = cameraOffset.x;
		mousePos = new Vector2();
		feedDuration = 3f;
	}
	
	void Update () {
		if(!Util.isPaused || cameraIsActiveWhenPaused)
		{
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
				RaycastHit detect;
				if(Physics.Raycast(transform.position, transform.forward, out detect, float.PositiveInfinity, Util.PLAYERWEAPONSIGNORELAYERS))
				{
					if(detect.collider.gameObject.tag.Equals("Deflective"))
					{
						detect.collider.gameObject.GetComponent<SixthSense>().Dodge();
					}
				}
				if(Physics.Raycast(new Vector3(Util.player.transform.position.x, Util.player.transform.position.y + cameraOffset.y, Util.player.transform.position.z), new Vector3(-transform.forward.x, 0, -transform.forward.z).normalized, out hit, 2*startZ, Util.PLAYERWEAPONSIGNORELAYERS))
				{
					if(hit.distance < startZ)
					{
						cameraOffset.x = Mathf.Lerp(cameraOffset.x, hit.distance-cameraRaycastOffset, lerpthThpeed*Time.deltaTime);
					}
					else
					{
						cameraOffset.x = Mathf.Lerp(cameraOffset.x, startZ, lerpthThpeed*Time.deltaTime);
					}
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
				if(isZoomed)
				{
					transform.localPosition = new Vector3(Util.player.transform.localPosition.x - .5f * cameraOffset.x*Mathf.Sin(Mathf.Deg2Rad*transform.eulerAngles.y), 
					                                      Util.player.transform.localPosition.y + .66f * cameraOffset.y, 
					                                      Util.player.transform.localPosition.z - .5f * cameraOffset.x*Mathf.Cos(Mathf.Deg2Rad*transform.eulerAngles.y));
				}
				else
				{
					transform.localPosition = new Vector3(Util.player.transform.localPosition.x - cameraOffset.x*Mathf.Sin(Mathf.Deg2Rad*transform.eulerAngles.y), 
				                                      Util.player.transform.localPosition.y + cameraOffset.y, 
				                                      Util.player.transform.localPosition.z - cameraOffset.x*Mathf.Cos(Mathf.Deg2Rad*transform.eulerAngles.y));
				}
				distanceToTarget = detect.distance;
				ShakeCamera();
			}
			else
			{
				//transform.localPosition = new Vector3(Util.player.transform.localPosition.x - cameraOffset.x*Mathf.Sin(Mathf.Deg2Rad*transform.eulerAngles.y), 
				//                                      Util.player.transform.localPosition.y + cameraOffset.y+2*Util.player.deathTimeoutTimer, 
				//                                      Util.player.transform.localPosition.z - cameraOffset.x*Mathf.Cos(Mathf.Deg2Rad*transform.eulerAngles.y));
				transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Util.player.transform.position-transform.position), deathZoomoutSpeed*Time.deltaTime);
			}
		}
		
		if (spiderFeeding) {
			feedTimer+= Time.deltaTime;
			transform.position = Vector3.Slerp(beforeSpiderPos, spiderPos, (Time.time-feedTimeStart)*0.5f);
			transform.LookAt(Util.player.transform.position);
			
			if(feedTimer>feedDuration){
				print ("feed count");
				spiderFeeding = false;
				//transform.position = beforeSpiderPos;
				feedTimer =0;
			}
		}
	}
	float shakeAmplitude;
	public float maxCameraShakeAmplitude;
	float shakeTimer;
	public float minimumCameraShakeAmplitude;
	public float shakeAmplitudeDecayRate;
	public float shakeRate;
	public void ActivateCameraShake(float amplitude)
	{
		shakeAmplitude = Mathf.Clamp(amplitude, 0, maxCameraShakeAmplitude);
		shakeTimer = 0.001f;
	}
	private void ShakeCamera()
	{
		if(shakeTimer > 0)
		{
			shakeAmplitude = Mathf.Lerp(shakeAmplitude, 0, shakeAmplitudeDecayRate * Time.deltaTime);
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, shakeAmplitude * Mathf.Sin(shakeRate * Time.timeSinceLevelLoad));
			if(shakeAmplitude < minimumCameraShakeAmplitude)
			{
				shakeTimer = 0;
				transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
			}
			shakeTimer += Time.deltaTime;
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
	public void SetCurrentMouseSensitivity(float intivity)
	{
		int sensitivity = Mathf.RoundToInt(intivity);
		mouseSensitivityState = Mathf.Clamp(sensitivity, 0, numberOfMouseSensitivityStates);
		mouseSensitivity = Vector2.Lerp(Vector2.one*mouseSensitivityRange.x, Vector2.one*mouseSensitivityRange.y, ((float)mouseSensitivityState)/numberOfMouseSensitivityStates);
		mouseSensitivity.y = -mouseSensitivity.y;
	}
	
	void SpiderEating(Transform Spider)
	{
		beforeSpiderPos = transform.position;
		transform.position = Vector3.Lerp (beforeSpiderPos, Spider.position, Time.deltaTime);
		spiderPos = Spider.position;
		transform.LookAt (Util.player.transform.position);
		spiderFeeding = true;
		feedTimeStart = Time.time;
	}
	public void EndGame(){
		GetComponent<GameGUI>().DeActivate();
		GetComponent<EndingCamera>().enabled = true;
		GetComponent<EndingCamera>().SetVantagePoint();
		enabled = false;
	}
}
