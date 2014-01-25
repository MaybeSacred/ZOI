using UnityEngine;
using System.Collections;

public class TristarParticle : MonoBehaviour {
	private Color startColor, endColor;
	public float ringScale;
	public float duration;
	private Vector3 scale;
	private float timeSinceCreation;
	void Start () {
		transform.localScale = Vector3.zero;
	}
	public void CreateMe(Color start, Color end)
	{
		startColor = start;
		endColor = end;
	}
	void Update () {
		if(timeSinceCreation > duration)
		{
			Destroy(this.gameObject);
		}
		renderer.material.color = Color.Lerp(startColor, endColor, timeSinceCreation/duration);
		scale.x = ringScale*Mathf.Sin(Mathf.PI*timeSinceCreation/duration);
		scale.y = ringScale*Mathf.Sin(Mathf.PI*timeSinceCreation/duration);
		timeSinceCreation += Time.deltaTime;
		transform.localScale = scale;
	}
}
