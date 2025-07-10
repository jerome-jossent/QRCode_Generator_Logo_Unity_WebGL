using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using ZXing;

public class QRCode_Decoder : MonoBehaviour
{
    [SerializeField]
    string lastResult = "";

    WebCamTexture camTexture;
    Color32[] cameraColorData;
    Rect screenRect;

    bool finalWebcamSetup;

    // create a reader with a custom luminance source
    IBarcodeReader barcodeReader = new BarcodeReader
    {
        AutoRotate = false,
        Options = new ZXing.Common.DecodingOptions
        {
            TryHarder = false
        }
    };

    Result result;


    public string Decode(WebCamTexture camTexture)
    {
        if (camTexture != null && camTexture.isPlaying)
        {
            // waiting for the browser to finally acknowledge the webcam
            if (camTexture.width < 100 && camTexture.height < 100)
            {
                return "";
            }

            if (!finalWebcamSetup)
            {
                cameraColorData = new Color32[camTexture.width * camTexture.height];
                screenRect = new Rect(0, 0, camTexture.width, camTexture.height);
                finalWebcamSetup = !finalWebcamSetup;
            }

            camTexture.GetPixels32(cameraColorData); // -> performance heavy method 
            result = barcodeReader.Decode(cameraColorData, camTexture.width, camTexture.height); // -> performance heavy method
            if (result != null)
            {
                lastResult = result.Text + " " + result.BarcodeFormat;
                print(lastResult);
            }

            return lastResult;
        }

        return "";
    }

    public void Decode(Texture2D texture)
    {
        if (texture != null)
        {
            // waiting for the browser to finally acknowledge the webcam
            if (texture.width < 100 && texture.height < 100)
            {
                return;
            }

            if (!finalWebcamSetup)
            {
                cameraColorData = new Color32[texture.width * texture.height];
                screenRect = new Rect(0, 0, texture.width, texture.height);
                finalWebcamSetup = !finalWebcamSetup;
            }

            cameraColorData = texture.GetPixels32(); // -> performance heavy method 
            result = barcodeReader.Decode(cameraColorData, texture.width, texture.height); // -> performance heavy method
            if (result != null)
            {
                lastResult = result.Text + " " + result.BarcodeFormat;
                print(lastResult);
            }
        }
    }

}
