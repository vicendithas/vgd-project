using UnityEngine;
using System.Collections;

public class LevelTransition : MonoBehaviour {

	public GUIText popupref;
	public GUIText popup;

	// Use this for initialization
	void Start ()
	{
		popup = (GUIText)Instantiate (popupref);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Player")
		{
			//Debug.Log ("Player collision");
			popup.text = "A (Xbox)/Q (Keyboard) for next level";

			if (Input.GetButtonDown(MyInput.A_name) || Input.GetKeyDown (KeyCode.Q))
			{
			    GameObject.FindWithTag("GameController").GetComponent<GenerateLevel>().proceedToNextLevel();
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.tag == "Player")
		{
			popup.text = "";
		}
	}
}
