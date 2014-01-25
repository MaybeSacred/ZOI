using UnityEngine;
using System.Collections;

public class EnemyGenerator : MonoBehaviour, PlayerEvent {
	public BaseEnemy[] enemyToGenerate;
	public Vector3[] initialOffset;
	public Vector3[] initialRotation;
	public int[] delayTimes;
	private int currentDelayTimeIndex;
	private bool awoken;
	private float timer;
	void Start () {
	
	}
	
	void Update () {
		if(awoken)
		{
			if(currentDelayTimeIndex >= delayTimes.Length)
			{
				awoken = false;
			}
			else
			{
				bool done = false;
				while(!done)
				{
					if(timer > delayTimes[currentDelayTimeIndex])
					{
						BaseEnemy temp = Instantiate(enemyToGenerate[currentDelayTimeIndex], initialOffset[currentDelayTimeIndex]+transform.position, Quaternion.LookRotation(initialRotation[currentDelayTimeIndex])) as BaseEnemy;
						temp.OnPlayerEnter();
						currentDelayTimeIndex++;
						if(currentDelayTimeIndex >= delayTimes.Length)
						{
							done = true;
							awoken = false;
						}
					}
					else
					{
						done = true;
					}
				}
			}
			timer += Time.deltaTime;
		}
	}
	public void OnPlayerEnter()
	{
		awoken = true;
	}
	public void OnPlayerExit()
	{
		
	}
}
