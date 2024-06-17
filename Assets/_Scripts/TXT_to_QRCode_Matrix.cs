using QRCoder;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TXT_to_QRCode_Matrix : MonoBehaviour
{
    QRCodeGenerator encoder; // https://github.com/codebude/QRCoder
    public QRCodeMatrixEvent _NewMatrix;

    [System.Serializable]
    public class QRCodeMatrixEvent : UnityEvent<QRCodeMatrix>
    {
    }

    void Start()
    {
        encoder = new QRCodeGenerator();
    }

    public void _NewTXT(string text_input)
    {
        QRCodeData qrCodeData = encoder.CreateQrCode(text_input, QRCodeGenerator.ECCLevel.H, forceUtf8:true);
        QRCodeMatrix matrix = new QRCodeMatrix(qrCodeData);
        //Debug.Log(matrix);
        _NewMatrix.Invoke(matrix);
    }
}