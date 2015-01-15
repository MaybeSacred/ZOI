using UnityEngine;
using System.Collections;

public class Ending : MonoBehaviour {
	bool playingEnding = false, infiniteExpand;
	public float expandingTime;
	ParticleSystem ps;
	// Use this for initialization
	void Start () {
		ps = GetComponent<ParticleSystem>();
	}
	void Update(){
		if(playingEnding){
			if(infiniteExpand){
				ps.startSize += 10 * Time.deltaTime;
			}
		}
	}

	public void ActivateBeam (){
		playingEnding = true;
		ps.Play();
		Util.mainCamera.EndGame();
	}

	void OnTriggerEnter(Collider other){
		if(other.tag.Equals("Player")){
			infiniteExpand = true;
		}
	}
}
