using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Bullet : MonoBehaviour
{
    public const float speed = 5f;
    public Vector2 heading = Vector2.up;
    public string team;

    void Start() {
        GetComponent<Rigidbody2D>().velocity = (Vector3)heading.normalized * speed;
		AudioManager.Main.PlayNewSound ("turret");
    }

    void Update() {
        Destroy(gameObject, 5f);
    }

    void OnTriggerEnter2D(Collider2D coll) {
        Player p = coll.gameObject.GetComponent<Player>();
        if (p && p.team != team) {
            p.Die();
            Destroy(gameObject);
        } else if (coll.gameObject.tag == "Edge") {
            Destroy(gameObject);
        }
    }
}
