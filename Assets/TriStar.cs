using UnityEngine;
using System.Collections.Generic;

public class TriStar : BaseEnemy {
	public float duration = 2.0F;
	public Material[] armMaterial;
	public float moveForce;
	private float forceTimer;
	private Vector3 currentForce;
	public float forceChangeTime;
	public float recognizingDistance;
	public float firingDistance;
	public float turningSpeed;
	public float maxFiringAngle;
	public Transform bulletEmitter;
	public BasicBullet bb;
	private float firingTimer;
	public float firingRate;
	void Start () {
		armMaterial[0].mainTextureOffset = new Vector2(0, .5f);
		armMaterial[1].mainTextureOffset = new Vector2(0, .25f);
		armMaterial[2].mainTextureOffset = Vector2.zero;
	}

	void Update () 
	{
		float lerp = Mathf.PingPong(Time.time, duration) / duration;
		armMaterial[2].color = new Color(armMaterial[2].color.r, armMaterial[2].color.g, armMaterial[2].color.b, 1-lerp);
		for(int i = 0; i < armMaterial.Length; i++)
		{
			armMaterial[i].mainTextureOffset += new Vector2(0, Time.deltaTime);
		}
		if(isAwake)
		{
			Vector3 positionVec = Util.player.transform.position - transform.position;
			if(forceTimer > forceChangeTime)
			{
				currentForce.x = -moveForce*Random.value;
				forceTimer -= forceChangeTime;
			}
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(positionVec), turningSpeed*Time.deltaTime);
			rigidbody.AddRelativeForce(currentForce);
			forceTimer += Time.deltaTime;
			if(Vector3.Angle(transform.forward, positionVec) < maxFiringAngle)
			{
				if(firingTimer > firingRate)
				{
					Util.Fire<BasicBullet>(bb, bulletEmitter.transform.position, Quaternion.LookRotation(Util.player.transform.position-bulletEmitter.transform.position), bb.initialSpeed*(Util.player.transform.position-bulletEmitter.transform.position).normalized);
					firingTimer = 0;
				}
			}
			firingTimer += Time.deltaTime;
		}
	}
	public void OnTriggerEnter(Collider other)
	{
		if(isAwake)
		{
			if(other.tag.Equals("BasicExplosion"))
			{
				try
				{
					if(!colliders.Contains(other.gameObject))
					{
						colliders.Add(other.gameObject);
						BasicExplosion be = (BasicExplosion)other.GetComponent<BasicExplosion>();
						rigidbody.AddExplosionForce(be.explosionForce, other.transform.position, be.explosionRadius);
						HealthChange(-be.shieldDamage, -be.healthDamage);
					}
				}
				catch(System.InvalidCastException ie)
				{
					Debug.Log("Incorrect tag assignment for tag \"Basic Explosion\"");
				}
			}
			if(other.tag.Equals("Bullet"))
			{
				other.GetComponent<BasicBullet>().DestroyMe();
			}
		}
	}
}
