using UnityEngine;
using System.Collections;

public class Ending : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	void OnTriggerEnter(){
		Util.mainCamera.EndGame();
	}
}
