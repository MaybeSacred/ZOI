using UnityEngine;
using System.Collections;
public class GUI : MonoBehaviour {
	public GUIStyle currentStyle;
	public Vector2 primaryWepStartPoint;
	private float dotUpdateDelta;
	public Texture2D fireUpdateDot;
	public Texture2D secondaryIcon;
	public Texture2D shieldBar;
	public Texture2D healthBar;
	public Texture2D cursorIcon;
	private float startSecondaryReloadBarScaleX;
	private float timeSinceLastCheckpoint;
	public float checkpointDisplayTimeout;
	void Start () {
		timeSinceLastCheckpoint = checkpointDisplayTimeout;
		dotUpdateDelta = Util.player.primaryCannonReloadTime/4;
		Debug.Log("JH:IH");Debug.Log("JH:IH");
		Debug.Log("JH:IH");Debug.Log("JH:IH");
		Debug.Log("JH:IH");Debug.Log("JH:IH");Debug.Log("JH:IH");Debug.Log("JH:IH");
		Debug.Log("JH:IH");Debug.Log("JH:IH");
		Debug.Log("JH:IH");Debug.Log("JH:IH");Debug.Log("JH:IH");Debug.Log("JH:IH");
		Debug.Log("JH:IH");Debug.Log("JH:IH");
		Debug.Log("JH:IH");Debug.Log("JH:IH");Debug.Log("JH:IH");Debug.Log("JH:IH");
		Debug.Log("JH:IH");Debug.Log("JH:IH");
		Debug.Log("JH:IH");Debug.Log("JH:IH");
		Debug.Log("JH:IH");Debug.Log("JH:IH");
		Debug.Log("JH:IH");Debug.Log("JH:IH");
		Debug.Log("JH:IH");Debug.Log("JH:IH");
		Debug.Log("JH:IH");Debug.Log("JH:IH");
		Debug.Log("JH:IH");Debug.Log("JH:IH");
	}
	void OnGUI()
	{
		UnityEngine.GUI.BeginGroup(new Rect(0, 0, 256, 68));
		UnityEngine.GUI.Box(new Rect(0, 0, 256, 68), UnityEngine.GUIContent.none);
		UnityEngine.GUI.BeginGroup(new Rect(8, 3, Util.player.shieldPct*2.4f, 32));
		UnityEngine.GUI.Box(new Rect(0, 0, 240, 32), shieldBar, currentStyle);
		UnityEngine.GUI.EndGroup();
		UnityEngine.GUI.BeginGroup(new Rect(8, 36, Util.player.health/Util.player.maxHealth*240, 32));
		UnityEngine.GUI.Box(new Rect(0, 0, 240, 32), healthBar, currentStyle);
		UnityEngine.GUI.EndGroup();
		UnityEngine.GUI.EndGroup();
		UnityEngine.GUI.Box(new Rect(Screen.width/2-cursorIcon.width/2, Screen.height/2-cursorIcon.height/2, cursorIcon.width, cursorIcon.height), cursorIcon, currentStyle);
		if (Util.player.primaryCannonTimer > dotUpdateDelta && Util.player.primaryCannonTimer < 4*dotUpdateDelta)
		{
			UnityEngine.GUILayout.BeginArea(new Rect(Screen.width/2 - primaryWepStartPoint.x, Screen.height/2 - primaryWepStartPoint.y, 16, 48));
			UnityEngine.GUILayout.Box(fireUpdateDot, currentStyle);
			if(Util.player.primaryCannonTimer > 2*dotUpdateDelta)
			{
				UnityEngine.GUILayout.Box(fireUpdateDot, currentStyle);
			}
			if(Util.player.primaryCannonTimer > 3*dotUpdateDelta)
			{
				UnityEngine.GUILayout.Box(fireUpdateDot, currentStyle);
			}
			UnityEngine.GUILayout.EndArea();
		}
		if (Util.player.possibleSecondaries[Util.player.currentSecondaryWep].name.Equals("PlayerGrenade"))
		{
			if(Util.player.secondaryBulletsLeft[Util.player.currentSecondaryWep] > 0)
			{
				UnityEngine.GUILayout.BeginArea(new Rect(Screen.width/2 + primaryWepStartPoint.x-16, Screen.height/2 - primaryWepStartPoint.y, 16, 48));
				for(int i = 0; i < Util.player.secondaryBulletsLeft[Util.player.currentSecondaryWep]; i++)
				{
					UnityEngine.GUILayout.Box(secondaryIcon, currentStyle);
				}
				UnityEngine.GUILayout.EndArea();
			}
		}
		else if (Util.player.possibleSecondaries[Util.player.currentSecondaryWep].name.Equals("PlayerRocket"))
		{
			if(Util.player.secondaryBulletsLeft[Util.player.currentSecondaryWep] > 0)
			{
				UnityEngine.GUILayout.BeginArea(new Rect(Screen.width/2 + primaryWepStartPoint.x-16, Screen.height/2 - primaryWepStartPoint.y, 16, 64));
				for(int i = 0; i < Util.player.secondaryBulletsLeft[Util.player.currentSecondaryWep]/2; i++)
				{
					UnityEngine.GUILayout.Box(secondaryIcon, currentStyle);
				}
				UnityEngine.GUILayout.EndArea();
				if(Util.player.secondaryBulletsLeft[Util.player.currentSecondaryWep] > Util.player.secondaryBulletsLeft[Util.player.currentSecondaryWep]/2)
				{
					UnityEngine.GUILayout.BeginArea(new Rect(Screen.width/2 + primaryWepStartPoint.x, Screen.height/2 - primaryWepStartPoint.y, 16, 64));
					for(int i = Util.player.secondaryBulletsLeft[Util.player.currentSecondaryWep]/2; i < Util.player.secondaryBulletsLeft[Util.player.currentSecondaryWep]; i++)
					{
						UnityEngine.GUILayout.Box(secondaryIcon, currentStyle);
					}
					UnityEngine.GUILayout.EndArea();
				}
			}
		}
		UnityEngine.GUI.Label(new Rect(Screen.width - 350, 0, 350, 20), "Current Secondary: " + Util.player.possibleSecondaries[Util.player.currentSecondaryWep].prettyName, currentStyle);
		if(timeSinceLastCheckpoint < checkpointDisplayTimeout)
		{
			timeSinceLastCheckpoint += Time.deltaTime;
			UnityEngine.GUI.Label(new Rect(Screen.width/2-75, 0, 200, 20), "Checkpoint Reached...", currentStyle);
		}
	}
	void Update () {
		/*if(Input.GetKey("escape"))
		{
			Time.timeScale = 0;
		}*/
	}
	public void CheckpointReached()
	{
		timeSinceLastCheckpoint = 0;
	}
}
