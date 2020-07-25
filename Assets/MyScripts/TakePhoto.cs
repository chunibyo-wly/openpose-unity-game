using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.Windows.WebCam;

public class TakePhoto : MonoBehaviour
{
    public WebCamTexture webCamTexture;
    public RawImage background;

    void OnEnable()
    {
        webCamTexture = new WebCamTexture();
        GetComponent<Renderer>().material.mainTexture =
            webCamTexture; //Add Mesh Renderer to the GameObject to which this script is attached to
        webCamTexture.Play();
        background.texture = webCamTexture;
    }

    IEnumerator TakePhotos() // Start this Coroutine on some button click
    {
        // NOTE - you almost certainly have to do this here:

        yield return new WaitForEndOfFrame();

        // it's a rare case where the Unity doco is pretty clear,
        // http://docs.unity3d.com/ScriptReference/WaitForEndOfFrame.html
        // be sure to scroll down to the SECOND long example on that doco page 

        Texture2D photo = new Texture2D(webCamTexture.width, webCamTexture.height);
        photo.SetPixels(webCamTexture.GetPixels());
        photo.Apply();

        //Encode to a PNG
        var bytes = photo.EncodeToPNG();
        //Write out the PNG. Of course you have to substitute your_path for something sensible
        File.WriteAllBytes("photo.png", bytes);
    }

    private void Update()
    {
    }
}