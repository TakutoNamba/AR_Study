using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PostEffect : MonoBehaviour
{
    
    //[SerializeField] private Shader _shader;
    [SerializeField] private AROcclusionManager occlusionManager;
    [SerializeField] private Material _material;
    public bool isThereHuman;

    void Awake()
    {
        //_material = new Material(_shader);
    }

    void checkStencilTex()
    {
        Texture2D stencilTex = occlusionManager.humanStencilTexture;
        Color[] pixels = stencilTex.GetPixels();
        Color[] change_pixels = new Color[pixels.Length];
        for (int i = 0; i < pixels.Length; i++)
        {
            Color pixel = pixels[i];
            if(pixel.r != 0 && pixel.g != 0 && pixel.b != 0)
            {
                isThereHuman = true;
                break;
            }
        }
        isThereHuman = false;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if(occlusionManager.humanStencilTexture != null){
            checkStencilTex();
            if(isThereHuman)
            {
                _material.SetTexture("_StencilTex", occlusionManager.humanStencilTexture);
                Graphics.Blit(source, destination, _material);
            }

        }
    }

}