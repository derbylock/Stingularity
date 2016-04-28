using UnityEngine;
using System.Collections;

public class ItemSpawn : MonoBehaviour {

	public bool spawned;

	float time=0f;
	float spawnTime;

	void Start() {
		Reset();
	}

	// Update is called once per frame
	void Update () {
		if (!spawned) {
			time += Time.deltaTime;
		} if (time > spawnTime) {
			if(Manager.S.itemPickups)
				SpawnPickup (Random.Range (1, 5));
			else 
				SpawnPickup(5);
			Reset();
		}
		if (Time.time % 3 < 0.1) {
			GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0,0, Extensions.Rand * 360) * Vector2.up * 10;
		}
	}

	void Reset() {
		spawnTime = Random.Range(10, 40);
		GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0,0, Extensions.Rand * 360) * Vector2.up * 10;
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
