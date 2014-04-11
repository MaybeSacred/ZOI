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
	void Start () {
		
	}
	
	void Update () {
		if(barrierTimer > 0)
		{
			leftGate.position += openingVector*Time.deltaTime;
			rightGate.position -= openingVector*Time.deltaTime;
			if((barrierTimer > timeToMove)&&closeBehindPlayer==false)
			{
				Destroy(this);
			}
			barrierTimer += Time.deltaTime;
			Util.mainCamera.ActivateCameraShake(cameraShakeStrength);
		}
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
