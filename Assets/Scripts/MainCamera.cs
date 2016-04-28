using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MainCamera : MonoBehaviour
{
    const float cameraEasing = 0.2f;
    const float maxCameraSize = 20f;
    const float minCameraSize = 6f;
	Vector3 opos;
	public Vector3 deltaPos;
 	
	void Start(){
		opos = transform.position;
	}

    void Update() {
        fitAllPlayersInCamera();
		deltaPos = transform.position - opos;
		opos = transform.position;
    }

    void fitAllPlayersInCamera() {
        List<Player> allPlayers = Manager.S.allPlayers;
        Vector2 padding = new Vector2(0.2f, 3f);

        float maxX = allPlayers.Max(p => p.transform.position.x) + padding.x;
        float maxY = allPlayers.Max(p => p.transform.position.y) + padding.y;
        float minX = allPlayers.Min(p => p.transform.position.x) - padding.x;
        float minY = allPlayers.Min(p => p.transform.position.y) - padding.y;

        float adjustedNewSize;
        if (!Manager.S.roundStarted) {
            adjustedNewSize = maxCameraSize;
        } else {
            float newSize = Mathf.Max(maxX - minX, maxY - minY) / 2f;
            adjustedNewSize = Mathf.Clamp(newSize, minCameraSize, maxCameraSize);
        }
        GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, adjustedNewSize, cameraEasing);

        Vector3 lastPos = GetComponent<Camera>().transform.position;
        Vector3 newPos = new Vector3(Mathf.Lerp(minX, maxX, 0.5f), Mathf.Lerp(minY, maxY, 0.5f), -10);
        GetComponent<Camera>().transform.position = Vector3.Lerp(lastPos, newPos, 0.1f);
    }
}
