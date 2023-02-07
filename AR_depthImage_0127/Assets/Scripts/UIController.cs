using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;



public class UIController : MonoBehaviour
{

    [SerializeField] private AROcclusionManager occlusionManager;
    [SerializeField] private GameObject focusIcon;
    private bool isScanReady;


    void Start()
    {
        isScanReady = false;
    }

    void Update()
    {
        if (occlusionManager.humanStencilTexture != null && isScanReady == true)
        {
            isScanReady = false;
        }
        else if(occlusionManager.humanStencilTexture == null && isScanReady == false)
        {
            isScanReady = true;
            iTween.ValueTo(gameObject, iTween.Hash("from", 1, "to", 0, "time", 1f, "onupdate", "ChangeIconOpacity"));
        }
    }

    void ChangeIconOpacity(float o)
    {
        Debug.Log("here");
        focusIcon.GetComponent<CanvasGroup>().alpha = o;
    }
}
