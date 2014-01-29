using UnityEngine;
using System.Collections;

public class HyperBlimpEngine : MonoBehaviour {
	public float rotationSpeed;
	private Quaternion rotQuat;
	void Start () {
		rotQuat = new Quaternion(0, 0, Mathf.Sin(Time.deltaTime*rotationSpeed*Mathf.Deg2Rad), Mathf.Cos(Time.deltaTime*rotationSpeed*Mathf.Deg2Rad));
	}
	
	void Update () {
		transform.localRotation *= rotQuat;
	}
}
