﻿
#if !NETSTANDARD1_3
using System;
using System.Drawing;
using static QRCoder.QRCodeGenerator;

namespace QRCoder
{
    /// <summary>
    /// Represents a QR code generator that outputs QR codes as PostScript code.
    /// </summary>
    public class PostscriptQRCode : AbstractQRCode, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostscriptQRCode"/> class.
        /// Constructor without parameters to be used in COM objects connections.
        /// </summary>
        public PostscriptQRCode() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostscriptQRCode"/> class with the specified <see cref="QRCodeData"/>.
        /// </summary>
        /// <param name="data"><see cref="QRCodeData"/> generated by the QRCodeGenerator.</param>
        public PostscriptQRCode(QRCodeData data) : base(data) { }

        /// <summary>
        /// Creates a black and white PostScript code representation of the QR code.
        /// </summary>
        /// <param name="pointsPerModule">The number of points each dark/light module of the QR code will occupy in the final QR code image.</param>
        /// <param name="epsFormat">Indicates if the output should be in EPS format.</param>
        /// <returns>Returns the QR code graphic as a PostScript string.</returns>
        public string GetGraphic(int pointsPerModule, bool epsFormat = false)
        {
            var viewBox = new Size(pointsPerModule * this.QrCodeData.ModuleMatrix.Count, pointsPerModule * this.QrCodeData.ModuleMatrix.Count);
            return this.GetGraphic(viewBox, Color.Black, Color.White, true, epsFormat);
        }

        /// <summary>
        /// Creates a colored PostScript code representation of the QR code.
        /// </summary>
        /// <param name="pointsPerModule">The number of points each dark/light module of the QR code will occupy in the final QR code image.</param>
        /// <param name="darkColor">The color of the dark modules.</param>
        /// <param name="lightColor">The color of the light modules.</param>
        /// <param name="drawQuietZones">Indicates if quiet zones around the QR code should be drawn.</param>
        /// <param name="epsFormat">Indicates if the output should be in EPS format.</param>
        /// <returns>Returns the QR code graphic as a PostScript string.</returns>
        public string GetGraphic(int pointsPerModule, Color darkColor, Color lightColor, bool drawQuietZones = true, bool epsFormat = false)
        {
            var viewBox = new Size(pointsPerModule * this.QrCodeData.ModuleMatrix.Count, pointsPerModule * this.QrCodeData.ModuleMatrix.Count);
            return this.GetGraphic(viewBox, darkColor, lightColor, drawQuietZones, epsFormat);
        }

        /// <summary>
        /// Creates a colored PostScript code representation of the QR code.
        /// </summary>
        /// <param name="pointsPerModule">The number of points each dark/light module of the QR code will occupy in the final QR code image.</param>
        /// <param name="darkColorHex">The color of the dark modules in HTML hex format.</param>
        /// <param name="lightColorHex">The color of the light modules in HTML hex format.</param>
        /// <param name="drawQuietZones">Indicates if quiet zones around the QR code should be drawn.</param>
        /// <param name="epsFormat">Indicates if the output should be in EPS format.</param>
        /// <returns>Returns the QR code graphic as a PostScript string.</returns>
        public string GetGraphic(int pointsPerModule, string darkColorHex, string lightColorHex, bool drawQuietZones = true, bool epsFormat = false)
        {
            var viewBox = new Size(pointsPerModule * this.QrCodeData.ModuleMatrix.Count, pointsPerModule * this.QrCodeData.ModuleMatrix.Count);
            return this.GetGraphic(viewBox, darkColorHex, lightColorHex, drawQuietZones, epsFormat);
        }

        /// <summary>
        /// Creates a black and white PostScript code representation of the QR code.
        /// </summary>
        /// <param name="viewBox">The dimensions of the viewbox for the QR code.</param>
        /// <param name="drawQuietZones">Indicates if quiet zones around the QR code should be drawn.</param>
        /// <param name="epsFormat">Indicates if the output should be in EPS format.</param>
        /// <returns>Returns the QR code graphic as a PostScript string.</returns>
        public string GetGraphic(Size viewBox, bool drawQuietZones = true, bool epsFormat = false)
        {
            return this.GetGraphic(viewBox, Color.Black, Color.White, drawQuietZones, epsFormat);
        }

        /// <summary>
        /// Creates a colored PostScript code representation of the QR code.
        /// </summary>
        /// <param name="viewBox">The dimensions of the viewbox for the QR code.</param>
        /// <param name="darkColorHex">The color of the dark modules in HTML hex format.</param>
        /// <param name="lightColorHex">The color of the light modules in HTML hex format.</param>
        /// <param name="drawQuietZones">Indicates if quiet zones around the QR code should be drawn.</param>
        /// <param name="epsFormat">Indicates if the output should be in EPS format.</param>
        /// <returns>Returns the QR code graphic as a PostScript string.</returns>
        public string GetGraphic(Size viewBox, string darkColorHex, string lightColorHex, bool drawQuietZones = true, bool epsFormat = false)
        {
            return this.GetGraphic(viewBox, ColorTranslator.FromHtml(darkColorHex), ColorTranslator.FromHtml(lightColorHex), drawQuietZones, epsFormat);
        }

        /// <summary>
        /// Creates a colored PostScript code representation of the QR code.
        /// </summary>
        /// <param name="viewBox">The dimensions of the viewbox for the QR code.</param>
        /// <param name="darkColor">The color of the dark modules.</param>
        /// <param name="lightColor">The color of the light modules.</param>
        /// <param name="drawQuietZones">Indicates if quiet zones around the QR code should be drawn.</param>
        /// <param name="epsFormat">Indicates if the output should be in EPS format.</param>
        /// <returns>Returns the QR code graphic as a PostScript string.</returns>
        public string GetGraphic(Size viewBox, Color darkColor, Color lightColor, bool drawQuietZones = true, bool epsFormat = false)
        {
            var offset = drawQuietZones ? 0 : 4;
            var drawableModulesCount = this.QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : offset * 2);
            var pointsPerModule = (double)Math.Min(viewBox.Width, viewBox.Height) / (double)drawableModulesCount;

            string psFile = string.Format(psHeader, new object[] {
                DateTime.Now.ToString("s"), CleanSvgVal(viewBox.Width), CleanSvgVal(pointsPerModule),
                epsFormat ? "EPSF-3.0" : string.Empty
            });
            psFile += string.Format(psFunctions, new object[] {
                CleanSvgVal(darkColor.R /255.0), CleanSvgVal(darkColor.G /255.0), CleanSvgVal(darkColor.B /255.0),
                CleanSvgVal(lightColor.R /255.0), CleanSvgVal(lightColor.G /255.0), CleanSvgVal(lightColor.B /255.0),
                drawableModulesCount
            });

            for (int xi = offset; xi < offset + drawableModulesCount; xi++)
            {
                if (xi > offset)
                    psFile += "nl\n";
                for (int yi = offset; yi < offset + drawableModulesCount; yi++)
                {
                    psFile += (this.QrCodeData.ModuleMatrix[xi][yi] ? "f " : "b ");
                }
                psFile += "\n";
            }
            return psFile + psFooter;
        }

        /// <summary>
        /// Cleans double values for international use/formats.
        /// </summary>
        /// <param name="input">The input double value.</param>
        /// <returns>Returns the cleaned string representation of the double value.</returns>
        private string CleanSvgVal(double input)
        {
            return input.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        private const string psHeader = @"%!PS-Adobe-3.0 {3}
%%Creator: QRCoder.NET
%%Title: QRCode
%%CreationDate: {0}
%%DocumentData: Clean7Bit
%%Origin: 0
%%DocumentMedia: Default {1} {1} 0 () ()
%%BoundingBox: 0 0 {1} {1}
%%LanguageLevel: 2 
%%Pages: 1
%%Page: 1 1
%%EndComments
%%BeginConstants
/sz {1} def
/sc {2} def
%%EndConstants
%%BeginFeature: *PageSize Default
<< /PageSize [ sz sz ] /ImagingBBox null >> setpagedevice
%%EndFeature
";

        private const string psFunctions = @"%%BeginFunctions 
/csquare {{
    newpath
    0 0 moveto
    0 1 rlineto
    1 0 rlineto
    0 -1 rlineto
    closepath
    setrgbcolor
    fill
}} def
/f {{ 
    {0} {1} {2} csquare
    1 0 translate
}} def
/b {{ 
    1 0 translate
}} def 
/background {{ 
    {3} {4} {5} csquare 
}} def
/nl {{
    -{6} -1 translate
}} def
%%EndFunctions
%%BeginBody
0 0 moveto
gsave
sz sz scale
background
grestore
gsave
sc sc scale
0 {6} 1 sub translate
";

        private const string psFooter = @"%%EndBody
grestore
showpage   
%%EOF
";
    }

    /// <summary>
    /// Provides static methods for creating PostScript QR codes.
    /// </summary>
    public static class PostscriptQRCodeHelper
    {
        /// <summary>
        /// Creates a PostScript QR code with a single function call.
        /// </summary>
        /// <param name="plainText">The text or payload to be encoded inside the QR code.</param>
        /// <param name="pointsPerModule">The number of points each dark/light module of the QR code will occupy in the final QR code image.</param>
        /// <param name="darkColorHex">The color of the dark modules in HTML hex format.</param>
        /// <param name="lightColorHex">The color of the light modules in HTML hex format.</param>
        /// <param name="eccLevel">The level of error correction data.</param>
        /// <param name="forceUtf8">Specifies whether the generator should be forced to work in UTF-8 mode.</param>
        /// <param name="utf8BOM">Specifies whether the byte-order-mark should be used.</param>
        /// <param name="eciMode">Specifies which ECI mode should be used.</param>
        /// <param name="requestedVersion">Sets the fixed QR code target version.</param>
        /// <param name="drawQuietZones">Indicates if quiet zones around the QR code should be drawn.</param>
        /// <param name="epsFormat">Indicates if the output should be in EPS format.</param>
        /// <returns>Returns the QR code graphic as a PostScript string.</returns>
        public static string GetQRCode(string plainText, int pointsPerModule, string darkColorHex, string lightColorHex, ECCLevel eccLevel, bool forceUtf8 = false, bool utf8BOM = false, EciMode eciMode = EciMode.Default, int requestedVersion = -1, bool drawQuietZones = true, bool epsFormat = false)
        {
            using (var qrGenerator = new QRCodeGenerator())
            using (var qrCodeData = qrGenerator.CreateQrCode(plainText, eccLevel, forceUtf8, utf8BOM, eciMode, requestedVersion))
            using (var qrCode = new PostscriptQRCode(qrCodeData))
                return qrCode.GetGraphic(pointsPerModule, darkColorHex, lightColorHex, drawQuietZones, epsFormat);
        }
    }
}
#endif