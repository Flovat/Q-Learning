using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemControllerUI : MonoBehaviour {

	public ItemScript.TYPE_ITEM TYPE;
	public GameObject Sprite_Empty;
	public GameObject Sprite_Fill;
	public GameObject Sprite_Store;
	public MeshRenderer MRenderer;
	public Color ColorButtonFill;
	public Color ColorButtonEmpty;

	void Start () {
	}
	
	void Update () {
		if (PlayerVRControllerLeap.getSingleton () != null) {
			if (!PlayerVRControllerLeap.getSingleton ().hasStoreItem (TYPE)) {
				if (PlayerVRControllerLeap.getSingleton ().hasItem (TYPE)) {
					MRenderer.material.color = ColorButtonFill;
					Sprite_Fill.SetActive (true);
					Sprite_Empty.SetActive (false);
					Sprite_Store.SetActive (false);
				} else {
					MRenderer.material.color = ColorButtonEmpty;
					Sprite_Empty.SetActive (true);
					Sprite_Fill.SetActive (false);
					Sprite_Store.SetActive (false);
				}
			} else {
				MRenderer.material.color = ColorButtonEmpty;
				Sprite_Empty.SetActive (false);
				Sprite_Fill.SetActive (false);
				Sprite_Store.SetActive (true);
			}
		}
	}
	public ItemScript.TYPE_ITEM getTypeItem(){
		return this.TYPE;
	}
}
