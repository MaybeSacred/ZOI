using UnityEngine;
using System.Collections.Generic;
public class GameGUI : MonoBehaviour {
	public GUIStyle currentStyle;
	public Vector2 primaryWepStartPoint;
	private float dotUpdateDelta;
	public Texture2D fireUpdateDot;
	public Texture2D secondaryIcon;
	public Texture2D secondaryIconGreyed;
	public Texture2D shieldBar;
	public Texture2D healthBar;
	public Texture2D cursorIcon;
	public Texture2D rechargingStrip;
	private float startSecondaryReloadBarScaleX;
	private float timeSinceLastCheckpoint;
	public float checkpointDisplayTimeout;
	public bool debug;
	public GUIContent decrementMouseSensitivityButton;
	public GUIContent incrementMouseSensitivityButton;
	public GUIStyle decrementMouseSensitivityButtonStyle;
	public GUIStyle incrementMouseSensitivityButtonStyle;
	private CameraScript theCamera;
	public Texture2D radarBackgroundTexture, radarBackgroundPingTexture;
	public Texture2D nearEnemyBlipSameHeight, nearEnemyBlipDifferentHeight, farEnemyBlip,
		nearCheckpointBlip, farCheckpointBlip;
	public float NEARRADARDISTANCE;
	/// <summary>
	/// This depends on the graphical radius of the radar circle graphic
	/// </summary>
	public float RADARGRAPHICALDISTANCE;
	public float FARRADARGRAPHICALRADIUS;
	public float radarMaxHeightDifference;
	private List<RadarObject> radar;
	public struct RadarObject
	{
		public Transform objectTransform;
		public OBJECTTYPE type;
		public enum OBJECTTYPE{ENEMY, CHECKPOINT};
		public RadarObject(Transform o, OBJECTTYPE t)
		{
			objectTransform = o;
			type = t;
		}
	}
	void Start () {
		radar = new List<RadarObject>();
		theCamera = GetComponent<CameraScript>();
		timeSinceLastCheckpoint = checkpointDisplayTimeout;
		dotUpdateDelta = Util.player.primaryCannonReloadTime/4;
	}
	void OnGUI()
	{
		if(Util.isPaused)
		{
			PauseScreen();
		}
		else
		{
			InGameGUI();
			DebugStats();
		}
	}
	void Update () {
		if(Input.GetKeyDown("escape"))
		{
			Util.FlipPausedState();
		}
	}
	void InGameGUI()
	{
		GUI.BeginGroup(new Rect(0, 0, 256, 68));
		GUI.Box(new Rect(0, 0, 256, 68), GUIContent.none);
		GUI.BeginGroup(new Rect(8, 3, Util.player.shieldPct*2.4f, 32));
		GUI.Box(new Rect(0, 0, 240, 32), shieldBar, currentStyle);
		GUI.EndGroup();
		GUI.BeginGroup(new Rect(8, 36, Util.player.health/Util.player.maxHealth*240, 32));
		GUI.Box(new Rect(0, 0, 240, 32), healthBar, currentStyle);
		GUI.EndGroup();
		GUI.EndGroup();
		GUI.Box(new Rect(Screen.width/2-cursorIcon.width/2, Screen.height/2-cursorIcon.height/2, cursorIcon.width, cursorIcon.height), cursorIcon, currentStyle);
		if (Util.player.primaryCannonTimer > dotUpdateDelta && Util.player.primaryCannonTimer < 4*dotUpdateDelta)
		{
			GUILayout.BeginArea(new Rect(Screen.width/2 - primaryWepStartPoint.x, Screen.height/2 - primaryWepStartPoint.y, 16, 48));
			GUILayout.Box(fireUpdateDot, currentStyle);
			if(Util.player.primaryCannonTimer > 2*dotUpdateDelta)
			{
				GUILayout.Box(fireUpdateDot, currentStyle);
			}
			if(Util.player.primaryCannonTimer > 3*dotUpdateDelta)
			{
				GUILayout.Box(fireUpdateDot, currentStyle);
			}
			GUILayout.EndArea();
		}
		if (Util.player.playerWeaps[Util.player.currentSecondaryWep].possibleSecondaries.name.Equals("PlayerGrenade"))
		{
			if(Util.player.playerWeaps[Util.player.currentSecondaryWep].HasBullet())
			{
				GUILayout.BeginArea(new Rect(Screen.width/2 + primaryWepStartPoint.x-16, Screen.height/2 - primaryWepStartPoint.y, 16, 48));
				for(int i = 0; i < Util.player.playerWeaps[Util.player.currentSecondaryWep].secondaryBulletsLeft; i++)
				{
					GUILayout.Box(secondaryIcon, currentStyle);
				}
				GUILayout.EndArea();
			}
		}
		else if (Util.player.playerWeaps[Util.player.currentSecondaryWep].possibleSecondaries.name.Equals("PlayerRocket"))
		{
			if(Util.player.playerWeaps[Util.player.currentSecondaryWep].secondaryBulletsLeft > 0)
			{
				GUILayout.BeginArea(new Rect(Screen.width/2 + primaryWepStartPoint.x-16, Screen.height/2 - primaryWepStartPoint.y, 16, 64));
				for(int i = 0; i < Util.player.playerWeaps[Util.player.currentSecondaryWep].secondaryBulletsLeft/2; i++)
				{
					GUILayout.Box(secondaryIcon, currentStyle);
				}
				GUILayout.EndArea();
				if(Util.player.playerWeaps[Util.player.currentSecondaryWep].secondaryBulletsLeft > Util.player.playerWeaps[Util.player.currentSecondaryWep].secondaryBulletsLeft/2)
				{
					GUILayout.BeginArea(new Rect(Screen.width/2 + primaryWepStartPoint.x, Screen.height/2 - primaryWepStartPoint.y, 16, 64));
					for(int i = Util.player.playerWeaps[Util.player.currentSecondaryWep].secondaryBulletsLeft/2; i < Util.player.playerWeaps[Util.player.currentSecondaryWep].secondaryBulletsLeft; i++)
					{
						GUILayout.Box(secondaryIcon, currentStyle);
					}
					GUILayout.EndArea();
				}
			}
		}
		GUI.Box(new Rect(Screen.width - 350, 0, 350, 30), GUIContent.none);
		GUI.Label(new Rect(Screen.width - 350, 0, 350, 30), Util.player.playerWeaps[Util.player.currentSecondaryWep].possibleSecondaries.prettyName, currentStyle);
		GUILayout.BeginArea(new Rect(Screen.width - 333, 32, 333, 22));
		GUILayout.BeginArea(new Rect(0, 0, 64, 22));
		GUILayout.BeginHorizontal();
		for(int i = 0; i < Util.player.playerWeaps[0].totalSecondaryBullets; i++)
		{
			if(i < Util.player.playerWeaps[0].secondaryBulletsLeft)
			{
				GUILayout.Box(secondaryIcon, currentStyle);
			}
			else
			{
				GUILayout.Box(secondaryIconGreyed, currentStyle);
			}
		}
		GUILayout.EndHorizontal();
		if(!Util.player.playerWeaps[0].IsFullyLoaded())
		{
			GUI.BeginGroup(new Rect(0, 16, Util.player.playerWeaps[0].secondaryCannonReloadTimers/Util.player.playerWeaps[0].secondaryCannonReloadTime*64, 6));
			GUI.Label(new Rect(0, 0, 64, 6), rechargingStrip, currentStyle);
			GUI.EndGroup();
		}
		GUILayout.EndArea();
		GUILayout.BeginArea(new Rect(222, 0, 96, 22));
		GUILayout.BeginHorizontal();
		for(int i = 0; i < Util.player.playerWeaps[2].totalSecondaryBullets; i++)
		{
			if(i < Util.player.playerWeaps[2].secondaryBulletsLeft)
			{
				GUILayout.Box(secondaryIcon, currentStyle);
			}
			else
			{
				GUILayout.Box(secondaryIconGreyed, currentStyle);
			}
		}
		GUILayout.EndHorizontal();
		if(!Util.player.playerWeaps[2].IsFullyLoaded())
		{
			GUI.BeginGroup(new Rect(0, 16, Util.player.playerWeaps[2].secondaryCannonReloadTimers/Util.player.playerWeaps[2].secondaryCannonReloadTime*64, 6));
			GUI.Label(new Rect(0, 0, 64, 6), rechargingStrip, currentStyle);
			GUI.EndGroup();
		}
		GUILayout.EndArea();
		GUILayout.EndArea();
		if(timeSinceLastCheckpoint < checkpointDisplayTimeout)
		{
			timeSinceLastCheckpoint += Time.deltaTime;
			GUI.Box(new Rect(Screen.width/2-125, 0, 250, 30), GUIContent.none);
			GUI.Label(new Rect(Screen.width/2-125, 0, 250, 30), "Checkpoint Reached...", currentStyle);
		}
		DrawRadar();
	}
	void PauseScreen()
	{
		GUI.BeginGroup(new Rect(Screen.width/2 - 112, Screen.height/2 - 128, 224, 256));
		GUI.Box(new Rect(0, 0, 224, 256), GUIContent.none);
		GUI.TextArea(new Rect(0, 0, 224, 32), "Options", currentStyle);
		GUI.TextArea(new Rect(0, 32, 224, 32), "Mouse Sensitivity", currentStyle);
		GUI.BeginGroup(new Rect(0, 64, 224, 48));
		if(GUI.Button(new Rect(0, 0, 48, 48), "", decrementMouseSensitivityButtonStyle))
		{
			theCamera.DecrementMouseSensitivity();
		}
		if(GUI.Button(new Rect(176, 0, 48, 48), "", incrementMouseSensitivityButtonStyle))
		{
			theCamera.IncrementMouseSensitivity();
		}
		GUI.TextArea(new Rect(0, 0, 224, 48), theCamera.GetCurrentMouseSensitivity().ToString(), currentStyle);
		GUI.EndGroup();
		GUI.EndGroup();
	}
	public void AddRadarObject(Transform theObject, RadarObject.OBJECTTYPE type)
	{
		foreach(RadarObject r in radar)
		{
			if(r.objectTransform == theObject)
			{
				return;
			}
		}
		radar.Add(new RadarObject(theObject, type));
	}
	public void RemoveRadarObject(Transform theObject)
	{
		RadarObject objToRemove = new RadarObject(null, RadarObject.OBJECTTYPE.CHECKPOINT);
		foreach(RadarObject r in radar)
		{
			if(r.objectTransform == theObject)
			{
				objToRemove = r;
			}
		}
		if(objToRemove.objectTransform != null)
		{
			radar.Remove(objToRemove);
		}
	}
	private void DrawRadar()
	{
		GUI.BeginGroup(new Rect(0, Screen.height - radarBackgroundTexture.height, radarBackgroundTexture.width, radarBackgroundTexture.height));
		GUI.Box(new Rect(0, 0, radarBackgroundTexture.width, radarBackgroundTexture.height), radarBackgroundTexture, currentStyle);
		Vector3 xzCamera = new Vector3(Util.mainCamera.transform.forward.x, 0, Util.mainCamera.transform.forward.z);
		foreach(RadarObject r in radar)
		{
			if(r.type == RadarObject.OBJECTTYPE.ENEMY)
			{
				Vector3 vectorToRO = r.objectTransform.position - Util.player.transform.position;
				Vector3 xzVectorToRO = new Vector3(vectorToRO.x, 0, vectorToRO.z);
				float angle = Vector3.Angle(Vector3.forward, xzCamera);
				xzVectorToRO = Quaternion.Inverse(Quaternion.LookRotation(xzCamera)) * xzVectorToRO;// Vector3.RotateTowards(xzVectorToRO, xzCamera, angle, 0);
				if(xzVectorToRO.magnitude < NEARRADARDISTANCE)
				{
					if(Mathf.Abs(vectorToRO.y) > radarMaxHeightDifference)
					{
						xzVectorToRO *= RADARGRAPHICALDISTANCE/NEARRADARDISTANCE;
						GUI.Box(new Rect(xzVectorToRO.x + radarBackgroundTexture.width/2 - nearEnemyBlipDifferentHeight.width/2, -xzVectorToRO.z + radarBackgroundTexture.height/2 - nearEnemyBlipDifferentHeight.height/2,
						                 nearEnemyBlipDifferentHeight.width, nearEnemyBlipDifferentHeight.height), nearEnemyBlipDifferentHeight, currentStyle);
					}
					else
					{
						xzVectorToRO *= RADARGRAPHICALDISTANCE/NEARRADARDISTANCE;
						GUI.Box(new Rect(xzVectorToRO.x + radarBackgroundTexture.width/2 - nearEnemyBlipSameHeight.width/2, -xzVectorToRO.z + radarBackgroundTexture.height/2 - nearEnemyBlipSameHeight.height/2,
					    	nearEnemyBlipSameHeight.width, nearEnemyBlipSameHeight.height), nearEnemyBlipSameHeight, currentStyle);
					}
				}
				else
				{
					xzVectorToRO = xzVectorToRO.normalized * FARRADARGRAPHICALRADIUS;
					GUI.Box(new Rect(xzVectorToRO.x + radarBackgroundTexture.width/2 - farEnemyBlip.width/2, -xzVectorToRO.z + radarBackgroundTexture.height/2 - farEnemyBlip.height/2,
					 farEnemyBlip.width, farEnemyBlip.height), farEnemyBlip, currentStyle);
				}
			}
			else if(r.type == RadarObject.OBJECTTYPE.CHECKPOINT)
			{
				
			}
			else
			{
				Debug.Log("WTF Mate?");
			}
		}
		GUI.EndGroup();
	}
	public void CheckpointReached()
	{
		timeSinceLastCheckpoint = 0;
	}
	private void DebugStats()
	{
		if(debug)
		{
			GUILayout.BeginArea(new Rect(0, Screen.height-425, 300, 100));
			GUILayout.Label(Util.player.rigidbody.velocity.magnitude.ToString());
			GUILayout.Label(Util.mainCamera.distanceToTarget.ToString());
			GUILayout.EndArea();
		}
	}
}
