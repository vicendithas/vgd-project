using UnityEngine;
using System.Collections;

public class SpikeTrap : MonoBehaviour {

	public Animator spikeAnim;

	// Use this for initialization
	void Start ()
	{
		spikeAnim = transform.GetComponentInChildren<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" || other.tag == "Enemy")
		{
			spikeAnim.SetTrigger("triggerProximity");
			this.GetComponent<AudioSource>().Play();
		}
	}
}
