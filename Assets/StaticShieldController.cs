using UnityEngine;
using System.Collections;

public class StaticShieldController : MonoBehaviour {
	public Material ringMaterial;
	public float ringMaterialStrengthRate;
	public float ringMaterialVarianceAlpha;
	public float originalRingMaterialAlpha;
	public Transform ring;
	private Vector3 originalRingScale;
	public float ringScaleFactor;
	public float ringScaleRate;
	private int topPiecesLeft = 3;
	public Light deathLightFX;
	public float deathTimeout;
	private float deathTimeoutTimer;
	public float finalDeathLightRange;
	void Start () {
		originalRingScale = ring.localScale;
	}
	
	void Update () {
		if(deathTimeoutTimer > 0)
		{
			if(deathTimeoutTimer > deathTimeout)
			{
				Destroy(gameObject);
			}
			ring.localScale = Vector3.Lerp(ring.localScale, Vector3.zero, Time.deltaTime);
			deathLightFX.range = Mathf.Lerp(2*finalDeathLightRange, 0, deathTimeoutTimer/deathTimeout);
			deathTimeoutTimer += Time.deltaTime;
		}
		ringMaterial.SetColor("_TintColor", new Color(1, 1, 1, Mathf.Sin(Time.timeSinceLevelLoad*ringMaterialStrengthRate)*ringMaterialVarianceAlpha + originalRingMaterialAlpha));
	}
	public void RemovePiece()
	{
		topPiecesLeft--;
		if(topPiecesLeft <= 0)
		{
			deathTimeoutTimer += Time.deltaTime;
		}
	}
}
