using UnityEngine;
using System.Collections;

public class ClingyDanLeg : MonoBehaviour {
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnJointBreak()
	{
		transform.parent.GetComponent<ClingyDanBehavior>().OnJointBreak();
	}
}
