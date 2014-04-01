using UnityEngine;
using System.Collections;

public class LevelLoadPart1: MonoBehaviour
{
	private Vector3 startingPosition;
	private Quaternion startingRotation;

	private Vector3 cameraPosition;
	private Quaternion cameraRotation;

	private Collider thisTrigger;

	public bool canReturn;

	public void Start()
	{
		thisTrigger = this.gameObject.GetComponent<Collider>();
	}

		// Load next level when trigger is hit.
	public void OnTriggerEnter(Collider tank)
	{
		if (tank.tag == "Player")
		{
			startingPosition = tank.transform.position;
			startingRotation = tank.transform.rotation;

			Transform cameraTransform = tank.transform.parent.GetComponentInChildren<Camera>().transform;
			cameraRotation = cameraTransform.rotation;
			cameraPosition = cameraTransform.position;

			thisTrigger.enabled = canReturn;

			DontDestroyOnLoad(this);

			// Save data here, not right now though.

				// Go to next level. Make sure levels are in sequential order.
			Application.LoadLevel(Application.loadedLevel + 1);
		}
	}

	public int GetLoadInfo(ref Vector3 startingPosition, ref Quaternion startingRotation, ref Vector3 cameraPosition, ref Quaternion cameraRotation)
	{
		int errorCode = 0x0;

		if (null == startingPosition)
			errorCode = errorCode | 0x1;
		if (null == startingRotation)
			errorCode = errorCode | 0x2;
		if (null == cameraPosition)
			errorCode = errorCode | 0x4;
		if (null == cameraRotation)
			errorCode = errorCode | 0x8;

		startingPosition = this.startingPosition;
		startingRotation = this.startingRotation;

		cameraPosition = this.cameraPosition;
		cameraRotation = this.cameraRotation;

		return errorCode;
	}
}
