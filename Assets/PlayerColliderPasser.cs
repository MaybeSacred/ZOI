using UnityEngine;
using System.Collections;

public class PlayerColliderPasser : MonoBehaviour {
	public float damageModifier;
	public Rigidbody explosionAccepter;
	void Start () {
	
	}
	void OnTriggerEnter(Collider other)
	{
		Util.player.RealCollisionHandler(other);
	}
}
