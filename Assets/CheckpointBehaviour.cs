using UnityEngine;
using System.Collections;

/// <summary>
/// Tells the world to save the state of all Resettable objects
/// when the player begins colliding with the checkpoint.
/// </summary>
public class CheckpointBehaviour : MonoBehaviour
{
	public Transform letterC;

	public bool isFirstCheckpoint;
	public float letterCRotateSpeed;
	public Material savedSwitchMaterial;
	private Material unSavedSwitchMaterial;
	public CheckpointBehaviour nextCheckpoint;
	public Vector3 desiredRotation;
	// Use this for initialization
	void Start () 
	{
		unSavedSwitchMaterial = letterC.renderer.material;
	}

	void OnTriggerEnter(Collider other)
	{
		switch (other.gameObject.tag) 
		{
			case "Player":
				if(Util.player.SetLastCheckpoint(transform.position, desiredRotation, this))
				{
					Util.theGUI.SetNextCheckpoint(nextCheckpoint);
				}
				letterC.renderer.material = savedSwitchMaterial;
				break;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		letterC.Rotate(0, letterCRotateSpeed*Time.deltaTime, 0);
	}
}
