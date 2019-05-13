using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasExitItemController : MonoBehaviour {

	public ItemScript.TYPE_ITEM TYPE_ITEM;
	public Color FillColor;
	public Color EmptyColor;
	public Image Background;
	public GameObject Fill;
	public GameObject Empty;

	void Start () {
		
	}
	
	void Update () {
		if (CanvasExitController.getSingleton ().hasThisItem (TYPE_ITEM)) {
			Fill.SetActive (true);
			Empty.SetActive (false);
			Background.color = FillColor;
		} else {
			Fill.SetActive (false);
			Empty.SetActive (true);
			Background.color = EmptyColor;	
		}
	}
}
