using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;


public class UIController : MonoBehaviour
{

    [SerializeField] private AROcclusionManager occlusionManager;
    [SerializeField] private GameObject focusIcon;
    [SerializeField] private Text MainText;
    [SerializeField] private Text SubText;
    [SerializeField] private float waitingTime;
    private bool isScanReady;
    private bool isTextMain;
    public PostEffect postEffect;
  

    public enum APP_STATUS
    {
        PLAY,
        WAIT,    
        WARN,  
    }

    APP_STATUS status = APP_STATUS.WAIT;


    void Start()
    {
        isScanReady = true;
        waitingTime = 0;
    }

    void Update()
    {
        if (postEffect.isThereHuman && isScanReady == false)
        {
            isScanReady = true;
            iTween.ValueTo(gameObject, iTween.Hash("from", 1, "to", 0, "time", .1f, "onupdate", "ChangeIconOpacity"));
            iTween.ValueTo(gameObject, iTween.Hash("from", MainText.GetComponent<CanvasGroup>().alpha, "to", 0, "time", .1f, "onupdate", "ChangeMainTextOpacity"));
            iTween.ValueTo(gameObject, iTween.Hash("from", SubText.GetComponent<CanvasGroup>().alpha, "to", 0, "time", .1f, "onupdate", "ChangeSubTextOpacity"));
        }
        else if(!postEffect.isThereHuman && isScanReady == true)
        {
            isScanReady = false;
            iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", .1f, "onupdate", "ChangeIconOpacity"));
            iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", .1f, "onupdate", "ChangeMainTextOpacity"));
        }

        if(isScanReady == false)
        {
            if(waitingTime > 5 && waitingTime < 10 && status == APP_STATUS.WAIT)
            {
                status = APP_STATUS.WARN;
                StartCoroutine(ChangeMainToSub());
                //Debug.Log("MainToSub");
            }
            if(waitingTime >= 0 && waitingTime <= 5 && status == APP_STATUS.WARN)
            {
                status = APP_STATUS.WAIT;
                StartCoroutine(ChangeSubToMain());
                //Debug.Log("SubToMain");
            }

            if(waitingTime > 10)
            {
                waitingTime = 0;
            }

            waitingTime += Time.deltaTime;
        }
        else
        {
            waitingTime = 0;
            status = APP_STATUS.PLAY;
        }
    }

    IEnumerator ChangeMainToSub()
    {
        iTween.ValueTo(gameObject, iTween.Hash("from", 1, "to", 0, "time", .2f, "onupdate", "ChangeMainTextOpacity"));
        yield return new WaitForSeconds(0.6f);
        iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", .2f, "onupdate", "ChangeSubTextOpacity"));
    }

    IEnumerator ChangeSubToMain()
    {
        iTween.ValueTo(gameObject, iTween.Hash("from", 1, "to", 0, "time", .2f, "onupdate", "ChangeSubTextOpacity"));
        yield return new WaitForSeconds(0.6f);
        iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", .2f, "onupdate", "ChangeMainTextOpacity"));
    }

    void ChangeIconOpacity(float o)
    {
        focusIcon.GetComponent<CanvasGroup>().alpha = o;
    }

    void ChangeMainTextOpacity(float o)
    {
        MainText.GetComponent<CanvasGroup>().alpha = o;
    }
    void ChangeSubTextOpacity(float o)
    {
        SubText.GetComponent<CanvasGroup>().alpha = o;
    }
}
