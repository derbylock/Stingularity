using UnityEngine;
using System.Collections;

public class StartScreen : MonoBehaviour {

	float timePassed = 0;
	float blinkTime = 0.75f;

	// Update is called once per frame
	void Update () {

		timePassed += Time.deltaTime;

		if(timePassed > blinkTime) {
			
			GameObject pS = GameObject.FindGameObjectWithTag("pressStart");
			pS.guiText.enabled = !pS.guiText.enabled;
			timePassed = 0;
		}

		if(Input.GetButtonDown("start") || Input.GetKeyDown(KeyCode.Return)) {
			Application.LoadLevel("_SelectTeam");
		}
	}
}
