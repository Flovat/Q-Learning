using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollider : MonoBehaviour {



	void OnTriggerStay(Collider collision){
		if (collision.gameObject.tag == "Obstacle") {
			if (PlayerVRControllerLeap.getSingleton ().ControlHandVelocity () >= 1f) {
				collision.GetComponent<BoardObstacle> ().DestroyEffect ();
			}
		}
	}


}
