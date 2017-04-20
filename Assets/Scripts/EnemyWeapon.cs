using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour {

	public float dmgAmount = 20f;
	public bool constDmg = false;
	public bool doDmg = true;

	void OnCollisionEnter(Collision col)
	{
		if (col.collider.gameObject.CompareTag("Player"))
		{
			Debug.Log("it's a direct hit!");
			if (doDmg)
			{
				col.gameObject.GetComponent<PlayerHealth>().TakeDmg(dmgAmount);
			}
		}
	}

	void OnCollisionStay(Collision col)
	{
		
		if (col.collider.gameObject.CompareTag("Player") && constDmg)
		{
			Debug.Log("Oh no it's poisoned!");
			if (doDmg)
			{
				col.gameObject.GetComponent<PlayerHealth>().TakeDmg(dmgAmount);
			}
		}
	}
}
