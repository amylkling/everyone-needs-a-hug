using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//controls all misc. aspects of the game
//such as score and UI and other management things

public class GameControl : MonoBehaviour {

	public GameObject enemyPrefab;
	public GameObject enemyGroup;
	private GameObject player;
	public Canvas mainCanvas;
	public int enemyRespawnCount = 4;
	public float spawnRadius = 5f;

	// Use this for initialization
	void Start () 
	{
		enemyGroup = GameObject.FindGameObjectWithTag("Holder");
		player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (enemyGroup.transform.childCount == 0)
		{
			for (int i = enemyRespawnCount; i > 0; i--)
			{
				Vector3 newPosition = Random.insideUnitSphere * spawnRadius + player.transform.position;
				newPosition.y = 0.7f;
				GameObject newEnemy = Instantiate(enemyPrefab, newPosition,
					Quaternion.identity, enemyGroup.transform);
				newEnemy.name = "Enemy " + i;
			}
		}
	}
}
