using UnityEngine;
using System.Collections.Generic;

public class BarrierBehavior : MonoBehaviour {
	public int attachedEnemies;
	private float barrierTimer;
	public Transform leftGate, rightGate;
	public Vector3 openingVector;
	public float timeToMove;
	public float cameraShakeStrength;
	public bool closeBehindPlayer;
	private bool closingDoors;
	void Start () {
		
	}
	
	void Update () {
		if(barrierTimer > 0 && closingDoors == false)
		{
			barrierTimer += Time.deltaTime;
			leftGate.localPosition += openingVector*Time.deltaTime;
			rightGate.localPosition -= openingVector*Time.deltaTime;
			if((barrierTimer > timeToMove))
			{
				if(closeBehindPlayer==false)
				{
					Destroy(this);
				}else
				{
					barrierTimer= 0;
				}
			}

			Util.mainCamera.ActivateCameraShake(cameraShakeStrength);
		}else if(barrierTimer > 0 && closingDoors==true)
		{
			leftGate.localPosition -= openingVector*Time.deltaTime;
			rightGate.localPosition += openingVector*Time.deltaTime;
			if((barrierTimer > timeToMove))
			{
					Destroy(this);
			}
			barrierTimer += Time.deltaTime;
			Util.mainCamera.ActivateCameraShake(cameraShakeStrength);
		}
	}

	public void CloseDoors()
	{
		closingDoors = true;
		barrierTimer = 0.00001f;
	}

	public void RegisterEnemy ()
	{
		attachedEnemies++;
	}
	public void UnregisterEnemy()
	{
		attachedEnemies--;
		if(attachedEnemies <= 0)
		{
			barrierTimer = .00001f;
		}
	}
}
