using UnityEngine;
using System.Collections;

public class SpellDamage : MonoBehaviour {

	public GameObject explosionParticle;
	
	GameObject playerReference;
	
	// Use this for initialization
	void Start ()
	{
		playerReference = GameObject.FindWithTag("Player");
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" || other.tag == "SpellProjectile" || other.tag == "Floor")
		{
			return;
		}
		else if (other.tag == "Enemy")
		{			
			other.gameObject.GetComponent<EnemyStats>().isAttackedBy(playerReference.GetComponent<PlayerStats>().currentSpell);
			other.gameObject.GetComponentInChildren<Animator>().SetTrigger("triggerDamaged");
		}
		
		this.GetComponent<AudioSource>().Play ();
		
		transform.GetComponent<TrailRenderer>().time = 0.1f;
		GameObject obj = (GameObject)Instantiate(explosionParticle);
		obj.transform.position = transform.position;
		Destroy(obj, 1.0f);
		
		transform.GetComponent<Rigidbody>().velocity = new Vector3(0f,0f,0f);
	}
}
