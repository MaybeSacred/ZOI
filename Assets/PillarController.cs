using UnityEngine;
using System.Collections;

public class PillarController : MonoBehaviour {
	public Material pillarTex;
	public Color colorStart;
	public float colorRange;
	public float frequency;
	public float overboardColorFactor;
	void Start () {
	}
	
	void Update () {
		pillarTex.color = new Color(colorStart.r + colorRange*Mathf.Sin(frequency*Time.timeSinceLevelLoad)/overboardColorFactor, colorStart.g + colorRange*Mathf.Sin(frequency*Time.timeSinceLevelLoad)/overboardColorFactor, colorStart.b + colorRange*Mathf.Sin(frequency*Time.timeSinceLevelLoad)/overboardColorFactor);
	}
}
