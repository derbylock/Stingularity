using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Decoy : DropItem
{
    bool drawPath = false;
    public PhysicsMaterial2D physMat;
    Player fakePlayer;
    DecoyInput input;
    LineRenderer line;
    public Vector2 target = new Vector2 (-1000, -1000);

    public override bool TryDrop(Player p) {
        base.TryDrop(p);
        
        line = GetComponent<LineRenderer>();
        lifeTime = 8f;
        CreateFakePlayer(p);
		AudioManager.Main.PlayNewSound ("robot");
        return true;
    }

    public void CreateFakePlayer(Player p) {
        GameObject go = Instantiate(owner.gameObject) as GameObject;
        fakePlayer = go.GetComponent<Player>();

        fakePlayer.GetComponent<Collider2D>().sharedMaterial = physMat;
        fakePlayer.canRespawn = false;
        fakePlayer.currentBoost = 1f;
        fakePlayer.flag = null;
        fakePlayer.inputCtrl = input = new DecoyInput(this);
        fakePlayer.spriteRenderer.sprite = Resources.Load(owner.team+"Drone", typeof(Sprite)) as Sprite;
        Manager.S.allPlayers.Add(fakePlayer);
    }

    public new void Update() {
        base.Update();

        if (fakePlayer.dead) {
            Deactivate();
        }

        LockToFakePlayer();
        FindTarget();
        if (fakePlayer.tackling) {
            input.tackleForce = Vector2.zero; //avoid infinite tackle
        }
    }

    void LockToFakePlayer() {
        transform.position = fakePlayer.transform.position;
    }

    void FindTarget() {
        Flag flag = Manager.S.flag;
        Vector2 newTarget = (flag.carrier == null || flag.carrier.team != fakePlayer.team) ? flag.transform.position
                          : flag.carrier != fakePlayer ? RandomEnemy().transform.position
                          : Manager.S.teamBases[fakePlayer.team].transform.position;

        //if new target is different enough or fakePlayer is
        //not at target & doesn't know where to go, recalc path
        if ((target - newTarget).magnitude > 0.5f ||
            (((Vector2)fakePlayer.transform.position - newTarget).magnitude > 0.5 && input.waypoints.Count == 0)) {
            CalculateNewPath(newTarget);
            target = newTarget;
        }
    }

    void CalculateNewPath(Vector3 t) {
        NavMesh.CalculatePath(transform.position.convertXYToXZ(), t.convertXYToXZ(), -1, input.path);
        if (drawPath) {
            line.SetVertexCount(input.path.corners.Length+1);
            line.SetPosition(0, transform.position);
            input.path.corners.Each((pt, i) => {
                line.SetPosition(i+1, pt.convertXZToXY());
            });
        }
        input.UpdateWaypoints();
    }

    Player RandomEnemy() {
        List<Player> enemies = Manager.S.allPlayers.Where(p => p.team != fakePlayer.team).ToList();
        int randIndex = (int)(StingExtensions.Rand * enemies.Count);
        return enemies[randIndex];
    }

    public override void Deactivate() {
        Manager.S.allPlayers.Remove(fakePlayer);
        if (!fakePlayer.dead) {
            fakePlayer.Die();
            Destroy(fakePlayer.gameObject, 1);
        }
        base.Deactivate();
    }

    void OnTriggerStay2D(Collider2D coll){
        Player p = coll.GetComponent<Player>();
        if (p != null && p.team != fakePlayer.team) {
            if (p.dead || p.invincible) {
                input.tackleForce = Vector2.zero;
            } else {
                //tackle him
                input.tackleForce = (p.transform.position - fakePlayer.transform.position).normalized;
            }
        }
    }
}
