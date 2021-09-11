using System;
using System.Diagnostics;
using System.Threading;

using GameOverlay.PInvoke;

namespace GameOverlay
{
    /// <summary>
    /// Adds support for high precision timers and sleep throughout the overlay.
    /// </summary>
    public static class TimerService
    {
        private static readonly object _lock;

        private static bool _isHighPrecision;
        private static bool _useDefaultTimers;

        /// <summary>
        /// Gets or sets a Boolean which determines whether high precision timers are currently enabled.
        /// </summary>
        public static bool IsHighPrecision
        {
            get
            {
                lock (_lock)
                {
                    return _isHighPrecision;
                }
            }
            set
            {
                lock (_lock)
                {
                    if (_useDefaultTimers) return;

                    if (value)
                    {
                        Winmm.TimeBeginPeriod(1);
                    }
                    else
                    {
                        Winmm.TimeEndPeriod(1);
                    }

                    _isHighPrecision = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a Boolean which determines whether the use of high precision timers is allowed.
        /// </summary>
        public static bool UseDefaultTimers
        {
            get
            {
                lock (_lock)
                {
                    return _useDefaultTimers;
                }
            }
            set
            {
                lock (_lock)
                {
                    _useDefaultTimers = value;
                }
            }
        }

        static TimerService()
        {
            _lock = new object();
        }

        /// <summary>
        /// Ensures that high precision timers are enabled and enables them otherwise.
        /// </summary>
        public static void EnsureHighPrecisionTimers()
        {
            lock (_lock)
            {
                if (_useDefaultTimers || _isHighPrecision) return;

                Winmm.TimeBeginPeriod(1);

                _isHighPrecision = true;

                return;
            }
        }

        /// <summary>
        /// Enables high precision timers. Use before calling anything else.
        /// </summary>
        public static void EnableHighPrecisionTimers()
        {
            lock (_lock)
            {
                if (_useDefaultTimers || _isHighPrecision) return;

                Winmm.TimeBeginPeriod(1);

                _isHighPrecision = true;
            }
        }

        /// <summary>
        /// Disables high precision timers. Use before exiting the app.
        /// </summary>
        public static void DisableHighPrecisionTimers()
        {
            lock (_lock)
            {
                if (_useDefaultTimers || !_isHighPrecision) return;

                Winmm.TimeEndPeriod(1);

                _isHighPrecision = false;
            }
        }

        /// <summary>
        /// Defines different methods to sleep for a given time.
        /// </summary>
        public static class Methods
        {
            [ThreadStatic] private static Stopwatch _watch;

            private static Stopwatch GetSharedStopwatch()
            {
                return _watch ?? (_watch = Stopwatch.StartNew());
            }

            /// <summary>
            /// Default Thread.Sleep
            /// </summary>
            /// <param name="milliseconds"></param>
            public static void Sleep(int milliseconds)
            {
                Thread.Sleep(milliseconds);
            }

            /// <summary>
            /// NtDelayExecution implementation.
            /// </summary>
            /// <param name="milliseconds"></param>
            public static void DelayExecution(int milliseconds)
            {
                if (Undocumented.NtDelayExecution == null)
                {
                    Loop(milliseconds);

                    return;
                }

                long delay = milliseconds * -10000L;
                Undocumented.NtDelayExecution(0, ref delay);
            }

            /// <summary>
            /// A more precise Thread.Sleep.
            /// </summary>
            /// <param name="milliseconds"></param>
            public static void Loop(int milliseconds)
            {
                var watch = GetSharedStopwatch();
                var start = watch.ElapsedMilliseconds;

                do
                {
                    Thread.Sleep(1);
                } while (watch.ElapsedMilliseconds - start < milliseconds);
            }
        }
    }
}
