using UnityEngine;
using System.Collections;

public class EndingCamera : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 temp = Util.player.transform.position;
		temp.y += 2;
		transform.position -= (temp - transform.position).normalized * Time.deltaTime * .25f;
		transform.LookAt(temp);
	}
}
