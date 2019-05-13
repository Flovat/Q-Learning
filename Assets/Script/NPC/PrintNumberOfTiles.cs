using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrintNumberOfTiles : MonoBehaviour
{

	public GameObject TileNumber;
	private Vector3 startPosition;

	private GameObject[] arrayTiles;

	public static PrintNumberOfTiles singleton = null;

	void Awake(){
		singleton = this;
		arrayTiles = new GameObject[AllData.X_Matrix * AllData.Y_Matrix];
		startPosition = new Vector3 (-23f,-23f, 0f);
		Vector3 pos = startPosition;
		int count = 0;
		for (int i = 0; i < AllData.X_Matrix; i++) {
			for (int j = 0; j < AllData.Y_Matrix; j++) {
				GameObject gm = (GameObject) Instantiate(TileNumber,new Vector3(i*2f + startPosition.x ,j*2f + startPosition.y,0f),Quaternion.identity);
				gm.transform.SetParent(this.gameObject.transform);
				gm.transform.position = new Vector3 (i * 2f + startPosition.x, 0f , j * 2f + startPosition.y);
				gm.transform.localRotation = new Quaternion (0f,0f,0f,0f);
				gm.GetComponent<TextMesh> ().text = count + "";
				if (count == 1 || count == 47) {
					gm.GetComponent<TextMesh> ().color = Color.green;
					gm.GetComponent<TextMesh> ().fontSize = 100;
				}
				arrayTiles [count] = gm;
				count++;
			}
		}
	}

    void Start()
    {
		
    }

    void Update()
    {
        
    }

	public void ShowTiles(bool show){
		for (int i = 0; i < AllData.X_Matrix * AllData.Y_Matrix; i++) {
			arrayTiles [i].gameObject.SetActive(show);
		}
	}
}
