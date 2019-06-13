using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public GameObject player;

	public float cameraHeight;
	private float direction;

	//private float maxheight = 20f;
	//private float minheight;

	// Use this for initialization
	void Start () {
		transform.eulerAngles = new Vector3 (65f, 45f, 0f);
		//minheight = cameraHeight;
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 playerpos = player.transform.position;

		transform.position = new Vector3 (playerpos.x - 2.5f, cameraHeight, playerpos.z - 2.5f);
		
		// added for playable core, can be commented out afterwards
		/*if (Input.GetButton(MyInput.A_name) || Input.GetKey (KeyCode.Q))
		{
			cameraHeight += 0.1f;
			
			if (cameraHeight > maxheight)
				cameraHeight = maxheight;
		}
		else
		{
			cameraHeight -= 0.1f;
			
			if (cameraHeight < minheight)
				cameraHeight = minheight;
		}*/
		
	}
}
