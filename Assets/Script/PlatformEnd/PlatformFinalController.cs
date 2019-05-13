using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformFinalController : MonoBehaviour {

	private bool Item_1,Item_2,Item_3,Item_4;
	private GameObject I_1,I_2,I_3,I_4;
	public GameObject ParticleEffectPicked;

	void Start () {
		Item_1 = false; Item_2 = false; Item_3 = false; Item_4 = false;
		I_1 = null; I_2 = null; I_3 = null; I_4 = null;
	}
	
	void Update () {}
		

	void OnCollisionEnter(Collision collision){
		if (collision.gameObject.tag == "Item") {
			if (collision.gameObject.GetComponent<ItemScript> () != null) {
				switch (collision.gameObject.GetComponent<ItemScript> ().getTypeItem ()) {
				case ItemScript.TYPE_ITEM.DICE:
					I_1 = collision.gameObject;
					Item_1 = true;
					StartCoroutine (StoreItem(collision.gameObject.GetComponent<ItemScript> ().getTypeItem ()));
					break;
				case ItemScript.TYPE_ITEM.BALL:
					I_2 = collision.gameObject;
					Item_2 = true;
					StartCoroutine (StoreItem(collision.gameObject.GetComponent<ItemScript> ().getTypeItem ()));
					break;
				case ItemScript.TYPE_ITEM.DECORATION:
					I_3 = collision.gameObject;
					Item_3 = true;
					StartCoroutine (StoreItem(collision.gameObject.GetComponent<ItemScript> ().getTypeItem ()));
					break;
				case ItemScript.TYPE_ITEM.PUMPKIN:
					I_4 = collision.gameObject;
					Item_4 = true;
					StartCoroutine (StoreItem(collision.gameObject.GetComponent<ItemScript> ().getTypeItem ()));
					break;
				}
			}
		}
	}
		

	void OnCollisionExit(Collision collision){
		if (collision.gameObject.tag == "Item") {
			if (collision.gameObject.GetComponent<ItemScript> () != null) {
				switch (collision.gameObject.GetComponent<ItemScript> ().getTypeItem ()) {
				case ItemScript.TYPE_ITEM.DICE:
					I_1 = null;
					Item_1 = false;
					break;
				case ItemScript.TYPE_ITEM.BALL:
					I_2 = null;
					Item_2 = false;
					break;
				case ItemScript.TYPE_ITEM.DECORATION:
					I_3 = null;
					Item_3 = false;
					break;
				case ItemScript.TYPE_ITEM.PUMPKIN:
					I_4 = null;
					Item_4 = false;
					break;
				}
			}
		}
	}

	private IEnumerator StoreItem(ItemScript.TYPE_ITEM typeItem){
		yield return new WaitForSeconds (2f);
		switch (typeItem) {
		case ItemScript.TYPE_ITEM.DICE:
			//Segno l'item come registrato, notificando l'evento alla porta.
			if (Item_1) {
				CanvasExitController.getSingleton ().storeItem (typeItem);
				Instantiate (ParticleEffectPicked,I_1.transform.position,Quaternion.identity);
				Destroy (I_1);
			}
			break;
		case ItemScript.TYPE_ITEM.BALL:
			if (Item_2) {
				CanvasExitController.getSingleton ().storeItem (typeItem);
				Instantiate (ParticleEffectPicked,I_2.transform.position,Quaternion.identity);
				Destroy (I_2);
			}
			break;
		case ItemScript.TYPE_ITEM.DECORATION:
			if (Item_3) {
				CanvasExitController.getSingleton ().storeItem (typeItem);
				Instantiate (ParticleEffectPicked,I_3.transform.position,Quaternion.identity);
				Destroy (I_3);
			}
			break;
		case ItemScript.TYPE_ITEM.PUMPKIN:
			if (Item_4) {
				CanvasExitController.getSingleton ().storeItem (typeItem);
				Instantiate (ParticleEffectPicked,I_4.transform.position,Quaternion.identity);
				Destroy (I_4);
			}
			break;
		}
	}


}
