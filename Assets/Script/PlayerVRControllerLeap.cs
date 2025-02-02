﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Leap;
using Leap.Unity;

public class PlayerVRControllerLeap : MonoBehaviour
{

	private static PlayerVRControllerLeap singleton = null;


    private LeapProvider leapProvider;
	private bool Started = false;
	private bool ThreadStart = false;
	private bool RUN = false;
	private bool PALM_FACE_BACKWARD = false;
	Vector3 direction;

    private CharacterController controller;

    private float speed = 0.5f;

	public GameObject CanvasPlayer;
	public Light TorchLight;
	private float TorchLightIntensity;
	public GameObject AttachmentHands;
	public GameObject Item_1;
	public GameObject Item_2;
	public GameObject Item_3;
	public GameObject Item_4;
	public GameObject PointSpawnItem;
	public Transform startRayCast, endRayCast;
	public GameObject WeaponRight;
	public GameObject Ghost;
	private bool ClearScene = false;
	private bool TriggerWeapon = false;
	private bool WeaponAppear = false;
	private IEnumerator CoroutineStart = null;
	private IEnumerator CoroutineWeapon = null;


	private bool TriggerGameOver = false;
	private bool TriggerLevelCompleted = false;

	private Dictionary<ItemScript.TYPE_ITEM,int> ITEMS = null;

	void Awake(){
		singleton = this;
		TorchLightIntensity = TorchLight.intensity;
		TorchLight.intensity = 0f;
		ITEMS = new Dictionary<ItemScript.TYPE_ITEM,int>();
		ITEMS[ItemScript.TYPE_ITEM.BALL] = 0;
		ITEMS[ItemScript.TYPE_ITEM.DICE] = 0;
		ITEMS[ItemScript.TYPE_ITEM.DECORATION] = 0;
		ITEMS[ItemScript.TYPE_ITEM.PUMPKIN] = 0;
	}

	public static PlayerVRControllerLeap getSingleton(){
		return singleton;
	}

    // Use this for initialization
    void Start()
    {
		WeaponRight.SetActive (false);
        leapProvider =  FindObjectOfType < LeapProvider >() as  LeapProvider;
        controller = GetComponent<CharacterController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
		AttachmentHands.SetActive (false); 
		CanvasPlayer.SetActive (false);
    }

	private void ClearSceneMethod(){
		AttachmentHands.SetActive (false);
		WeaponRight.SetActive (false);
	}

    // Update is called once per frame
    void Update()
    {
		if (!TriggerLevelCompleted) {
			if (!TriggerGameOver) {
				if (Started) {
					if (!ClearScene) {
						DeactivateTimerCanvas (true);
						Invoke ("StartLight",1f);
						ClearScene = true;
					}
					ControlPlayerRunTime ();	
				} else {
					ControlPlayerStart (false);
				}
			} else {
				ActivateGameOverCanvas ();
				ControlPlayerStart (true);
			}
		} else {
			ActivateLevelCompletedCanvas ();
			ControlPlayerStart (true);
		}
    }

	//Controllo sullo start
	private void ControlPlayerStart(bool restart){
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None; 
		}
		CheckRayCast ();

		Frame frame = leapProvider.CurrentFrame;

		foreach (Hand hand in frame.Hands) {
			if (hand.IsRight) {
				if (hand.Fingers [0].IsExtended && !hand.Fingers [1].IsExtended && !hand.Fingers [2].IsExtended && !hand.Fingers [3].IsExtended && !hand.Fingers [4].IsExtended) {
					if (!ThreadStart) {
						ThreadStart = true;
						CoroutineStart = WaitToStart (restart);
						StartCoroutine (CoroutineStart);
						ActivateTimerCanvas ();
					}
				} else {
					if(CoroutineStart != null)
						StopCoroutine (CoroutineStart);
					CoroutineStart = null;
					DeactivateTimerCanvas (restart);
					ThreadStart = false;
					Started = false;
				}
			}
		}
		if (frame.Hands.Count == 0) {	
			if(CoroutineStart != null)
				StopCoroutine (CoroutineStart);
			CoroutineStart = null;
			DeactivateTimerCanvas (restart);		
			ThreadStart = false;
			Started = false;
		}

	}


	private void StartLight(){
		TorchLight.intensity = TorchLightIntensity;
	}

	private void ActivateTimerCanvas(){
		this.CanvasPlayer.SetActive (true);
		this.CanvasPlayer.GetComponent<CanvasPlayer> ().StartLoading (3f);
	}
	private void DeactivateTimerCanvas(bool d){
		this.CanvasPlayer.GetComponent<CanvasPlayer> ().StopLoading();
		this.CanvasPlayer.SetActive (d);
	}

	private void ActivateGameOverCanvas(){
		this.CanvasPlayer.SetActive (true);
		this.CanvasPlayer.GetComponent<CanvasPlayer> ().ShowGameOverText ();
	}
	private void ActivateLevelCompletedCanvas(){
		this.CanvasPlayer.SetActive (true);
		this.CanvasPlayer.GetComponent<CanvasPlayer> ().ShowLevelCompletedText ();
	}

	private IEnumerator WaitToStart(bool restart){
		yield return new WaitForSeconds(3f);
		if (ThreadStart) {
			if (!restart) {
				Started = true;
				CanvasStartController.getSinleton ().StartGame ();
			} else {
				//Restart
				SceneManager.LoadScene("SampleScene");
			}
		}
	}



	public bool isStarted(){
		return this.Started;
	}

	private void ControlPlayerRunTime(){

		if (Input.GetKeyDown (KeyCode.Escape)) {
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None; 
		}
		//MovePlayer();
		CheckRayCast ();
		//Glitch Torch
		//ControlTorchLight();

		Frame frame = leapProvider.CurrentFrame;

		foreach (Hand hand in frame.Hands) {
			if (hand.IsLeft) {
				if (!AttachmentHands.activeSelf)
					AttachmentHands.SetActive (true);
				if (!hand.Fingers [0].IsExtended && !hand.Fingers [1].IsExtended && !hand.Fingers [2].IsExtended && !hand.Fingers [3].IsExtended && !hand.Fingers [4].IsExtended) {
					//Fermati
					RUN = false;
				} else {
					//Cammina verso dove si guarda.
					if (hand.Fingers [0].IsExtended && hand.Fingers [1].IsExtended && hand.Fingers [2].IsExtended && hand.Fingers [3].IsExtended && hand.Fingers [4].IsExtended) {
						if (this.PALM_FACE_BACKWARD) {
							RUN = true;
						} else {
							RUN = false;
						}
					}
				}
			}
			if (hand.IsRight) {
				//Pollice in su, attiva modalità arma.
				if (hand.Fingers [0].IsExtended && !hand.Fingers [1].IsExtended && !hand.Fingers [2].IsExtended && !hand.Fingers [3].IsExtended && !hand.Fingers [4].IsExtended) {
					if (!WeaponRight.GetComponentInChildren<MeshRenderer> ().isVisible && !TriggerWeapon && Started) {
						TriggerWeapon = true;
						CoroutineWeapon = ShowWeapon (2f);
						StartCoroutine (CoroutineWeapon);
					}
				} else {
					if(CoroutineWeapon != null)
						StopCoroutine (CoroutineWeapon);
					TriggerWeapon = false;
					WeaponAppear = false;
					WeaponRight.SetActive (false);
					CoroutineWeapon = null;
				}
			}
		}

		//Se non ci sono mani identificate allora setto a false l'arma.
		if (frame.Hands.Count != 0) {	
			if (WeaponAppear && TriggerWeapon) {
				WeaponRight.SetActive (true);
			}
		} else {
			if(CoroutineWeapon != null)
				StopCoroutine (CoroutineWeapon);
			CoroutineWeapon = null;
			TriggerWeapon = false;
			WeaponAppear = false;
			AttachmentHands.SetActive (false);
		}

		//Controllo sul moviemento
		if (!this.PALM_FACE_BACKWARD)
			RUN = false;
		//Muovo il player usando il Leap.
		MovePlayerUsingLeap ();	
	}
		


	//Controllo la velocità della mano per il colpo fisico.
	public float ControlHandVelocity(){

		Frame frame = leapProvider.CurrentFrame;

		foreach (Hand hand in frame.Hands)
		{
			if (WeaponAppear) {
				if (hand.IsRight) {
					//Debug.Log ("Velocity Magnitude: "+hand.PalmVelocity.Magnitude);
					return hand.PalmVelocity.Magnitude;	
				}	
			}
				
		}	
		return 0f;
	}


	//Metodo per visualizzare l'arma dopo un periodo di tempo.
	private IEnumerator ShowWeapon(float time){
		yield return new WaitForSeconds (time);
		if (TriggerWeapon) {
			WeaponAppear = true;
		}
	}


    private void CheckRayCast()
    {
        RaycastHit2D hit = Physics2D.Raycast(startRayCast.position, Vector3.forward - startRayCast.position, Vector3.Distance(startRayCast.position, endRayCast.position), 1 << LayerMask.NameToLayer("Player"));

		direction = endRayCast.position - startRayCast.position;
        
		Debug.DrawLine(startRayCast.position, endRayCast.position, Color.red);

        if (hit.collider != null && hit.collider.tag == "Player")
        {
            //do something
        }
    }

	private void MovePlayerUsingLeap(){
		Vector3 velocity;
		if (RUN) {
			velocity = speed * new Vector3(direction.x,0f,direction.z);
		} else {
			velocity = new Vector3(0, 0, 0);
		}
		controller.Move (velocity * Time.deltaTime);
	}
		
	public void addItem(ItemScript.TYPE_ITEM type,int pin){
		this.ITEMS [type] = pin;
		Invoke ("ShowCanvasItemStoredPlayer", 0.5f); 
		Invoke ("HideCanvasItemStoredPlayer", 2.5f);
	}
	public void removeItem(ItemScript.TYPE_ITEM type){
		this.ITEMS [type] = 0;
	}
	//Metodo che rilascia l'oggetto dall'inventario.
	public void leaveItem(ItemScript.TYPE_ITEM type){
		if (this.ITEMS [type] == 1) {
			removeItem (type);
			switch (type) {
			case ItemScript.TYPE_ITEM.DICE:
				Instantiate (Item_1,PointSpawnItem.transform.position,Quaternion.identity);
				break;
			case ItemScript.TYPE_ITEM.BALL:
				Instantiate (Item_2,PointSpawnItem.transform.position,Quaternion.identity);
				break;
			case ItemScript.TYPE_ITEM.DECORATION:
				Instantiate (Item_3,PointSpawnItem.transform.position,Quaternion.identity);
				break;
			case ItemScript.TYPE_ITEM.PUMPKIN:
				Instantiate (Item_4,PointSpawnItem.transform.position,Quaternion.identity);
				break;
			}
		}
	}

	public int getNumItemsStored(){
		int count = 0;
		foreach (KeyValuePair<ItemScript.TYPE_ITEM,int> aux in ITEMS) {
			if (aux.Value != 0)
				count++;
		}
		return count;
	}

	public bool hasItem(ItemScript.TYPE_ITEM type){
		return (this.ITEMS[type] == 1) ? true : false;
	}
	public bool hasStoreItem(ItemScript.TYPE_ITEM type){
		return (this.ITEMS[type] == 2) ? true : false;
	}
	public int getItemPin(ItemScript.TYPE_ITEM type){
		return this.ITEMS [type];
	}

	public void setPalmFace(bool p){
		this.PALM_FACE_BACKWARD = p;
	}

	public void StoreItemExit(ItemScript.TYPE_ITEM type){
		this.ITEMS [type] = 2;
	}

	public void ShowCanvasItemPlayer(){
		this.CanvasPlayer.SetActive (true);
		this.CanvasPlayer.GetComponent<CanvasPlayer> ().StartLoading (3f);
	}
	public void HideCanvasItemPlayer(){
		this.CanvasPlayer.GetComponent<CanvasPlayer> ().StopLoading ();
		this.CanvasPlayer.SetActive (false);
	}
	public void ShowCanvasItemStoredPlayer(){
		this.CanvasPlayer.SetActive (true);
		this.CanvasPlayer.GetComponent<CanvasPlayer> ().ShowItemText ();
	}
	public void HideCanvasItemStoredPlayer(){
		this.CanvasPlayer.GetComponent<CanvasPlayer> ().HideItemText ();
		this.CanvasPlayer.SetActive (false);
	}

	void OnTriggerEnter(Collider collision){
		if (collision.gameObject.tag == "Enemy") {
			ClearSceneMethod ();
			TriggerGameOver = true;
		}
		if (collision.gameObject.tag == "EndLevel") {
			ClearSceneMethod ();
			TriggerLevelCompleted = true;
		}
	}
}
