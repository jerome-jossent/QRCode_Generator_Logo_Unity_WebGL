using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using UnityEngine;
using UnityEngine.UI;

public class QRCode_Matrix_to_QRCode_Image : MonoBehaviour
{
    public RawImage rawImage;
    public Texture2D texture;

    public int facteur = 10;
    public int marge = 3;

    public Color couleurFond = new Color(0, 0, 0, 0.5f);
    public Color couleurPixel = new Color(0.7f, 0, 0.7f, 1);

    QRCodeMatrix current_matrix;

    private void OnValidate() { Draw(); }

    void Draw()
    {
        if (current_matrix != null)
            _NewMatrix(current_matrix);
    }

    public void _SetColorFond(Color color) { couleurFond = color; Draw(); }
    public void _SetColorPixel(Color color) { couleurPixel = color; Draw(); }
    public void _SetFactorSize(float val) { facteur = (int)val; Draw(); }
    public void _SetMarge(float val) { marge = (int)val; Draw(); }

    public void _NewMatrix(QRCodeMatrix matrix)
    {
        //Debug.Log(matrix);
        current_matrix = matrix;

        bool[,] d = matrix.data;

        Scalar couleurFond = 255 * new Scalar(this.couleurFond.r, this.couleurFond.g, this.couleurFond.b, this.couleurFond.a);
        Scalar couleurPixel = 255 * new Scalar(this.couleurPixel.r, this.couleurPixel.g, this.couleurPixel.b, this.couleurPixel.a);

        Mat mat = new Mat(matrix.width * facteur + marge * 2,
                          matrix.height * facteur + marge * 2,
                          CvType.CV_8UC4,
                          couleurFond);

        // Dessiner quelque chose sur la matrice
        for (int x = 0; x < matrix.width; x++)
        {
            for (int y = 0; y < matrix.height; y++)
            {
                if (d[x, y])
                {
                    //rectangle ABCD
                    Point A = new Point(x * facteur + marge, y * facteur + marge);
                    Point D = new Point((x + 1) * facteur + marge, (y + 1) * facteur + marge);

                    Imgproc.rectangle(mat, A, D, couleurPixel, -1);

                    //Imgproc.rectangle(mat, new Point(50, 50), new Point(200, 200), new Scalar(0, 255, 0), -1);
                    //Imgproc.circle(mat, new Point(128, 128), 50, new Scalar(255, 0, 0), -1);
                }
            }
        }

        // Convertir la matrice en Texture2D
        texture = new Texture2D(mat.cols(), mat.rows(), TextureFormat.ARGB32, false);
        Utils.matToTexture2D(mat, texture);

        // Assigner la texture au RawImage
        rawImage.texture = texture;
    }
}
