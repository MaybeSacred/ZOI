using UnityEngine;
using System.Collections;

public class SpinObjectScript : MonoBehaviour
{
	public float spinRateInDegrees;

	void Update()
	{
		transform.rotation *= Quaternion.Euler(0f, 0f, spinRateInDegrees * Time.deltaTime);
	}
}
