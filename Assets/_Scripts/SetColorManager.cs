using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class SetColorManager : MonoBehaviour
{
    public FlexibleColorPicker flexibleColorPicker;
    public QRCode_Matrix_to_QRCode_Image qrCodeImage;
    public UnityEngine.UI.Image btn_fond;
    public UnityEngine.UI.Image btn_pixel;

    enum Color_type { aucun, fond, pixel };
    Color_type type = Color_type.aucun;

    public void Start()
    {
        btn_fond.color = qrCodeImage.couleurFond;
        btn_pixel.color = qrCodeImage.couleurPixel;
    }

    public void _PixelSelect()
    {
        type = Color_type.pixel;
        flexibleColorPicker.color = qrCodeImage.couleurPixel;
    }

    public void _FondSelect()
    {
        type = Color_type.fond;
        flexibleColorPicker.color = qrCodeImage.couleurFond;
    }

    public void _ChangeColor(UnityEngine.Color color)
    {
        switch (type)
        {
            case Color_type.aucun: return;
            case Color_type.fond:
                qrCodeImage._SetColorFond(color);
                btn_fond.color = qrCodeImage.couleurFond;
                break;
            case Color_type.pixel:
                qrCodeImage._SetColorPixel(color);
                btn_pixel.color = qrCodeImage.couleurPixel; break;
        }
    }
}
