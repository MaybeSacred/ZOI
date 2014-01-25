using UnityEngine;
using System.Collections;

public class TimeBasedDestroy : MonoBehaviour {
	public float timeout;
	private float timeoutTimer;
	void Start () {
	
	}
	
	void Update () {
		if(timeoutTimer > timeout)
		{
			Destroy(gameObject);
		}
		timeoutTimer += Time.deltaTime;
	}
}
