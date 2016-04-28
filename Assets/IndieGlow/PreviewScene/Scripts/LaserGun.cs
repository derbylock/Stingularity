using UnityEngine;
using System.Collections;

public class LaserGun : MonoBehaviour {
	
	public GameObject LaserPrefab;
	public AudioClip LaserShootClip;
	private float FireRate = 0.2f;
	private float NextFireTime;
	
	public Texture2D crosshair;
	
	void Update () {
		if(!GlowCamera.GlowMenuActive()) {
			if(Input.GetMouseButton(0)) {
				if(Time.time >= NextFireTime) {
					Instantiate(LaserPrefab, transform.position, transform.rotation);
					GetComponent<AudioSource>().PlayOneShot(LaserShootClip);
					NextFireTime = Time.time + FireRate;
				}
			}
		}
	}
	
	void OnGUI()
    {
		GUI.DrawTexture(new Rect((Screen.width / 2) - (crosshair.width / 2), (Screen.height / 2) - (crosshair.height / 2), crosshair.width, crosshair.height), crosshair);
	}
}
