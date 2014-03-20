using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SixthSense : MonoBehaviour {
	private List<GameObject> colliders;
	public RiggedyAnnBehavior SpideySenseOwner;
	private bool cooldown;
	private float cooldownTimer; 
	public float cooldownDuration;
	
	// Use this for initialization
	void Start () {
		colliders = new List<GameObject> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(cooldown) 
		{
			cooldownTimer += Time.deltaTime;
			if (cooldownTimer > cooldownDuration)
			{
				cooldown = false;
				cooldownTimer = 0;
				collider.enabled = true;
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
	public void Dodge()
	{
		if(!cooldown)
		{
			SpideySenseOwner.Jump();
			cooldown = true;
			collider.enabled = false;
		}
	}
}
