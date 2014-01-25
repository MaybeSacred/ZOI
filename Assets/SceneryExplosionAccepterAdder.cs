using UnityEngine;
using System.Collections;

public class SceneryExplosionAccepterAdder : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Transform[] temp = GetComponentsInChildren<Transform>();
		for(int i = 0; i < temp.Length; i++)
		{
			if(temp[i].rigidbody != null)
			{
				temp[i].gameObject.AddComponent<SceneryCollisionReacter>();
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
