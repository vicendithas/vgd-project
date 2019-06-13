using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {

	public GameObject player;
	public GameObject mycamera;
	private bool paused;

	// Use this for initialization
	void Start () {
		paused = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.P) || Input.GetButtonDown(MyInput.Start_name)){
			paused = !paused;

			GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
			GameObject[] guiitems = GameObject.FindGameObjectsWithTag("HUD");

			foreach(GameObject enemy in enemies){
				enemy.GetComponent<EnemyAI>().enabled = !paused;
			}

			player.GetComponent<PlayerController>().enabled = !paused;
			player.GetComponent<Animator>().enabled = !paused;

			foreach(GameObject gui in guiitems){
				gui.SetActive(!paused);
			}

			GameObject.Find ("GeneralScripts").GetComponent<PlayerHUD>().Pause (paused);

			GameObject.Find ("Level").GetComponent<GUIText>().enabled = !paused;

			GameObject.Find ("PauseGUI").GetComponent<GUIText>().enabled = paused;
			GameObject.Find ("QuitGUI").GetComponent<GUIText>().enabled = paused;
			GameObject.Find ("ShieldGUI").GetComponent<GUIText>().enabled = paused;
			GameObject.Find ("SpellGUI").GetComponent<GUIText>().enabled = paused;
			GameObject.Find ("WeaponGUI").GetComponent<GUIText>().enabled = paused;
			GameObject.Find ("KillsGUI").GetComponent<GUIText>().enabled = paused;
			GameObject.Find ("DamageGUI").GetComponent<GUIText>().enabled = paused;
			GameObject.Find ("SpellCD").GetComponent<GUIText>().enabled = !paused;

			if(paused){
				mycamera.transform.eulerAngles = new Vector3 (-90f, 45f, 0f);
			} else {
				mycamera.transform.eulerAngles = new Vector3 (60f, 45f, 0f);
			}
		}

		if(paused && (Input.GetKeyDown (KeyCode.Q) || Input.GetButtonDown (MyInput.B_name))){
            UnityEngine.SceneManagement.SceneManager.LoadScene("mainMenu");
        }


	}
}
