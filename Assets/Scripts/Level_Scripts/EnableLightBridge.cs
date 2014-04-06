using UnityEngine;
using System.Collections;

public class EnableLightBridge : MonoBehaviour
{
	public GameObject lightBridge;
	private MeshRenderer lightBridgeRenderer;
	private MeshCollider lightBridgeCollider;

	void OnTriggerEnter(Collider tank)
	{
		if (tank.tag == "Player")
		{
			lightBridgeRenderer.enabled = true;
			lightBridgeCollider.enabled = true;
		}
	}

	void OnTriggerExit(Collider tank)
	{
		if (tank.tag == "Player")
		{
			lightBridgeRenderer.enabled = false;
			lightBridgeCollider.enabled = false;
		}
	}

	void Start()
	{
		lightBridgeRenderer = lightBridge.GetComponent<MeshRenderer>();
		lightBridgeCollider = lightBridge.GetComponent<MeshCollider>();
	}
}
