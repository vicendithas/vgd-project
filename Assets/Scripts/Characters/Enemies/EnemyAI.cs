using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour {
		
	public float chaseSpeed;
	public float wanderSpeed;
	public float chaseTurnRate;
	public float wanderTurnRate;
	public float desiredAttackDistance;
	public float desiredAvoidDistance;
	
	public float FOVAngle; // 45 would mean 90 total FOV (45 on each side)
	public float FOVDistance; // how many units away in the FOV can they see
	public float autoSensePlayer;
	
	[HideInInspector]
	public Animator enemyAnimator;
	[HideInInspector]
	public bool targetIsFromMemory = false;
	[HideInInspector]
	public GameObject playerReference;
	[HideInInspector]
	public Vector3 currentTarget;
	[HideInInspector]
	public Vector3 targetDirection;
	[HideInInspector]
	private Vector3[] objectChecks = {new Vector3(0f,0f,1f), new Vector3(0f,0f,-1f), new Vector3(1f,0f,0f), new Vector3(-1f,0f,0f),
									new Vector3(1f,0f,1f), new Vector3(-1f,0f,-1f), new Vector3(1f,0f,-1f), new Vector3(-1f,0f,1f)};
	
	// moves enemy towards currentTarget
	public void moveToTarget(float speed, float turnRate)
	{
		if (speed == chaseSpeed)
			enemyAnimator.SetBool("isChasing", true);
		else
			enemyAnimator.SetBool("isChasing", false);
	
		transform.position = transform.position + speed*transform.forward * Time.deltaTime;
		
		targetDirection = currentTarget - transform.position;
		
		// calculate cross product between forward vector and vector connecting enemy and player
		if (Vector3.Cross(transform.forward, targetDirection).y < 0f)
			transform.Rotate(transform.up, -turnRate*Time.deltaTime);
		else
			transform.Rotate(transform.up, turnRate*Time.deltaTime);
		
		return;
	}
	
	// tries to keep some distance away from objects
	public void obstacleAvoidance()
	{
		// cast 8 raycasts out from enemy, looking for obstacles
		// if obstacles are found, make sure distance from them is greater than 0.2
		foreach (Vector3 dir in objectChecks)
		{
			RaycastHit hit = new RaycastHit();
			//Debug.DrawRay (transform.position, dir, Color.red, 0.5f);
			
			// performs raycast calcuations, 9 = wall layer
			if (Physics.Raycast(transform.position, dir, out hit, 1f, (1 << 9 | 1 << 11 | 1 << 12)))
			{
				//Debug.Log("Wall detected: " + dir);
				//Debug.DrawRay (transform.position, dir, Color.red, 0.5f);
				
				// if raycast hits wall that's closer than 0.5 units away
				if ((hit.transform.tag == "Wall" || hit.transform.tag == "Enemy" || hit.transform.tag == "Chest") && hit.distance < desiredAvoidDistance)
				{
					//Debug.Log("Pushing away from wall: " + dir);
					transform.position = transform.position - 0.7f*dir*Time.deltaTime;
				}
			}
		}
		
		return;
	}
	
	// return true or false based on if enemy has unobstructed LOS to player
	public bool canSeePlayer()
	{
		RaycastHit hit = new RaycastHit();
		Vector3 playerDirection = playerReference.transform.position - transform.position;
		
		// if player is in field of view and raycast isn't blocked
		if (Vector3.Angle(playerDirection, transform.forward) < FOVAngle || Vector3.Distance(playerReference.transform.position, transform.position) < autoSensePlayer)
		{
			// performs raycast calcuations, ~(1 << 8) is a layer mask saying we want to test every layer BUT 8, which is floor
			// if floor is not cut out of tests, enemies won't move since transform.position is at y = 0
			if (Physics.Raycast(transform.position, playerDirection, out hit, FOVDistance, ~(1 << 8 | 1 << 14)))
			{
				//Debug.DrawRay (transform.position, playerDirection, Color.red, 2);
				
				if (hit.transform.tag == "Player" || hit.transform.tag == "PlayerModel")
				{
					currentTarget = hit.transform.position;
					//targetDirection = playerDirection;
					
					targetIsFromMemory = false;
					
					return true;
				}
				else
					return false;
			}
		}
		
		return false;
	}
	
	// checks if enemy is within range and can actually attack player
	public bool canAttackPlayer()
	{
		if (Vector3.Distance(playerReference.transform.position, transform.position) < desiredAttackDistance)
			return true;
		else
			return false;
	}
	
	// checks for direct LOS to smellpoint
	public bool canSeeSmellPoint()
	{
		RaycastHit hit = new RaycastHit();;
		Vector3 smellPointDirection;
		
		foreach (SmellPoint item in playerReference.GetComponent<PlayerStats>().smellPoints)
		{
			smellPointDirection = item.point - transform.position;
			
			// if player is in field of view and raycast isn't blocked
			if (Vector3.Angle(smellPointDirection, transform.forward) < FOVAngle)
			{
				// 8 = floor, 11 = enemies, 12 = chests, 13 = items, 15 = traps
				if (Physics.Raycast(transform.position, smellPointDirection, out hit, FOVDistance, ~(1 << 8 | 1 << 11 | 1 << 12 | 1 << 13 | 1 << 15)))
				{
					//Debug.DrawRay (transform.position, smellPointDirection, Color.blue, 0.4f);
					if (hit.transform.tag == "SmellPoint")
					{
						// set target variables
						currentTarget = hit.transform.position;
						//targetDirection = smellPointDirection;
						
						// target might get deleted, so notify logic that it might go to a "memory" target
						targetIsFromMemory = true;
						
						return true;
					}
				}
			}
		}
		
		return false;
	}
	
	// checks for LOS to wanderpoint
	public bool canSeeWanderPoint()
	{
		
		return false;
	}
	
	// executes attack
	public void attackPlayer()
	{
		enemyAnimator.SetBool("isChasing", true);
		enemyAnimator.SetBool("isDead", false);
		enemyAnimator.SetTrigger("triggerAttack");
		
		return;
	}
	
	//void OnCollisionEnter(Collision other)
	void OnTriggerEnter(Collider other)
	{
		//if (other.transform.tag == "Player")
		if (other.tag == "Player")
		{
			//other.transform.GetComponent<PlayerStats>().isAttackedBy(transform.gameObject);
			other.GetComponent<PlayerStats>().isAttackedBy(transform.gameObject);
		}
	}
	
	
	// helper function to calculate absolute distance from enemy to target
	public float distanceToTarget()
	{
		return Vector3.Distance(transform.position, currentTarget);
	}
	
}
