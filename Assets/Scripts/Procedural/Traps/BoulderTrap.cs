using UnityEngine;
using System.Collections;

public class BoulderTrap : MonoBehaviour {

	public GameObject boulderPrefab;

	private int numberOfBoulders;

	// Use this for initialization
	void Start ()
	{
		// each boulder trap has a set amount of boulders to throw out
		numberOfBoulders = Random.Range(1,5);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (numberOfBoulders > 0 && (other.tag == "Player" || other.tag == "Enemy"))
		{
			GameObject obj = (GameObject)Instantiate(boulderPrefab);
			obj.transform.position = transform.position + new Vector3(0f, 5f, 0f);
			obj.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-1f,2f), -10f, Random.Range (-1f,2f));
			numberOfBoulders--;
		}
		else if (other.tag == "Boulder")
		{
			other.GetComponent<Rigidbody>().velocity = 3f * Random.insideUnitSphere;
		}
	}
}
