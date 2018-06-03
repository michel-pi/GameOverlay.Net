using System;
using System.Diagnostics;
using System.Threading;

namespace Yato.DirectXOverlay
{
    /// <summary>
    /// Offers methods to create a frame loop
    /// </summary>
    public class FrameTimer
    {
        private bool _exitTimerThread;
        private Stopwatch _stopwatch;
        private Thread _timerThread;

        /// <summary>
        /// Callback definition for <c>OnFrameStart</c>
        /// </summary>
        public delegate void FrameStart();

        /// <summary>
        /// Occurs when a new frame starts
        /// </summary>
        public event FrameStart OnFrameStart;

        /// <summary>
        /// Gets or sets how many frames should be executed in a second
        /// </summary>
        /// <value>0 = unlimited</value>
        public int FPS { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is paused.
        /// </summary>
        /// <value><c>true</c> if this instance is paused; otherwise, <c>false</c>.</value>
        public bool IsPaused { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameTimer"/> class.
        /// </summary>
        public FrameTimer()
        {
            _stopwatch = new Stopwatch();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameTimer"/> class.
        /// </summary>
        /// <param name="fps">Frames per second</param>
        public FrameTimer(int fps)
        {
            FPS = fps;
            _stopwatch = new Stopwatch();
        }

        ~FrameTimer()
        {
            _stopwatch.Stop();
        }

        /// <summary>
        /// Pauses the loop
        /// </summary>
        public void Pause()
        {
            IsPaused = true;
        }

        /// <summary>
        /// Resumes the loop
        /// </summary>
        public void Resume()
        {
            IsPaused = false;
        }

        /// <summary>
        /// Starts the loop
        /// </summary>
        public void Start()
        {
            if (_exitTimerThread) return;
            if (_timerThread != null) return;

            _timerThread = new Thread(FrameTimerMethod)
            {
                IsBackground = true
            };

            _timerThread.Start();
        }

        /// <summary>
        /// Stops the loop
        /// </summary>
        public void Stop()
        {
            if (_exitTimerThread) return;
            if (_timerThread == null) return;

            _exitTimerThread = true;

            try
            {
                _timerThread.Join();
            }
            catch
            {
            }

            _exitTimerThread = false;
            _timerThread = null;
        }

        private void FrameTimerMethod()
        {
            while (!_exitTimerThread)
            {
                while (IsPaused)
                {
                    Thread.Sleep(100);
                }

                int currentFps = FPS;

                if (currentFps == 0)
                {
                    OnFrameStart?.Invoke();

                    continue;
                }

                int sleepTimePerFrame = 1000 / currentFps;

                for (int i = 0; i < currentFps; i++)
                {
                    _stopwatch.Restart();

                    OnFrameStart?.Invoke();

                    _stopwatch.Stop();

                    int currentSleepTime = sleepTimePerFrame - (int)_stopwatch.ElapsedMilliseconds;

                    if (currentSleepTime >= 0)
                    {
                        Thread.Sleep(currentSleepTime);
                    }
                }
            }
        }
    }
}