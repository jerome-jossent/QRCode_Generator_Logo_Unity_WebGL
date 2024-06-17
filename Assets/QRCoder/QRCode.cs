#if SYSTEM_DRAWING
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using static QRCoder.QRCodeGenerator;

namespace QRCoder
{
#if NET6_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
    /// <summary>
    /// Represents a QR code generator that outputs QR codes as bitmap images.
    /// </summary>
    public class QRCode : AbstractQRCode, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QRCode"/> class.
        /// Constructor without parameters to be used in COM objects connections.
        /// </summary>
        public QRCode() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QRCode"/> class with the specified <see cref="QRCodeData"/>.
        /// </summary>
        /// <param name="data"><see cref="QRCodeData"/> generated by the QRCodeGenerator.</param>
        public QRCode(QRCodeData data) : base(data) { }

        /// <summary>
        /// Creates a black and white bitmap image of the QR code.
        /// </summary>
        /// <param name="pixelsPerModule">The number of pixels each dark/light module of the QR code will occupy in the final QR code image.</param>
        /// <returns>Returns the QR code graphic as a bitmap.</returns>
        public Bitmap GetGraphic(int pixelsPerModule)
        {
            return this.GetGraphic(pixelsPerModule, Color.Black, Color.White, true);
        }

        /// <summary>
        /// Creates a colored bitmap image of the QR code.
        /// </summary>
        /// <param name="pixelsPerModule">The number of pixels each dark/light module of the QR code will occupy in the final QR code image.</param>
        /// <param name="darkColorHtmlHex">The color of the dark modules in HTML hex format.</param>
        /// <param name="lightColorHtmlHex">The color of the light modules in HTML hex format.</param>
        /// <param name="drawQuietZones">Indicates if quiet zones around the QR code should be drawn.</param>
        /// <returns>Returns the QR code graphic as a bitmap.</returns>
        public Bitmap GetGraphic(int pixelsPerModule, string darkColorHtmlHex, string lightColorHtmlHex, bool drawQuietZones = true)
        {
            return this.GetGraphic(pixelsPerModule, ColorTranslator.FromHtml(darkColorHtmlHex), ColorTranslator.FromHtml(lightColorHtmlHex), drawQuietZones);
        }

        /// <summary>
        /// Creates a colored bitmap image of the QR code.
        /// </summary>
        /// <param name="pixelsPerModule">The number of pixels each dark/light module of the QR code will occupy in the final QR code image.</param>
        /// <param name="darkColor">The color of the dark modules.</param>
        /// <param name="lightColor">The color of the light modules.</param>
        /// <param name="drawQuietZones">Indicates if quiet zones around the QR code should be drawn.</param>
        /// <returns>Returns the QR code graphic as a bitmap.</returns>
        public Bitmap GetGraphic(int pixelsPerModule, Color darkColor, Color lightColor, bool drawQuietZones = true)
        {
            var size = (this.QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : 8)) * pixelsPerModule;
            var offset = drawQuietZones ? 0 : 4 * pixelsPerModule;

            var bmp = new Bitmap(size, size);
            using (var gfx = Graphics.FromImage(bmp))
            using (var lightBrush = new SolidBrush(lightColor))
            using (var darkBrush = new SolidBrush(darkColor))
            {
                for (var x = 0; x < size + offset; x = x + pixelsPerModule)
                {
                    for (var y = 0; y < size + offset; y = y + pixelsPerModule)
                    {
                        var module = this.QrCodeData.ModuleMatrix[(y + pixelsPerModule) / pixelsPerModule - 1][(x + pixelsPerModule) / pixelsPerModule - 1];

                        if (module)
                        {
                            gfx.FillRectangle(darkBrush, new Rectangle(x - offset, y - offset, pixelsPerModule, pixelsPerModule));
                        }
                        else
                        {
                            gfx.FillRectangle(lightBrush, new Rectangle(x - offset, y - offset, pixelsPerModule, pixelsPerModule));
                        }
                    }
                }

                gfx.Save();
            }

            return bmp;
        }

        /// <summary>
        /// Creates a colored bitmap image of the QR code with an optional icon in the center.
        /// </summary>
        /// <param name="pixelsPerModule">The number of pixels each dark/light module of the QR code will occupy in the final QR code image.</param>
        /// <param name="darkColor">The color of the dark modules.</param>
        /// <param name="lightColor">The color of the light modules.</param>
        /// <param name="icon">An optional icon to be placed in the center of the QR code.</param>
        /// <param name="iconSizePercent">The size of the icon as a percentage of the QR code size.</param>
        /// <param name="iconBorderWidth">The width of the border around the icon.</param>
        /// <param name="drawQuietZones">Indicates if quiet zones around the QR code should be drawn.</param>
        /// <param name="iconBackgroundColor">The background color of the icon.</param>
        /// <returns>Returns the QR code graphic as a bitmap.</returns>
        public Bitmap GetGraphic(int pixelsPerModule, Color darkColor, Color lightColor, Bitmap? icon = null, int iconSizePercent = 15, int iconBorderWidth = 0, bool drawQuietZones = true, Color? iconBackgroundColor = null)
        {
            var size = (this.QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : 8)) * pixelsPerModule;
            var offset = drawQuietZones ? 0 : 4 * pixelsPerModule;

            var bmp = new Bitmap(size, size, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (var gfx = Graphics.FromImage(bmp))
            using (var lightBrush = new SolidBrush(lightColor))
            using (var darkBrush = new SolidBrush(darkColor))
            {
                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gfx.CompositingQuality = CompositingQuality.HighQuality;
                gfx.Clear(lightColor);
                var drawIconFlag = icon != null && iconSizePercent > 0 && iconSizePercent <= 100;

                for (var x = 0; x < size + offset; x = x + pixelsPerModule)
                {
                    for (var y = 0; y < size + offset; y = y + pixelsPerModule)
                    {
                        var moduleBrush = this.QrCodeData.ModuleMatrix[(y + pixelsPerModule) / pixelsPerModule - 1][(x + pixelsPerModule) / pixelsPerModule - 1] ? darkBrush : lightBrush;
                        gfx.FillRectangle(moduleBrush, new Rectangle(x - offset, y - offset, pixelsPerModule, pixelsPerModule));
                    }
                }

                if (drawIconFlag)
                {
                    float iconDestWidth = iconSizePercent * bmp.Width / 100f;
                    float iconDestHeight = drawIconFlag ? iconDestWidth * icon!.Height / icon.Width : 0;
                    float iconX = (bmp.Width - iconDestWidth) / 2;
                    float iconY = (bmp.Height - iconDestHeight) / 2;
                    var centerDest = new RectangleF(iconX - iconBorderWidth, iconY - iconBorderWidth, iconDestWidth + iconBorderWidth * 2, iconDestHeight + iconBorderWidth * 2);
                    var iconDestRect = new RectangleF(iconX, iconY, iconDestWidth, iconDestHeight);
                    var iconBgBrush = iconBackgroundColor != null ? new SolidBrush((Color)iconBackgroundColor) : lightBrush;
                    //Only render icon/logo background, if iconBorderWith is set > 0
                    if (iconBorderWidth > 0)
                    {
                        using (GraphicsPath iconPath = CreateRoundedRectanglePath(centerDest, iconBorderWidth * 2))
                        {
                            gfx.FillPath(iconBgBrush, iconPath);
                        }
                    }
                    gfx.DrawImage(icon!, iconDestRect, new RectangleF(0, 0, icon!.Width, icon.Height), GraphicsUnit.Pixel);
                }

                gfx.Save();
            }

            return bmp;
        }

        /// <summary>
        /// Creates a rounded rectangle path for drawing.
        /// </summary>
        /// <param name="rect">The rectangle for which to create the rounded path.</param>
        /// <param name="cornerRadius">The radius of the corners.</param>
        /// <returns>Returns the rounded rectangle path.</returns>
        internal GraphicsPath CreateRoundedRectanglePath(RectangleF rect, int cornerRadius)
        {
            var roundedRect = new GraphicsPath();
            roundedRect.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            roundedRect.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
            roundedRect.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            roundedRect.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
            roundedRect.AddArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            roundedRect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
            roundedRect.CloseFigure();
            return roundedRect;
        }
    }

#if NET6_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
    /// <summary>
    /// Provides static methods for creating bitmap QR codes.
    /// </summary>
    public static class QRCodeHelper
    {
        /// <summary>
        /// Creates a bitmap QR code with a single function call.
        /// </summary>
        /// <param name="plainText">The text or payload to be encoded inside the QR code.</param>
        /// <param name="pixelsPerModule">The number of pixels each dark/light module of the QR code will occupy in the final QR code image.</param>
        /// <param name="darkColor">The color of the dark modules.</param>
        /// <param name="lightColor">The color of the light modules.</param>
        /// <param name="eccLevel">The level of error correction data.</param>
        /// <param name="forceUtf8">Specifies whether the generator should be forced to work in UTF-8 mode.</param>
        /// <param name="utf8BOM">Specifies whether the byte-order-mark should be used.</param>
        /// <param name="eciMode">Specifies which ECI mode should be used.</param>
        /// <param name="requestedVersion">Sets the fixed QR code target version.</param>
        /// <param name="icon">An optional icon to be placed in the center of the QR code.</param>
        /// <param name="iconSizePercent">The size of the icon as a percentage of the QR code size.</param>
        /// <param name="iconBorderWidth">The width of the border around the icon.</param>
        /// <param name="drawQuietZones">Indicates if quiet zones around the QR code should be drawn.</param>
        /// <returns>Returns the QR code graphic as a bitmap.</returns>
        public static Bitmap GetQRCode(string plainText, int pixelsPerModule, Color darkColor, Color lightColor, ECCLevel eccLevel, bool forceUtf8 = false, bool utf8BOM = false, EciMode eciMode = EciMode.Default, int requestedVersion = -1, Bitmap? icon = null, int iconSizePercent = 15, int iconBorderWidth = 0, bool drawQuietZones = true)
        {
            using (var qrGenerator = new QRCodeGenerator())
            using (var qrCodeData = qrGenerator.CreateQrCode(plainText, eccLevel, forceUtf8, utf8BOM, eciMode, requestedVersion))
            using (var qrCode = new QRCode(qrCodeData))
                return qrCode.GetGraphic(pixelsPerModule, darkColor, lightColor, icon, iconSizePercent, iconBorderWidth, drawQuietZones);
        }
    }
}

#endif