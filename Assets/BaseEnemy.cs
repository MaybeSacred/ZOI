using System;
using UnityEngine;
using System.Collections.Generic;
public abstract class BaseEnemy : MonoBehaviour, PlayerEvent
{
	public float health;
	public float maxHealth;
	protected bool isAwake;
	protected float deathTimeoutTimer;
	public float deathTimeout;
	protected List<GameObject> colliders;
	public BarrierBehavior attachedBarrier;
	protected bool calledBarrier;
	public ParticleSystem deathParticles;
	void Awake()
	{
		if(attachedBarrier != null)
		{
			attachedBarrier.RegisterEnemy();
		}
		colliders = new List<GameObject>();
	}
	///<summary>Called to handle the final destruction of BaseEnemy</summary>
	public virtual void KillMe()
	{
		Util.theGUI.RemoveRadarObject(transform);
		if(attachedBarrier != null&& calledBarrier==false)
		{
			calledBarrier = true;
			attachedBarrier.UnregisterEnemy();
		}
		Destroy(gameObject);
	}
	///<summary>Updates the health of the BaseEnemy, negative values decrease health</summary>
	public virtual void HealthChange(float shieldDmg, float healthDmg)
	{
		if(deathTimeoutTimer <= 0)
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
	///<summary>Synthesizes various Unity collision calls, does not necessarily need to be invoked by derived classes</summary>
	public virtual void RealCollisionHandler(Collider other)
	{

	}
	///<summary>Called when player enters surrounding check collider</summary>
	public virtual void OnPlayerEnter()
	{
		isAwake = true;
		Util.theGUI.AddRadarObject(transform, GameGUI.RadarObject.OBJECTTYPE.ENEMY);
	}
	///<summary>Called when player exits surrounding check collider</summary>
	public virtual void OnPlayerExit()
	{
		
	}
}

