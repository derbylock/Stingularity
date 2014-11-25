﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Bullet : MonoBehaviour
{
    public const float speed = 4f;
    public Vector2 heading = Vector2.up;

    void Update() {
        transform.position += (Vector3)heading.normalized * speed * Time.deltaTime;
        Destroy(gameObject, 5f);
    }

    void OnTriggerEnter2D(Collider2D coll) {
        Player p = coll.gameObject.GetComponent<Player>();
        if (p) {
            p.Die();
            Destroy(gameObject);
        } else if (coll.gameObject.tag == "Edge") {
            Destroy(gameObject);
        }
    }
}
