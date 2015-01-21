using UnityEngine;
using System.Collections.Generic;
public class GameGUI : MonoBehaviour, Pauseable {
	//gui classic
	private float dotUpdateDelta;
	public Texture2D secondaryIcon;
	public Texture2D secondaryIconGreyed;
	private float timeSinceLastCheckpoint;
	public float checkpointDisplayTimeout;
	public bool debug;
	private CameraScript theCamera;
	CheckpointBehaviour currentCheckpoint;
	public Texture2D radarBackgroundTexture, radarBackgroundPingTexture;
	public float NEARRADARDISTANCE;
	/// <summary>
	/// This depends on the graphical radius of the radar circle graphic
	/// </summary>
	public float RADARGRAPHICALDISTANCE;
	public float FARRADARGRAPHICALRADIUS;
	public float radarMaxHeightDifference;
	private List<RadarObject> radar;
	
	//new gui objects
	public GameObject inGamePanel, pausePanel, debugPanel;
	public RectTransform healthMask, shieldMask;
	public RectTransform[] primaryCannonUpdateDots, secondaryCannonUpdateDots;
	public RectTransform grenadeMask, rocketMask;
	public UnityEngine.UI.RawImage[] grenadeAmmunitionCounts, rocketsAmmunitionCounts;
	public UnityEngine.UI.Text secondaryDisplayText, debugSpeed, debugWeaponRaycastDistance, checkpointText;
	public UnityEngine.UI.Image radarBackground, radarEnemyBlip, radarEnemyFarBlip, radarEnemyDifferentHeightBlip, checkpointBlip, farCheckpointBlip;
	public struct RadarObject
	{
		public Transform objectTransform;
		public UnityEngine.UI.Image image;
		public OBJECTTYPE type;
		public enum OBJECTTYPE{ENEMY, CHECKPOINT};
		public string name;
		public RadarObject(Transform o, OBJECTTYPE t, string n, UnityEngine.UI.Image inImage)
		{
			objectTransform = o;
			type = t;
			name = n;
			image = inImage;
		}
	}
	void Awake() {
		radar = new List<RadarObject>();
	}
	void Start () {
		theCamera = GetComponent<CameraScript>();
		timeSinceLastCheckpoint = checkpointDisplayTimeout;
		dotUpdateDelta = Util.player.primaryCannonReloadTime/4;
		OnUnPause();
		if(debug){
			debugPanel.gameObject.SetActive(true);
		}
		else{
			debugPanel.gameObject.SetActive(false);
		}
	}
	/// <summary>
	/// For setting up radar blip images
	/// </summary>
	/// <param name="inImage">In image.</param>
	void SetImageUp(UnityEngine.UI.Image inImage){
		inImage.rectTransform.SetParent(radarBackground.rectTransform, false);
		inImage.rectTransform.localScale = new Vector3(1, 1, 1);
		inImage.rectTransform.localRotation = Quaternion.identity;
	}
	void OnGUI(){
		if(inGamePanel.activeSelf){
			//debug check and display
			if(Input.GetKeyDown("o")){
				debug = !debug;
				debugPanel.gameObject.SetActive(debug);
			}
			if(debug){
				debugSpeed.text = Util.player.rigidbody.velocity.magnitude.ToString();
				debugWeaponRaycastDistance.text = Util.mainCamera.distanceToTarget.ToString();
			}
			//shield, health, and other masking images
			shieldMask.offsetMax = new Vector2(-(1 - Util.player.shieldPct / 100) * 360 - 20, shieldMask.offsetMax.y);
			healthMask.offsetMax = new Vector2(-(1 - Util.player.health / Util.player.maxHealth) * 360 - 20, healthMask.offsetMax.y);
			if(!Util.player.playerWeaps[0].IsFullyLoaded())
			{
				grenadeMask.offsetMax = new Vector2(-(1 - Util.player.playerWeaps[0].cannonReloadTimers / Util.player.playerWeaps[0].cannonReloadTime) * 190 - 5, grenadeMask.offsetMax.y);
			}
			if(!Util.player.playerWeaps[2].IsFullyLoaded())
			{
				rocketMask.offsetMax = new Vector2(-(1 - Util.player.playerWeaps[2].cannonReloadTimers / Util.player.playerWeaps[2].cannonReloadTime) * 190 - 5, rocketMask.offsetMax.y);
			}
			//displays around cursor
			if (Util.player.primaryCannonTimer > dotUpdateDelta && Util.player.primaryCannonTimer < 4*dotUpdateDelta)
			{
				primaryCannonUpdateDots[0].gameObject.SetActive(true);
				if(Util.player.primaryCannonTimer > 2*dotUpdateDelta)
				{
					primaryCannonUpdateDots[1].gameObject.SetActive(true);
				}
				if(Util.player.primaryCannonTimer > 3*dotUpdateDelta)
				{
					primaryCannonUpdateDots[2].gameObject.SetActive(true);
				}
			}
			else{
				foreach(RectTransform p in primaryCannonUpdateDots){
					p.gameObject.SetActive(false);
				}
			}
			if(Util.player.playerWeaps[Util.player.currentSecondaryWep].HasBullet() && Util.player.playerWeaps[Util.player.currentSecondaryWep].autoFireTime > .15f)
			{
				for(int i = 0; i < 6; i++)
				{
					if(i < Util.player.playerWeaps[Util.player.currentSecondaryWep].bulletsLeft){
						secondaryCannonUpdateDots[i].gameObject.SetActive(true);
					}
					else{
						secondaryCannonUpdateDots[i].gameObject.SetActive(false);
					}
				}
			}
			else{
				foreach(RectTransform p in secondaryCannonUpdateDots){
					p.gameObject.SetActive(false);
				}
			}
			//grenade and rocket ammo displays
			for(int i = 0; i < grenadeAmmunitionCounts.Length; i++){
				if(i < Util.player.playerWeaps[0].bulletsLeft){
					grenadeAmmunitionCounts[i].texture = secondaryIcon;
				}
				else{
					grenadeAmmunitionCounts[i].texture = secondaryIconGreyed;
				}
			}
			for(int i = 0; i < rocketsAmmunitionCounts.Length; i++){
				if(i < Util.player.playerWeaps[2].bulletsLeft){
					rocketsAmmunitionCounts[i].texture = secondaryIcon;
				}
				else{
					rocketsAmmunitionCounts[i].texture = secondaryIconGreyed;
				}
			}
			//current secondary text grab and display
			secondaryDisplayText.text = Util.player.playerWeaps[Util.player.currentSecondaryWep].bullet.prettyName;
			//Checkpoint displaying
			if(timeSinceLastCheckpoint < checkpointDisplayTimeout)
			{
				checkpointText.gameObject.SetActive(true);
			}
			else{
				checkpointText.gameObject.SetActive(false);
			}
			//radar stuff
			//camera vector projected onto ground plane
			Vector3 xzCamera = new Vector3(Util.mainCamera.transform.forward.x, 0, Util.mainCamera.transform.forward.z);
			for(int i = 0; i < radar.Count; i++)
			{
				if(radar[i].objectTransform != null)
				{
					if(radar[i].type == RadarObject.OBJECTTYPE.ENEMY)
					{
						//vector to the object we wish to display
						Vector3 vectorToRO = radar[i].objectTransform.position - Util.player.transform.position;
						//and also projecting it on the ground plane
						Vector3 xzVectorToRO = new Vector3(vectorToRO.x, 0, vectorToRO.z);
						//rotates the vector to have camera vector as its' forward direction, i.e. from xz coordinates to x(camera)z(camera) coordinates
						xzVectorToRO = Quaternion.Inverse(Quaternion.LookRotation(xzCamera)) * xzVectorToRO;
						if(xzVectorToRO.magnitude < NEARRADARDISTANCE)
						{
							//display normal sized if height difference is less than radarMax
							if(Mathf.Abs(vectorToRO.y) < radarMaxHeightDifference)
							{
								//bring the radar blip in a little graphically
								xzVectorToRO *= RADARGRAPHICALDISTANCE/NEARRADARDISTANCE;
								//if the radar object does not have the right image, destroy the old, instantiate and set up the right one
								if(!radar[i].image.name.Contains(radarEnemyBlip.name)){
									Destroy(radar[i].image);
									radar[i] = new RadarObject(radar[i].objectTransform, radar[i].type, radar[i].name, Instantiate(radarEnemyBlip) as UnityEngine.UI.Image);
									SetImageUp(radar[i].image);
								}
								//finally, set the position of the blip relative to parent radar background
								radar[i].image.rectTransform.anchoredPosition3D = new Vector3(xzVectorToRO.x, xzVectorToRO.z, 0);
							}
							//otherwise display smaller graphic
							else
							{
								xzVectorToRO *= RADARGRAPHICALDISTANCE/NEARRADARDISTANCE;
								if(!radar[i].image.name.Contains(radarEnemyDifferentHeightBlip.name)){
									Destroy(radar[i].image);
									radar[i] = new RadarObject(radar[i].objectTransform, radar[i].type, radar[i].name, Instantiate(radarEnemyDifferentHeightBlip) as UnityEngine.UI.Image);
									SetImageUp(radar[i].image);
								}
								radar[i].image.rectTransform.anchoredPosition3D = new Vector3(xzVectorToRO.x, xzVectorToRO.z, 0);
							}
						}
						//when a blip is outside the radar's range, we show a flattened graphic
						//this graphic must be oriented to look right on the edge of the radar disc i.e. major axis is tangential to radar circle
						else{
							//adjust vector to radar object to lie roughly on outer edge of graphic
							xzVectorToRO = xzVectorToRO.normalized * FARRADARGRAPHICALRADIUS;
							if(!radar[i].image.name.Contains(radarEnemyFarBlip.name)){
								Destroy(radar[i].image);
								radar[i] = new RadarObject(radar[i].objectTransform, radar[i].type, radar[i].name, Instantiate(radarEnemyFarBlip) as UnityEngine.UI.Image);
								SetImageUp(radar[i].image);
							}
							//rotate around z axis
							if(xzVectorToRO.x >=0){
								radar[i].image.rectTransform.localRotation = Quaternion.AngleAxis(Vector3.Angle(xzVectorToRO, -Vector3.forward), Vector3.forward);
								radar[i].image.rectTransform.localEulerAngles = new Vector3(0, 0, radar[i].image.rectTransform.localEulerAngles.z);
							}
							else{
								radar[i].image.rectTransform.localRotation = Quaternion.AngleAxis(Vector3.Angle(xzVectorToRO, Vector3.forward), Vector3.forward);
								radar[i].image.rectTransform.localEulerAngles = new Vector3(0, 0, radar[i].image.rectTransform.localEulerAngles.z);
							}
							radar[i].image.rectTransform.anchoredPosition3D = new Vector3(xzVectorToRO.x, xzVectorToRO.z, 0);
						}
					}
					else if(radar[i].type == RadarObject.OBJECTTYPE.CHECKPOINT)
					{
						Vector3 vectorToRO = radar[i].objectTransform.position - Util.player.transform.position;
						Vector3 xzVectorToRO = new Vector3(vectorToRO.x, 0, vectorToRO.z);
						xzVectorToRO = Quaternion.Inverse(Quaternion.LookRotation(xzCamera)) * xzVectorToRO;
						if(xzVectorToRO.magnitude < NEARRADARDISTANCE)
						{
							xzVectorToRO *= RADARGRAPHICALDISTANCE/NEARRADARDISTANCE;
							if(!radar[i].image.name.Contains(checkpointBlip.name)){
								Destroy(radar[i].image);
								radar[i] = new RadarObject(radar[i].objectTransform, radar[i].type, radar[i].name, Instantiate(checkpointBlip) as UnityEngine.UI.Image);
								SetImageUp(radar[i].image);
							}
							radar[i].image.rectTransform.anchoredPosition3D = new Vector3(xzVectorToRO.x, xzVectorToRO.z, 0);
						}
						else
						{
							xzVectorToRO = xzVectorToRO.normalized * FARRADARGRAPHICALRADIUS;
							if(!radar[i].image.name.Contains(farCheckpointBlip.name)){
								Destroy(radar[i].image);
								radar[i] = new RadarObject(radar[i].objectTransform, radar[i].type, radar[i].name, Instantiate(farCheckpointBlip) as UnityEngine.UI.Image);
								SetImageUp(radar[i].image);
							}
							//account for x-axis
							if(xzVectorToRO.x >=0){
								radar[i].image.rectTransform.localRotation = Quaternion.AngleAxis(Vector3.Angle(xzVectorToRO, -Vector3.forward), Vector3.forward);
								radar[i].image.rectTransform.localEulerAngles = new Vector3(0, 0, radar[i].image.rectTransform.localEulerAngles.z);
							}
							else{
								radar[i].image.rectTransform.localRotation = Quaternion.AngleAxis(Vector3.Angle(xzVectorToRO, Vector3.forward), Vector3.forward);
								radar[i].image.rectTransform.localEulerAngles = new Vector3(0, 0, radar[i].image.rectTransform.localEulerAngles.z);
							}
							radar[i].image.rectTransform.anchoredPosition3D = new Vector3(xzVectorToRO.x, xzVectorToRO.z, 0);
						}
					}
					else
					{
						Debug.Log("Error with Game Radar");
					}
				}
			}
		}
	}
	void Update(){
		timeSinceLastCheckpoint += Time.deltaTime;
	}
	/*void InGameGUI()
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
		
		DrawRadar();
	}*/
	/*
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
		if(GUI.Button(new Rect(0, 180, 224, 48), "Go to Menu", currentStyle))
		{
			Util.UnPause();
			Application.LoadLevel("L00_StartScreen");
		}
		GUI.EndGroup();
	}*/
	public void AddRadarObject(Transform theObject, RadarObject.OBJECTTYPE type)
	{
		foreach(RadarObject r in radar)
		{
			if(r.objectTransform == theObject)
			{
				return;
			}
		}
		RadarObject ro = new RadarObject(theObject, type, theObject.name, (type == RadarObject.OBJECTTYPE.CHECKPOINT?Instantiate(checkpointBlip) as UnityEngine.UI.Image:Instantiate(radarEnemyBlip) as UnityEngine.UI.Image));
		SetImageUp(ro.image);
		radar.Add(ro);
	}
	public void RemoveRadarObject(Transform theObject)
	{
		List<RadarObject> objToRemove = new List<RadarObject>();
		foreach(RadarObject r in radar)
		{
			if(r.objectTransform == theObject)
			{
				objToRemove.Add(r);
			}
		}
		foreach(RadarObject r in objToRemove)
		{
			radar.Remove(r);
			Destroy(r.image);
		}
	}
	/*private void DrawRadar()
	{
		GUI.BeginGroup(new Rect(0, Screen.height - radarBackgroundTexture.height, radarBackgroundTexture.width, radarBackgroundTexture.height));
		GUI.Box(new Rect(0, 0, radarBackgroundTexture.width, radarBackgroundTexture.height), radarBackgroundTexture, currentStyle);
		Vector3 xzCamera = new Vector3(Util.mainCamera.transform.forward.x, 0, Util.mainCamera.transform.forward.z);
		foreach(RadarObject r in radar)
		{
			if(r.objectTransform != null)
			{
				if(r.type == RadarObject.OBJECTTYPE.ENEMY)
				{
					Vector3 vectorToRO = r.objectTransform.position - Util.player.transform.position;
					Vector3 xzVectorToRO = new Vector3(vectorToRO.x, 0, vectorToRO.z);
					xzVectorToRO = Quaternion.Inverse(Quaternion.LookRotation(xzCamera)) * xzVectorToRO;
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
						Matrix4x4 backup = GUI.matrix;
						xzVectorToRO = xzVectorToRO.normalized * FARRADARGRAPHICALRADIUS;
						if(xzVectorToRO.x >=0)
						{
							GUIUtility.RotateAroundPivot(Vector3.Angle(xzVectorToRO, Vector3.forward), new Vector2(xzVectorToRO.x + radarBackgroundTexture.width/2, radarBackgroundTexture.height/2 - xzVectorToRO.z));
						}
						else
						{
							GUIUtility.RotateAroundPivot(Vector3.Angle(xzVectorToRO, -Vector3.forward), new Vector2(xzVectorToRO.x + radarBackgroundTexture.width/2, radarBackgroundTexture.height/2 - xzVectorToRO.z));
						}
						GUI.Box(new Rect(xzVectorToRO.x + radarBackgroundTexture.width/2 - farEnemyBlip.width/2, -xzVectorToRO.z + radarBackgroundTexture.height/2 - farEnemyBlip.height/2,
						 farEnemyBlip.width, farEnemyBlip.height), farEnemyBlip, currentStyle);
						GUI.matrix = backup;
					}
				}
				else if(r.type == RadarObject.OBJECTTYPE.CHECKPOINT)
				{
					Vector3 vectorToRO = r.objectTransform.position - Util.player.transform.position;
					Vector3 xzVectorToRO = new Vector3(vectorToRO.x, 0, vectorToRO.z);
					xzVectorToRO = Quaternion.Inverse(Quaternion.LookRotation(xzCamera)) * xzVectorToRO;
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
							GUI.Box(new Rect(xzVectorToRO.x + radarBackgroundTexture.width/2 - nearCheckpointBlip.width/2, -xzVectorToRO.z + radarBackgroundTexture.height/2 - nearCheckpointBlip.height/2,
						    	nearCheckpointBlip.width, nearCheckpointBlip.height), nearCheckpointBlip, currentStyle);
						}
					}
					else
					{
						Matrix4x4 backup = GUI.matrix;
						xzVectorToRO = xzVectorToRO.normalized * FARRADARGRAPHICALRADIUS;
						if(xzVectorToRO.x >=0)
						{
							GUIUtility.RotateAroundPivot(Vector3.Angle(xzVectorToRO, Vector3.forward), new Vector2(xzVectorToRO.x + radarBackgroundTexture.width/2, radarBackgroundTexture.height/2 - xzVectorToRO.z));
						}
						else
						{
							GUIUtility.RotateAroundPivot(Vector3.Angle(xzVectorToRO, -Vector3.forward), new Vector2(xzVectorToRO.x + radarBackgroundTexture.width/2, radarBackgroundTexture.height/2 - xzVectorToRO.z));
						}
						GUI.Box(new Rect(xzVectorToRO.x + radarBackgroundTexture.width/2 - farCheckpointBlip.width/2, -xzVectorToRO.z + radarBackgroundTexture.height/2 - farCheckpointBlip.height/2,
						 farCheckpointBlip.width, farCheckpointBlip.height), farCheckpointBlip, currentStyle);
						GUI.matrix = backup;
					}
				}
				else
				{
					Debug.Log("Error with Game Radar");
				}
			}
		}
		GUI.EndGroup();
	}*/
	/*
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
	*/

	public void SetNextCheckpoint (CheckpointBehaviour nextCheckpoint)
	{
		if(nextCheckpoint != null)
		{
			if(currentCheckpoint != null)
			{
				RemoveRadarObject(currentCheckpoint.transform);
			}
			currentCheckpoint = nextCheckpoint;
			AddRadarObject(currentCheckpoint.transform, RadarObject.OBJECTTYPE.CHECKPOINT);
		}
		timeSinceLastCheckpoint = 0;
	}

	#region Pauseable implementation


	public void OnPause ()
	{
		inGamePanel.SetActive(false);
		pausePanel.SetActive(true);
		Screen.lockCursor = false;
	}


	public void OnUnPause ()
	{
		inGamePanel.SetActive(true);
		pausePanel.SetActive(false);
		Screen.lockCursor = true;
	}


	#endregion

}
