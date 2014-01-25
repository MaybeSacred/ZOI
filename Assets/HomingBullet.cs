using UnityEngine;
using System.Collections;

public class HomingBullet : BasicBullet {
	public float homingStrength;
	void Start () {
	}
	
	void Update () {
		rigidbody.AddForce(homingStrength*(Util.player.transform.position - transform.position).normalized);
		if(timeOutCounter > 0)
		{
			if(timeOutCounter > endTime)
			{
				Destroy(this.gameObject);
			}
			timeOutCounter += Time.deltaTime;
		}
		if(lifetimeTimer > lifetime)
		{
			timeOutCounter += Time.deltaTime;
		}
		lifetimeTimer += Time.deltaTime;
	}
}
