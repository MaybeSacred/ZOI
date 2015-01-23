using UnityEngine;
using System.Collections;

public class EndingBeamTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	void OnTriggerEnter(Collider other){
		if(other.tag.Equals("Player")){
			GetComponentInParent<Ending>().ActivateBeam();
		}
	}
}
