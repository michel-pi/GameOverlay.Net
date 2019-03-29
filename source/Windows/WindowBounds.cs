using System;
using System.Runtime.InteropServices;

namespace GameOverlay.Windows
{
    /// <summary>
    /// Represents the boundaries of a window.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WindowBounds
    {
        /// <summary>
        /// The position on the x-axis of the upper-left corner of a window.
        /// </summary>
        public int Left;
        /// <summary>
        /// The position on the y-axis of the upper-left corner of a window.
        /// </summary>
        public int Top;
        /// <summary>
        /// The position on the x-axis of the lower-right corner of a window.
        /// </summary>
        public int Right;
        /// <summary>
        /// The position on the y-axis of the lower-right corner of a window.
        /// </summary>
        public int Bottom;

        /// <summary>
        /// Returns a value indicating whether this instance and a specified <see cref="T:System.Object" /> represent the same type and value.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true" /> if <paramref name="obj" /> is a WindowBounds and equal to this instance; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            if (obj is WindowBounds value)
            {
                return value.Left == Left
                    && value.Right == Right
                    && value.Top == Top
                    && value.Bottom == Bottom;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a value indicating whether two specified instances of WindowBounds represent the same value.
        /// </summary>
        /// <param name="value">An object to compare to this instance.</param>
        /// <returns><see langword="true" /> if <paramref name="value" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
        public bool Equals(WindowBounds value)
        {
            return value.Left == Left
                && value.Right == Right
                && value.Top == Top
                && value.Bottom == Bottom;
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
        /// Converts this WindowBounds structure to a human-readable string.
        /// </summary>
        /// <returns>A string representation of this WindowBounds.</returns>
        public override string ToString()
        {
            return OverrideHelper.ToString(
                "Left", Left.ToString(),
                "Top", Top.ToString(),
                "Right", Right.ToString(),
                "Bottom", Bottom.ToString());
        }

        /// <summary>
        /// Returns a value indicating whether two specified instances of WindowBounds represent the same value.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns> <see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, <see langword="false" />.</returns>
        public static bool Equals(WindowBounds left, WindowBounds right)
        {
            return left.Equals(right);
        }
    }
}
