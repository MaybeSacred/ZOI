using UnityEngine;
using System.Collections;

public class TowerScript : MonoBehaviour {
	public Transform towerCenterLightGO;
	public Transform centerInnerSphere;
	public Transform centerOuterSphere;
	public float centerSpheresAmplitude;
	public float centerSpheresSpeed;
	public Transform[] movingParticles;
	private Quaternion[] particleSpeeds;
	private Quaternion[] nextParticleSpeedVec;
	private float[] particleTimers;
	private float[] particleResetTimes;
	public float particleAcceleration;
	public float particleSpeed;
	public Material glowTex;
	public float glowSpeed;
	private float towerCenterLightInitialY;
	void Start () {
		particleSpeeds = new Quaternion[movingParticles.Length];
		nextParticleSpeedVec = new Quaternion[movingParticles.Length];
		particleTimers = new float[movingParticles.Length];
		particleResetTimes = new float[movingParticles.Length];
		towerCenterLightInitialY = towerCenterLightGO.localPosition.y;
	}
	
	void Update () 
	{
		for(int i = 0; i < movingParticles.Length; i++)
		{
			if(particleTimers[i] > particleResetTimes[i])
			{
				particleTimers[i] = 0;
				particleResetTimes[i] = Random.Range(1f,2);
				nextParticleSpeedVec[i] = Random.rotationUniform;
			}
			particleSpeeds[i] = Quaternion.RotateTowards(particleSpeeds[i], nextParticleSpeedVec[i], Time.deltaTime*particleAcceleration);
			movingParticles[i].localRotation *= Quaternion.RotateTowards(Quaternion.identity, particleSpeeds[i], Time.deltaTime*particleSpeed);
			particleTimers[i] += Time.deltaTime;
		}
		centerInnerSphere.localRotation *= new Quaternion(Time.deltaTime*Mathf.Sin(Mathf.PerlinNoise(Time.realtimeSinceStartup, 0)), Time.deltaTime*Mathf.Sin(Mathf.PerlinNoise(0, Time.realtimeSinceStartup)), Time.deltaTime*Mathf.Sin(Mathf.PerlinNoise(Time.realtimeSinceStartup, Time.realtimeSinceStartup)),1);
		towerCenterLightGO.localPosition = new Vector3(0,centerSpheresAmplitude*Mathf.Sin(centerSpheresSpeed*Time.timeSinceLevelLoad)+towerCenterLightInitialY,0);
		float temp = Mathf.Abs(Mathf.Sin(Time.timeSinceLevelLoad)*glowSpeed);
		glowTex.color = new Color(temp, temp, temp);
	}
}
