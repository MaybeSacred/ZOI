using UnityEngine;
using System.Collections;

public class EnemyColliderPasser : MonoBehaviour, PlayerEvent{
	public float damageModifier;
	public Rigidbody explosionAccepter;
	public BaseEnemy realParents;
	void Start () {
		
	}
	void OnTriggerEnter(Collider other)
	{
		realParents.RealCollisionHandler(other);
	}
	public void OnPlayerEnter()
	{
		realParents.OnPlayerEnter();
	}
	public void OnPlayerExit()
	{
		realParents.OnPlayerExit();
	}
}
