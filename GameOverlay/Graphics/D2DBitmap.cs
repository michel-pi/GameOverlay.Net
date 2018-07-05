using System;
using System.IO;

using SharpDX.Direct2D1;

namespace GameOverlay.Graphics
{
    /// <summary>
    /// Stores a Bitmap compatible with <c>Direct2DRenderer</c>
    /// </summary>
    public class D2DBitmap
    {
        private static SharpDX.WIC.ImagingFactory ImagingFactory = new SharpDX.WIC.ImagingFactory();

        /// <summary>
        /// A <c>SharpDX.Direct2D1.Bitmap</c> object
        /// </summary>
        public Bitmap SharpDXBitmap;

        private D2DBitmap()
        {
        }

        /// <summary>
        /// Internal use only
        /// </summary>
        /// <param name="device"><c>RenderTarget</c> device</param>
        /// <param name="bytes"><c>Bitmap</c> bytes</param>
        public D2DBitmap(RenderTarget device, byte[] bytes)
        {
            LoadBitmap(device, bytes);
        }

        /// <summary>
        /// Internal use only
        /// </summary>
        /// <param name="device"><c>RenderTarget</c> device</param>
        /// <param name="file">Path to an image file</param>
        public D2DBitmap(RenderTarget device, string file)
        {
            LoadBitmap(device, File.ReadAllBytes(file));
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="D2DBitmap"/> class.
        /// </summary>
        ~D2DBitmap()
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

        /// <summary>
        /// Performs an implicit conversion from <see cref="D2DBitmap"/> to <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="bmp">The BMP.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Bitmap(D2DBitmap bmp)
        {
            return bmp.SharpDXBitmap;
        }
    }
}