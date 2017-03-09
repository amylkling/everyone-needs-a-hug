using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour 
{

	GameObject player;
	NavMeshAgent agent;
	bool moveIn = true;
	public float backOffDist = 3f;
	public float maxDist = 5f;


	// Use this for initialization
	void Start () 
	{
		player = GameObject.FindGameObjectWithTag("Player");
		agent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));

		if (moveIn)
		{
			agent.SetDestination(player.transform.position);
		}

		if (agent.remainingDistance <= agent.stoppingDistance)
		{
			agent.Move(transform.forward*-1*backOffDist);
			agent.Stop();
			moveIn = false;
		}

		if(Vector3.Distance(transform.position, player.transform.position) > maxDist)
		{
			agent.Resume();
			moveIn = true;
		}

	}

	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.tag == "Player")
		{
			agent.Move(Vector3.back);
		}
	}
}
