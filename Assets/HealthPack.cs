using UnityEngine;
using System.Collections;

public class HealthPack : MonoBehaviour {
	public float deltaHealth;
	public float floatSpeed;
	public float angularSpeed;
	public float floatAmplitude;
	private float startY;
	void Start () {
		startY = transform.position.y;
	}
	void Update () {
		if(!Util.isPaused)
		{
			transform.localEulerAngles += new Vector3(0, angularSpeed*Mathf.Sin(Time.timeSinceLevelLoad), 0);
			transform.position = new Vector3(transform.position.x, startY + floatAmplitude*Mathf.Sin(floatSpeed*Time.timeSinceLevelLoad), transform.position.z);
		}
	}
	public void OnTriggerEnter(Collider other)
	{
		if(other.tag.Equals("Player"))
		{
			if(Util.player.health < Util.player.maxHealth)
			{
				Destroy(gameObject);
			}
		}
	}
}
