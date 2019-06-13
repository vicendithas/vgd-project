using UnityEngine;
using System.Collections;

public class SpellStats : MonoBehaviour {

	public GUIText popupref;
	public int damage;
	public float cooldown;
	public GameObject playerReference;
	
	private GUIText popup;
	
	// Use this for initialization
	void Start ()
	{
		playerReference = GameObject.FindWithTag("Player");
		
		// when spell is created, give it random damage
		// down to current weapon damage, up to double current weapon damage
		GameObject currentweapon = playerReference.GetComponent<PlayerStats> ().currentWeapon;
		int currdamage = currentweapon.GetComponent<WeaponStats>().damage;

		damage = Random.Range (currdamage * 2, currdamage * 4);
		cooldown = Random.Range (10f, 20f);
		GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value);
        ParticleSystem ps = GetComponent<ParticleSystem>();
        ParticleSystem.MainModule psmain = ps.main;
        psmain.startColor = GetComponent<Renderer>().material.color;
		
		popup = (GUIText) Instantiate (popupref);
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 playerpos = playerReference.transform.position;
		Vector3 spellpos = gameObject.transform.position;
		
		if(spellpos.y < 0){
			spellpos.y = 1;
			transform.position = spellpos;
		}
		
		if(Vector3.Distance(playerpos, spellpos) < 1 && transform.root.tag != "Player"){

			int currdamage;
			float currcooldown;
			if(playerReference.GetComponent<PlayerStats>().currentSpell == null){
				popup.text = "A (Xbox)/Q (Key) for spell (Damage/Cooldown: No Spell -> <" + damage + ", " + cooldown.ToString("#.##") + ">)";
			} else {
				currdamage = playerReference.GetComponent<PlayerStats>().currentSpell.GetComponent<SpellStats>().damage;
				currcooldown = playerReference.GetComponent<PlayerStats>().currentSpell.GetComponent<SpellStats>().cooldown;

				popup.text = "A (Xbox)/Q (Key) for spell (Damage/Cooldown: <" + currdamage + ", " + currcooldown.ToString("#.##")
					+ "> -> <" + damage + ", " + cooldown.ToString("#.##") + ">)";
			}

			if(Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown(MyInput.A_name)){
				playerReference.GetComponent<PlayerStats>().PickUpSpell(gameObject);
				this.GetComponent<AudioSource>().Play ();
			}
		} else {
			popup.text = "";
		}
		
	}
}
