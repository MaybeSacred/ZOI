using UnityEngine;
using System.Collections;

public class ElevatorArena : MonoBehaviour
{
	private delegate void elevatorUpdateDelegate();
	private elevatorUpdateDelegate ElevatorUpdate;

	//private float timeElevatorSection = 0.0f;

	//public Rigidbody tankRigidbody;
	public Transform tankCameraSnow;

	public ElevatorArena elevatorScript;
	public Collider trigger;
	public Collider deathTrigger;

	public Transform elevatorRail;
	private Vector3 elevatorRailOriginalPosition;

	public Transform elevatorTransform;
	private Vector3 elevatorOriginalPosition;

	public Transform elevatorLevel1Transform;
	private Vector3 elevatorLevel1Position;

	public Transform elevatorLevel2Transform;
	private Vector3 elevatorLevel2Position;

	public Transform elevatorTopLevelTransform;
	private Vector3 elevatorTopLevelPosition;

	private float idleTimeSeconds = 0.0f;
	private elevatorUpdateDelegate NextElevatorUpdate;

	public Transform finalDoorLeft;
	public Transform finalDoorRight;
	private Vector3 finalDoorLeftOriginalPosition;
	private Vector3 finalDoorRightOriginalPosition;
	private float finalDoorsDistance = 0f;

	void Awake()
	{
		elevatorRailOriginalPosition = new Vector3(elevatorRail.localPosition.x, elevatorRail.localPosition.y, elevatorRail.localPosition.z);

		elevatorLevel1Position = new Vector3(elevatorLevel1Transform.position.x, elevatorLevel1Transform.position.y, elevatorLevel1Transform.position.z);
		elevatorLevel2Position = new Vector3(elevatorLevel2Transform.position.x, elevatorLevel2Transform.position.y, elevatorLevel2Transform.position.z);
		elevatorTopLevelPosition = new Vector3(elevatorTopLevelTransform.position.x, elevatorTopLevelTransform.position.y, elevatorTopLevelTransform.position.z);

		elevatorOriginalPosition = new Vector3(elevatorTransform.position.x, elevatorTransform.position.y, elevatorTransform.position.z);

		finalDoorLeftOriginalPosition = new Vector3(finalDoorLeft.position.x, finalDoorLeft.position.y, finalDoorLeft.position.z);
		finalDoorRightOriginalPosition = new Vector3(finalDoorRight.position.x, finalDoorRight.position.y, finalDoorRight.position.z);

			// Create Bezier Curve Constant Matrix
			// [-1,  3, -3,  1]
			// [ 3, -6,  3,  0]
			// [-3,  3,  0,  0]
			// [ 1,  0,  0,  0]
		bezierWeightsMatrix = new Matrix4x4();
		for (int n = 0; n < 4; ++n)
			bezierWeightsMatrix.SetRow(n, Vector4.zero);
		bezierWeightsMatrix.m00 = -1;
		bezierWeightsMatrix.m01 = 3;
		bezierWeightsMatrix.m02 = -3;
		bezierWeightsMatrix.m03 = 1;
		bezierWeightsMatrix.m10 = 3;
		bezierWeightsMatrix.m11 = -6;
		bezierWeightsMatrix.m12 = 3;
		bezierWeightsMatrix.m13 = 0;
		bezierWeightsMatrix.m20 = -3;
		bezierWeightsMatrix.m21 = 3;
		bezierWeightsMatrix.m22 = 0;
		bezierWeightsMatrix.m23 = 0;
		bezierWeightsMatrix.m30 = 1;
		bezierWeightsMatrix.m31 = 0;
		bezierWeightsMatrix.m32 = 0;
		bezierWeightsMatrix.m33 = 0;
	}

	void OnTriggerEnter(Collider tank)
	{
		if (tank.tag == "Player")
		{
			//Debug.Log("Starting Elevator");

			tankCameraSnow.parent = elevatorTransform;

			elevatorScript.enabled = true;
			trigger.enabled = false;
			deathTrigger.enabled = true;
		}
	}

	void Start()
	{
		ResetElevator();
	}

	public void ResetElevator()
	{
		tankCameraSnow.parent = null;

		elevatorScript.enabled = false;
		trigger.enabled = true;
		deathTrigger.enabled = false;

		ElevatorUpdate = PrepareElevator;

		idleTimeSeconds = 0.0f;
		timeIdling = 0.0f;

		//railVerticalPosition = 0.0f;

		newRailPosition = new Vector3(elevatorRail.localPosition.x, elevatorRail.localPosition.y, elevatorRail.localPosition.z);

		finalDoorLeft.position = finalDoorLeftPosition = new Vector3(finalDoorLeftOriginalPosition.x, finalDoorLeftOriginalPosition.y, finalDoorLeftOriginalPosition.z);
		finalDoorRight.position = finalDoorRightPosition = new Vector3(finalDoorRightOriginalPosition.x, finalDoorRightOriginalPosition.y, finalDoorRightOriginalPosition.z);

		parameterTIncreaseRate = 1.0f;
		parameterT = 0.0f;
	}

	void FixedUpdate()
	{
		ElevatorUpdate();
	}

	private float timeIdling = 0.0f;
	private void Idle()
	{
		timeIdling += Time.fixedDeltaTime;

		if (timeIdling > idleTimeSeconds)
		{
			ElevatorUpdate = NextElevatorUpdate;

			timeIdling = 0.0f;
			//Debug.Log ("done Ilding");
		}
	}

	//private float railVerticalPosition = 0.0f;
	private Vector3 newRailPosition;
	private void PrepareElevator()
	{
		newRailPosition.z += 0.2f * Time.fixedDeltaTime;

		if (newRailPosition.z >= 1f)
		{
			newRailPosition.z = 1f;
			ElevatorUpdate = Idle;
			idleTimeSeconds = 3f;
			NextElevatorUpdate = GotoFirstFloor;
			parameterTIncreaseRate = 1 / 8f;
			SetupBezierCurve();
			//Debug.Log ("Idle before going to First Floor");
		}

		elevatorRail.localPosition = newRailPosition; 
	}

	private float parameterTIncreaseRate = 1.0f;
	private float parameterT = 0.0f;
	private Matrix4x4 bezierWeightsMatrix;
	private Vector4 parameterTVector;
	private void GotoFirstFloor()
	{
		if (parameterT < 1)
		{
			//parameterTVector.x = parameterT * parameterT * parameterT;
			//parameterTVector.y = parameterT * parameterT;
			//parameterTVector.z = parameterT;
			//parameterTVector.w = 1;

				// Operation is not built into Unity...
			//elevatorTransform.position = CalculateBezierLocation(parameterTVector,
			//                                                     bezierCurveLocations[0],
			//                                                     bezierCurveLocations[1],
			//                                                     bezierCurveLocations[2],
			//                                                     bezierCurveLocations[3]);

			elevatorTransform.position = CalculateLocationWithEasing(parameterT, bezierCurveLocations[0], bezierCurveLocations[3]);

				// This should reach 1 in 1 second if the increase rate is 1
			parameterT += Time.fixedDeltaTime * parameterTIncreaseRate;
		}
		else
		{
			parameterT = 0.0f;
			ElevatorUpdate = Idle;
			idleTimeSeconds = 15f;
			NextElevatorUpdate = GotoSecondFloor;
			parameterTIncreaseRate = 1 / 8f;
			SetupBezierCurve();
		}
	}
	private void GotoSecondFloor()
	{
		if (parameterT < 1)
		{			
			elevatorTransform.position = CalculateLocationWithEasing(parameterT, bezierCurveLocations[0], bezierCurveLocations[3]);
			
			// This should reach 1 in 1 second if the increase rate is 1
			parameterT += Time.fixedDeltaTime * parameterTIncreaseRate;
		}
		else
		{
			parameterT = 0.0f;
			ElevatorUpdate = Idle;
			idleTimeSeconds = 15f;
			NextElevatorUpdate = GotoTopFloor;
			parameterTIncreaseRate = 1 / 18f;
			SetupBezierCurve();
		}
	}
	private void GotoTopFloor()
	{
		if (parameterT < 1)
		{			
			elevatorTransform.position = CalculateLocationWithEasing(parameterT, bezierCurveLocations[0], bezierCurveLocations[3]);
			
			// This should reach 1 in 1 second if the increase rate is 1
			parameterT += Time.fixedDeltaTime * parameterTIncreaseRate;
		}
		else
		{
			parameterT = 0.0f;
			ElevatorUpdate = LowerRail;
		}
	}
	private void LowerRail()
	{
		newRailPosition.z -= 0.2f * Time.fixedDeltaTime;
		
		if (newRailPosition.z < 0f)
		{
			newRailPosition.z = 0f;
			ElevatorUpdate = OpenLargeDoors;
		}
		
		elevatorRail.localPosition = newRailPosition; 
	}
	private Vector3 finalDoorLeftPosition;
	private Vector3 finalDoorRightPosition;
	private void OpenLargeDoors()
	{
		if (parameterT < 30)
		{
			finalDoorLeftPosition.x -= 0.5f * 0.4f * Time.fixedDeltaTime;
			finalDoorRightPosition.x += 0.5f * 0.4f * Time.fixedDeltaTime;

			parameterT += Time.fixedDeltaTime;
			
			finalDoorLeft.position = finalDoorLeftPosition;
			finalDoorRight.position = finalDoorRightPosition;
		}
		else
		{
			parameterT = 0f;
			ElevatorUpdate = FinishElevator;
		}
	}
	private void FinishElevator()
	{
		tankCameraSnow.parent = null;
		
		elevatorScript.enabled = false;
	}

	public void StopDoors()
	{
		parameterT = 0f;
		ElevatorUpdate = FinishElevator;

		tankCameraSnow.parent = null;
		
		elevatorScript.enabled = false;
	}

	private const float BEZIER_INTERPOLATION_CONSTANT = 0.125f;

	private Vector3[] bezierCurveLocations = new Vector3[4];
	private void SetupBezierCurve()
	{
		parameterTVector = new Vector4(0, 0, 0, 1);

		if (GotoFirstFloor == NextElevatorUpdate)
		{
			bezierCurveLocations[0] = elevatorOriginalPosition;
			bezierCurveLocations[3] = elevatorLevel1Position;
		}
		else if (GotoSecondFloor == NextElevatorUpdate)
		{
			bezierCurveLocations[0] = elevatorLevel1Position;
			bezierCurveLocations[3] = elevatorLevel2Position;
		}
		else if (GotoTopFloor == NextElevatorUpdate)
		{
			bezierCurveLocations[0] = elevatorLevel2Position;
			bezierCurveLocations[3] = elevatorTopLevelPosition;
		}

		bezierCurveLocations[1] = new Vector3(BEZIER_INTERPOLATION_CONSTANT * (bezierCurveLocations[0].x) + (1 - BEZIER_INTERPOLATION_CONSTANT) * (bezierCurveLocations[3].x),
		                                      BEZIER_INTERPOLATION_CONSTANT * (bezierCurveLocations[0].y) + (1 - BEZIER_INTERPOLATION_CONSTANT) * (bezierCurveLocations[3].y),
		                                      BEZIER_INTERPOLATION_CONSTANT * (bezierCurveLocations[0].z) + (1 - BEZIER_INTERPOLATION_CONSTANT) * (bezierCurveLocations[3].z));

		bezierCurveLocations[2] = new Vector3(BEZIER_INTERPOLATION_CONSTANT * (bezierCurveLocations[0].x) + (1 - BEZIER_INTERPOLATION_CONSTANT) * (bezierCurveLocations[3].x),
		                                      BEZIER_INTERPOLATION_CONSTANT * (bezierCurveLocations[0].y) + (1 - BEZIER_INTERPOLATION_CONSTANT) * (bezierCurveLocations[3].y),
		                                      BEZIER_INTERPOLATION_CONSTANT * (bezierCurveLocations[0].z) + (1 - BEZIER_INTERPOLATION_CONSTANT) * (bezierCurveLocations[3].z));

		//Debug.Log(bezierCurveLocations[0] + " | " + bezierCurveLocations[1] + " | " + bezierCurveLocations[2] + " | " + bezierCurveLocations[3]);
	}

	private Vector3 CalculateBezierLocation(Vector4 parameterTVector, Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3)
	{
		Vector3 finalLocation = new Vector3();

		Vector4 vectorTM = new Vector4(parameterTVector.x * bezierWeightsMatrix.m00 + parameterTVector.y * bezierWeightsMatrix.m10 + parameterTVector.z * bezierWeightsMatrix.m20 + parameterTVector.w * bezierWeightsMatrix.m30,
		                               parameterTVector.x * bezierWeightsMatrix.m01 + parameterTVector.y * bezierWeightsMatrix.m11 + parameterTVector.z * bezierWeightsMatrix.m21,
		                               parameterTVector.x * bezierWeightsMatrix.m02 + parameterTVector.y * bezierWeightsMatrix.m12,
		                               parameterTVector.x * bezierWeightsMatrix.m03);

		finalLocation.x = vectorTM.x * P0.x + vectorTM.y * P1.x + vectorTM.z * P2.x + vectorTM.w * P3.x;
		finalLocation.y = vectorTM.x * P0.y + vectorTM.y * P1.y + vectorTM.z * P2.y + vectorTM.w * P3.y;
		finalLocation.z = vectorTM.x * P0.z + vectorTM.y * P1.z + vectorTM.z * P2.z + vectorTM.w * P3.z;

		return finalLocation;
	}

	private Vector3 CalculateLocationWithEasing(float parameterT, Vector3 pointStart, Vector3 pointEnd)
	{
		float tAfterEasing = 0.5f * Mathf.Sin((Mathf.PI * (parameterT - 0.5f))) + 0.5f;

		return (1 - tAfterEasing) * pointStart + tAfterEasing * pointEnd;
	}
}
