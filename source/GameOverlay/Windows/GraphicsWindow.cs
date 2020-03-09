using System;
using System.Threading;
using System.Diagnostics;

using GameOverlay.Drawing;
using GameOverlay.PInvoke;

namespace GameOverlay.Windows
{
    /// <summary>
    /// Represents an OverlayWindow which is used to draw at any given frame rate.
    /// </summary>
    public class GraphicsWindow : OverlayWindow
    {
        private Thread _thread;
        private readonly Stopwatch _watch;

        private volatile bool _isRunning;
        private volatile bool _isPaused;

        private volatile int _fps;

        /// <summary>
        /// Gets or sets the used Graphics surface.
        /// </summary>
        public Graphics Graphics { get; }

        /// <summary>
        /// Gets or sets a Boolean which determines whether this instance is running.
        /// </summary>
        public bool IsRunning { get => _isRunning; set => _isRunning = value; }

        /// <summary>
        /// Gets or sets a Boolean which determines whether this instance is paused.
        /// </summary>
        public bool IsPaused { get => _isPaused; set => _isPaused = value; }

        /// <summary>
        /// Gets or sets the frames per second (frame rate) at which this instance invokes its DrawGraphics event.
        /// </summary>
        public int FPS { get => _fps; set => _fps = value; }

        /// <summary>
        /// Fires when a new Scene / frame needs to be rendered.
        /// </summary>
        public event EventHandler<DrawGraphicsEventArgs> DrawGraphics;

        /// <summary>
        /// Fires when you should free any resources used for drawing with this instance.
        /// </summary>
        public event EventHandler<DestroyGraphicsEventArgs> DestroyGraphics;

        /// <summary>
        /// Fires when you should allocate any resources you use to draw using this instance.
        /// </summary>
        public event EventHandler<SetupGraphicsEventArgs> SetupGraphics;

        /// <summary>
        /// Initializes a new GraphicsWindow.
        /// </summary>
        /// <param name="device">Optionally specify a Graphics device to use.</param>
        public GraphicsWindow(Graphics device = null) : base()
        {
            _watch = Stopwatch.StartNew();

            SizeChanged += GraphicsWindow_SizeChanged;
            VisibilityChanged += GraphicsWindow_VisibilityChanged;

            Graphics = device ?? new Graphics();
        }

        /// <summary>
        /// Initializes a new GraphicsWindow with the specified window position and size.
        /// </summary>
        /// <param name="x">The window position on the X-Axis.</param>
        /// <param name="y">The window position on the Y-Axis.</param>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        /// <param name="device">Optionally specify a Graphics device to use.</param>
        public GraphicsWindow(int x, int y, int width, int height, Graphics device = null) : this(device)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~GraphicsWindow() => Dispose(false);

        /// <summary>
        /// Runs a timer thread which invokes the DrawGraphics event and measures frames per second.
        /// </summary>
        public void StartThread()
        {
            if (_isRunning) throw new InvalidOperationException("Graphics window is already running");

            _isRunning = true;
            _isPaused = !IsVisible;

            _thread = new Thread(GraphicsWindowThread)
            {
                IsBackground = true
            };

            _thread.Start();
        }

        /// <summary>
        /// Stops the timer thread.
        /// </summary>
        public void StopThread()
        {
            StopThreadAsync();

            try
            {
                _thread.Join();
            }
            catch { } // ignore "already exited" exception
        }

        /// <summary>
        /// Stops the timer thread and does not wait for it to finish execution.
        /// </summary>
        public void StopThreadAsync()
        {
            if (!_isRunning) throw new InvalidOperationException("Graphics window is not running");

            _isRunning = false;
            _isPaused = false;
        }

        /// <summary>
        /// Pauses the timer thread.
        /// </summary>
        public void Pause()
        {
            if (!_isRunning) throw new InvalidOperationException("Graphics window is not running");

            _isPaused = true;
        }

        /// <summary>
        /// Resumes the timer thread.
        /// </summary>
        public void Unpause()
        {
            if (!_isRunning) throw new InvalidOperationException("Graphics window is not running");

            _isPaused = false;
        }

        /// <summary>
        /// Waits until the Thread used by this instance has exited.
        /// </summary>
        public void JoinGraphicsThread()
        {
            if (!_isRunning) throw new InvalidOperationException("Graphics window is not running");

            try
            {
                _thread.Join();
            }
            catch { }
        }

        private void GraphicsWindowThread()
        {
            if (!IsInitialized)
            {
                CreateWindow();
            }

            if (!Graphics.IsInitialized)
            {
                Graphics.Width = Width;
                Graphics.Height = Height;
                Graphics.WindowHandle = Handle;

                Graphics.Setup();
            }

            Graphics.RecreateResources += Graphics_RecreateDevice;

            OnSetupGraphics(Graphics);

            while (_isRunning)
            {
                int currentFPS = _fps;

                if (currentFPS > 0)
                {
                    long startTime = _watch.ElapsedMilliseconds;

                    OnDrawGraphics(Graphics);

                    long endTime = _watch.ElapsedMilliseconds;

                    int sleepTimePerFrame = 1000 / currentFPS;

                    int remainingTime = (int)(sleepTimePerFrame - (endTime - startTime));

                    if (remainingTime > 0)
                    {
                        Thread.Sleep(remainingTime);
                    }
                }
                else
                {
                    OnDrawGraphics(Graphics);
                }

                while (_isPaused)
                {
                    Thread.Sleep(10);
                }
            }

            OnDestroyGraphics(Graphics);
        }

        private void Graphics_RecreateDevice(object sender, RecreateResourcesEventArgs e)
        {
            SetupGraphics?.Invoke(this, new SetupGraphicsEventArgs(e.Graphics, true));
        }

        private void GraphicsWindow_SizeChanged(object sender, OverlaySizeEventArgs e)
        {
            if (Graphics.IsInitialized)
            {
                Graphics.Resize(e.Width, e.Height);
            }
            else
            {
                Graphics.Width = e.Width;
                Graphics.Height = e.Height;
            }
        }

        private void GraphicsWindow_VisibilityChanged(object sender, OverlayVisibilityEventArgs e)
        {
            _isPaused = !e.IsVisible;
        }

        /// <summary>
        /// Gets called when the timer thread needs to render a new Scene / frame.
        /// </summary>
        /// <param name="graphics">A Graphics surface.</param>
        protected virtual void OnDrawGraphics(Graphics graphics)
        {
            graphics.BeginScene();

            DrawGraphics?.Invoke(this, new DrawGraphicsEventArgs(graphics));

            graphics.EndScene();
        }

        /// <summary>
        /// Gets called when the timer thread setups the Graphics surface.
        /// </summary>
        /// <param name="graphics">A Graphics surface.</param>
        protected virtual void OnSetupGraphics(Graphics graphics)
        {
            SetupGraphics?.Invoke(this, new SetupGraphicsEventArgs(graphics));
        }

        /// <summary>
        /// Gets called when the timer thread destorys the Graphics surface.
        /// </summary>
        /// <param name="graphics">A Graphics surface.</param>
        protected virtual void OnDestroyGraphics(Graphics graphics)
        {
            DestroyGraphics?.Invoke(this, new DestroyGraphicsEventArgs(graphics));
        }

        /// <summary>
        /// Releases all resources used by this GraphicsWindow.
        /// </summary>
        /// <param name="disposing">A Boolean value indicating whether this is called from the destructor.</param>
        protected override void Dispose(bool disposing)
        {
            if (_isRunning)
            {
                StopThread();
            }

            Graphics?.Dispose();

            base.Dispose(disposing);
        }
    }
}
