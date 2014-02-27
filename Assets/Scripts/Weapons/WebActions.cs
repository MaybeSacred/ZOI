using UnityEngine;
using System.Collections;

public class WebActions : MonoBehaviour {

	public float freezeTime, lifeTime;
	private float lifeTimer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		lifeTimer += Time.deltaTime;
		if (lifeTimer > lifeTime) 
		{
			GetComponent<Collider>().enabled = false;
			if(GetComponent<MeshRenderer>() != null)
			{
				GetComponent<MeshRenderer>().enabled = false;
			}
			Destroy (this.gameObject);
		}
	
	}
}
