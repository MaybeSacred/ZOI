//@Chris Tansey
//email: cmtansey@gatech.edu

using UnityEngine;
using System.Collections;

public class debugVec3 : MonoBehaviour {

	public Vector3 here;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		transform.position = here;
	
	}

	void sendPosition(Vector3 pos)
	{
		here = pos;
	}
}
