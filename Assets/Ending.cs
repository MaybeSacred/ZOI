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
			if(ps.startSize < expandingTime || infiniteExpand){
				ps.startSize += 10 * Time.deltaTime;
			}
		}
	}

	public void ActivateBeam ()
	{
		playingEnding = true;
		ps.Play();
	}

	void OnTriggerEnter(){
		infiniteExpand = true;
		Util.mainCamera.EndGame();
	}
}
