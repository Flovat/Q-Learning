using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasExitController : MonoBehaviour {

	private static CanvasExitController singleton = null;

	private bool Item_1,Item_2,Item_3,Item_4,AlreadyOpenDoor;

	public GameObject ExitDoor;

	void Awake(){
		singleton = this;
	}

	public static CanvasExitController getSingleton(){
		return singleton;
	}

	void Start () {
		Item_1 = false;
		Item_2 = false;
		Item_3 = false;
		Item_4 = false;
		AlreadyOpenDoor = false;
	}
	
	void Update () {
		if (!AlreadyOpenDoor) {
			if (AllKeys ()) {
				//Apri la porta di fine gioco.
				OpenDoor();
			}
		}
	}

	public void storeItem(ItemScript.TYPE_ITEM typeItem){
		switch (typeItem) {
		case ItemScript.TYPE_ITEM.DICE:
			Item_1 = true;
			PlayerVRControllerLeap.getSingleton ().StoreItemExit (typeItem);
			break;
		case ItemScript.TYPE_ITEM.BALL:
			Item_2 = true;
			PlayerVRControllerLeap.getSingleton ().StoreItemExit (typeItem);
			break;
		case ItemScript.TYPE_ITEM.DECORATION:
			Item_3 = true;
			PlayerVRControllerLeap.getSingleton ().StoreItemExit (typeItem);
			break;
		case ItemScript.TYPE_ITEM.PUMPKIN:
			Item_4 = true;
			PlayerVRControllerLeap.getSingleton ().StoreItemExit (typeItem);
			break;
		}
	}

	public bool AllKeys(){
		return Item_1 && Item_2 && Item_3 && Item_4;
	}

	public void OpenDoor(){
		ExitDoor.GetComponent<Animator>().SetTrigger("Open");
	}

	public bool hasThisItem(ItemScript.TYPE_ITEM typeItem){
		switch (typeItem) {
		case ItemScript.TYPE_ITEM.DICE:
			if (Item_1)
				return true;
			break;
		case ItemScript.TYPE_ITEM.BALL:
			if (Item_2)
				return true;
			break;
		case ItemScript.TYPE_ITEM.DECORATION:
			if (Item_3)
				return true;
			break;
		case ItemScript.TYPE_ITEM.PUMPKIN:
			if (Item_4)
				return true;
			break;
		}
		return false;
	}
}
