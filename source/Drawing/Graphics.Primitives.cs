using System;
using System.Runtime.InteropServices;

using SharpDX.Mathematics.Interop;

namespace GameOverlay.Drawing
{
    /// <summary>
    /// Represents the dimension of a circle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Circle
    {
        /// <summary>
        /// The position of this Circle.
        /// </summary>
        public Point Location;
        
        /// <summary>
        /// The Radius of this Circle.
        /// </summary>
        public float Radius;
        
        /// <summary>
        /// Initializes a new Circle by using the given location and radius.
        /// </summary>
        /// <param name="location">A Point structure including the x- and y-coordinates of the center of a circle.</param>
        /// <param name="radius">The radius of a circle.</param>
        public Circle(Point location, float radius)
        {
            Location = location;
            Radius = radius;
        }

        /// <summary>
        /// Initializes a new Circle by using the given location and radius.
        /// </summary>
        /// <param name="x">The x-coordinate of the center of a circle.</param>
        /// <param name="y">The y-coordinate of the center of a circle.</param>
        /// <param name="radius">The radius of a circle.</param>
        public Circle(float x, float y, float radius)
        {
            Location = new Point(x, y);
            Radius = radius;
        }

        /// <summary>
        /// Initializes a new Circle by using the given location and radius.
        /// </summary>
        /// <param name="location">A Point structure including the x- and y-coordinates of the center of a circle.</param>
        /// <param name="radius">The radius of a circle.</param>
        public Circle(Point location, int radius)
        {
            Location = location;
            Radius = radius;
        }

        /// <summary>
        /// Initializes a new Circle by using the given location and radius.
        /// </summary>
        /// <param name="x">The x-coordinate of the center of a circle.</param>
        /// <param name="y">The y-coordinate of the center of a circle.</param>
        /// <param name="radius">The radius of a circle.</param>
        public Circle(int x, int y, int radius)
        {
            Location = new Point(x, y);
            Radius = radius;
        }

        /// <summary>
        /// Returns a value indicating whether this instance and a specified <see cref="T:System.Object" /> represent the same type and value.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true" /> if <paramref name="obj" /> is a Circle and equal to this instance; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Circle value)
            {
                return value.Location.Equals(Location)
                    && value.Radius == Radius;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return OverrideHelper.HashCodes(
                Location.GetHashCode(),
                Radius.GetHashCode());
        }

        /// <summary>
        /// Converts the Circle structure to a human-readable string.
        /// </summary>
        /// <returns>A string representation of this Circle.</returns>
        public override string ToString()
        {
            return OverrideHelper.ToString(
                "Location", Location.ToString(),
                "Radius", Radius.ToString());
        }

        /// <summary>
        /// Converts this Circle to a SharpDX ellipse.
        /// </summary>
        /// <param name="circle">A Circle structure.</param>
        public static implicit operator SharpDX.Direct2D1.Ellipse(Circle circle)
        {
            return new SharpDX.Direct2D1.Ellipse(circle.Location, circle.Radius, circle.Radius);
        }
    }

    /// <summary>
    /// Represents the dimension of an ellipse.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Ellipse
    {
        /// <summary>
        /// The position of this ellipse.
        /// </summary>
        public Point Location;
        
        /// <summary>
        /// The radius on the x-axis of this ellipse.
        /// </summary>
        public float RadiusX;
        /// <summary>
        /// The radius on the y-axis of this ellipse.
        /// </summary>
        public float RadiusY;

        /// <summary>
        /// Initializes a new Ellipse using the given location and radius.
        /// </summary>
        /// <param name="location">A Location structure including the center x- and y-coordinate of an ellipse.</param>
        /// <param name="radiusX">The radius on the x-axis of this ellipse.</param>
        /// <param name="radiusY">The radius on the y-axis of this ellipse.</param>
        public Ellipse(Point location, float radiusX, float radiusY)
        {
            Location = location;
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        /// <summary>
        /// Initializes a new Ellipse using the given location and radius.
        /// </summary>
        /// <param name="location">A Location structure including the center x- and y-coordinate of an ellipse.</param>
        /// <param name="radiusX">The radius on the x-axis of this ellipse.</param>
        /// <param name="radiusY">The radius on the y-axis of this ellipse.</param>
        public Ellipse(Point location, int radiusX, int radiusY)
        {
            Location = location;
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        /// <summary>
        /// Initializes a new Ellipse using the given location and radius.
        /// </summary>
        /// <param name="x">The center x-coordinate of an ellipse.</param>
        /// <param name="y">The center y-coordinate of an ellipse.</param>
        /// <param name="radiusX">The radius on the x-axis of this ellipse.</param>
        /// <param name="radiusY">The radius on the y-axis of this ellipse.</param>
        public Ellipse(float x, float y, float radiusX, float radiusY)
        {
            Location = new Point(x, y);
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        /// <summary>
        /// Initializes a new Ellipse using the given location and radius.
        /// </summary>
        /// <param name="x">The center x-coordinate of an ellipse.</param>
        /// <param name="y">The center y-coordinate of an ellipse.</param>
        /// <param name="radiusX">The radius on the x-axis of this ellipse.</param>
        /// <param name="radiusY">The radius on the y-axis of this ellipse.</param>
        public Ellipse(int x, int y, int radiusX, int radiusY)
        {
            Location = new Point(x, y);
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        /// <summary>
        /// Returns a value indicating whether this instance and a specified <see cref="T:System.Object" /> represent the same type and value.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true" /> if <paramref name="obj" /> is a Ellipse and equal to this instance; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Ellipse value)
            {
                return value.Location.Equals(Location)
                    && value.RadiusX == RadiusX
                    && value.RadiusY == RadiusY;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return OverrideHelper.HashCodes(
                Location.GetHashCode(),
                RadiusX.GetHashCode(),
                RadiusY.GetHashCode());
        }

        /// <summary>
        /// Converts the Ellipse structure to a human-readable string.
        /// </summary>
        /// <returns>A string representation of this Ellipse.</returns>
        public override string ToString()
        {
            return OverrideHelper.ToString(
                "Location", Location.ToString(),
                "RadiusX", RadiusX.ToString(),
                "RadiusY", RadiusY.ToString());
        }
        
        /// <summary>
        /// Converts an Ellipse to a SharpDX Ellipse.
        /// </summary>
        /// <param name="ellipse">An Ellipse structure.</param>
        public static implicit operator SharpDX.Direct2D1.Ellipse(Ellipse ellipse)
        {
            return new SharpDX.Direct2D1.Ellipse(ellipse.Location, ellipse.RadiusX, ellipse.RadiusY);
        }
    }

    /// <summary>
    /// Represents the start and end Point of a line.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Line
    {
        /// <summary>
        /// The staring Point of this Line.
        /// </summary>
        public Point Start;
        /// <summary>
        /// The ending Point of this Line.
        /// </summary>
        public Point End;

        /// <summary>
        /// Initializes a new Line using the given points.
        /// </summary>
        /// <param name="start">A Point structure including the start coordinates of the line.</param>
        /// <param name="end">A Point structure including the end coordinates of the line.</param>
        public Line(Point start, Point end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Initializes a new Line using the given points.
        /// </summary>
        /// <param name="startX">The x-coordinate of the start point of the line.</param>
        /// <param name="startY">The y-coordinate of the start point of the line.</param>
        /// <param name="endX">The x-coordinate of the end point of the line.</param>
        /// <param name="endY">The y-coordinate of the end point of the line.</param>
        public Line(float startX, float startY, float endX, float endY)
        {
            Start = new Point(startX, startY);
            End = new Point(endX, endY);
        }

        /// <summary>
        /// Initializes a new Line using the given points.
        /// </summary>
        /// <param name="startX">The x-coordinate of the start point of the line.</param>
        /// <param name="startY">The y-coordinate of the start point of the line.</param>
        /// <param name="endX">The x-coordinate of the end point of the line.</param>
        /// <param name="endY">The y-coordinate of the end point of the line.</param>
        public Line(int startX, int startY, int endX, int endY)
        {
            Start = new Point(startX, startY);
            End = new Point(endX, endY);
        }

        /// <summary>
        /// Returns a value indicating whether this instance and a specified <see cref="T:System.Object" /> represent the same type and value.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true" /> if <paramref name="obj" /> is a Line and equal to this instance; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Line value)
            {
                return value.Start.Equals(Start)
                    && value.End.Equals(End);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return OverrideHelper.HashCodes(
                Start.GetHashCode(),
                End.GetHashCode());
        }

        /// <summary>
        /// Converts the Line structure to a human-readable string.
        /// </summary>
        /// <returns>A string representation of this Line.</returns>
        public override string ToString()
        {
            return OverrideHelper.ToString(
                "Start", Start.ToString(),
                "End", End.ToString());
        }
    }

    /// <summary>
    /// Represents the x- and y-coordinates of a point.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        /// <summary>
        /// The x-coordinate of this Point.
        /// </summary>
        public float X;
        /// <summary>
        /// The y-coordinate of this Point.
        /// </summary>
        public float Y;

        /// <summary>
        /// Initializes a new Point using the given coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate of this Point.</param>
        /// <param name="y">The y-coordinate of this Point.</param>
        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Initializes a new Point using the given coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate of this Point.</param>
        /// <param name="y">The y-coordinate of this Point.</param>
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Returns a value indicating whether this instance and a specified <see cref="T:System.Object" /> represent the same type and value.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true" /> if <paramref name="obj" /> is a Point and equal to this instance; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Point value)
            {
                return value.X == X
                    && value.Y == Y;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return OverrideHelper.HashCodes(
                X.GetHashCode(),
                Y.GetHashCode());
        }

        /// <summary>
        /// Converts the Point structure to a human-readable string.
        /// </summary>
        /// <returns>A string representation of this Point.</returns>
        public override string ToString()
        {
            return OverrideHelper.ToString(
                "X", X.ToString(),
                "Y", Y.ToString());
        }

        /// <summary>
        /// Converts a Point structure to a SharpDX RawVector2.
        /// </summary>
        /// <param name="point">A Point structure.</param>
        public static implicit operator RawVector2(Point point)
        {
            return new RawVector2(point.X, point.Y);
        }

        /// <summary>
        /// Converts a SharpDX RawVector2 structure to a Point structure.
        /// </summary>
        /// <param name="vector">A SharpDX RawVector2.</param>
        public static implicit operator Point(RawVector2 vector)
        {
            return new Point(vector.X, vector.Y);
        }
    }

    /// <summary>
    /// Represents the dimension of a rectangle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rectangle
    {
        /// <summary>
        /// The x-coordinate of the upper-left corner of the Rectangle.
        /// </summary>
        public float Left;
        /// <summary>
        /// The y-coordinate of the upper-left corner of the Rectangle.
        /// </summary>
        public float Top;
        /// <summary>
        /// The x-coordinate of the bottom-right corner of the Rectangle.
        /// </summary>
        public float Right;
        /// <summary>
        /// The y-coordinate of the bottom-right corner of the Rectangle.
        /// </summary>
        public float Bottom;

        /// <summary>
        /// Gets the width of this Rectangle.
        /// </summary>
        public float Width => Right - Left;
        /// <summary>
        /// Gets the height of this Rectangle.
        /// </summary>
        public float Height => Bottom - Top;

        /// <summary>
        /// Initializes a new Rectangle using the given coordinates.
        /// </summary>
        /// <param name="left">The x-coordinate of the upper-left corner of the Rectangle.</param>
        /// <param name="top">The y-coordinate of the upper-left corner of the Rectangle.</param>
        /// <param name="right">The x-coordinate of the bottom-right corner of the Rectangle.</param>
        /// <param name="bottom">The y-coordinate of the bottom-right corner of the Rectangle.</param>
        public Rectangle(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        /// <summary>
        /// Initializes a new Rectangle using the given coordinates.
        /// </summary>
        /// <param name="left">The x-coordinate of the upper-left corner of the Rectangle.</param>
        /// <param name="top">The y-coordinate of the upper-left corner of the Rectangle.</param>
        /// <param name="right">The x-coordinate of the bottom-right corner of the Rectangle.</param>
        /// <param name="bottom">The y-coordinate of the bottom-right corner of the Rectangle.</param>
        public Rectangle(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        /// <summary>
        /// Creates a new Rectangle structure using the given dimension.
        /// </summary>
        /// <param name="x">The x-coordinate of the upper-left corner of the Rectangle.</param>
        /// <param name="y">The y-coordinate of the upper-left corner of the Rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle</param>
        /// <returns>The Rectangle this method creates.</returns>
        public static Rectangle Create(float x, float y, float width, float height)
        {
            return new Rectangle(x, y, x + width, y + height);
        }

        /// <summary>
        /// Creates a new Rectangle structure using the given dimension.
        /// </summary>
        /// <param name="x">The x-coordinate of the upper-left corner of the Rectangle.</param>
        /// <param name="y">The y-coordinate of the upper-left corner of the Rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle</param>
        /// <returns>The Rectangle this method creates.</returns>
        public static Rectangle Create(int x, int y, int width, int height)
        {
            return new Rectangle(x, y, x + width, y + height);
        }

        /// <summary>
        /// Returns a value indicating whether this instance and a specified <see cref="T:System.Object" /> represent the same type and value.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true" /> if <paramref name="obj" /> is a Rectangle and equal to this instance; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Rectangle value)
            {
                return value.Left == Left
                    && value.Top == Top
                    && value.Right == Right
                    && value.Bottom == Bottom;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return OverrideHelper.HashCodes(
                Left.GetHashCode(),
                Top.GetHashCode(),
                Right.GetHashCode(),
                Bottom.GetHashCode());
        }

        /// <summary>
        /// Converts this Rectangle structure to a human-readable string.
        /// </summary>
        /// <returns>A string representation of this Rectangle.</returns>
        public override string ToString()
        {
            return OverrideHelper.ToString(
                "Left", Left.ToString(),
                "Top", Top.ToString(),
                "Right", Right.ToString(),
                "Bottom", Bottom.ToString());
        }
        
        /// <summary>
        /// Converts a Rectangle structure to a SharpDX RawRectangleF.
        /// </summary>
        /// <param name="rectangle">A Rectangle structure.</param>
        public static implicit operator RawRectangleF(Rectangle rectangle)
        {
            return new RawRectangleF(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
        }
    }

    /// <summary>
    /// Represents the dimension of a rectangle with rounded edges.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RoundedRectangle
    {
        /// <summary>
        /// The Rectangle.
        /// </summary>
        public Rectangle Rectangle;

        /// <summary>
        /// The radius on the x-axis of this RoundedRectangle.
        /// </summary>
        public float RadiusX;
        /// <summary>
        /// The radius on the y-axis of this RoundedRectangle.
        /// </summary>
        public float RadiusY;
        
        /// <summary>
        /// Initializes a new RoundedRectangle structure using then given dimension and radius.
        /// </summary>
        /// <param name="rectangle">A Rectangle structure including the dimension of the rectangle.</param>
        /// <param name="radius">A value indicating the radius of the corners of a RoundedRectangle.</param>
        public RoundedRectangle(Rectangle rectangle, float radius)
        {
            Rectangle = rectangle;
            RadiusX = radius;
            RadiusY = radius;
        }

        /// <summary>
        /// Initializes a new RoundedRectangle structure using then given dimension and radius.
        /// </summary>
        /// <param name="rectangle">A Rectangle structure including the dimension of the rectangle.</param>
        /// <param name="radiusX">A value indicating the radius on the x-axis of the corners of a RoundedRectangle.</param>
        /// <param name="radiusY">A value indicating the radius on the y-axis of the corners of a RoundedRectangle.</param>
        public RoundedRectangle(Rectangle rectangle, float radiusX, float radiusY)
        {
            Rectangle = rectangle;
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        /// <summary>
        /// Initializes a new RoundedRectangle structure using then given dimension and radius.
        /// </summary>
        /// <param name="left">The x-coordinate of the upper-left corner of the Rectangle.</param>
        /// <param name="top">The y-coordinate of the upper-left corner of the Rectangle.</param>
        /// <param name="right">The x-coordinate of the bottom-right corner of the Rectangle.</param>
        /// <param name="bottom">The y-coordinate of the bottom-right corner of the Rectangle.</param>
        /// <param name="radius">A value indicating the radius of the corners of a RoundedRectangle.</param>
        public RoundedRectangle(float left, float top, float right, float bottom, float radius)
        {
            Rectangle = new Rectangle(left, top, right, bottom);
            RadiusX = radius;
            RadiusY = radius;
        }

        /// <summary>
        /// Initializes a new RoundedRectangle structure using then given dimension and radius.
        /// </summary>
        /// <param name="left">The x-coordinate of the upper-left corner of the Rectangle.</param>
        /// <param name="top">The y-coordinate of the upper-left corner of the Rectangle.</param>
        /// <param name="right">The x-coordinate of the bottom-right corner of the Rectangle.</param>
        /// <param name="bottom">The y-coordinate of the bottom-right corner of the Rectangle.</param>
        /// <param name="radiusX">A value indicating the radius on the x-axis of the corners of a RoundedRectangle.</param>
        /// <param name="radiusY">A value indicating the radius on the y-axis of the corners of a RoundedRectangle.</param>
        public RoundedRectangle(float left, float top, float right, float bottom, float radiusX, float radiusY)
        {
            Rectangle = new Rectangle(left, top, right, bottom);
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        /// <summary>
        /// Returns a value indicating whether this instance and a specified <see cref="T:System.Object" /> represent the same type and value.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true" /> if <paramref name="obj" /> is a RoundedRectangle and equal to this instance; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            if (obj is RoundedRectangle value)
            {
                return value.Rectangle.Equals(Rectangle)
                    && value.RadiusX == RadiusX
                    && value.RadiusY == RadiusY;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return OverrideHelper.HashCodes(
                Rectangle.GetHashCode(),
                RadiusX.GetHashCode(),
                RadiusY.GetHashCode());
        }

        /// <summary>
        /// Converts this RoundedRectangle structure to a human-readable string.
        /// </summary>
        /// <returns>A string representation of this RoundedRectangle</returns>
        public override string ToString()
        {
            return OverrideHelper.ToString(
                "Rectangle", Rectangle.ToString(),
                "RadiusX", RadiusX.ToString(),
                "RadiusY", RadiusY.ToString());
        }

        /// <summary>
        /// Creates a new RoundedRectangle using the given dimension and radius.
        /// </summary>
        /// <param name="x">The x-coordinate of the upper-left corner of the Rectangle.</param>
        /// <param name="y">The y-coordinate of the upper-left corner of the Rectangle.</param>
        /// <param name="width">The width of the Rectangle.</param>
        /// <param name="height">The height of the Rectangle.</param>
        /// <param name="radius">A value indicating the radius of the corners of a RoundedRectangle.</param>
        /// <returns>The RoundedRectangle this method creates.</returns>
        public static RoundedRectangle Create(float x, float y, float width, float height, float radius)
        {
            return new RoundedRectangle(Rectangle.Create(x, y, width, height), radius);
        }

        /// <summary>
        /// Creates a new RoundedRectangle using the given dimension and radius.
        /// </summary>
        /// <param name="x">The x-coordinate of the upper-left corner of the Rectangle.</param>
        /// <param name="y">The y-coordinate of the upper-left corner of the Rectangle.</param>
        /// <param name="width">The width of the Rectangle.</param>
        /// <param name="height">The height of the Rectangle.</param>
        /// <param name="radiusX">A value indicating the radius on the x-axis of the corners of a RoundedRectangle.</param>
        /// <param name="radiusY">A value indicating the radius on the y-axis of the corners of a RoundedRectangle.</param>
        /// <returns>The RoundedRectangle this method creates.</returns>
        public static RoundedRectangle Create(float x, float y, float width, float height, float radiusX, float radiusY)
        {
            return new RoundedRectangle(Rectangle.Create(x, y, width, height), radiusX, radiusY);
        }

        /// <summary>
        /// Converts a RoundedRectangle structure to a SharpDX RoundedRectangle.
        /// </summary>
        /// <param name="rectangle">A RoundedRectangle struct</param>
        public static implicit operator SharpDX.Direct2D1.RoundedRectangle(RoundedRectangle rectangle)
        {
            return new SharpDX.Direct2D1.RoundedRectangle
            {
                RadiusX = rectangle.RadiusX,
                RadiusY = rectangle.RadiusY,
                Rect = rectangle.Rectangle
            };
        }
    }
    
    /// <summary>
    /// Represents the dimension of a triangle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Triangle
    {
        /// <summary>
        /// The lower-left Point of this Triangle.
        /// </summary>
        public Point A;
        /// <summary>
        /// The lower-right Point of this Triangle.
        /// </summary>
        public Point B;
        /// <summary>
        /// The upper-center Point of this Triangle.
        /// </summary>
        public Point C;

        /// <summary>
        /// Initializes a new Triangle using the given Points.
        /// </summary>
        /// <param name="a">The lower-left Point of this Triangle.</param>
        /// <param name="b">The lower-right Point of this Triangle.</param>
        /// <param name="c">The upper-center Point of this Triangle.</param>
        public Triangle(Point a, Point b, Point c)
        {
            A = a;
            B = b;
            C = c;
        }

        /// <summary>
        /// Initializes a new Triangle using the given Points.
        /// </summary>
        /// <param name="a_x">The x-coordinate of the lower-left Point of this Triangle.</param>
        /// <param name="a_y">The y-coordinate of the lower-left Point of this Triangle.</param>
        /// <param name="b_x">The x-coordinate of the lower-right Point of this Triangle.</param>
        /// <param name="b_y">The y-coordinate lower-right Point of this Triangle.</param>
        /// <param name="c_x">The x-coordinate of the upper-center Point of this Triangle.</param>
        /// <param name="c_y">The y-coordinate upper-center Point of this Triangle.</param>
        public Triangle(float a_x, float a_y, float b_x, float b_y, float c_x, float c_y)
        {
            A = new Point(a_x, a_y);
            B = new Point(b_x, b_y);
            C = new Point(c_x, c_y);
        }

        /// <summary>
        /// Initializes a new Triangle using the given Points.
        /// </summary>
        /// <param name="a_x">The x-coordinate of the lower-left Point of this Triangle.</param>
        /// <param name="a_y">The y-coordinate of the lower-left Point of this Triangle.</param>
        /// <param name="b_x">The x-coordinate of the lower-right Point of this Triangle.</param>
        /// <param name="b_y">The y-coordinate lower-right Point of this Triangle.</param>
        /// <param name="c_x">The x-coordinate of the upper-center Point of this Triangle.</param>
        /// <param name="c_y">The y-coordinate upper-center Point of this Triangle.</param>
        public Triangle(int a_x, int a_y, int b_x, int b_y, int c_x, int c_y)
        {
            A = new Point(a_x, a_y);
            B = new Point(b_x, b_y);
            C = new Point(c_x, c_y);
        }

        /// <summary>
        /// Returns a value indicating whether this instance and a specified <see cref="T:System.Object" /> represent the same type and value.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true" /> if <paramref name="obj" /> is a Triangle and equal to this instance; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Triangle value)
            {
                return value.A.Equals(A)
                    && value.B.Equals(B)
                    && value.C.Equals(C);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return OverrideHelper.HashCodes(
                A.GetHashCode(),
                B.GetHashCode(),
                C.GetHashCode());
        }

        /// <summary>
        /// Converts this Triangle structure to a human-readable string.
        /// </summary>
        /// <returns>The string representation of this Triangle.</returns>
        public override string ToString()
        {
            return OverrideHelper.ToString(
                "A", A.ToString(),
                "B", B.ToString(),
                "C", C.ToString());
        }
    }
}
