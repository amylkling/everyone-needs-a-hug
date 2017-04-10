using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//controls all misc. aspects of the game
//such as score and UI and other management things

public class GameControl : MonoBehaviour {

	public GameObject enemyTrainingPrefab;
	public GameObject enemyPrefab;
	public GameObject enemyGroup;
	private GameObject player;
	public Canvas mainCanvas;
	public int enemyRespawnCount = 4;
	public float spawnRadius = 5f;
	public Text scoreText;
	private float scoreTotal;
	bool trainingMode;

	// Use this for initialization
	void Start () 
	{
		enemyGroup = GameObject.FindGameObjectWithTag("Holder");
		player = GameObject.FindGameObjectWithTag("Player");
		scoreText = GameObject.Find("Score").GetComponent<Text>();
		scoreText.text = "Score: ";
		if (GameObject.Find("UI") != null)
		{
			trainingMode = GameObject.Find("UI").GetComponent<TrainingMode>().TrainingModeCheck;
		}

	}
	
	// Update is called once per frame
	void Update () 
	{
		enemyGroup = GameObject.FindGameObjectWithTag("Holder");

		if (enemyGroup.transform.childCount == 0)
		{
			if (player.GetComponent<PlayerCharacter>().GHugParticle != null)
			{
				Destroy(player.GetComponent<PlayerCharacter>().GHugParticle);
			}

			if (trainingMode != null && trainingMode)
			{
				for (int i = enemyRespawnCount; i > 0; i--)
				{
					Vector3 newPosition = Random.insideUnitSphere * spawnRadius + player.transform.position;
					newPosition.y = 0.7f;
					GameObject newEnemy = Instantiate(enemyTrainingPrefab, newPosition,
						Quaternion.identity, enemyGroup.transform);
					newEnemy.name = "Training Dummy " + i;
				}
			}
			else
			{
				for (int i = enemyRespawnCount; i > 0; i--)
				{
					Vector3 newPosition = Random.insideUnitSphere * spawnRadius * 2f + player.transform.position;
					newPosition.y = 0.7f;
					GameObject newEnemy = Instantiate(enemyPrefab, newPosition,
						Quaternion.identity, enemyGroup.transform);
					newEnemy.name = "Enemy " + i;
				}
			}
		}
	}

	public void Scoreboard(float score)
	{
		scoreTotal += score;
		scoreText.text = "Score: " + scoreTotal.ToString("0000");
	}
}
