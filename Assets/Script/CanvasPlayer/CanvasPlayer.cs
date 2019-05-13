using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasPlayer : MonoBehaviour {

	public GameObject GameOver;
	public GameObject LevelCompleted;

	public GameObject LoadingBackground;
	public GameObject LoadingImage;
	public GameObject LoadingText;
	public GameObject TextItemStored;

	private IEnumerator CoroutineLoading = null;
	   

    private void Awake(){
		GameOver.SetActive (false);
		LevelCompleted.SetActive (false);
		LoadingBackground.SetActive (false);
		LoadingImage.SetActive (false);
		LoadingText.SetActive (false);
		TextItemStored.SetActive (false);
    }
		
    void Start () {
	}
	
	void Update () {
        
    }

	public void StartLoading(float SecondsToWait)
	{
		LoadingBackground.SetActive (true);
		LoadingImage.SetActive (true);
		LoadingText.SetActive (true);
		CoroutineLoading = TimerLoading (0.1f, SecondsToWait);
		StartCoroutine(CoroutineLoading);
    }

	private IEnumerator TimerLoading(float time,float SecondsToWait)
    {
		while (LoadingImage.GetComponent<Image>().fillAmount<1f)
        {
			LoadingImage.GetComponent<Image>().fillAmount += (1f/(10f * SecondsToWait));
			LoadingText.GetComponent<Text>().text = (SecondsToWait - Mathf.Floor(LoadingImage.GetComponent<Image>().fillAmount * SecondsToWait)) + "" ;
            yield return new WaitForSeconds(0.1f);
        }
    }

	public void StopLoading()
    {
		if(CoroutineLoading != null)
			StopCoroutine(CoroutineLoading);
		CoroutineLoading = null;
		LoadingImage.GetComponent<Image>().fillAmount = 0f;
		LoadingBackground.SetActive (false);
		LoadingImage.SetActive (false);
		LoadingText.SetActive (false);
    }

	public void ShowItemText(){
		TextItemStored.SetActive (true);
	}
	public void HideItemText(){
		TextItemStored.SetActive (false);
	}

	public void ShowGameOverText(){
		GameOver.SetActive (true);
	}
	public void ShowLevelCompletedText(){
		LevelCompleted.SetActive (true);
	}
}
