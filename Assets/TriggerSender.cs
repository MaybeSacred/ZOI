using UnityEngine;
using System.Collections.Generic;

public class TriggerSender : MonoBehaviour {
	public List<Transform> attachedEventHandlers;
	void Start () {
		if(attachedEventHandlers.Count == 0)
		{
			attachedEventHandlers = new List<Transform>();
		}
	}

	void Update () {
	
	}
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.layer == LayerMask.NameToLayer("EnemyLayer") || other.tag.Equals("EnemyGenerator"))
		{
			if(other.name.Equals("KatneenCenterMesh"))
			{
				if(!attachedEventHandlers.Contains(other.transform.parent))
					attachedEventHandlers.Add(other.transform.parent);
			}
			if(other.gameObject.name.Equals("SmallSlinkyEnemy")||other.gameObject.name.Equals("SlinkyEnemy"))
			{
				if(!attachedEventHandlers.Contains(other.transform))
				{

					attachedEventHandlers.Add(other.transform);
				}
			}
			else if(other.gameObject.GetComponent<MonoBehaviour>() is BaseEnemy)
			{
				if(!attachedEventHandlers.Contains(other.transform.parent))
					attachedEventHandlers.Add(other.transform);
			}
			else if(other.transform.parent.GetComponent<MonoBehaviour>() is BaseEnemy)
			{
				if(!attachedEventHandlers.Contains(other.transform.parent))
					attachedEventHandlers.Add(other.transform.parent);
			}
		}
		else if(other.tag.Equals("Player"))
		{
			foreach(Transform enemy in attachedEventHandlers)
			{
				if(enemy != null)
				{
					((PlayerEvent)enemy.GetComponent<MonoBehaviour>()).OnPlayerEnter();
				}
			}
		}
	}
	void OnTriggerExit(Collider other)
	{
		if(other.tag.Equals("Player"))
		{
			foreach(Transform enemy in attachedEventHandlers)
			{
				if(enemy != null)
				{
					((PlayerEvent)enemy.GetComponent<MonoBehaviour>()).OnPlayerExit();
				}
			}
		}
	}
}
