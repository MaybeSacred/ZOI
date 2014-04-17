using UnityEngine;
using System.Collections;

public class MenuGui : MonoBehaviour {
	
	public GUIStyle mainStyle; //background = menuTexture
	public GUIStyle buttonStyle; //ICE AGE font, black size 50 aligned middle center, normal background = whitebar, active background = whitebarpressed
	public GUIStyle leftButtonStyle;
	public GUIStyle rightButtonStyle;
	public GUIStyle mouseSensitivityButtonStyle; //size 40, upper center
	public GUIStyle currentSensitivityStyle;
	public GUIStyle menuTitleStyle; //IceCaps font, white size 65
	public GUIStyle titleStyle; //IceCaps white size 115, aligned middle center

	private CameraScript theCamera;

	int imagex,imagey,imagewidth,imageheight;
	bool main = true;

	void Start () {

		//stuff that makes it snow :D
		theCamera = GetComponent<CameraScript>();

		imagex = Screen.width-490;
		imagey = 0;
		imagewidth = 500;
		imageheight = Screen.height;
	
	}

	void Update () {

	}

	void OnGUI()
	{
		if(main==true)
			MenuScreen();
		else
			Options();
	}

	void MenuScreen()
	{
		GUI.Box(new Rect(imagex, imagey, imagewidth, imageheight), "", mainStyle);
		GUI.Label(new Rect(imagex+200,imagey+100,100,100), "Main Menu", menuTitleStyle);
		GUI.Label(new Rect(400,200,100,100), "Zone" + "\n" + "of" + "\n" + "Inaccessibility", titleStyle);
		if(GUI.Button(new Rect(imagex+100,imagey+250,imagewidth-200,50), "Start Game", buttonStyle))
		{
			Application.LoadLevel(1);
		}
		if(GUI.Button(new Rect(imagex+100,imagey+250,imagewidth-200,50), "Level Select", buttonStyle))
		{
			Application.LoadLevel(1);
		}
		if(GUI.Button(new Rect(imagex+100,imagey+350,imagewidth-200,50), "Options", buttonStyle))
		{
			main = false;
		}
	}

	void Options()
	{
		GUI.BeginGroup(new Rect(Screen.width/2 - imagewidth/2, Screen.height/2 - 300, imagewidth, imageheight));
		GUI.Box(new Rect(0, 0, imagewidth, imageheight), "", mainStyle);
		GUI.Label(new Rect(125,95,250,50), "Options", menuTitleStyle);
		GUI.Label(new Rect(125,210,250,50), "Mouse Sensitivity", mouseSensitivityButtonStyle);

		if(GUI.Button(new Rect(100,imageheight/2-35,50,50), "", leftButtonStyle)) 
		{
			theCamera.DecrementMouseSensitivity();
		}
		if(GUI.Button(new Rect(350,imageheight/2-35,50,50),"",rightButtonStyle))
		{
			theCamera.IncrementMouseSensitivity();
		}
		if(GUI.Button(new Rect(155,imageheight/2+125,200,50), "OK", buttonStyle))
		{
			main = true;
		}
		GUI.TextArea(new Rect(235, imageheight/2-35, 224, 48), theCamera.GetCurrentMouseSensitivity().ToString(), currentSensitivityStyle);
		GUI.EndGroup();
	}

}
