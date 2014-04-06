using UnityEngine;
using System.Collections;

public class FogAdjust : MonoBehaviour
{
	private FogAdjust_Helper[] fogAdjustHelpers;
	private FogAdjust_Helper upcomingTrigger;

	private float previousSqrDistance;
	private float currentInterpolation;

	private float previousFogDensity;
	private float nextFogDensity;

	public Transform player;
	
	void Start()
	{
		fogAdjustHelpers = GetComponentsInChildren<FogAdjust_Helper>();

		if (null == fogAdjustHelpers || 0 == fogAdjustHelpers.Length)
		{
			gameObject.SetActive(false);
			return;
		}

		FogAdjust_Helper firstTrigger = fogAdjustHelpers[0];
		while (null != firstTrigger.previousTrigger)
			firstTrigger = firstTrigger.previousTrigger;

		upcomingTrigger = firstTrigger;

		previousSqrDistance = Vector3.SqrMagnitude(upcomingTrigger.transform.position - player.position);

		previousFogDensity = RenderSettings.fogDensity;
		nextFogDensity = upcomingTrigger.fogDensity;

		currentInterpolation = 1.0f;
	}

	public void OnHelperTrigger(FogAdjust_Helper triggerCollidedWith)
	{
		if (triggerCollidedWith.fogDensity == 0.0f)
		{
			gameObject.SetActive(false);
			RenderSettings.fog = false;
		}

		if (null == triggerCollidedWith.nextTrigger)
		{
			gameObject.SetActive(false);
			return;
		}

		previousSqrDistance = Vector3.SqrMagnitude(triggerCollidedWith.nextTrigger.transform.position - player.position);
		
		previousFogDensity = upcomingTrigger.fogDensity;
		nextFogDensity = triggerCollidedWith.nextTrigger.fogDensity;

		upcomingTrigger = triggerCollidedWith.nextTrigger;

		currentInterpolation = 1.0f;
	}

	void Update()
	{
		float interpolation = 10f * Vector3.SqrMagnitude(upcomingTrigger.transform.position - player.position) / previousSqrDistance;

		//Debug.Log("interpolation = " + interpolation + ", Mathf.Round(interpolation) = " + Mathf.Round (interpolation) + ", currentInterpolation * 10 = " + (currentInterpolation * 10));

		if (Mathf.Round(interpolation) < currentInterpolation * 10)
		{
			currentInterpolation = Mathf.Round(interpolation) * 0.1f;
			RenderSettings.fogDensity = (currentInterpolation) * previousFogDensity + (1 - currentInterpolation) * nextFogDensity;
		}
	}
}
