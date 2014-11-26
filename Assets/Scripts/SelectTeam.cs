﻿using UnityEngine;
using System.Collections;

public class SelectTeam : MonoBehaviour {

	float timePassed = 0;
	float blinkTime = 0.75f;
	bool canPressStart = false;

	// Use this for initialization
	void Start () {
		for (int i=0; i < 4; ++i) {
			GameObject p = GameObject.FindGameObjectWithTag("player"+(i+1));
			p.renderer.enabled = false;
		}

		GameObject pS = GameObject.FindGameObjectWithTag("pressStart");
		pS.guiText.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {

		for (int i=0; i < 4; ++i) {
			// Everytime a player hits 'A', add a controller that 
			// they can move left and right to pick a team
			if (InputControl.S.ItemButtonDown(i, 1)) {
				GameObject p = GameObject.FindGameObjectWithTag("player"+(i+1));
				if(!p.renderer.enabled) {
					p.renderer.enabled = true;
					PlayerOptions.numPlayers++;
				}
				if(p.transform.position.x < 0 || p.transform.position.x > 0) {
					if(p.transform.position.x < 0) {
						PlayerOptions.playerColors[i] = "Red";
					} else {
						PlayerOptions.playerColors[i] = "Blue";
					}
					p.renderer.material.color = Color.grey;
					PlayerOptions.ready++;
				}
			}			
			if(Mathf.Abs(InputControl.S.RunVelocity(i).x) > 0.5) {
				GameObject p = GameObject.FindGameObjectWithTag("player"+(i+1));
				if(p.renderer.enabled && PlayerOptions.playerColors[i] == "") {
					Vector3 cursor = p.transform.position;
					cursor.x = 3 * Mathf.Sign(InputControl.S.RunVelocity(i).x);
					p.transform.position = cursor;
				}
			}
		}

		// Blink Press Start when teams are ready
		if(PlayerOptions.numPlayers >= 2 && (PlayerOptions.ready == PlayerOptions.numPlayers) ) {
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
