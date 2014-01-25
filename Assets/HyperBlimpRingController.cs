using UnityEngine;
using System.Collections;

public class HyperBlimpRingController : MonoBehaviour {
	public float rotationSpeed;
	public float rotationDelta;
	private Quaternion currentDesiredAngle;
	private Quaternion lastRotation;
	private float rotationTimer;
	private float rotationTotalTime;
	void Start () {
		rotationTotalTime = rotationDelta/rotationSpeed;
		rotationTimer = rotationTotalTime;
		lastRotation = transform.localRotation;
	}
	
	void Update ()
	{
		if(rotationTimer > rotationTotalTime)
		{
			int temp = Random.Range(-1, 2);
			lastRotation = currentDesiredAngle;
			if(temp < 0)
			{
				currentDesiredAngle = transform.localRotation*(Quaternion.Euler(-rotationDelta, 0, 0));
			}
			else if(temp > 0)
			{
				currentDesiredAngle = transform.localRotation*(Quaternion.Euler(rotationDelta, 0, 0));
			}
			else
			{
				currentDesiredAngle = transform.localRotation;
			}
			rotationTimer = 0;
		}
		else
		{
			rotationTimer += Time.deltaTime;
			transform.localRotation = Quaternion.Slerp(lastRotation, currentDesiredAngle, rotationTimer/rotationTotalTime);
		}
	}
}
