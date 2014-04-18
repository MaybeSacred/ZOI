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
	private enum MenuState{MAIN, OPTIONS, LEVELSELECT, CREDITS}
	MenuState state;
	private CameraScript theCamera;

	int imagex,imagey,imagewidth,imageheight;

	void Start () {

		//stuff that makes it snow :D
		theCamera = GetComponent<CameraScript>();
		state = MenuState.MAIN;
		imagex = Screen.width-490;
		imagey = 0;
		imagewidth = 500;
		imageheight = Screen.height;
	
	}

	void Update () {

	}

	void OnGUI()
	{
		switch(state)
		{
			case MenuState.MAIN:
			{
				MenuScreen();
				break;
			}
			case MenuState.OPTIONS:
			{
				Options();
				break;
			}
			case MenuState.LEVELSELECT:
			{
				LevelSelect();
				break;
			}
			case MenuState.CREDITS:
			{
				Credits();
				break;
			}
		}
	}
	int border = 15;
	void Credits()
	{
		
		GUI.BeginGroup(new Rect(border, border, Screen.width - 2*border, Screen.height-2*border));
		GUI.Label(new Rect(0, 0, Screen.width/2 - border, 50), "Jon Tyson", menuTitleStyle);
		GUI.Label(new Rect(0, 75, Screen.width/2 - border, 50), "Daniel Dias", menuTitleStyle);
		GUI.Label(new Rect(0, 150, Screen.width/2 - border, 50), "Chris Tansey", menuTitleStyle);
		GUI.Label(new Rect(0, 225, Screen.width/2 - border, 50), "Erica Pramer", menuTitleStyle);
		GUI.Label(new Rect(0, 300, Screen.width/2 - border, 50), "Jay Belmont", menuTitleStyle);
		GUI.Label(new Rect(Screen.width/2, 0, Screen.width/2 - border, 50), "Lead Designer", menuTitleStyle);
		GUI.Label(new Rect(Screen.width/2, 75, Screen.width/2 - border, 50), "Level Modeler", menuTitleStyle);
		GUI.Label(new Rect(Screen.width/2, 150, Screen.width/2 - border, 50), "Enemy Designer", menuTitleStyle);
		GUI.Label(new Rect(Screen.width/2, 225, Screen.width/2 - border, 50), "GUI Designer", menuTitleStyle);
		GUI.Label(new Rect(Screen.width/2, 300, Screen.width/2 - border, 50), "Enemy Modeleler", menuTitleStyle);
		if(GUI.Button(new Rect(Screen.width/2 - border - 100, Screen.height - 2*border - 50, 200, 50), "Back", buttonStyle))
		{
			state = MenuState.MAIN;
		}
		GUI.EndGroup();
	}
	void LevelSelect()
	{
		GUI.BeginGroup(new Rect(border, border, Screen.width - 2*border, Screen.height-2*border));
		if(GUI.Button(new Rect(155,imageheight/2+125,200,50), "Back", buttonStyle))
		{
			state = MenuState.MAIN;
		}
		GUI.EndGroup();
	}
	void MenuScreen()
	{
		GUI.Box(new Rect(imagex, imagey, imagewidth, imageheight), "", mainStyle);
		GUI.Label(new Rect(imagex+200,imagey+100,100,100), "Main Menu", menuTitleStyle);
		GUI.Label(new Rect(400,200,100,100), "Zone" + "\n" + "of" + "\n" + "Inaccessibility", titleStyle);
		if(GUI.Button(new Rect(imagex+100,imagey+200,imagewidth-200,50), "Start Game", buttonStyle))
		{
			Application.LoadLevel(1);
		}
		if(GUI.Button(new Rect(imagex+100,imagey+260,imagewidth-200,50), "Level Select", buttonStyle))
		{
			state = MenuState.LEVELSELECT;
		}
		if(GUI.Button(new Rect(imagex+100,imagey+320,imagewidth-200,50), "Credits", buttonStyle))
		{
			state = MenuState.CREDITS;
		}
		if(GUI.Button(new Rect(imagex+100,imagey+380,imagewidth-200,50), "Options", buttonStyle))
		{
			state = MenuState.OPTIONS;
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
			state = MenuState.MAIN;
		}
		GUI.TextArea(new Rect(235, imageheight/2-35, 224, 48), theCamera.GetCurrentMouseSensitivity().ToString(), currentSensitivityStyle);
		GUI.EndGroup();
	}

}
