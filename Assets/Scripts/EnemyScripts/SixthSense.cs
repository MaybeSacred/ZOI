using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SixthSense : MonoBehaviour {
	private List<GameObject> colliders;
	public GameObject SpideySenseOwner;
	public BasicBullet[] dodgeableBullets;
	private string[] prettyNamesOfDodgeableBullets;
	// Use this for initialization
	void Start () {
		colliders = new List<GameObject> ();
		prettyNamesOfDodgeableBullets = new string[dodgeableBullets.Length];
		for(int i = 0; i < dodgeableBullets.Length; i++)
		{
			prettyNamesOfDodgeableBullets[i] = dodgeableBullets[i].prettyName;
		}
		dodgeableBullets = null;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void RealCollisionHandler(Collider other)
	{
		if(other.gameObject.tag.Equals ("Bullet"))
		{
			try
			{
				if(!colliders.Contains(other.gameObject))
				{
					colliders.Add(other.gameObject);
					string bulletString = ((BasicBullet)other.gameObject.GetComponent<BasicBullet>()).prettyName;
					for(int i = 0; i < prettyNamesOfDodgeableBullets.Length; i++)
					{
						if(bulletString.Equals(prettyNamesOfDodgeableBullets[i]))
						{
							SpideySenseOwner.SendMessage("Jump", true);
							break;
						}
					}
				}
			}
			catch
			{
				Debug.Log("Incorrect tag assignment for tag \"Bullet\"");
			}
		}
	}
	public void OnTriggerEnter(Collider other)
	{
		RealCollisionHandler(other);
	}
}
