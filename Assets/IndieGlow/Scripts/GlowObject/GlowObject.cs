using UnityEngine;
using System.Collections;

public class GlowObject : MonoBehaviour {
	
	private Material SourceMaterial;
	private int SourceLayer;
	public Material GlowMaterial;
	
	void Awake() {
		if(GlowMaterial != null) {
			GlowCamera.GlowObjects.Add(this);
		} else {
			Debug.Log("GameObject: <" + gameObject.name + "> using GlowObject.cs but missing the Glow Material.");
		}
	}
	
	void Start ()
	{
		SourceMaterial = gameObject.GetComponent<Renderer>().material;
		SourceLayer = gameObject.layer;
	}
	
	public Material GetSourceMaterial { get { return this.SourceMaterial; } }
	
	public int GetSourceLayer { get { return this.SourceLayer; } }

	void OnDestroy() {
		GlowCamera.GlowObjects.Remove(this);
	}	
}
