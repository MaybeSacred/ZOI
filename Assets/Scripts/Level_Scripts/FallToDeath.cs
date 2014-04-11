using UnityEngine;
using System.Collections;

public class FallToDeath : MonoBehaviour
{
	void OnTriggerEnter(Collider tank)
	{
		if (tank.tag == "Player")
		{
			Util.player.GameOver();
		}
	}
}
