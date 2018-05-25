using System;
using System.Diagnostics;
using System.Threading;

namespace Yato.DirectXOverlay
{
    public class FrameTimer
    {
        private bool exitTimerThread;
        private bool pause;
        private Stopwatch stopwatch;
        private Thread timerThread;

        public FrameTimer()
        {
            stopwatch = new Stopwatch();
        }

        public FrameTimer(int fps)
        {
            FPS = fps;
            stopwatch = new Stopwatch();
        }

        ~FrameTimer()
        {
            stopwatch.Stop();
        }

        public delegate void FrameStart();

        public event FrameStart OnFrameStart;

        public int FPS { get; set; }

        public bool IsPaused => pause;

        private void FrameTimerMethod()
        {
            while (!exitTimerThread)
            {
                while (pause)
                {
                    Thread.Sleep(100);
                }

                int currentFps = FPS;

                if (currentFps == 0)
                {
                    OnFrameStart?.Invoke();
                }

                int sleepTimePerFrame = 1000 / currentFps;

                for (int i = 0; i < currentFps; i++)
                {
                    stopwatch.Restart();

                    OnFrameStart?.Invoke();

                    stopwatch.Stop();

                    int currentSleepTime = sleepTimePerFrame - (int)stopwatch.ElapsedMilliseconds;

                    if (currentSleepTime >= 0)
                    {
                        Thread.Sleep(currentSleepTime);
                    }
                    else
                    {
                        Thread.Sleep(sleepTimePerFrame);
                    }
                }
            }
        }

        public void Pause()
        {
            pause = true;
        }

        public void Resume()
        {
            pause = false;
        }

        public void Start()
        {
            if (exitTimerThread) return;
            if (timerThread != null) return;

            timerThread = new Thread(FrameTimerMethod)
            {
                IsBackground = true
            };

            timerThread.Start();
        }

        public void Stop()
        {
            if (exitTimerThread) return;
            if (timerThread == null) return;

            exitTimerThread = true;

            try
            {
                timerThread.Join();
            }
            catch
            {
            }

            exitTimerThread = false;
            timerThread = null;
        }
    }
}