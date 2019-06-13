using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class CharacterStats : MonoBehaviour {
	
	public float damagedCooldown;
	
	[HideInInspector]
	public int totalHealth, currentHealth;
	[HideInInspector]
	public List<int> currentStatuses = new List<int>();
	[HideInInspector]
	public AudioSource characterHitSound;
	
	private float damageTimeStamp;
	
	
	// Use this for initialization
	void Start ()
	{
		//damageTimeStamp = Time.time;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
	
	public void isAttackedBy(GameObject source)
	{
		int damage = 0;
		
		// if cooldown has worn off, calculate possible damage
		if (damageTimeStamp < Time.time)
		{			
			if (source.transform.root.tag == "Player")
			{
				// the original damage
				damage = source.GetComponent<WeaponStats>().damage;
				
				// add in any resistances/damage reductions here
				
				
				// if still some damage, hurt player and make invincible for short time
				if (damage > 0)
				{					
					int damagedealt;
					if(currentHealth >= damage){
						damagedealt = damage;
					} else {
						damagedealt = currentHealth;
					}
					source.transform.root.GetComponent<PlayerStats>().damagedealt += damagedealt;
					currentHealth -= damage;
					
					characterHitSound.Play();
					
					damageTimeStamp = Time.time + damagedCooldown;
				}

				if(currentHealth <= 0){
					//record kill
					source.transform.root.GetComponent<PlayerStats>().kills ++;

					//small chance to heal
					if(Random.value <= source.GetComponent<WeaponStats>().heal){
						source.transform.root.GetComponent<PlayerStats>().currentHealth++;
					}
				}

			}
			else if (source.transform.root.tag == "Enemy")
			{
				// the original damage
				damage = source.GetComponent<EnemyStats>().attackDamage;
				
				// add in any resistances/damage reductions here
				if (this.gameObject.GetComponent<Animator>().GetBool("blocking") && Vector3.Angle(this.transform.forward, source.transform.forward) > 100)
				{
					damage = 0;
				}
				
				// if still some damage, hurt player and make invincible for short time
				if (damage > 0 && source.gameObject.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attacking"))
				{
					currentHealth -= damage;
					characterHitSound.Play();
					damageTimeStamp = Time.time + damagedCooldown;
				}
				
			}
			// traps
			else if (source.transform.root.tag == "Floor" || source.transform.root.tag == "Boulder")
			{
				currentHealth -= 1;
				characterHitSound.Play();
				damageTimeStamp = Time.time + damagedCooldown;
			}
			// spells, they are parented to MainCamera
			else if (source.transform.root.tag == "MainCamera")
			{
				// the original damage
				damage = GameObject.FindWithTag("MainCamera").GetComponentInChildren<SpellStats>().damage;
				//Debug.Log ("Spell damage is: " + damage);
				
				// add in any resistances/damage reductions here
				
				
				// if still some damage, hurt player and make invincible for short time
				if (damage > 0)
				{
					int damagedealt;
					if(currentHealth >= damage){
						damagedealt = damage;
					} else {
						damagedealt = currentHealth;
					}
					GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>().damagedealt += damagedealt;

					currentHealth -= damage;
					characterHitSound.Play();
					damageTimeStamp = Time.time + damagedCooldown;
				}

				if(currentHealth <= 0){
					//record kill
					GameObject.FindGameObjectWithTag("Player").transform.root.GetComponent<PlayerStats>().kills ++;
				}
			}
			
			// make sure negative health isn't a thing
			if (currentHealth < 0)
				currentHealth = 0;
		}
			
		return;
	}
}



/*private XmlDocument weaponsDoc = new XmlDocument();
	private XmlDocument shieldsDoc = new XmlDocument();
	private XmlDocument spellsDoc = new XmlDocument();
	private XmlDocument statusesDoc = new XmlDocument();*/


/*weaponsDoc.Load(Application.dataPath + "/Resources/XML/weapons.xml");
		shieldsDoc.Load(Application.dataPath + "/Resources/XML/shields.xml");
		spellsDoc.Load(Application.dataPath + "/Resources/XML/spells.xml");
		statusesDoc.Load(Application.dataPath + "/Resources/XML/statuses.xml");*/
