using UnityEngine;
using System.Collections;

public class Ending : MonoBehaviour {
	bool playingEnding = false, infiniteExpand;
	public float expandingTime;
	private float expandingTimeTimer = 0;
	ParticleSystem ps;
	// Use this for initialization
	void Start () {
		ps = GetComponent<ParticleSystem>();
	}
	void Update(){
		if(playingEnding){
			if(expandingTimeTimer > expandingTime){
				if(expandingTimeTimer > 2 * expandingTime){
					Application.LoadLevel("L00_StartScreen");
				}
				expandingTimeTimer += Time.deltaTime;
			}
			else if(infiniteExpand){
				expandingTimeTimer += Time.deltaTime;
				ps.startSize += 10 * Time.deltaTime;
				Util.player.Brake();
			}
			else{
				Util.player.MoveTowardsPosition(transform.position);
			}
		}
	}

	public void ActivateBeam (){
		playingEnding = true;
		Util.player.DisablePlayerControl(10);
		ps.Play();
		Util.mainCamera.EndGame();
	}

	void OnTriggerEnter(Collider other){
		if(other.tag.Equals("Player")){
			infiniteExpand = true;
		}
	}
}
