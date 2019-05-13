using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour {

	public float TIME;

	// Use this for initialization
	void Start () {
		Invoke ("DestroyObjectMethod", TIME);
	}

	private void DestroyObjectMethod(){
		Destroy (this.gameObject);
	}

}
