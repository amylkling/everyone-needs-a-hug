using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour 
{

	GameObject player;						//reference player object
	NavMeshAgent agent;						//the NavMeshAgent attached to this enemy object
	bool moveIn = true;						//determines when the enemy can move towards the player
	public float backOffDist = 3f;			//how far back the enemy will go after hitting the player
	public float maxDist = 5f;				//the furthest the player can be before the enemy starts moving again
	public float backOffTime = 3f;			//how much time the enemy waits before attempting another attack
	private float backOffTimer = 0f;		//timer that counts down how long the enemy waits between attacks
	bool backOff = false;					//determines when the timer starts
	public float attackDist = 3f;


	// Use this for initialization
	void Start () 
	{
		player = GameObject.FindGameObjectWithTag("Player");
		agent = GetComponent<NavMeshAgent>();
		backOffTimer = backOffTime;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//ensure the enemy is always facing the player when not in a hug
		if (!GetComponent<Enemy>().Hugged)
		{
			transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
		}

		//move to/follow the player
		if (moveIn && !GetComponent<Enemy>().Dead)
		{
			agent.SetDestination(player.transform.position);
		}

		//when it gets in range, start preparing to attack
		if (Vector3.Distance(transform.position, player.transform.position) == attackDist)
		{
			agent.Stop();
			/*call attack function to "charge" a leap attack
			 *with flashing color that gets faster as a timer goes down
			 *Then call the actual attack function
			 *that makes it leap at the player and then stay still for a short time*/
			agent.Resume();
		}

		/*
		//back off when it gets too close
		if (agent.remainingDistance <= agent.stoppingDistance && !GetComponent<Enemy>().Hugged)
		{
			agent.Move(transform.forward*-1*backOffDist);
			agent.Stop();
			moveIn = false;
			if (!GetComponent<Enemy>().Dead)
			{
				backOff = true;
			}
		}*/

		/*attack and then leap back
		if (agent.remainingDistance <= agent.stoppingDistance)
		{
			agent.Move(transform.forward*-1*backOffDist);
			agent.Stop();
			moveIn = false;
		}*/

		/*
		//when the player gets too far away, move to them again
		if(Vector3.Distance(transform.position, player.transform.position) > maxDist)
		{
			agent.Resume();
			moveIn = true;
		}*/

		//wait a certain amount of time before moving in to attack again
		if (backOff)
		{
			backOffTimer -= Time.deltaTime;
			if (backOffTimer <= 0)
			{
				backOffTimer = backOffTime;
				agent.Resume();
				moveIn = true;
				backOff = false;
			}
		}

	}

	/*void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.tag == "Player")
		{
			agent.Move(Vector3.back);
		}
	}*/
}
