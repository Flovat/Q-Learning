using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CheckCanvasScript : MonoBehaviour {

	public Text textNumItems;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnEnable(){
		if (PlayerVRControllerLeap.getSingleton () != null)
			textNumItems.text = PlayerVRControllerLeap.getSingleton ().getNumItemsStored () + "/4";
	}
}
