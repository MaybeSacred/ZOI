using UnityEngine;
using System.Collections.Generic;

public class StartCameraScript : MonoBehaviour {
	public float nextPointTime;
	public float cameraLookOffset;
	private float pointTimer;
	private int currentLookAtPoint;
	public Transform[] pointsToLookAt;
	public float rotationSpeed;
	void Start () {
		Vector3 randVec = Random.onUnitSphere;
		randVec.y = 0;
		randVec.Normalize();
		randVec.y = .1f;
		transform.position = pointsToLookAt[currentLookAtPoint].position + cameraLookOffset * randVec;
		transform.LookAt(pointsToLookAt[currentLookAtPoint].position);
	}
	
	void Update () {
		pointTimer += Time.deltaTime;
		if(pointTimer > nextPointTime)
		{
			pointTimer = 0;
			if(currentLookAtPoint >= pointsToLookAt.Length - 1)
			{
				currentLookAtPoint = 0;
			}
			else
			{
				currentLookAtPoint++;
			}
			Vector3 randVec = Random.onUnitSphere;
			randVec.y = 0;
			randVec.Normalize();
			randVec.y = .1f;
			transform.position = pointsToLookAt[currentLookAtPoint].position + cameraLookOffset * randVec;
			transform.LookAt(pointsToLookAt[currentLookAtPoint].position);
		}
		transform.localRotation *= Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.up);
	}
}
