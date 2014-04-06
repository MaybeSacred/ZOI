using UnityEngine;
using System.Collections;

public class FallToDeath : MonoBehaviour
{
	public PlayerController player;

	void OnTriggerEnter(Collider tank)
	{
		if (tank.tag == "Player")
		{
			Debug.Log("Player has fallen to his death.");
			// Call player.deathSomething();
			// Reset to Checkpoint
		}
	}
}
