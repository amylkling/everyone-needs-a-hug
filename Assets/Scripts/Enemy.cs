using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

	#region Variables
	public int minHealth = 0;
	public int maxHealth = 50;
	public float health;
	private bool dead;
	public Slider healthBar;
	public EnemyUI uiScript;
	public EnemyUIDirectControl uiControl;
	private bool finished = false;

	public float pullInSpeed = 2f;
	#endregion
	
	#region Start
	//initiate once object is ready
	void Start()
	{
		health = maxHealth;
		uiScript = gameObject.GetComponent<EnemyUI>();
		//healthBar = uiScript.healthSlider;
		healthBar.maxValue = maxHealth;
		uiControl = uiScript.uiScript;
	}
	#endregion
	

	#region Update
	// Update is called once per frame
	void Update()
	{
		//do things when the enemy "dies"
		if (dead)
		{
			Debug.Log("I don't want to fight you anymore");
			//Destroy (gameObject);
		}

		//do things when the enemy is finished off
		if (finished)
		{
			Debug.Log(gameObject.name + ": love is over!");

			//move the enemy towards the player, dramatically
			//do this with navmesh?
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			transform.LookAt(player.transform.position);
			float step = pullInSpeed * Time.deltaTime;
			Vector3.MoveTowards(transform.position, player.transform.position, step);

			/*
			RaycastHit hit;

			if (Physics.Raycast(transform.position, transform.forward, out hit))
			{
				if (hit.collider.tag == "Player")
				{
					
				}
			}*/

			Destroy(gameObject);
		}
	}
	#endregion
	
	#region TakeDamage
	//track the amount of damage taken by this enemy,
	//called by outside scripts like the player's
	public void TakeDmg(float amount)
	{
		health -= amount;
		healthBar.value = health;
		Debug.Log ("ARGHHHHH");
		if (health <= minHealth && !dead)
		{
			Death();
		}
	}
	#endregion
	
	#region Death
	//set the enemy's state to "dead"
	void Death()
	{
		dead = true;
	}
	#endregion

	//allow other scripts to check if the enemy is dead
	public bool Dead
	{
		get{return dead;}
	}

	//allow other scripts to set the enemy's state to "finished"
	public bool Finished
	{
		set{finished = value;}
	}
}
