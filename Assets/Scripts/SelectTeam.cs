using UnityEngine;
using System.Collections;

public class SelectTeam : MonoBehaviour {

	float timePassed = 0;
	float blinkTime = 0.25f;
	bool canPressStart = false;

	// Use this for initialization
	void Start () {
		for (int i=0; i < PlayerOptions.maxPlayers; ++i) {
			GameObject p = GameObject.FindGameObjectWithTag("player"+(i+1));
			p.renderer.enabled = false;
		}

		GameObject pS = GameObject.FindGameObjectWithTag("pressStart");
		pS.guiText.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {

		for (int i=0; i < PlayerOptions.maxPlayers; ++i) {
			// Everytime a player hits 'A', add a controller that 
			// they can move left and right to pick a team
			if (InputControl.S.ItemButtonDown(i, 1)) {
				GameObject p = GameObject.FindGameObjectWithTag("player"+(i+1));
				if(!p.renderer.enabled) {
					p.renderer.enabled = true;
					PlayerOptions.numPlayers++;
				} else if (PlayerOptions.teamForPlayer.ContainsKey(i)) {
					PlayerOptions.teamForPlayer.Remove(i);
					p.renderer.material.color = Color.white;
				} else if (p.transform.position.x != 0) {
					PlayerOptions.teamForPlayer[i] = (p.transform.position.x < 0) ? "Red" : "Blue";
					p.renderer.material.color = Color.grey;
				}
			}
			if (Mathf.Abs(InputControl.S.RunVelocity(i).x) > 0.5) {
				GameObject p = GameObject.FindGameObjectWithTag("player"+(i+1));
				if(p.renderer.enabled && !PlayerOptions.teamForPlayer.ContainsKey(i)) {
					Vector3 cursor = p.transform.position;
					cursor.x = 3 * Mathf.Sign(InputControl.S.RunVelocity(i).x);
					p.transform.position = cursor;
				}
			}
		}

		// Blink Press Start when teams are ready
		if(PlayerOptions.numPlayers >= 2 && (PlayerOptions.teamForPlayer.Count == PlayerOptions.numPlayers) ) {
			canPressStart = true;
			timePassed += Time.deltaTime;
			
			if(timePassed > blinkTime) {
				
				GameObject pS = GameObject.FindGameObjectWithTag("pressStart");
				pS.guiText.enabled = !pS.guiText.enabled;
				timePassed = 0;
			}
		}

		// Press Start Handler
		if ((Input.GetButtonDown("start") || Input.GetKeyDown(KeyCode.Return)) && canPressStart) {
			Application.LoadLevel("_Scene1");
		}
	}
}
