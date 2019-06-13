using UnityEngine;
using System.Collections;

public class SpikeDamage : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" || other.tag == "Enemy")
		{
			other.gameObject.GetComponent<CharacterStats>().isAttackedBy(transform.gameObject);
			//Debug.Log (other.tag + " HIT for 1 damage");
		}
	}
	
	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Player" || other.tag == "Enemy")
		{
			other.gameObject.GetComponent<CharacterStats>().isAttackedBy(transform.gameObject);
			//Debug.Log (other.tag + " HIT for 1 damage");
		}
	}
}
