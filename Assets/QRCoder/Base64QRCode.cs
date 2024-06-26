﻿#if !NETSTANDARD1_3
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using static QRCoder.Base64QRCode;
using static QRCoder.QRCodeGenerator;

namespace QRCoder
{
    /// <summary>
    /// Represents a QR code generator that outputs base64-encoded images.
    /// </summary>
    public class Base64QRCode : AbstractQRCode, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Base64QRCode"/> class.
        /// Constructor without parameters to be used in COM objects connections.
        /// </summary>
        public Base64QRCode() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Base64QRCode"/> class with the specified <see cref="QRCodeData"/>.
        /// </summary>
        /// <param name="data"><see cref="QRCodeData"/> generated by the QRCodeGenerator.</param>
        public Base64QRCode(QRCodeData data) : base(data) {
        }

        /// <summary>
        /// Returns a base64-encoded string that contains the resulting QR code as a PNG image.
        /// </summary>
        /// <param name="pixelsPerModule">The number of pixels each dark/light module of the QR code will occupy in the final QR code image.</param>
        /// <returns>Returns the QR code graphic as a base64-encoded string.</returns>
        public string GetGraphic(int pixelsPerModule)
        {
            return this.GetGraphic(pixelsPerModule, Color.Black, Color.White, true);
        }

        /// <summary>
        /// Returns a base64-encoded string that contains the resulting QR code as an image.
        /// </summary>
        /// <param name="pixelsPerModule">The number of pixels each dark/light module of the QR code will occupy in the final QR code image.</param>
        /// <param name="darkColorHtmlHex">The color of the dark modules in HTML hex format.</param>
        /// <param name="lightColorHtmlHex">The color of the light modules in HTML hex format.</param>
        /// <param name="drawQuietZones">Indicates if quiet zones around the QR code should be drawn.</param>
        /// <param name="imgType">The type of image to generate (PNG, JPEG, GIF).</param>
        /// <returns>Returns the QR code graphic as a base64-encoded string.</returns>
        public string GetGraphic(int pixelsPerModule, string darkColorHtmlHex, string lightColorHtmlHex, bool drawQuietZones = true, ImageType imgType = ImageType.Png)
        {
            return this.GetGraphic(pixelsPerModule, ColorTranslator.FromHtml(darkColorHtmlHex), ColorTranslator.FromHtml(lightColorHtmlHex), drawQuietZones, imgType);
        }

        /// <summary>
        /// Returns a base64-encoded string that contains the resulting QR code as an image.
        /// </summary>
        /// <param name="pixelsPerModule">The number of pixels each dark/light module of the QR code will occupy in the final QR code image.</param>
        /// <param name="darkColor">The color of the dark modules.</param>
        /// <param name="lightColor">The color of the light modules.</param>
        /// <param name="drawQuietZones">Indicates if quiet zones around the QR code should be drawn.</param>
        /// <param name="imgType">The type of image to generate (PNG, JPEG, GIF).</param>
        /// <returns>Returns the QR code graphic as a base64-encoded string.</returns>
        public string GetGraphic(int pixelsPerModule, Color darkColor, Color lightColor, bool drawQuietZones = true, ImageType imgType = ImageType.Png)
        {
            if (imgType == ImageType.Png)
            {
                var pngCoder = new PngByteQRCode(QrCodeData);

                byte[] pngData;
                if (darkColor == Color.Black && lightColor == Color.White)
                {
                    pngData = pngCoder.GetGraphic(pixelsPerModule, drawQuietZones);
                }
                else
                {
                    byte[] darkColorBytes;
                    byte[] lightColorBytes;
                    if (darkColor.A != 255 || lightColor.A != 255)
                    {
                        darkColorBytes = new byte[] { darkColor.R, darkColor.G, darkColor.B, darkColor.A };
                        lightColorBytes = new byte[] { lightColor.R, lightColor.G, lightColor.B, lightColor.A };
                    }
                    else
                    {
                        darkColorBytes = new byte[] { darkColor.R, darkColor.G, darkColor.B };
                        lightColorBytes = new byte[] { lightColor.R, lightColor.G, lightColor.B };
                    }
                    pngData = pngCoder.GetGraphic(pixelsPerModule, darkColorBytes, lightColorBytes, drawQuietZones);
                }

                return Convert.ToBase64String(pngData, Base64FormattingOptions.None);
            }

#if SYSTEM_DRAWING
#pragma warning disable CA1416 // Validate platform compatibility
            var qr = new QRCode(QrCodeData);
            var base64 = string.Empty;
            using (Bitmap bmp = qr.GetGraphic(pixelsPerModule, darkColor, lightColor, drawQuietZones))
            {
                base64 = BitmapToBase64(bmp, imgType);
            }
            return base64;
#pragma warning restore CA1416 // Validate platform compatibility
#else
            throw new PlatformNotSupportedException("Only the PNG image type is supported on this platform.");
#endif
        }

#if SYSTEM_DRAWING
        /// <summary>
        /// Returns a base64-encoded string that contains the resulting QR code as an image with an embedded icon.
        /// </summary>
        /// <param name="pixelsPerModule">The number of pixels each dark/light module of the QR code will occupy in the final QR code image.</param>
        /// <param name="darkColor">The color of the dark modules.</param>
        /// <param name="lightColor">The color of the light modules.</param>
        /// <param name="icon">The icon to embed in the center of the QR code.</param>
        /// <param name="iconSizePercent">The size of the icon as a percentage of the QR code.</param>
        /// <param name="iconBorderWidth">The width of the border around the icon.</param>
        /// <param name="drawQuietZones">Indicates if quiet zones around the QR code should be drawn.</param>
        /// <param name="imgType">The type of image to generate (PNG, JPEG, GIF).</param>
        /// <returns>Returns the QR code graphic as a base64-encoded string.</returns>
#if NET6_0_OR_GREATER
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
        public string GetGraphic(int pixelsPerModule, Color darkColor, Color lightColor, Bitmap icon, int iconSizePercent = 15, int iconBorderWidth = 6, bool drawQuietZones = true, ImageType imgType = ImageType.Png)
        {
            var qr = new QRCode(QrCodeData);
            var base64 = string.Empty;
            using (Bitmap bmp = qr.GetGraphic(pixelsPerModule, darkColor, lightColor, icon, iconSizePercent, iconBorderWidth, drawQuietZones))
            {
                base64 = BitmapToBase64(bmp, imgType);
            }
            return base64;
        }
#endif

#if SYSTEM_DRAWING
        /// <summary>
        /// Converts a bitmap to a base64-encoded string.
        /// </summary>
        /// <param name="bmp">The bitmap to convert.</param>
        /// <param name="imgType">The type of image (PNG, JPEG, GIF).</param>
        /// <returns>Returns the base64-encoded string representation of the bitmap.</returns>
#if NET6_0_OR_GREATER
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
        private string BitmapToBase64(Bitmap bmp, ImageType imgType)
        {
            var base64 = string.Empty;
            ImageFormat iFormat;
            switch (imgType) {
                case ImageType.Png:
                    iFormat = ImageFormat.Png;
                    break;
                case ImageType.Jpeg:
                    iFormat = ImageFormat.Jpeg;
                    break;
                case ImageType.Gif:
                    iFormat = ImageFormat.Gif;
                    break;
                default:
                    iFormat = ImageFormat.Png;
                    break;
            }
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bmp.Save(memoryStream, iFormat);
                base64 = Convert.ToBase64String(memoryStream.ToArray(), Base64FormattingOptions.None);
            }
            return base64;
        }
#endif

        /// <summary>
        /// Specifies the type of image to generate.
        /// </summary>
        public enum ImageType
        {
#if NET6_0_OR_GREATER
            [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
            /// <summary>
            /// GIF image format.
            /// </summary>
            Gif,
#if NET6_0_OR_GREATER
            [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
            /// <summary>
            /// JPEG image format.
            /// </summary>
            Jpeg,
            /// <summary>
            /// PNG image format.
            /// </summary>
            Png
        }

    }

    /// <summary>
    /// Provides static methods for creating base64-encoded QR codes.
    /// </summary>
    public static class Base64QRCodeHelper
    {
        /// <summary>
        /// Creates a base64-encoded QR code with a single function call.
        /// </summary>
        /// <param name="plainText">The text or payload to be encoded inside the QR code.</param>
        /// <param name="pixelsPerModule">The number of pixels each dark/light module of the QR code will occupy in the final QR code image.</param>
        /// <param name="darkColorHtmlHex">The color of the dark modules in HTML hex format.</param>
        /// <param name="lightColorHtmlHex">The color of the light modules in HTML hex format.</param>
        /// <param name="eccLevel">The level of error correction data.</param>
        /// <param name="forceUtf8">Specifies whether the generator should be forced to work in UTF-8 mode.</param>
        /// <param name="utf8BOM">Specifies whether the byte-order-mark should be used.</param>
        /// <param name="eciMode">Specifies which ECI mode should be used.</param>
        /// <param name="requestedVersion">Sets the fixed QR code target version.</param>
        /// <param name="drawQuietZones">Indicates if quiet zones around the QR code should be drawn.</param>
        /// <param name="imgType">The type of image to generate (PNG, JPEG, GIF).</param>
        /// <returns>Returns the QR code graphic as a base64-encoded string.</returns>
        public static string GetQRCode(string plainText, int pixelsPerModule, string darkColorHtmlHex, string lightColorHtmlHex, ECCLevel eccLevel, bool forceUtf8 = false, bool utf8BOM = false, EciMode eciMode = EciMode.Default, int requestedVersion = -1, bool drawQuietZones = true, ImageType imgType = ImageType.Png)
        {
            using (var qrGenerator = new QRCodeGenerator())
            using (var qrCodeData = qrGenerator.CreateQrCode(plainText, eccLevel, forceUtf8, utf8BOM, eciMode, requestedVersion))
            using (var qrCode = new Base64QRCode(qrCodeData))
                return qrCode.GetGraphic(pixelsPerModule, darkColorHtmlHex, lightColorHtmlHex, drawQuietZones, imgType);
        }
    }
}

#endif
