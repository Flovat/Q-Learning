using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour {

	public enum TYPE_ITEM{DICE,BALL,DECORATION,PUMPKIN};
	public TYPE_ITEM MY_TYPE_ITEM;
	public GameObject ParticleEffectPicked;

	private int PIN;
	private bool picked = false;

	private bool CoroutineAlreadyLaunched = false;
	private IEnumerator CoroutinePick = null;

	void Start () {
		this.PIN = 1;
	}
	
	void Update () {}

	public void setPicked(bool p){
		if (p && !CoroutineAlreadyLaunched) {
			CoroutineAlreadyLaunched = true;
			CoroutinePick = CountTimePick ();
			StartCoroutine (CoroutinePick);
		} else {
			if (CoroutinePick != null) {	
				StopCoroutine (CoroutinePick);
			}
			CoroutineAlreadyLaunched = false;
		}
		this.picked = p;
	}

	private IEnumerator CountTimePick(){
		yield return new WaitForSeconds (3.5f);
		//Oggetto preso e conservato.
		if (this.picked) {
			PlayerVRControllerLeap.getSingleton ().addItem (this.MY_TYPE_ITEM,this.PIN);
			Instantiate (ParticleEffectPicked,this.transform.position,Quaternion.identity);
			Destroy (this.gameObject);
		}
	}

	public bool isPicked(){
		return this.picked;
	}
	public int getPin(){
		return this.PIN;
	}

	public ItemScript.TYPE_ITEM getTypeItem(){
		return this.MY_TYPE_ITEM;
	}
}
