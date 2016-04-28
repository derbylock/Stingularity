using UnityEngine;
using System.Collections;

public class LightScript : MonoBehaviour {
	
	int LightValue;
	float LightTime;
	
	public GameObject GlowObj;

	void Start () {
		LightValue = Random.Range(0, 10);
		LightTime = Time.time + Random.Range(0.05f, 0.1f);
		GlowObj.SetActive(GetComponent<Light>().enabled);
	}
	
	void Update () {
		LightValue = Random.Range(0, 10);
		if(Time.time > LightTime) {
			LightTime = Time.time + Random.Range(0.05f, 0.1f);
			if (LightValue == 0 || LightValue == 3 || LightValue == 9) {
				GetComponent<Light>().enabled = !GetComponent<Light>().enabled;
				GlowObj.SetActive(GetComponent<Light>().enabled);
			}
		}
	}
}
