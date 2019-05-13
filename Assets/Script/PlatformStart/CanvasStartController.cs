using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasStartController : MonoBehaviour {

	private static CanvasStartController singleton = null;

	public GameObject Canvas;
	public GameObject Bongbay;
	public GameObject BongbayLR;
	public GameObject Fire_1;
	public GameObject Fire_2;

	void Awake(){
		singleton = this;
	}
	public static CanvasStartController getSinleton(){
		return singleton;
	}

	void Start () {
		
	}
	
	void Update () {
		/*if (PlayerVRControllerLeap.getSingleton () != null) {
			if (PlayerVRControllerLeap.getSingleton ().isStarted ()) {
			}
		}*/
	}

	public void StartGame(){
		Canvas.SetActive (false);
		Bongbay.SetActive (false);
		BongbayLR.SetActive (false);
		Fire_1.SetActive (false);
		Fire_2.SetActive (false);
		Invoke ("AnimationDoor", 2f);
	}

	private void AnimationDoor(){
		this.GetComponent<Animator> ().SetTrigger ("Open");
	}
}
