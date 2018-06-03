using System;
using System.Diagnostics;
using System.Threading;

namespace Yato.DirectXOverlay
{
    public class FrameTimer
    {
        private bool _exitTimerThread;
        private Stopwatch _stopwatch;
        private Thread _timerThread;

        public delegate void FrameStart();

        public event FrameStart OnFrameStart;

        public int FPS { get; set; }

        public bool IsPaused { get; private set; }

        public FrameTimer()
        {
            _stopwatch = new Stopwatch();
        }

        public FrameTimer(int fps)
        {
            FPS = fps;
            _stopwatch = new Stopwatch();
        }

        ~FrameTimer()
        {
            _stopwatch.Stop();
        }

        public void Pause()
        {
            IsPaused = true;
        }

        public void Resume()
        {
            IsPaused = false;
        }

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