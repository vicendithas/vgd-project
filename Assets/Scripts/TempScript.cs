using UnityEngine;
using System.Collections;

public class TempScript : MonoBehaviour {

	public GameObject player;
	private Animator myAnim;

	// Use this for initialization
	void Start () {

		myAnim = GameObject.FindWithTag("Chest").GetComponent<Animator> ();

		Vector3 ppos = player.transform.position;
		GameObject.FindWithTag("Chest").transform.position = new Vector3(ppos.x + 1, 0.1f, ppos.z + 1);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown (KeyCode.Space)){
			myAnim.SetBool("isOpen", true);
		}
	}
}
