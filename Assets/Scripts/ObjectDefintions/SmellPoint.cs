using UnityEngine;
using System.Collections;

public class SmellPoint : Object
{
	public Vector3 point;
	public float lifetime;
	
	public GameObject smellPointObject;
	public GameObject smellPointPrefab = (GameObject)Resources.Load ("Prefabs/Testing/smellPointPrefab");

	// constructor
	public SmellPoint(Vector3 point, float lifetime)
	{
		this.point = point;
		this.lifetime = lifetime;
		this.smellPointObject = (GameObject)Instantiate(smellPointPrefab, point, new Quaternion());
	}
}
