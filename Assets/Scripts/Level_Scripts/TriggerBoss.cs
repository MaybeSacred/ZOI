using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TriggerBoss : MonoBehaviour {

	public List<Transform> bossList;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.tag.Equals("Player"))
		{
			foreach(Transform enemy in bossList)
			{
				if(enemy != null)
				{
					((PlayerEvent)enemy.GetComponent<MonoBehaviour>()).OnPlayerEnter();
				}
			}
		}
	}
}
