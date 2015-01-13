using UnityEngine;
using System.Collections;

public class EndingBeamTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	void OnTriggerEnter(){
		GetComponentInParent<Ending>().ActivateBeam();
	}
}
