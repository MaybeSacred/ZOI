using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SixthSense : MonoBehaviour {
	private List<GameObject> colliders;
	public GameObject SpideySenseOwner;
	private bool cooldown;
	public float cooldownTimer; 
	public float cooldownDuration;
	
	// Use this for initialization
	void Start () {
		colliders = new List<GameObject> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (cooldown) 
		{
			cooldownTimer+= Time.deltaTime;
			if (cooldownTimer > cooldownDuration)
			{
				cooldown = false;
				cooldownTimer = 0;
			}
		}
	}
	void RealCollisionHandler(Collider other)
	{
		
		
	}
	public void OnTriggerEnter(Collider other)
	{
		RealCollisionHandler(other);
	}
	public void OnTriggerStay(Collider other)
	{
		//only consistent triggers
		RealCollisionHandler(other);
	}
	
	void Dodge()
	{
		if(!cooldown)
		{
			float jump = Random.Range(-10f, 10f);
			if(Mathf.Abs(jump)>4f)
			{
				print ("yes, dodge");
				SpideySenseOwner.SendMessage("Jump",jump);
				cooldown = true;
			}
		}
	}
}
