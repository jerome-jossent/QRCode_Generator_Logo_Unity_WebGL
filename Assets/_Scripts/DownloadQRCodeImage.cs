using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class DownloadQRCodeImage : MonoBehaviour
{
    public QRCode_Matrix_to_QRCode_Image qr_image;

    [DllImport("__Internal")]
    private static extern void DownloadFile(byte[] array, int byteLength, string fileName);

    public void _Download()
    {
        byte[] textureBytes = qr_image.texture.EncodeToPNG();
        DownloadFile(textureBytes, textureBytes.Length, "QRCode.png");
    }
}
