//Jordan Hobgood

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicSystem: MonoBehaviour {


	
	[Range(0f,1f)]
	public float MasterVolume = 1;

	[Range(0f,1f)]
	public float BattleVolume = 1;

	[Range(0f,1f)]
	public float BackgroundVolume = 1;

	[Range(0f,1f)]
	public float PauseVolume = 1;
	public int pauseFade = 1000;
	private bool paused = false;
	
	public int fadeIn = 2000;
	public int fadeOut = 2000;
	
	public List<AudioSource> audioSources = new List<AudioSource>();
	public List<Group> groups = new List<Group>();
	private Hashtable direction = new Hashtable();
	public List<float> volumes = new List<float>();
	
	private Hashtable lerpStarts = new Hashtable();
	private Hashtable origVolumes = new Hashtable();

	private int sampleOffset = 1486452; //The ammount of samples in a 16 bar phrase
	
	public enum Group {
		Drone,
		Battle,
		Background
	}
	
	public void ChangeMasterVolume(float change){
		MasterVolume = Mathf.Clamp01(change);
	}
	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(transform.gameObject);
		randomizePhrases();
	}
	void OnLevelWasLoaded(int input){
		CameraScript cs = FindObjectOfType<CameraScript>();
		StartCameraScript scs = FindObjectOfType<StartCameraScript>();
		if(cs != null){
			transform.parent = cs.transform;
			transform.localPosition = Vector3.zero;
		}
		else if(scs != null){
			transform.parent = scs.transform;
			transform.localPosition = Vector3.zero;
		}
	}
	// Update is called once per frame
	void Update () {
		adjustVolumes();
	}

	public void playBattleMusic(bool b){
		if (b){
			setGroupVolume (Group.Battle, 1);
			
		}else{
			setGroupVolume (Group.Battle, 0);
		}
	}

	public void pauseMusic(bool b){
		if (b){
			paused = true;
		}else{
			paused = false;
		}
	}

	private void adjustVolumes(){
		for (int counter = 0; counter < audioSources.Capacity; counter++){
			
			float currentVol = audioSources[counter].volume;
			
			//Calc desired volume
			float desiredVol = volumes[counter];
			desiredVol *= MasterVolume;
			
			if (groups[counter] == Group.Battle){
				desiredVol *= BattleVolume;
			}else{
				desiredVol *= BackgroundVolume;
			}
			
			if (paused){
				desiredVol *= PauseVolume;
			}
			
			desiredVol = Mathf.Clamp (desiredVol, 0, 1);
			//If Volumes are not equal
			if (!Mathf.Approximately(currentVol, desiredVol)){
				audioSources[counter].volume = Mathf.Lerp(currentVol, desiredVol, Time.deltaTime * fadeIn);
				/*
				//Save the start time and initial volume
				if (!lerpStarts.ContainsKey(counter)){
					lerpStarts.Add(counter, Time.time);
					origVolumes.Add (counter, currentVol);
					
					//Save whether we are fading up or down
					direction.Add(counter, currentVol > desiredVol ? true : false);
				}
				
				//If the fade direction has changed before we finished
				if ((bool)direction[counter] != currentVol > desiredVol ? true : false){
					lerpStarts.Remove(counter);
					origVolumes.Remove(counter);
					direction.Remove(counter);
					return;
				}
				
				//Lerp or round to the desired value
				if (Mathf.Abs(currentVol - desiredVol) > 0.01){
					
					float startTime = (float)lerpStarts[counter];
					float elapsedTime = (Time.time - startTime);
					float t = (elapsedTime/(getFadeAmount(currentVol, desiredVol)/1000));
					
					audioSources[counter].volume = Mathf.Lerp ((float)origVolumes[counter], desiredVol, t);
					
				}else{
					audioSources[counter].volume = desiredVol;
					
					//Remove the saved time and volume
					lerpStarts.Remove(counter);
					origVolumes.Remove(counter);
					direction.Remove(counter);
				}*/
			}
		}
	}


	private void randomizePhrases(){
		for (int counter = 0; counter < audioSources.Capacity; counter++){
			if (groups[counter] == Group.Background){

				int times = Random.Range (0,6);

				audioSources[counter].timeSamples = audioSources[counter].timeSamples + (sampleOffset * times);
			}
		}
	}
	
	private void setGroupVolume(Group group, int volume){
		for (int counter = 0; counter < audioSources.Capacity; counter++){
			if (group.Equals(groups[counter])){
				volumes[counter] = volume;
			}
		}
	}

	private int getFadeAmount(float currentVol, float desiredVol){
		if (paused){
			return pauseFade;
		}else if (currentVol > desiredVol){
			return fadeOut;
		}else{
			return fadeIn;
		}
	}
}
