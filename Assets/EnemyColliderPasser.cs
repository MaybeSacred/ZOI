using UnityEngine;
using System.Collections;

public class EnemyColliderPasser : MonoBehaviour {
	public float damageModifier;
	public Rigidbody explosionAccepter;
	public BaseEnemy realParents;
	void Start () {
		
	}
	void OnTriggerEnter(Collider other)
	{
		realParents.RealCollisionHandler(other);
	}
}
