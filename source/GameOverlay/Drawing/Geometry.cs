using System;

using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace GameOverlay.Drawing
{
    /// <summary>
    /// Represents a Geometry which can be drawn by a Graphics device.
    /// </summary>
    public class Geometry : IDisposable
    {
        private readonly PathGeometry _geometry;
        private readonly GeometrySink _sink;

        /// <summary>
        /// Determines whether this Geometry is open.
        /// </summary>
        public bool IsOpen { get; private set; }

        private Geometry()
        {
        }

        /// <summary>
        /// Initializes a new Geometry using a Graphics device.
        /// </summary>
        /// <param name="device"></param>
        public Geometry(Graphics device)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));
            if (!device.IsInitialized) throw new InvalidOperationException("The render target needs to be initialized first");

            _geometry = new PathGeometry(device.GetFactory());
            _sink = _geometry.Open();

            IsOpen = true;
        }

        /// <summary>
        /// Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~Geometry() => Dispose(false);

        /// <summary>
        /// Starts a new figure within this Geometry using a starting point.
        /// </summary>
        /// <param name="point">The start point for this figure.</param>
        /// <param name="fill">A Boolean value determining whether this figure can be filled by a Graphics device.</param>
        public void BeginFigure(Point point, bool fill = false)
        {
            _sink.BeginFigure(point, fill ? FigureBegin.Filled : FigureBegin.Hollow);
        }

        /// <summary>
        /// Starts a new figure within this Geometry using a starting line.
        /// </summary>
        /// <param name="line">The first line within this figure.</param>
        /// <param name="fill">A Boolean value determining whether this figure can be filled by a Graphics device.</param>
        public void BeginFigure(Line line, bool fill = false)
        {
            _sink.BeginFigure(line.Start, fill ? FigureBegin.Filled : FigureBegin.Hollow);
            _sink.AddLine(line.End);
        }

        /// <summary>
        /// Ends the currently started figure.
        /// </summary>
        /// <param name="closed">A Boolean value indicating whether this figure should automatically be closen by the Graphics device.</param>
        public void EndFigure(bool closed = true)
        {
            _sink.EndFigure(closed ? FigureEnd.Closed : FigureEnd.Open);
        }

        /// <summary>
        /// Adds a new Point within the current figure.
        /// </summary>
        /// <param name="point">A Point which will be added to this figure</param>
        public void AddPoint(Point point)
        {
            _sink.AddLine(point);
        }

        /// <summary>
        /// Creates a new figure from a Rectangle.
        /// </summary>
        /// <param name="rectangle">The Rectangle used to create a new figure.</param>
        /// <param name="fill">A Boolean value determining whether this figure can be filled by a Graphics device.</param>
        public void AddRectangle(Rectangle rectangle, bool fill = false)
        {
            _sink.BeginFigure(new RawVector2(rectangle.Left, rectangle.Top), fill ? FigureBegin.Filled : FigureBegin.Hollow);
            _sink.AddLine(new RawVector2(rectangle.Right, rectangle.Top));
            _sink.AddLine(new RawVector2(rectangle.Right, rectangle.Bottom));
            _sink.AddLine(new RawVector2(rectangle.Left, rectangle.Bottom));
            _sink.EndFigure(FigureEnd.Closed);
        }

        /// <summary>
        /// Adds a curved line to the currently open figure.
        /// </summary>
        /// <param name="point">The end point of the curved line.</param>
        /// <param name="radius">The radius of the resulting arc in degrees.</param>
        /// <param name="rotationAngle">A value determining the rotation angle this curve.</param>
        public void AddCurve(Point point, float radius, float rotationAngle = 0.0f)
        {
            bool minus = radius < 0.0f;

            if (minus) radius *= -1.0f;

            var arc = new ArcSegment()
            {
                ArcSize = radius >= 180.0f ? ArcSize.Large : ArcSize.Small,
                Point = point,
                RotationAngle = rotationAngle,
                Size = new Size2F(radius, radius),
                SweepDirection = minus ? SweepDirection.Clockwise : SweepDirection.CounterClockwise
            };

            _sink.AddArc(arc);
        }

        /// <summary>
        /// Adds a curved line to the currently open figure.
        /// </summary>
        /// <param name="point">The end point of the curved line.</param>
        /// <param name="radius_x">The radius on the X-Axis of the resulting arc in degrees.</param>
        /// <param name="radius_y">The radius on the Y-Axis of the resulting arc in degrees.</param>
        /// <param name="rotationAngle">A value determining the rotation angle this curve.</param>
        public void AddCurve(Point point, float radius_x, float radius_y, float rotationAngle = 0.0f)
        {
            _sink.AddArc(new ArcSegment
            {
                Point = point,
                Size = new Size2F(radius_x, radius_y),
                RotationAngle = rotationAngle
            });
        }

        /// <summary>
        /// Closes this Geometry and prevents further manipulation.
        /// </summary>
        public void Close()
        {
            if (!IsOpen) return;

            IsOpen = false;

            _sink.Close();
        }

        /// <summary>
        /// Returns a value indicating whether this instance and a specified <see cref="T:System.Object" /> represent the same type and value.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true" /> if <paramref name="obj" /> is a Geometry and equal to this instance; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Geometry geometry)
            {
                return geometry.IsOpen == IsOpen
                    && geometry._geometry.NativePointer == _geometry.NativePointer;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a value indicating whether two specified instances of Geometry represent the same value.
        /// </summary>
        /// <param name="value">An object to compare to this instance.</param>
        /// <returns><see langword="true" /> if <paramref name="value" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
        public bool Equals(Geometry value)
        {
            return value != null
                && value.IsOpen == IsOpen
                && value._geometry.NativePointer == _geometry.NativePointer;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return OverrideHelper.HashCodes(
                IsOpen.GetHashCode(),
                _geometry.NativePointer.GetHashCode());
        }

        /// <summary>
        /// Converts this Geometry instance to a human-readable string.
        /// </summary>
        /// <returns>A string representation of this Geometry.</returns>
        public override string ToString()
        {
            return OverrideHelper.ToString(
                "Geometry", "PathGeometry",
                "FigureCount", _geometry.FigureCount.ToString(),
                "SegmentCount", _geometry.SegmentCount.ToString(),
                "IsOpen", IsOpen.ToString());
        }

        #region IDisposable Support
        private bool disposedValue = false;

        /// <summary>
        /// Releases all resources used by this Geometry.
        /// </summary>
        /// <param name="disposing">A Boolean value indicating whether this is called from the destructor.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                try
                {
                    if (IsOpen) _sink.Close();

                    _sink.Dispose();
                    _geometry.Dispose();
                }
                catch { }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Releases all resources used by this Geometry.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        /// <summary>
        /// Returns the Direct2D Geometry used by this object.
        /// </summary>
        /// <param name="geometry"></param>
        public static implicit operator SharpDX.Direct2D1.Geometry(Geometry geometry)
        {
            return geometry._geometry;
        }

        /// <summary>
        /// Returns a value indicating whether two specified instances of Geometry represent the same value.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns> <see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, <see langword="false" />.</returns>
        public static bool Equals(Geometry left, Geometry right)
        {
            return left?.Equals(right) == true;
        }
    }
}
