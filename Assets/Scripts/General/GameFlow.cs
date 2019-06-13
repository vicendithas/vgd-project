using UnityEngine;
using System.Collections;

public class GameFlow : MonoBehaviour {

	public GameObject playerReference;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (playerReference.GetComponent<PlayerStats>().currentHealth <= 0)
		{
			//lose the game
			GameObject.Find("GameOverText").SetActive(true);
			Time.timeScale = 0.0f;
			
			if (Input.GetButtonDown(MyInput.A_name) || Input.GetKeyDown (KeyCode.Space))
			{
                UnityEngine.SceneManagement.SceneManager.LoadScene("mainGame");
            }
		}
		else
		{
			GameObject.Find ("GameOverText").SetActive(false);
			Time.timeScale = 1.0f;
		}
	}
}
