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
	public GUIStyle textStuff;
	public RectTransform MAIN, OPTIONS, LEVELSELECT, CREDITS, HOWTOPLAY, HUDEXAMPLE;
	public GameObject mainGameGUI;
	RectTransform currentPanel;
	private CameraScript theCamera;

	int imagex,imagey,imagewidth,imageheight;

	void Start () {

		//stuff that makes it snow :D
		theCamera = GetComponent<CameraScript>();
		currentPanel = MAIN;
		MAIN.gameObject.SetActive(false);
		OPTIONS.gameObject.SetActive(false);
		LEVELSELECT.gameObject.SetActive(false);
		CREDITS.gameObject.SetActive(false);
		HOWTOPLAY.gameObject.SetActive(false);
		HUDEXAMPLE.gameObject.SetActive(false);
		SwitchToPanel(MAIN);
		imagex = Screen.width-490;
		imagey = 0;
		imagewidth = 500;
		imageheight = Screen.height;
		Screen.lockCursor = false;
	}
	void SwitchToPanel(RectTransform switchTo){
		mainGameGUI.SetActive(false);
		currentPanel.gameObject.SetActive(false);
		currentPanel = switchTo;
		currentPanel.gameObject.SetActive(true);
	}
	void Update () {

	}

	void OnGUI()
	{
		/*switch(state)
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
			case MenuState.HOWTOPLAY:
			{
				HowToPlay();
				break;
			}
		}*/
	}
	int border = 15;
	public void SwitchToPlayInstructions(){
		SwitchToPanel(HOWTOPLAY);
	}
	public void BackToMain(){
		SwitchToPanel(MAIN);
	}
	public void SwitchToCredits(){
		SwitchToPanel(CREDITS);
	}
	public void SwitchToLevelSelect(){
		SwitchToPanel(LEVELSELECT);
	}
	public void SwitchToOptions(){
		SwitchToPanel(OPTIONS);
	}
	public void SwitchToHUD(){
		SwitchToPanel(HUDEXAMPLE);
		mainGameGUI.SetActive(true);
	}
	public void Quit(){
		Application.Quit();
	}
	public void LoadLevel(string levelToLoad){
		FindObjectOfType<MusicSystem>().transform.parent = null;
		Application.LoadLevel(levelToLoad);
	}
	/*void HowToPlay()
	{
		GUI.BeginGroup(new Rect(border, border, Screen.width - 2*border, Screen.height-2*border));
		GUI.Label(new Rect(0, 0, Screen.width - 2*border, 50), "Instructions", menuTitleStyle);
		GUI.Label(new Rect(0, 75, Screen.width/2 - border, 50), "W/S", textStuff);
		GUI.Label(new Rect(0, 150, Screen.width/2 - border, 50), "A/D", textStuff);
		GUI.Label(new Rect(0, 225, Screen.width/2 - border, 50), "Tab/1/2/3", textStuff);
		GUI.Label(new Rect(0, 300, Screen.width/2 - border, 50), "Left Click", textStuff);
		GUI.Label(new Rect(0, 375, Screen.width/2 - border, 50), "Right Click", textStuff);
		GUI.Label(new Rect(0, 450, Screen.width/2 - border, 50), "Scroll Wheel", textStuff);
		GUI.Label(new Rect(Screen.width/2, 75, Screen.width/2 - border, 50), "Forwards/Backwards", textStuff);
		GUI.Label(new Rect(Screen.width/2, 150, Screen.width/2 - border, 50), "Steer", textStuff);
		GUI.Label(new Rect(Screen.width/2, 225, Screen.width/2 - border, 50), "Switch Weapon", textStuff);
		GUI.Label(new Rect(Screen.width/2, 300, Screen.width/2 - border, 50), "Fire Cannon", textStuff);
		GUI.Label(new Rect(Screen.width/2, 375, Screen.width/2 - border, 50), "Fire Secondary", textStuff);
		GUI.Label(new Rect(Screen.width/2, 450, Screen.width/2 - border, 50), "Zoom", textStuff);
		if(GUI.Button(new Rect(Screen.width/2 - border - 100, Screen.height - 2*border - 50, 200, 50), "Back", buttonStyle))
		{
			state = MenuState.MAIN;
		}
		GUI.EndGroup();
	}
	void Credits()
	{
		
		GUI.BeginGroup(new Rect(border, border, Screen.width - 2*border, Screen.height-2*border));
		GUI.Label(new Rect(0, 0, Screen.width/2 - border, 50), "Jon Tyson", menuTitleStyle);
		GUI.Label(new Rect(0, 75, Screen.width/2 - border, 50), "Daniel Dias", menuTitleStyle);
		GUI.Label(new Rect(0, 150, Screen.width/2 - border, 50), "Chris Tansey", menuTitleStyle);
		GUI.Label(new Rect(0, 225, Screen.width/2 - border, 50), "Erica Pramer", menuTitleStyle);
		GUI.Label(new Rect(0, 300, Screen.width/2 - border, 50), "Jay Belmont", menuTitleStyle);
		GUI.Label(new Rect(0, 375, Screen.width/2 - border, 50), "Jordan Hobgood", menuTitleStyle);
		GUI.Label(new Rect(Screen.width/2, 0, Screen.width/2 - border, 50), "Lead Designer", menuTitleStyle);
		GUI.Label(new Rect(Screen.width/2, 75, Screen.width/2 - border, 50), "Level Modeler", menuTitleStyle);
		GUI.Label(new Rect(Screen.width/2, 150, Screen.width/2 - border, 50), "Enemy Designer", menuTitleStyle);
		GUI.Label(new Rect(Screen.width/2, 225, Screen.width/2 - border, 50), "GUI Designer", menuTitleStyle);
		GUI.Label(new Rect(Screen.width/2, 300, Screen.width/2 - border, 50), "Modeleler", menuTitleStyle);
		GUI.Label(new Rect(Screen.width/2, 375, Screen.width/2 - border, 50), "Sound Engineer", menuTitleStyle);
		if(GUI.Button(new Rect(Screen.width/2 - border - 100, Screen.height - 2*border - 50, 200, 50), "Back", buttonStyle))
		{
			state = MenuState.MAIN;
		}
		GUI.EndGroup();
	}
	void LevelSelect()
	{
		GUI.BeginGroup(new Rect(border, border, Screen.width - 2*border, Screen.height-2*border));
		if(GUI.Button(new Rect(Screen.width/2 -border - 150, 60, 300, 60), "Antarcticle", buttonStyle))
		{
			Application.LoadLevel("L01_OpeningCliff");
		}
		if(GUI.Button(new Rect(Screen.width/2 -border - 150, 120, 300, 60), "Plateaued", buttonStyle))
		{
			Application.LoadLevel("L02_FlatLakes");
		}
		if(GUI.Button(new Rect(Screen.width/2 -border - 150, 180,300, 60), "Tight Ally", buttonStyle))
		{
			Application.LoadLevel("L03_AlleyWay");
		}
		if(GUI.Button(new Rect(Screen.width/2 -border - 150, 240, 300, 60), "Gorged", buttonStyle))
		{
			Application.LoadLevel("L04A_Gorge");
		}
		if(GUI.Button(new Rect(Screen.width/2 -border - 150, 300, 300, 60), "End", buttonStyle))
		{
			Application.LoadLevel("L04B_Gorge");
		}
		if(GUI.Button(new Rect(Screen.width/2 - border - 100, Screen.height - 2*border - 50, 200, 50), "Back", buttonStyle))
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
			Application.LoadLevel("L01_OpeningCliff");
		}
		if(GUI.Button(new Rect(imagex+100,imagey+260,imagewidth-200,50), "Level Select", buttonStyle))
		{
			state = MenuState.LEVELSELECT;
		}
		if(GUI.Button(new Rect(imagex+100,imagey+320,imagewidth-200,50), "Credits", buttonStyle))
		{
			state = MenuState.CREDITS;
		}
		if(GUI.Button(new Rect(imagex+100,imagey+380,imagewidth-200,50), "How To Play", buttonStyle))
		{
			state = MenuState.HOWTOPLAY;
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
			//theCamera.DecrementMouseSensitivity();
		}
		if(GUI.Button(new Rect(350,imageheight/2-35,50,50),"",rightButtonStyle))
		{
			//theCamera.IncrementMouseSensitivity();
		}
		if(GUI.Button(new Rect(155,imageheight/2+125,200,50), "OK", buttonStyle))
		{
			state = MenuState.MAIN;
		}
		GUI.TextArea(new Rect(235, imageheight/2-35, 224, 48), theCamera.GetCurrentMouseSensitivity().ToString(), currentSensitivityStyle);
		GUI.EndGroup();
	}*/
}
