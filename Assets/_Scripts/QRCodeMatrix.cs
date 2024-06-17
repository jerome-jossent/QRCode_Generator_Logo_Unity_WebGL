using QRCoder;

public class QRCodeMatrix
{
    public bool[,] data = null;

    public QRCodeMatrix(QRCodeData qr_m)
    {
        var m = qr_m.ModuleMatrix;
        data = new bool[m.Count, m[0].Count];
        for (int y = 0; y < m.Count; y++)
        {
            System.Collections.BitArray l = m[y];
            for (int x = 0; x < l.Length; x++)
                data[x, y] = l[x];
        }
    }

    public int width { get => data.GetLength(0); }
    public int height { get => data.GetLength(1); }

    public override string ToString()
    {
        string m = "";
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
                m += data[x, y] ? "1" : "0";
            m += "\n";
        }
        return m;
    }
}