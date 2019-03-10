using System;
using System.IO;

using SharpDX.WIC;
using SharpDX.Direct2D1;

using Bitmap = SharpDX.Direct2D1.Bitmap;
using PixelFormat = SharpDX.WIC.PixelFormat;

namespace GameOverlay.Drawing
{
    /// <summary>
    /// Represents an Image which can be drawn using a Graphics surface.
    /// </summary>
    public class Image : IDisposable
    {
        private static readonly ImagingFactory ImageFactory = new ImagingFactory();

        /// <summary>
        /// The SharpDX Bitmap
        /// </summary>
        public Bitmap Bitmap;

        /// <summary>
        /// Gets the width of this Image
        /// </summary>
        public float Width => Bitmap.PixelSize.Width;
        /// <summary>
        /// Gets the height of this Image
        /// </summary>
        public float Height => Bitmap.PixelSize.Height;

        private Image()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new Image for the given device by using a byte[].
        /// </summary>
        /// <param name="device">The Graphics device.</param>
        /// <param name="bytes">A byte[] containing image data.</param>
        public Image(RenderTarget device, byte[] bytes)
        {
            Bitmap = LoadBitmapFromMemory(device, bytes);
        }

        /// <summary>
        /// Initializes a new Image for the given device by using a file on disk.
        /// </summary>
        /// <param name="device">The Graphics device.</param>
        /// <param name="path">The path to an image file on disk.</param>
        public Image(RenderTarget device, string path)
        {
            Bitmap = LoadBitmapFromFile(device, path);
        }

        /// <summary>
        /// Initializes a new Image for the given device by using a byte[].
        /// </summary>
        /// <param name="device">The Graphics device.</param>
        /// <param name="bytes">A byte[] containing image data.</param>
        public Image(Graphics device, byte[] bytes) : this(device.GetRenderTarget(), bytes)
        {
        }

        /// <summary>
        /// Initializes a new Image for the given device by using a file on disk.
        /// </summary>
        /// <param name="device">The Graphics device.</param>
        /// <param name="path">The path to an image file on disk.</param>
        public Image(Graphics device, string path) : this(device.GetRenderTarget(), path)
        {
        }

        /// <summary>
        /// Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~Image()
        {
            Dispose(false);
        }

        #region IDisposable Support
        private bool disposedValue = false;

        /// <summary>
        /// Releases all resources used by this Image.
        /// </summary>
        /// <param name="disposing">A Boolean value indicating whether this is called from the destructor.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (Bitmap != null) Bitmap.Dispose();

                disposedValue = true;
            }
        }

        /// <summary>
        /// Releases all resources used by this Image.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        /// <summary>
        /// Converts an Image to a SharpDX Bitmap.
        /// </summary>
        /// <param name="image">The Image object.</param>
        public static implicit operator Bitmap(Image image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));

            return image.Bitmap;
        }

        private static Bitmap LoadBitmapFromMemory(RenderTarget device, byte[] bytes)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            if (bytes.Length == 0) throw new ArgumentOutOfRangeException(nameof(bytes));

            Bitmap bmp = null;
            MemoryStream stream = null;
            BitmapDecoder decoder = null;
            BitmapFrameDecode frame = null;
            FormatConverter converter = null;

            try
            {
                stream = new MemoryStream(bytes);
                decoder = new BitmapDecoder(ImageFactory, stream, DecodeOptions.CacheOnDemand);

                var pixelFormat = ImagePixelFormats.GetBestPixelFormat(decoder.DecoderInfo?.PixelFormats);

                frame = decoder.GetFrame(0);

                converter = new FormatConverter(ImageFactory);

                try
                {
                    converter.Initialize(frame, pixelFormat);
                    bmp = Bitmap.FromWicBitmap(device, converter);
                }
                catch
                {
                    TryCatch(() => converter.Dispose());

                    converter = new FormatConverter(ImageFactory);
                    converter.Initialize(frame, PixelFormat.Format32bppRGB);
                    bmp = Bitmap.FromWicBitmap(device, converter);
                }

                converter.Dispose();
                frame.Dispose();
                decoder.Dispose();
                stream.Dispose();

                return bmp;
            }
            catch
            {
                if (converter != null && !converter.IsDisposed) converter.Dispose();
                if (frame != null && !frame.IsDisposed) frame.Dispose();
                if (decoder != null && !decoder.IsDisposed) decoder.Dispose();
                if (stream != null) TryCatch(() => stream.Dispose());
                if (bmp != null && !bmp.IsDisposed) bmp.Dispose();

                throw;
            }
        }

        private static Bitmap LoadBitmapFromFile(RenderTarget device, string path) => LoadBitmapFromMemory(device, File.ReadAllBytes(path));
        
        private static void TryCatch(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            try
            {
                action();
            }
            catch { }
        }
    }
}
