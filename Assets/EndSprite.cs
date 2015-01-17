using UnityEngine;
using System.Collections;

public class EndSprite : MonoBehaviour {
	public Transform[] moveToPoints;
	public float nextPointMoveDistance;
	public float minDistanceFromGround;
	public float pullForce;
	int currentTransform = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		SwitchToNextWaypoint();
		Vector3 midpointToMoveTo = CalculateMidpoint();
		rigidbody.AddForce((midpointToMoveTo - transform.position).normalized * Time.deltaTime * pullForce * (midpointToMoveTo - transform.position).sqrMagnitude * (Util.player.rigidbody.velocity.magnitude/Util.player.maxSpeed));
		if(rigidbody.velocity.magnitude > Util.player.maxSpeed){
			rigidbody.AddForce(-rigidbody.velocity * 5);
		}
	}
	Vector3 CalculateMidpoint(){
		if(currentTransform < moveToPoints.Length - 1){
			Vector3 outVec = moveToPoints[currentTransform].position;
			RaycastHit hit;
			if(Physics.Raycast(outVec, Vector3.down, out hit)){
				if(outVec.y - hit.point.y < minDistanceFromGround){
					outVec.y = hit.point.y + minDistanceFromGround;
				}
			}
			return outVec;
		}
		else{
			return moveToPoints[currentTransform].position;
		}
		
	}
	//Updates to next waypoint if player is closer to that one than current one or below nextPointMoveDistance
	void SwitchToNextWaypoint(){
		if(currentTransform < moveToPoints.Length - 1){
			if((Util.player.transform.position - moveToPoints[currentTransform + 1].position).magnitude < 
			   (Util.player.transform.position - moveToPoints[currentTransform].position).magnitude ||
			   (Util.player.transform.position - moveToPoints[currentTransform].position).magnitude < nextPointMoveDistance){
				currentTransform++;
			}
		}
	}
}
