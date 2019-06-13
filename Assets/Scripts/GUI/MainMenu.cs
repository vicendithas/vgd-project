using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	public GUITexture logo;
	public GUIText start;
	public GUIText quit;

	private int index = 0;
	private int menuitems = 2;
	private GUIText[] menu;

	// Use this for initialization
	void Start () {
		menu = new GUIText[menuitems];
		menu [0] = start;
		menu [1] = quit;
	}
	
	// Update is called once per frame
	void Update () {
	
		float yscale = 0.4f;
		float xscale = (0.4f * Screen.height) * (8.5f / 3.5f) / Screen.width;

		logo.transform.localScale = new Vector3 (xscale, yscale, 1);

		if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown (KeyCode.W) || Input.GetAxis(MyInput.L_YAxisname) < -0.5f){
			index--;
		}
		if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown (KeyCode.S) || Input.GetAxis(MyInput.L_YAxisname) > 0.5f){
			index++;
		}

		if(index > 1)
			index = 1;
		if(index < 0)
			index = 0;
			
		for(int i = 0; i < menuitems; i++){
			if(i == index)
				menu[i].color = Color.red;
			else
				menu[i].color = Color.yellow;
		}

		if(Input.GetKeyDown (KeyCode.Space) || Input.GetAxis(MyInput.A_name) > 0.1f){
			if(index == 1){
				Application.Quit();
			} else {
                UnityEngine.SceneManagement.SceneManager.LoadScene("mainGame");
            }
		}



	}
}
