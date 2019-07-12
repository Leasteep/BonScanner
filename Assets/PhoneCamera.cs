using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PhoneCamera : MonoBehaviour
{
    private bool camAvailable;
    private WebCamTexture backCam;
    private Texture defaultBackground;

    public RawImage background;
    public AspectRatioFitter fit;

    private void Start()
    {
        defaultBackground = background.texture;
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length < 1)
        {
            Debug.Log("No Camera available");
            camAvailable = false;
            return;
        }

        // only uses last found back camera...
        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isFrontFacing)
            {
                backCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
            }
        }

        if(backCam == null)
        {
            Debug.Log("No front facing camera found");
            return;
        }

        backCam.Play();
        background.texture = backCam;
        camAvailable = true;

    }

    private void Update()
    {
        if (!camAvailable)
            return;

        float ratio = (float)backCam.width / (float)backCam.height;
        fit.aspectRatio = ratio;

        // landscape vs. portrait mode
        float scaleY = backCam.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        int orient = -backCam.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
    }

    public void TakePhoto()  
    {

        Texture2D photo = new Texture2D(backCam.width, backCam.height);
        photo.SetPixels(backCam.GetPixels());
        photo.Apply();

        //Encode to a PNG
        byte[] bytes = photo.EncodeToPNG();
        File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "Bons", "picture.png"), bytes);
    }

}


