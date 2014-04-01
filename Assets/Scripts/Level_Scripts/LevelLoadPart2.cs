using UnityEngine;
using System.Collections;

public class LevelLoadPart2 : MonoBehaviour
{
	private Vector3 startingPosition;
	private Quaternion startingRotation;

	private Vector3 cameraPosition;
	private Quaternion cameraRotation;
	
	public Transform tankTransform;
	public GameObject tankAndWorld;

	public LevelLoadPart1 nextTrigger;

	private const int PREVIOUS_STARTING_POSITION_FAILED = 0x1;
	private const int PREVIOUS_STARTING_ROTATION_FAILED = 0x2;
	private const int PREVIOUS_CAMERA_POSITION_FAILED = 0x4;
	private const int PREVIOUS_CAMERA_ROTATION_FAILED = 0x8;

	public void Start()
	{
		tankAndWorld.SetActive(true);

		LevelLoadPart1[] loadTriggers = (LevelLoadPart1[])FindObjectsOfType(typeof(LevelLoadPart1));
		LevelLoadPart1 levelLoadTrigger = null;

		for (int n = 0; n < loadTriggers.Length; ++n)
			if (loadTriggers[n] != nextTrigger)
				levelLoadTrigger = loadTriggers[n];

		if (null != levelLoadTrigger)
			PreviousLevelWorked(levelLoadTrigger);
	}

	private void PreviousLevelWorked(LevelLoadPart1 levelLoadTrigger)
	{
			// not actually error checking right now...
		int errorCode = levelLoadTrigger.GetLoadInfo(ref startingPosition, ref startingRotation, ref cameraPosition, ref cameraRotation);
		
		tankTransform.position = startingPosition;
		tankTransform.rotation = startingRotation;

		Transform cameraTransform = tankAndWorld.GetComponentInChildren<Camera>().transform;
		cameraTransform.rotation = cameraRotation;
		cameraTransform.position = cameraPosition;
		
		// Load other info like ammo and health here.
	}
}

