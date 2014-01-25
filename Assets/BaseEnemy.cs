using System;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour, PlayerEvent
{
	public float health;
	public float maxHealth;
	protected bool isAwake;
	protected float deathTimeoutTimer;
	public float deathTimeout;
	public virtual void KillMe()
	{
		Destroy(gameObject);
	}
	public virtual void HealthChange(float shieldDmg, float healthDmg)
	{
		if(health > 0)
		{
			health += healthDmg;
			if(health > maxHealth)
			{
				health = maxHealth;
			}
			if(health <= 0)
			{
				health = 0;
				KillMe();
			}
		}
	}
	public virtual void RealCollisionHandler(Collider other)
	{

	}
	public virtual void OnPlayerEnter()
	{
		isAwake = true;
	}
	public virtual void OnPlayerExit()
	{
		if(deathTimeoutTimer <=0)
		{
			isAwake = false;
		}
	}
}

