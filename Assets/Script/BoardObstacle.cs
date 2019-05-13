using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardObstacle : MonoBehaviour {

	public ParticleSystem particle;
	public GameObject PointEffect;

	public void DestroyEffect(){
		Debug.Log ("Creo l'effetto particelle.");
		Instantiate (particle,PointEffect.transform.position,Quaternion.identity);
		Destroy (this.gameObject);
	}
}
