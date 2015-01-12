using UnityEngine;
using System.Collections;

public class EndingBeamTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	void OnTriggerEnter(){
		Debug.Log("here");
		GetComponentInParent<Ending>().particleSystem.Play();
	}
}
