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

	public void DeActivate ()
	{
		inGamePanel.SetActive(false);
		pausePanel.SetActive(false);
		debugPanel.SetActive(false);
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
