using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerStats : CharacterStats {

	//public GameObject currentWeapon;
	//public GameObject currentShield;
	
	public Animator myAnim;
	public int currentLevel;
	public GameObject currentWeapon, currentShield, currentSpell;

	[HideInInspector]
	public List<SmellPoint> smellPoints = new List<SmellPoint>();

	private Vector3 weaponpos;
	private Vector3 weaponrot;
	private Vector3 shieldpos;
	private Vector3 shieldrot;

	public int kills;
	public int damagedealt;
	
	// Use this for initialization
	void Start ()
	{
		kills = 0;
		damagedealt = 0;

		myAnim = GetComponent<Animator>();
		totalHealth = 3;
		currentHealth = totalHealth;
		
		currentWeapon = (GameObject)Instantiate(Resources.Load("Prefabs/Items/Sword"));
		currentWeapon.transform.parent = GameObject.Find("Right_Forearm").transform;
		currentWeapon.transform.position = currentWeapon.transform.parent.position + new Vector3(0.5f,0.2f,0f);
		weaponpos = currentWeapon.transform.localPosition;
		weaponrot = currentWeapon.transform.localEulerAngles;

		//disable the non-trigger collider for the sword in hand
		BoxCollider[] colliders = currentWeapon.GetComponents<BoxCollider> ();
		foreach (BoxCollider coll in colliders){
			if(!coll.isTrigger){
				coll.enabled = false;
			}
		}
		
		currentShield = (GameObject)Instantiate(Resources.Load("Prefabs/Items/Shield"));
		currentShield.transform.parent = GameObject.Find("Left_Forearm").transform;
		currentShield.transform.position = currentShield.transform.parent.position + new Vector3(-.05f,.075f,.075f);
		currentShield.transform.rotation = currentShield.transform.parent.rotation * Quaternion.Euler(40f, 160f, 80f);
		shieldpos = currentShield.transform.localPosition;
		shieldrot = currentShield.transform.localEulerAngles;

		//disable the non-trigger collider for the sword in hand
		colliders = currentShield.GetComponents<BoxCollider> ();
		foreach (BoxCollider coll in colliders){
			if(!coll.isTrigger){
				coll.enabled = false;
			}
		}

		currentSpell = null;

		GameObject.FindWithTag ("GameController").GetComponent<GenerateLevel> ().swordList.Add (currentWeapon);
		GameObject.FindWithTag ("GameController").GetComponent<GenerateLevel> ().shieldList.Add (currentShield);
		
		//currentSpell = null;

		GameObject.FindWithTag ("GameController").GetComponent<PlayerHUD> ().updateItems (currentWeapon, currentShield);

		characterHitSound = this.GetComponent<AudioSource>();

		//currentLevel = 1;

		// initialize "smellPoints" to allow smarter enemy chasing
		InvokeRepeating("UpdateSmellPoints", 0, 0.5f);
	}
	
	// Update is called once per frame
	void Update ()
	{
		int currdamage = currentWeapon.GetComponent<WeaponStats>().damage;
		float currheal = currentWeapon.GetComponent<WeaponStats> ().heal;
		float currturnspeed = currentShield.GetComponent<ShieldStats> ().turnspeed;
		float currmovespeed = currentShield.GetComponent<ShieldStats> ().movespeed;
		int currkills = currentWeapon.transform.root.GetComponent<PlayerStats> ().kills;
		int currdamagedealt = currentWeapon.transform.root.GetComponent<PlayerStats> ().damagedealt;

		GameObject.Find ("ShieldGUI").GetComponent<GUIText> ().text = "Shield: (Move/Turn):  <" + currmovespeed.ToString("#%") + ",  " + currturnspeed + (char)176 + ">";
		GameObject.Find ("WeaponGUI").GetComponent<GUIText>().text = "Weapon (Damage/Heal):  <" + currdamage + ",  " + currheal.ToString("#%") + ">";
		GameObject.Find ("KillsGUI").GetComponent<GUIText>().text = "Kills:  " + currkills;
		GameObject.Find ("DamageGUI").GetComponent<GUIText>().text = "Damage Dealt:  " + currdamagedealt;

		if(currentSpell == null){
			GameObject.Find ("SpellGUI").GetComponent<GUIText>().text = "Spell: Damage/Cooldown):  No Spell";
		} else {
			int currspelldamage = currentSpell.GetComponent<SpellStats> ().damage;
			float currcooldown = currentSpell.GetComponent<SpellStats> ().cooldown;
			GameObject.Find ("SpellGUI").GetComponent<GUIText>().text = "Spell: Damage/Cooldown):  <" + currspelldamage + ",  " + currcooldown.ToString("0.##") + ">";
		}

		//keep the currentweapon from falling out of the players hand
		currentWeapon.transform.localPosition = weaponpos;
		currentWeapon.transform.localEulerAngles = weaponrot;

		//keep the current shield from fallingout of the players hand
		currentShield.transform.localPosition = shieldpos;
		currentShield.transform.localEulerAngles = shieldrot;

		//keep the current spell from falling from GUI
		if(currentSpell != null){
			float aspect = (float) Screen.width / (float) Screen.height;
			currentSpell.transform.localPosition = new Vector3 ((0.5f * aspect), 0.45f , 1f);
			currentSpell.transform.localScale = new Vector3 (0.001f, 0.05f, 0.05f);
		}


		//prevent player from healing past max health
		if(currentHealth > totalHealth){
			currentHealth = totalHealth;
		}

		//lose the game
		if (currentHealth <= 0)
		{

			GameObject.Find ("DeathText").GetComponent<GUIText>().enabled = true;
			GameObject.Find ("KillsGUI").GetComponent<GUIText>().enabled = true;
			GameObject.Find ("DamageGUI").GetComponent<GUIText>().enabled = true;
			GameObject.Find ("QuitGUI").GetComponent<GUIText>().enabled = true;

			Time.timeScale = 0.0f;
			/*myAnim.SetBool("dead", true);
			transform.root.GetComponent<PlayerController>().enabled = false;
			transform.rigidbody.freezeRotation = false;*/
			
			if (Input.GetKeyDown (KeyCode.Q) || Input.GetButton (MyInput.B_name))
			{
				/*transform.root.GetComponent<PlayerController>().enabled = true;
				myAnim.SetBool("dead", false);
				transform.rigidbody.freezeRotation = true;*/
				Time.timeScale = 1.0f;
                UnityEngine.SceneManagement.SceneManager.LoadScene("mainMenu");
			}
		}


	}
	
	// handles smellpoints
	void UpdateSmellPoints()
	{
		// remove smellpoint if liftime <= 0
		for (int i = 0; i < smellPoints.Count; )
		{
			smellPoints[i].lifetime -= 0.5f;
			
			if (smellPoints[i].lifetime <= 0f)
			{
				GameObject.Destroy(smellPoints[i].smellPointObject);
				smellPoints.RemoveAt(i);
				continue;
			}
			else
				i++;
		}
		
		smellPoints.Insert(0, new SmellPoint(transform.position, 2.5f));
		//Debug.Log("New smellpoint at: " + smellPoints[smellPoints.Count - 1].point);
	}

	public void PickUpSword(GameObject newsword){
		GameObject oldsword = currentWeapon;
		currentWeapon = newsword;

		oldsword.transform.parent = null;
		currentWeapon.transform.parent = GameObject.Find("Right_Forearm").transform;

		//disable the non-trigger collider for the sword in hand
		BoxCollider[] colliders = currentWeapon.GetComponents<BoxCollider> ();
		foreach (BoxCollider coll in colliders){
			if(!coll.isTrigger){
				coll.enabled = false;
			}
		}

		//enable the non-trigger collider for the sword on ground
		colliders = oldsword.GetComponents<BoxCollider> ();
		foreach (BoxCollider coll in colliders){
			if(!coll.isTrigger){
				coll.enabled = true;
			}
		}

		oldsword.GetComponent<Rigidbody>().velocity = new Vector3 (1f, 3f, 1f);
	}

	public void PickUpShield(GameObject newShield){
		GameObject oldShield = currentShield;
		currentShield = newShield;
		
		oldShield.transform.parent = null;
		currentShield.transform.parent = GameObject.Find("Left_Forearm").transform;
		
		//disable the non-trigger collider for the sword in hand
		BoxCollider[] colliders = currentShield.GetComponents<BoxCollider> ();
		foreach (BoxCollider coll in colliders){
			if(!coll.isTrigger){
				coll.enabled = false;
			}
		}
		
		//enable the non-trigger collider for the sword on ground
		colliders = oldShield.GetComponents<BoxCollider> ();
		foreach (BoxCollider coll in colliders){
			if(!coll.isTrigger){
				coll.enabled = true;
			}
		}
		
		oldShield.GetComponent<Rigidbody>().velocity = new Vector3 (1f, 3f, 1f);
	}

	public void PickUpSpell(GameObject newSpell){
		GameObject oldSpell = currentSpell;
		currentSpell = newSpell;

		currentSpell.transform.parent = GameObject.FindGameObjectWithTag ("MainCamera").transform;
        currentSpell.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		currentSpell.GetComponent<MeshRenderer> ().receiveShadows = false;
		currentSpell.GetComponent<Rigidbody> ().useGravity = false;

		float aspect = (float) Screen.width / (float) Screen.height;
		currentSpell.transform.localPosition = new Vector3 ((0.5f * aspect), 0.45f , 1f);
		currentSpell.transform.localScale = new Vector3 (0.001f, 0.05f, 0.05f);

        ParticleSystem ps = currentSpell.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule psmain = ps.main;
        psmain.startSize = 0.1f;
        psmain.startLifetime = 0.1f;

		if(oldSpell != null)
		{
			GameObject.FindWithTag ("GameController").GetComponent<GenerateLevel> ().spellList.Remove(oldSpell);
			Destroy(oldSpell);
		}
	}
}
