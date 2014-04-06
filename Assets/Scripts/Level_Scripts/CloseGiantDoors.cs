using UnityEngine;
using System.Collections;

public class CloseGiantDoors : MonoBehaviour
{
	public ElevatorArena elevatorArena;

	public Transform finalDoorLeft;
	public Transform finalDoorRight;
	private Vector3 finalDoorLeftOriginalPosition;
	private Vector3 finalDoorRightOriginalPosition;

	private Vector3 finalDoorLeftPosition;
	private Vector3 finalDoorRightPosition;

	private float finalDoorsDistance = 0f;

	void Awake()
	{
		finalDoorLeftOriginalPosition = new Vector3(finalDoorLeft.position.x, finalDoorLeft.position.y, finalDoorLeft.position.z);
		finalDoorRightOriginalPosition = new Vector3(finalDoorRight.position.x, finalDoorRight.position.y, finalDoorRight.position.z);
	}
	
	void FixedUpdate()
	{
		if (finalDoorsDistance > 0)
		{
			finalDoorsDistance -= 2 * 0.4f * Time.fixedDeltaTime;

			if (finalDoorsDistance < 0)
			{
				finalDoorLeftPosition = finalDoorLeftOriginalPosition;
				finalDoorRightPosition = finalDoorRightOriginalPosition;

				this.enabled = false;
			}
			else
			{
				finalDoorLeftPosition.x += 2 * 0.4f * Time.fixedDeltaTime;
				finalDoorRightPosition.x -= 2 * 0.4f * Time.fixedDeltaTime;
			}
			
			finalDoorLeft.position = finalDoorLeftPosition;
			finalDoorRight.position = finalDoorRightPosition;
		}
	}

	void OnTriggerEnter(Collider tank)
	{
		if (tank.tag == "Player")
		{
			this.enabled = true;

			finalDoorsDistance = - (finalDoorRightOriginalPosition.x - finalDoorRight.position.x);

			finalDoorLeftPosition = new Vector3(finalDoorLeft.position.x, finalDoorLeft.position.y, finalDoorLeft.position.z);
			finalDoorRightPosition = new Vector3(finalDoorLeft.position.x, finalDoorLeft.position.y, finalDoorLeft.position.z);

			elevatorArena.StopDoors();
		}
	}
}
