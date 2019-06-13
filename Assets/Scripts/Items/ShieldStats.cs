using UnityEngine;
using System.Collections;

public class ShieldStats : MonoBehaviour {
	public GUIText popupref;
	public int turnspeed;
	public float movespeed;
	//public List<Attribute> attributes = new List<Attribute>();
	public GameObject playerReference;
	
	private GUIText popup;
	
	// Use this for initialization
	void Start ()
	{
		playerReference = GameObject.FindWithTag("Player");
		
		// when shield is created, give it random properties
		GameObject currentshield = playerReference.GetComponent<PlayerStats> ().currentShield;
		int currturnspeed = currentshield.GetComponent<ShieldStats>().turnspeed;
		if(currturnspeed == 0){
			movespeed = 0.33f;
			turnspeed = 45;
			GetComponent<Renderer>().material.color = new Color(0.75f, 0.75f, 0.75f);
			
		} else {
			movespeed = Random.Range(0.33f, 1.0f);
			turnspeed = Random.Range (45, 180);
			GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value);
		}
		
		popup = (GUIText) Instantiate (popupref);
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 playerpos = playerReference.transform.position;
		Vector3 shieldpos = gameObject.transform.position;
		
		if(shieldpos.y < 0f){
			shieldpos.y = 1f;
			transform.position = shieldpos;
		}
		
		if(Vector3.Distance(playerpos, shieldpos) < 1 && transform.root.tag != "Player"){
			
			float currmovespeed = playerReference.GetComponent<PlayerStats>().currentShield.GetComponent<ShieldStats>().movespeed;
			int currturnspeed = playerReference.GetComponent<PlayerStats>().currentShield.GetComponent<ShieldStats>().turnspeed;

			popup.text = "A (Xbox)/Q (Key) for shield (Move/Turn: <" + 
				currmovespeed.ToString("#%") + ", " + currturnspeed + (char)176 +
					"> -> <" + movespeed.ToString("#%") + ", " + turnspeed + (char)176 + ">)";
			if(Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown(MyInput.A_name)){
				playerReference.GetComponent<PlayerStats>().PickUpShield(gameObject);
				this.GetComponent<AudioSource>().Play ();
			}
		} else {
			popup.text = "";
		}
		
	}
}
