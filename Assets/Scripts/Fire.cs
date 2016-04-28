using UnityEngine;
using System.Collections;

public class Fire : DropItem {
	private Vector3 startPos;
	public Vector2 heading = Vector2.up;
	private float turnChance = 0.1f;
	private float speed = 4f;
	private float radius = 1f;
    public float safetime = 1f;

	void Start () {
		startPos = transform.position;
		lifeTime = 6f;
	}
	
	new void Update () {
		base.Update();
		Move();		
		if (time >= safetime)
			gameObject.GetComponent<Renderer>().material.color = Color.yellow;
	}

	void Move() {
		if (Random.value < turnChance) {
			heading = RandomDir();		
		}
		
		Vector3 newDir = (Vector3)((heading) * speed * Time.deltaTime);		
		if (!OutsideBounds(transform.position + newDir)) {
			transform.position += newDir;
		} else {
			transform.position -= newDir;
		}
			
	}

	Vector3 RandomDir() {
		System.Random rand = new System.Random();
		Vector3 heading = new Vector3(rand.Next(-1, 2), rand.Next(-1, 2), 0f);
		heading.Normalize();
		return heading;
	}

	bool OutsideBounds(Vector3 newPos) {
		if (Mathf.Abs(newPos.x - startPos.x) > radius) {
			return true;
		} else if (Mathf.Abs(newPos.y - startPos.y) > radius) {
			return true;
		} else return false;
	}

	void OnTriggerEnter2D (Collider2D coll) {
		Player player = coll.GetComponent<Player>();

		if (player && time >= safetime) {
			player.Die();
		}
	}
}
