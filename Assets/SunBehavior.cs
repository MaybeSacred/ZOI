using UnityEngine;
using System.Collections;

public class SunBehavior : MonoBehaviour {
	public float speed;
	private Quaternion preCompSpeed;
	void Start () {
		preCompSpeed = Quaternion.Euler(new Vector3(0, 0, speed));
	}

	void Update () {
		transform.localRotation *= preCompSpeed;
	}
}
