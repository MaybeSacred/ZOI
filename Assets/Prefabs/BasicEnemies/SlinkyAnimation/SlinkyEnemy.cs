using UnityEngine;
using System.Collections;

public class SlinkyEnemy : MonoBehaviour {

	public float moveTimer,moveDuration;
	public Animator anim;
	public bool moving;
	private bool offsetLate;
	public GameObject parent;
	SlinkyEnemyBehavior be;

	// Use this for initialization
	void Start () {
		anim.SetBool("isMoving",moving);
	}
	
	// Update is called once per frame
	void Update () {

		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("slinkyIdle")) 
		{
			offsetLate =true;
		}

		if (offsetLate) 
		{
			if(anim.IsInTransition(0))
				parent.gameObject.SendMessage("moveOffset");
				//transform.position =  new Vector3(transform.position.x-10f,transform.position.y,transform.position.z);

			offsetLate=false;
		}
	
	}
	void SetMoving(bool move)
	{
		anim.SetBool ("isMoving", move);
		moving = move;
	}
}
