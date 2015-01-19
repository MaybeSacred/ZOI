using UnityEngine;
using System.Collections;

public class LaserBullet : BasicBullet {
	public Transform endPointEffect;
	private Transform endPointPS;
	public ParticleSystem laserEffect;
	public Material outerTransparency;
	public float laserWaveSpeed;
	void Start () {
		endPointPS = (Transform)Instantiate(endPointEffect, transform.position + transform.forward*transform.localScale.z, transform.rotation);
		endTime = timeOutCounter;
		timeOutCounter = .0001f;
		outerTransparency.mainTextureOffset += new Vector2(0, -laserWaveSpeed*Time.deltaTime);
	}
	
	void Update () {
		outerTransparency.mainTextureOffset += new Vector2(0, -laserWaveSpeed*Time.deltaTime);
		laserEffect.transform.position = transform.position - transform.forward*transform.localScale.z;
		endPointPS.position = transform.position + transform.forward*transform.localScale.z;
		endPointPS.rotation = transform.rotation;
		if(timeOutCounter > 0)
		{
			if(timeOutCounter > endTime)
			{
				Destroy(endPointPS.gameObject);
				Destroy(this.gameObject);
			}
			timeOutCounter += Time.deltaTime;
		}
	}
	public override void DestroyMe(Vector3 input)
	{
		//GetComponent<Collider>().enabled = false;
	}
}
