using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

	public float currentHealth = 0f;
	public float maxHealth = 100;
	public Slider healthBar;

	// Use this for initialization
	void Start () 
	{
		currentHealth = maxHealth;
		if (GameObject.Find("PlayerHealth") != null)
		{
			healthBar = GameObject.Find("PlayerHealth").GetComponent<Slider>();
			healthBar.value = currentHealth;
		}

	}

	public void TakeDmg(float dmg)
	{
		currentHealth -= dmg;

		if (healthBar != null)
		{
			healthBar.value = currentHealth;
		}

		if (currentHealth <= 0)
		{
			Death();
		}
	}

	void Death()
	{
		Destroy(GameObject.Find("UI"));
		SceneManager.LoadScene(0);
	}
}
