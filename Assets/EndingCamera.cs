using UnityEngine;
using System.Collections;

public class EndingCamera : MonoBehaviour {
	public Vector3[] possibleVantages;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 temp = Util.player.transform.position;
		temp.y += 2;
		//transform.position -= (temp - transform.position).normalized * Time.deltaTime * .25f;
		transform.LookAt(temp);
	}
	public void SetVantagePoint(){
		Vector3 best = Util.player.transform.position;
		foreach(Vector3 v in possibleVantages){
			if((Util.player.transform.position - v).magnitude > (best - Util.player.transform.position).magnitude){
				best = v;
			}
		}
		transform.position = best;
	}
}
