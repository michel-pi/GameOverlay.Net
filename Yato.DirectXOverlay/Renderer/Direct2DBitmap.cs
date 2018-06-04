using System;
using System.IO;

using SharpDX.Direct2D1;

namespace Yato.DirectXOverlay.Renderer
{
    /// <summary>
    /// Stores a Bitmap compatible with <c>Direct2DRenderer</c>
    /// </summary>
    public class Direct2DBitmap
    {
        private static SharpDX.WIC.ImagingFactory ImagingFactory = new SharpDX.WIC.ImagingFactory();

        /// <summary>
        /// A <c>SharpDX.Direct2D1.Bitmap</c> object
        /// </summary>
        public Bitmap SharpDXBitmap;

        private Direct2DBitmap()
        {
        }

        /// <summary>
        /// Internal use only
        /// </summary>
        /// <param name="device"><c>RenderTarget</c> device</param>
        /// <param name="bytes"><c>Bitmap</c> bytes</param>
        public Direct2DBitmap(RenderTarget device, byte[] bytes)
        {
            LoadBitmap(device, bytes);
        }

        /// <summary>
        /// Internal use only
        /// </summary>
        /// <param name="device"><c>RenderTarget</c> device</param>
        /// <param name="file">Path to an image file</param>
        public Direct2DBitmap(RenderTarget device, string file)
        {
            LoadBitmap(device, File.ReadAllBytes(file));
        }

        ~Direct2DBitmap()
        {
            SharpDXBitmap.Dispose();
        }

        private void LoadBitmap(RenderTarget device, byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            SharpDX.WIC.BitmapDecoder decoder = new SharpDX.WIC.BitmapDecoder(ImagingFactory, stream, SharpDX.WIC.DecodeOptions.CacheOnDemand);
            var frame = decoder.GetFrame(0);
            SharpDX.WIC.FormatConverter converter = new SharpDX.WIC.FormatConverter(ImagingFactory);
            try
            {
                // normal ARGB images (Bitmaps / png tested)
                converter.Initialize(frame, SharpDX.WIC.PixelFormat.Format32bppRGBA1010102);
            }
            catch
            {
                // falling back to RGB if unsupported
                converter.Initialize(frame, SharpDX.WIC.PixelFormat.Format32bppRGB);
            }
            SharpDXBitmap = Bitmap.FromWicBitmap(device, converter);

            converter.Dispose();
            frame.Dispose();
            decoder.Dispose();
            stream.Dispose();
        }

        public static implicit operator Bitmap(Direct2DBitmap bmp)
        {
            return bmp.SharpDXBitmap;
        }
    }
}