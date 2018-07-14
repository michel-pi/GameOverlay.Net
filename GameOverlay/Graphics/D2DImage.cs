using System;
using System.IO;

using SharpDX.Direct2D1;

namespace GameOverlay.Graphics
{
    /// <summary>
    /// Stores a Bitmap compatible with <c>Direct2DRenderer</c>
    /// </summary>
    public class D2DImage : IDisposable
    {
        private static SharpDX.WIC.ImagingFactory ImagingFactory = new SharpDX.WIC.ImagingFactory();

        /// <summary>
        /// A <c>SharpDX.Direct2D1.Bitmap</c> object
        /// </summary>
        public Bitmap SharpDXBitmap;

        private D2DImage()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Internal use only
        /// </summary>
        /// <param name="device"><c>RenderTarget</c> device</param>
        /// <param name="bytes"><c>Bitmap</c> bytes</param>
        public D2DImage(RenderTarget device, byte[] bytes)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));

            LoadBitmap(device, bytes);
        }

        /// <summary>
        /// Internal use only
        /// </summary>
        /// <param name="device"><c>RenderTarget</c> device</param>
        /// <param name="file">Path to an image file</param>
        public D2DImage(RenderTarget device, string file)
        {
            if (string.IsNullOrEmpty(file)) throw new ArgumentNullException(nameof(file));
            if (!File.Exists(file)) throw new FileNotFoundException(nameof(D2DImage), file);

            LoadBitmap(device, File.ReadAllBytes(file));
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="D2DImage"/> class.
        /// </summary>
        ~D2DImage()
        {
            Dispose();
        }

        private void LoadBitmap(RenderTarget device, byte[] bytes)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));

            try
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
            catch(Exception ex)
            {
                throw new FormatException("Invalid or unsupported image format!", ex);
            }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="D2DImage"/> to <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="bmp">The BMP.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Bitmap(D2DImage bmp)
        {
            return bmp.SharpDXBitmap;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "{Bitmap=" + SharpDXBitmap.PixelSize.Width + "x" + SharpDXBitmap.PixelSize.Height + "}";
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                SharpDXBitmap.Dispose();
                SharpDXBitmap = null;

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}