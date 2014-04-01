using UnityEngine;
using System.Collections;

public class Squash : MonoBehaviour {
	public int shieldDamage, healthDamage;
	public SlinkyEnemyBehavior slinkyParent;

	// Use this for initialization
	void Start () {
		shieldDamage = slinkyParent.squashShieldDamage;
		healthDamage = slinkyParent.squashHealthDamage;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
