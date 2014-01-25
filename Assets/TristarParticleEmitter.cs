using UnityEngine;
using System.Collections;

public class TristarParticleEmitter : MonoBehaviour {
	public Color startColor, endColor;
	public float emissionRate;
	private float timer;
	public TristarParticle tp;
	private BasicBullet bb;
	void Start () {
		bb = GetComponent<BasicBullet>();
	}

	void Update () {
		if(bb.collider.enabled)
		{
			if(timer > emissionRate)
			{
				TristarParticle tri = (TristarParticle)Instantiate(tp, transform.position, transform.rotation);
				tri.CreateMe(startColor, endColor);
				timer -= emissionRate;
			}
			timer += Time.deltaTime;
		}
	}
}
