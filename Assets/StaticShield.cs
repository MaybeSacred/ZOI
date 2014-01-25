using UnityEngine;
using System.Collections;

public class StaticShield : MonoBehaviour {
	public float shieldStrength;
	public float shieldBulletStrength;
	public Transform reactionSystem;
	public float reflectionAddedDrag;
	private float shieldPct = 100;
	public Color highAlphaMainColor;
	public Color lowAlphaMainColor;
	public float shieldRechargeRate;
	public float shieldKnockedDownRechargeDelay;
	private float timeSinceKnockDownHit;
	// Use this for initialization
	void Start () {
	
	}
	
	void Update () {
		if(shieldPct > 0)
		{
			shieldPct += Time.deltaTime*shieldRechargeRate;
			if(shieldPct > 100)
			{
				shieldPct = 100;
			}
		}
		else
		{
			if(Time.timeSinceLevelLoad > timeSinceKnockDownHit + shieldKnockedDownRechargeDelay)
			{
				renderer.enabled = true;
				collider.enabled = true;
				shieldPct += Time.deltaTime*shieldRechargeRate;
			}
		}
		renderer.material.color = Color.Lerp(lowAlphaMainColor, highAlphaMainColor, shieldPct/100);
	}
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.layer == 8)
		{
			if(!other.tag.Equals("Laser"))
			{
				if((-Time.deltaTime*2*other.rigidbody.velocity + other.transform.position - transform.position).magnitude > transform.localScale.x/2)
				{
					ParticleSystem temp = ((Transform)Instantiate(reactionSystem, (other.transform.position - transform.position).normalized*transform.localScale.x/2 + transform.position, Quaternion.LookRotation(other.transform.position - transform.position))).GetComponentInChildren<ParticleSystem>();
					temp.startSize = 1.5f*other.rigidbody.mass;
					other.rigidbody.useGravity = true;
					other.rigidbody.drag += reflectionAddedDrag;
					other.rigidbody.velocity = ((other.transform.position-transform.position).normalized*shieldBulletStrength);
					shieldPct -= other.GetComponent<BasicBullet>().shieldDamage;
					if(shieldPct < 0)
					{
						renderer.enabled = false;
						collider.enabled = false;
						shieldPct = 0;
						timeSinceKnockDownHit = Time.timeSinceLevelLoad;
					}
				}
			}
		}
	}
	void OnTriggerStay(Collider other)
	{
		if(other.tag.Equals("Player"))
		{
			if(other.GetComponent<PlayerController>() != null)
			{
				Vector3 temp = (other.transform.position-transform.position).normalized*shieldStrength;
				temp.y = 100;
				other.rigidbody.AddForce(temp);
			}
		}
	}
}
