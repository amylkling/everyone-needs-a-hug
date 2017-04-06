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
	public float attackDistLow = 3f;
	public float attackDistHigh = 4f;
	private float chargeTimer = 0f;
	public float chargeTime = 5f;
	private bool chargeUp = true;
	public float firstThreshold = 3f;
	public float secondThreshold = 1f;
	public Color flashCol;
	private Color baseCol;
	private Vector3 curPos;
	private bool theLoop = false;
	private bool navEngage = false;
	private float coolTimer = 0f;
	public float coolDownTime = 5f;
	private bool pathPlacement = true;
	private bool attEngage = true;


	// Use this for initialization
	void Start () 
	{
		player = GameObject.FindGameObjectWithTag("Player");
		agent = GetComponent<NavMeshAgent>();
		backOffTimer = backOffTime;
		baseCol = GetComponent<Renderer>().material.color;
		coolTimer = coolDownTime;
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

		float dist = Vector3.Distance(transform.position, player.transform.position);

		//Debug.Log(dist);

		//recalculate leap path, unless already leaping
		if(pathPlacement)
		{
			Debug.Log("YOU'RE MINE NOW!");
			Vector3 newPos = Vector3.Lerp(transform.position, player.transform.position, 0.5f);
			GetComponent<iTweenPath>().nodes[0] = transform.position;
			GetComponent<iTweenPath>().nodes[1] = new Vector3(newPos.x, 3f, newPos.z);
			GetComponent<iTweenPath>().nodes[2] = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
		}


		//when it gets in range, start preparing to attack
		if (dist > attackDistLow && dist <= attackDistHigh && attEngage)
		{
			//agent.Stop();
			agent.enabled = false;
			moveIn = false;
			navEngage = false;
			/*call attack function to "charge" a leap attack
			 *with flashing color that gets faster as a timer goes down
			 *Then call the actual attack function
			 *that makes it leap at the player and then stay still for a short time*/
			curPos = transform.position;
			ChargeUp();
			//agent.Resume();
		}

		//when the leap is done, call activateNav
		if (V3Equal(transform.position, GetComponent<iTweenPath>().nodes[2]))
		{
			Debug.Log("called it");
			activateNav();
		}

		//reactivate navigation
		if (navEngage)
		{
			agent.enabled = true;
			moveIn = true;
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

		//leap cooldown
		if (theLoop)
		{
			coolTimer -= Time.deltaTime;
			Debug.Log("Timer: " + coolTimer);
			if (coolTimer <= 0)
			{
				attEngage = true;
				navEngage = true;
				coolTimer = coolDownTime;
				theLoop = false;
			}
		}

		if(chargeUp)
		{
			chargeTimer -= Time.deltaTime;
			if (chargeTimer <= chargeTime && chargeTimer > firstThreshold)
			{
				//flash at a slow speed
				//Debug.Log("Time: " + chargeTimer + " HRAA-");
				ChargeUp("slow", 1.5f);
				//Debug.Log("HRAA-");
			}
			else if (chargeTimer <= firstThreshold && chargeTimer > secondThreshold)
			{
				//flash at a faster speed
				//iTween.ColorTo(gameObject, flashCol, 1f);
				//Debug.Log("Time: " + chargeTimer + " -AAAAA-");
				ChargeUp("faster", 1f);
				//Debug.Log("-AAAAA-");
			}
			else if (chargeTimer <= secondThreshold && chargeTimer > 0)
			{
				//flash at fastest speed
				//iTween.ColorTo(gameObject, flashCol, 0.5f);
				//Debug.Log("Time: " + chargeTimer + " -AAAAAAAAAAAA");
				ChargeUp("fastest", 0.5f);
				//Debug.Log("-AAAAAAAAAAAA");
			}
			else if (chargeTimer <= 0)
			{
				chargeUp = false;
				chargeTimer = chargeTime;
				Attack();
			}
		}
			
	}

	//prepare to attack
	void ChargeUp()
	{
		//function to "charge" a leap attack
		//with flashing color that gets faster as a timer goes down
		chargeUp = true;
	}

	void ChargeUp(string howFast, float speed)
	{
		switch(howFast)
		{
			case "slow":
				Debug.Log("slow" + Time.time);
				iTween.ColorTo(gameObject, flashCol, speed);
				return;
			case "faster":
				Debug.Log("faster" + Time.time);
				iTween.ColorTo(gameObject, flashCol, speed);
				return;
			case "fastest":
				Debug.Log("fastest" + Time.time);
				iTween.ColorTo(gameObject, flashCol, speed);
				return;
		}
	}

	//leap at the player
	void Attack()
	{
		//if it hasn't leaped yet or isn't cooling down from a leap
		if (!theLoop)
		{
			Debug.Log("HYUP");
			attEngage = false;
			pathPlacement = false;
			//iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath("jumper"), "time", 2f, "easetype", iTween.EaseType.linear, "oncomplete", "activateNav", "oncompletetarget", gameObject, "looptype", iTween.LoopType.none));
			iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath("jumper"), "time", 2f, "easetype", iTween.EaseType.linear, "looptype", iTween.LoopType.none));
		}

	}

	//activate leap cooldown and reactivates dynamic pathing
	void activateNav()
	{
		Debug.Log("stahp");
		theLoop = true;
		pathPlacement = true;
	}

	//compare two vector3's
	public bool V3Equal(Vector3 a, Vector3 b)
	{
		//Debug.Log("sqr mag: " + Vector3.SqrMagnitude(a - b));
		return Vector3.SqrMagnitude(a - b) < 0.1;
	}

	/*void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.tag == "Player")
		{
			agent.Move(Vector3.back);
		}
	}*/
}
