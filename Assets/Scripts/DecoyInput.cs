using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DecoyInput : InputControl
{
    public NavMeshPath path;
    public Vector2 tackleForce = Vector2.zero;

    public List<Vector3> waypoints;
    int pathIndex;
    Decoy decoy;
    Vector3 currentWaypoint {
        get {
            return (pathIndex < waypoints.Count && pathIndex >= 0)
                   ? waypoints[pathIndex] 
                   : decoy.transform.position;
        }
    }

    public DecoyInput(Decoy d) {
        decoy = d;
        path = new NavMeshPath();
    }

    public void UpdateWaypoints() {
        waypoints = path.corners.Select(v => v.convertXZToXY()).ToList();
        UpdateWaypoint();
    }

    void UpdateWaypoint() {
        pathIndex = waypoints
        .Select((pt, i) => i)
        .Aggregate(-1, (acc, next) => {
            float accDist = (acc == -1) ? 99999 : (decoy.transform.position - waypoints[acc]).magnitude;
            float nextDist = (decoy.transform.position - waypoints[next]).magnitude;
            return (nextDist < accDist) ? next : acc;
        });
    }

    public override Vector2 RunVelocity(int playerNum)
    {
        Vector2 toWaypoint = currentWaypoint - decoy.transform.position;
        if (toWaypoint.magnitude < 0.2f && waypoints.Count > 0) {
            waypoints.RemoveRange(0, pathIndex+1);
            UpdateWaypoint();
            toWaypoint = currentWaypoint - decoy.transform.position;
        }
        return toWaypoint.normalized * 0.9f;
    }

    public override Vector2 TackleVelocity(int playerNum)
    {
        return tackleForce;
    }

    public override bool ItemButtonDown(int playerNum, int itemNum)
    {
        return false;
    }
}
