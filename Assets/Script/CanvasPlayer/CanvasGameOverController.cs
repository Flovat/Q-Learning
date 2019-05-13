using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasGameOverController : MonoBehaviour {

    private static CanvasGameOverController singleton = null;

    public Image LoadingImage;
    public Text LoadingText;
    public float SecondsToWait;

    private IEnumerator coroutineTimer;
   

    private void Awake()
    {
        singleton = this;
    }

    public static CanvasGameOverController getSingleton()
    {
        return singleton;
    }
    void Start () {
        StartLoading();
	}
	
	void Update () {
        
    }

    void StartLoading()
    {

        StartCoroutine(Timer(0.1f));
    }

    private IEnumerator Timer(float time)
    {
        while (LoadingImage.fillAmount<1f)
        {
         
        
            LoadingImage.fillAmount += (1f/(10f * SecondsToWait));
            LoadingText.text = Mathf.Floor(LoadingImage.fillAmount * SecondsToWait) + "" ;

            yield return new WaitForSeconds(0.1f);

        }
        //EndLoading();
        
    }

    void EndLoading()
    {
        StopCoroutine(coroutineTimer);
        LoadingImage.fillAmount = 0f;
        //StartLoading();
    }
}
