using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampCheckpoint : MonoBehaviour {

	public Light PointLight;
	public float intensity;

	void Start () {
		this.PointLight.intensity = 0f;	
	}
	
	void Update () {
		
	}

	void OnTriggerEnter(Collider collision){
		if (collision.gameObject.tag == "Player") {
			this.PointLight.intensity = intensity;
		}
	}
}
