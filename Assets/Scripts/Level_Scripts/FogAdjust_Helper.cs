using UnityEngine;
using System.Collections;

public class FogAdjust_Helper : MonoBehaviour
{
	public float fogDensity;
	public FogAdjust_Helper previousTrigger;
	public FogAdjust_Helper nextTrigger;

	private FogAdjust fogAdjust;

	void Start()
	{
		fogAdjust = this.transform.parent.gameObject.GetComponent<FogAdjust>();
	}

	void OnTriggerEnter(Collider tank)
	{
		if (tank.tag == "Player")
		{
			fogAdjust.OnHelperTrigger(this);
			gameObject.SetActive(false);
		}
	}
}
