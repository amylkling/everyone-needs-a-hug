using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleControl : MonoBehaviour {

	private float timer = 0f;
	public float aliveTime = 3f;

	// Use this for initialization
	void Awake () {
		timer = aliveTime;
	}
	
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;
		if (timer <= 0)
		{
			Destroy(gameObject);
		}
	
	}
}
