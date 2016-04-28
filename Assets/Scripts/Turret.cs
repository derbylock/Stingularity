using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Turret : DropItem
{
    public const float secondsBetweenShots = 1f;
    public float secondsSinceLastShot = 0f;    
    private float range = 6f;
    private int numShots = 5;
    public bool dead = false;

    new void Update() {
        if (dead)
            return;

        base.Update();
        lifeTime = 30;
        Vector2 toTarget = ClosestEnemyPlayer().transform.position - transform.position;
        transform.rotation = toTarget.ToQuaternion();
        secondsSinceLastShot += Time.deltaTime;
        if (toTarget.magnitude < range &&
            secondsSinceLastShot > secondsBetweenShots) {
            Shoot(toTarget);
        }
    }

    void Shoot(Vector2 toTarget) {
        Vector2 heading = toTarget.normalized;
        Vector3 bulletStart = transform.position + (Vector3)(heading * transform.lossyScale.y) + (-1 * Vector3.forward);
        GameObject bulletGO = Instantiate(Resources.Load("Bullet"),
                                          bulletStart,
                                          transform.rotation) as GameObject;
        Bullet bullet = bulletGO.GetComponent<Bullet>();
        bullet.heading = heading;
        bullet.team = owner.team;
        secondsSinceLastShot = 0f;
        if (--numShots <= 0) {
            Destroy(gameObject);
        }
    }

    Player ClosestEnemyPlayer() {
        List<Player> players = (GameObject.FindObjectsOfType(typeof(Player)) as Player[]).ToList();     
        return players.Where(p => p.team != owner.team)
                      .Aggregate((closestSoFar, cur) => HowFar(cur) < HowFar(closestSoFar) ? cur : closestSoFar);
    }

    float HowFar(Player p) {
        return (transform.position - p.transform.position).magnitude;
    }

    void OnCollisionEnter2D(Collision2D coll){
        if (coll.relativeVelocity.magnitude > 3)
            Deactivate();
    }

    public override void Deactivate() {
        StartCoroutine(DieCoroutine());
    }

    public IEnumerator DieCoroutine()
    {
        dead = true;
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
