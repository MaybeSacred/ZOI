using UnityEngine;
using System.Collections;

public class SceneryTrigger : MonoBehaviour {
	public Transform sceneryElement;
	private bool isActive;
	public float elementDropOffRate;
	public float cutoff;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(isActive)
		{
			sceneryElement.localScale -= Time.deltaTime*elementDropOffRate*sceneryElement.localScale.normalized;
			if(sceneryElement.localScale.normalized.magnitude <= cutoff)
			{
				isActive = false;
				Destroy(sceneryElement.gameObject);
				Destroy(this);
			}
		}
	}
	public void OnTriggerEnter(Collider other)
	{
		if(other.tag.Equals("Player"))
		{
			isActive = true;
		}
	}
}
