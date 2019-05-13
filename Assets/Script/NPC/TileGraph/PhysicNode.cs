using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicNode : MonoBehaviour {

	private bool Walkable = true;

	void OnTriggerEnter(Collider collision){
		if (collision.gameObject.tag == "Wall") {
			//Debug.Log ("Non percorribile "+this.gameObject.name);
			this.Walkable = false;
		}
	}

	void OnTriggerStay(Collider collision){
		if (collision.gameObject.tag == "Wall") {
			//Debug.Log ("Non percorribile "+this.gameObject.name);
			this.Walkable = false;
		}
	}

	public bool isWalkable(){
		return this.Walkable;
	}
}
