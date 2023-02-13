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

        // Create a temporary RenderTexture of the same size as the texture
        RenderTexture tmp = RenderTexture.GetTemporary(
                            stencilTex.width,
                            stencilTex.height,
                            0,
                            RenderTextureFormat.Default,
                            RenderTextureReadWrite.Linear);


        // Blit the pixels on texture to the RenderTexture
        Graphics.Blit(stencilTex, tmp);


        // Backup the currently set RenderTexture
        RenderTexture previous = RenderTexture.active;


        // Set the current RenderTexture to the temporary one we created
        RenderTexture.active = tmp;


        // Create a new readable Texture2D to copy the pixels to it
        Texture2D myTexture2D = new Texture2D(stencilTex.width, stencilTex.height);


        // Copy the pixels from the RenderTexture to the new Texture
        myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
        myTexture2D.Apply();


        // Reset the active RenderTexture
        RenderTexture.active = previous;


        // Release the temporary RenderTexture
        RenderTexture.ReleaseTemporary(tmp);


        // "myTexture2D" now has the same pixels from "texture"


        Color[] pixels = myTexture2D.GetPixels();
        Color[] change_pixels = new Color[pixels.Length];
        for (int i = 0; i < pixels.Length; i++)
        {
            Color pixel = pixels[i];
            if (pixel.r != 0 || pixel.g != 0 || pixel.b != 0)
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
            Debug.Log(isThereHuman);
            if(isThereHuman)
            {
                _material.SetTexture("_StencilTex", occlusionManager.humanStencilTexture);
                Graphics.Blit(source, destination, _material);
            }

        }
    }

}