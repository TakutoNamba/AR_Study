using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class TestDepthImage : MonoBehaviour
{
    [SerializeField] private Shader _shader;
    [SerializeField] private AROcclusionManager occlusionManager;
    private Material _material;

    // Start is called before the first frame update
    void Awake()
    {
        _material = new Material(_shader);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if(occlusionManager != null)
        {
            _material.SetTexture("_StencilTex", occlusionManager.humanStencilTexture);
        }
        Graphics.Blit(source, destination, _material);
    }
}
