using UnityEngine;
using System.Collections;

public class ChestController : MonoBehaviour {

	private GameObject player;
	public GUIText popupref;
	private Animator myAnim;
	private GUIText popup;

	// Use this for initialization
	void Start () {
		player = GameObject.Find("simpleplayer");
		popup = (GUIText) Instantiate (popupref);
		myAnim = GetComponent<Animator> ();

		myAnim.SetBool ("isOpen", false);
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 playerpos = player.transform.position;
		Vector3 chestpos = gameObject.transform.position;

		if(Vector3.Distance(playerpos, chestpos) < 1 && myAnim.GetCurrentAnimatorStateInfo(0).IsName("Closed")){
			popup.text = "A (Xbox)/Q (Key) to open chest";
			if(Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown(MyInput.A_name)){
				myAnim.SetBool("isOpen", true);
				this.GetComponent<AudioSource>().Play ();
				Vector3 direction = Vector3.Normalize(chestpos - playerpos);
				spawnItem(direction);
			}
		} else {
			popup.text = "";
		}
	}

	void spawnItem(Vector3 direction){
		GameObject newitem;
		float itemchance = Random.value;

		//itemchance = 0.95f;

		if(itemchance > 0.8f){
			newitem = (GameObject)Instantiate (Resources.Load ("Prefabs/Items/Spell"));
			GameObject.FindWithTag ("GameController").GetComponent<GenerateLevel> ().spellList.Add (newitem);
		} else if(itemchance > 0.45){
			newitem = (GameObject)Instantiate(Resources.Load("Prefabs/Items/Shield"));
			GameObject.FindWithTag ("GameController").GetComponent<GenerateLevel> ().shieldList.Add (newitem);
		} else {
			newitem = (GameObject)Instantiate(Resources.Load("Prefabs/Items/Sword"));
			GameObject.FindWithTag ("GameController").GetComponent<GenerateLevel> ().swordList.Add (newitem);
		}

		newitem.transform.position = transform.position + new Vector3(0f, 1f, 0f);
		newitem.GetComponent<Rigidbody>().velocity = new Vector3(-direction.x, 4f, -direction.z);


	}
}
