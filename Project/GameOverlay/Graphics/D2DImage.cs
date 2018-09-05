using System;
using System.IO;
using SharpDX.Direct2D1;
using SharpDX.WIC;
using Bitmap = SharpDX.Direct2D1.Bitmap;
using PixelFormat = SharpDX.WIC.PixelFormat;

namespace GameOverlay.Graphics
{
    /// <inheritdoc />
    /// <summary>
    ///     Stores a Bitmap compatible with <c>Direct2DRenderer</c>
    /// </summary>
    public class D2DImage : IDisposable
    {
        private static readonly ImagingFactory ImagingFactory = new ImagingFactory();

        /// <summary>
        ///     A <c>SharpDX.Direct2D1.Bitmap</c> object
        /// </summary>
        public Bitmap SharpDxBitmap;

        private D2DImage()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Internal use only
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
        ///     Internal use only
        /// </summary>
        /// <param name="device"><c>RenderTarget</c> device</param>
        /// <param name="file">Path to an image file</param>
        public D2DImage(RenderTarget device, string file)
        {
            if (string.IsNullOrEmpty(file)) throw new ArgumentNullException(nameof(file));
            if (!File.Exists(file)) throw new FileNotFoundException(nameof(D2DImage), file);

            LoadBitmap(device, File.ReadAllBytes(file));
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:GameOverlay.Graphics.D2DImage" /> class.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="bytes">The bytes.</param>
        public D2DImage(D2DDevice device, byte[] bytes) : this(device.GetRenderTarget(), bytes)
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:GameOverlay.Graphics.D2DImage" /> class.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="file">The file.</param>
        public D2DImage(D2DDevice device, string file) : this(device.GetRenderTarget(), file)
        {
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="D2DImage" /> class.
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
                var decoder = new BitmapDecoder(ImagingFactory, stream, DecodeOptions.CacheOnDemand);
                var frame = decoder.GetFrame(0);
                var converter = new FormatConverter(ImagingFactory);
                try
                {
                    converter.Initialize(frame, PixelFormat.Format32bppRGBA1010102);
                }
                catch
                {
                    // falling back to RGB if unsupported
                    converter.Initialize(frame, PixelFormat.Format32bppRGB);
                }
                SharpDxBitmap = Bitmap.FromWicBitmap(device, converter);

                converter.Dispose();
                frame.Dispose();
                decoder.Dispose();
                stream.Dispose();
            }
            catch (Exception ex)
            {
                throw new FormatException("Invalid or unsupported image format!", ex);
            }
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="D2DImage" /> to <see cref="Bitmap" />.
        /// </summary>
        /// <param name="bmp">The BMP.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Bitmap(D2DImage bmp)
        {
            return bmp.SharpDxBitmap;
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "{Bitmap=" + SharpDxBitmap.PixelSize.Width + "x" + SharpDxBitmap.PixelSize.Height + "}";
        }

        #region IDisposable Support

        private bool _disposedValue; // To detect redundant calls

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            SharpDxBitmap.Dispose();
            SharpDxBitmap = null;

            _disposedValue = true;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}