using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//controls all misc. aspects of the game
//such as score and UI and other management things

public class GameControl : MonoBehaviour {

	public GameObject enemyTrainingPrefab;
	public GameObject enemyPrefab;
	public GameObject enemyGroup;
	private GameObject player;
	public Canvas mainCanvas;
	public int dummyRespawnCount = 4;
	public int enemyRespawnCount = 4;
	public float spawnRadius = 5f;
	public Text scoreText;
	private float scoreTotal;
	bool trainingMode;
	public GameObject gameOverPanel;
	private Pause pauseScript;
	private bool gameOver = false;
	public Text loseText;
	public Text winText;
	public Text finalScoreText;
	private int roundCountDown;
	private int roundCountUp;
	public int wavesNum = 2;
	public int healthReward = 30;				//amount of health to get back after a group hug
	public Slider gHugMeter;
	public Text waveNumText;

	// Use this for initialization
	void Start () 
	{
		enemyGroup = GameObject.FindGameObjectWithTag("Holder");
		player = GameObject.FindGameObjectWithTag("Player");
		scoreText = GameObject.Find("Score").GetComponent<Text>();
		scoreText.text = "Score: ";
		scoreTotal = 0;
		if (GameObject.Find("UI") != null)
		{
			trainingMode = GameObject.Find("UI").GetComponent<TrainingOptions>().TrainingMode;
			pauseScript = GameObject.Find("UI").GetComponent<Pause>();
		}

		if (!trainingMode)
		{
			gameOverPanel = GameObject.Find("GameOverPanel");
			winText = GameObject.Find("WinText").GetComponent<Text>();
			loseText = GameObject.Find("LoseText").GetComponent<Text>();
			finalScoreText = GameObject.Find("Score (Final)").GetComponent<Text>();
			gameOverPanel.SetActive(false);
		}
		else
		{
			//set up tutorial and story panels/texts
			//and the tutorial finished one too
		}
		gameOver = false;
		roundCountDown = wavesNum;
		gHugMeter = GameObject.Find("GroupHugMeter").GetComponent<Slider>();
		roundCountUp = 1;
		waveNumText = GameObject.Find("WaveNumber").GetComponent<Text>();
		waveNumText.text = "Wave: " + roundCountUp.ToString("D");
	}
	
	// Update is called once per frame
	void Update () 
	{
		//find the gameobject that holds all the enemies
		enemyGroup = GameObject.FindGameObjectWithTag("Holder");

		//keep tabs on if it's in training mode or not
		if (GameObject.Find("UI") != null)
		{
			trainingMode = GameObject.Find("UI").GetComponent<TrainingOptions>().TrainingMode;
		}

		//set the wave counter
		if (trainingMode)
		{
			waveNumText.text = "Wave: " + roundCountUp.ToString("D") + "/" + "∞";
		}
		else
		{
			waveNumText.text = "Wave: " + roundCountUp.ToString("D") + "/" + (wavesNum + 1).ToString("D");
		}

		//if there are no more enemies
		if (enemyGroup.transform.childCount == 0)
		{
			//destroy the group hug particle efect, if it still exists
			if (player.GetComponent<PlayerCharacter>().GHugParticle != null)
			{
				Destroy(player.GetComponent<PlayerCharacter>().GHugParticle);
			}

			gHugMeter.value = gHugMeter.minValue;

			//make sure the player is able to take damage again
			player.GetComponent<PlayerHealth>().Invincible = false;

			//reward the player with health
			if (player.GetComponent<PlayerHealth>().changeRegen)
			{
				player.GetComponent<PlayerHealth>().RegenHealthVar = false;
				player.GetComponent<PlayerHealth>().Heal(healthReward);
			}

			//if the enemies were training dummies, respawn the set amount of them
			if (trainingMode)
			{
				for (int i = dummyRespawnCount; i > 0; i--)
				{
					Vector3 newPosition = Random.insideUnitSphere * spawnRadius + player.transform.position;
					newPosition.y = 0.7f;
					GameObject newEnemy = Instantiate(enemyTrainingPrefab, newPosition,
						Quaternion.identity, enemyGroup.transform);
					newEnemy.name = "Training Dummy " + i;
				}
				roundCountUp++;
				waveNumText.text = "Wave: " + roundCountUp.ToString("D") + "/" + "∞";
			}
			else
			{
				//otherwise they are AI enemies, so respawn the set amount of them
				//as long as there are still rounds left
				//when all rounds are complete, the player wins
				if (roundCountDown > 0)
				{
					for (int i = enemyRespawnCount; i > 0; i--)
					{
						Vector3 newPosition = Random.insideUnitSphere * spawnRadius * 2f + player.transform.position;
						newPosition.y = 0.7f;
						GameObject newEnemy = Instantiate(enemyPrefab, newPosition,
							Quaternion.identity, enemyGroup.transform);
						newEnemy.name = "Enemy " + i;
					}
					roundCountUp++;
					waveNumText.text = "Wave: " + roundCountUp.ToString("D") + "/" + (wavesNum + 1).ToString("D");
					roundCountDown--;
				}
				else
				{
					GameOver(true);
				}
			}

			player.GetComponent<Huggles>().NoMore = false;
		}
	}

	//update the score UI
	public void Scoreboard(float score)
	{
		scoreTotal += score;
		scoreText.text = "Score: " + scoreTotal.ToString("0000");
	}

	//when the game is over, pause everything and display the game over screen
	//the bool determines which message appears
	public void GameOver(bool win)
	{
		gameOver = true;
		if(pauseScript != null)
		{
			pauseScript.Paused = true;
		}
		//Set time.timescale to 0, this will cause animations and physics to stop updating
		Time.timeScale = 0;
		//deactivate all UI then activate the game over panel
		for(int i = mainCanvas.transform.childCount; i > 0; i--)
		{
			mainCanvas.transform.GetChild(i-1).gameObject.SetActive(false);
			Debug.Log(i + ": " + mainCanvas.transform.GetChild(i-1).gameObject.name);
		}
		gameOverPanel.SetActive(true);
		//change message depending on circumstance of game over
		if (win)
		{
			winText.gameObject.SetActive(true);
			loseText.gameObject.SetActive(false);
		}
		else
		{
			winText.gameObject.SetActive(false);
			loseText.gameObject.SetActive(true);
		}
		//show final score
		finalScoreText.text = "Final " + scoreText.text;
	}

	//reset the game back to the main menu
	public void ResetGame()
	{
		if(pauseScript != null)
		{
			pauseScript.Paused = false;
		}
		if (GameObject.Find("UI") != null)
		{
			Destroy(GameObject.Find("UI"));
		}
		//Set time.timescale to 1, this will cause animations and physics to continue updating at regular speed
		Time.timeScale = 1;
		SceneManager.LoadScene(0);
	}

	//so other scripts can see if the game is over
	public bool CheckGameOver
	{
		get {return gameOver;}
	}
}
