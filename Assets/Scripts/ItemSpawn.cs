﻿using UnityEngine;
using System.Collections;

public class ItemSpawn : MonoBehaviour {

	public bool spawned;

	float time=0f;
	public float spawnTime=20f;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!spawned)
			time += Time.deltaTime;
		if (time > spawnTime) {
			if(Manager.S.itemPickups)
				SpawnPickup (Random.Range (1, 5));
			else 
				SpawnPickup(5);
		}
	}

	void SpawnPickup(int number)
	{
		spawned = true;
		time = 0f;
		GameObject go = 
			Instantiate (Resources.Load ("Item" + number.ToString ())) as GameObject;
		go.transform.position = this.transform.position;
		go.GetComponent<ItemPickup> ().isp = this;
	}
}
