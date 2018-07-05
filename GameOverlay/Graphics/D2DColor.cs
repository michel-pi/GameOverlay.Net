using System;

using SharpDX.Mathematics.Interop;

namespace GameOverlay.Graphics
{
    /// <summary>
    /// Represents a ARGB color
    /// </summary>
    public struct Direct2DColor
    {
        /// <summary>
        /// The alpha
        /// </summary>
        public float Alpha;

        /// <summary>
        /// The blue
        /// </summary>
        public float Blue;

        /// <summary>
        /// The green
        /// </summary>
        public float Green;

        /// <summary>
        /// The red
        /// </summary>
        public float Red;

        /// <summary>
        /// Initializes a new instance of the <see cref="Direct2DColor"/> struct with an alpha of 255
        /// </summary>
        /// <param name="red">Red 0 - 255</param>
        /// <param name="green">Green 0 - 255</param>
        /// <param name="blue">Blue 0 - 255</param>
        public Direct2DColor(int red, int green, int blue)
        {
            Red = red / 255.0f;
            Green = green / 255.0f;
            Blue = blue / 255.0f;
            Alpha = 1.0f;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Direct2DColor"/> struct
        /// </summary>
        /// <param name="red">Red 0 - 255</param>
        /// <param name="green">Green 0 - 255</param>
        /// <param name="blue">Blue 0 - 255.</param>
        /// <param name="alpha">Alpha 0 - 255</param>
        public Direct2DColor(int red, int green, int blue, int alpha)
        {
            Red = red / 255.0f;
            Green = green / 255.0f;
            Blue = blue / 255.0f;
            Alpha = alpha / 255.0f;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Direct2DColor"/> struct with an alpha of 1.0f
        /// </summary>
        /// <param name="red">Red 0.0f - 1.0f</param>
        /// <param name="green">Green 0.0f - 1.0f</param>
        /// <param name="blue">Blue 0.0f - 1.0f</param>
        public Direct2DColor(float red, float green, float blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = 1.0f;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Direct2DColor"/> struct
        /// </summary>
        /// <param name="red">Red 0.0f - 1.0f</param>
        /// <param name="green">Green 0.0f - 1.0f</param>
        /// <param name="blue">Blue 0.0f - 1.0f</param>
        /// <param name="alpha">Alpha 0.0f - 1.0f</param>
        public Direct2DColor(float red, float green, float blue, float alpha)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="RawColor4"/> to <see cref="Direct2DColor"/>.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Direct2DColor(RawColor4 color)
        {
            return new Direct2DColor(color.R, color.G, color.B, color.A);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Direct2DColor"/> to <see cref="RawColor4"/>.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator RawColor4(Direct2DColor color)
        {
            return new RawColor4(color.Red, color.Green, color.Blue, color.Alpha);
        }
    }
}