using UnityEngine;
using System.Collections;

public class MenuGui : MonoBehaviour {
	
	public GUIStyle mainStyle; //background = menuTexture
	public GUIStyle buttonStyle; //ICE AGE font, black size 50 aligned middle center, normal background = whitebar, active background = whitebarpressed
	public GUIStyle menuTitleStyle; //IceCaps font, white size 65
	public GUIStyle titleStyle; //IceCaps white size 115, aligned middle center

	int imagex,imagey,imagewidth,imageheight;

	void Start () {
		imagex = Screen.width-490;
		imagey = 0;
		imagewidth = 500;
		imageheight = Screen.height;
	
	}

	void Update () {

	}

	void OnGUI()
	{
		MenuScreen();
	}

	void MenuScreen()
	{
		GUI.Box(new Rect(imagex, imagey, imagewidth, imageheight), "", mainStyle);
		GUI.Label(new Rect(imagex+200,imagey+100,100,100), "Main Menu", menuTitleStyle);
		GUI.Label(new Rect(400,200,100,100), "Zone" + "\n" + "of" + "\n" + "Inaccessibility", titleStyle);
		if(GUI.Button(new Rect(imagex+100,imagey+250,imagewidth-200,50), "Start Game", buttonStyle))
		{
			//do stuff
		}
		if(GUI.Button(new Rect(imagex+100,imagey+350,imagewidth-200,50), "Options", buttonStyle))
		{
			//do stuff
		}
	}
}
