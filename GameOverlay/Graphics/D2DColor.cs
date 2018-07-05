using System;

using SharpDX.Mathematics.Interop;

namespace GameOverlay.Graphics
{
    /// <summary>
    /// Represents a ARGB color
    /// </summary>
    public struct D2DColor
    {
        /// <summary>
        /// a
        /// </summary>
        public float A;

        /// <summary>
        /// The r
        /// </summary>
        public float R;
        /// <summary>
        /// The g
        /// </summary>
        public float G;
        /// <summary>
        /// The b
        /// </summary>
        public float B;

        /// <summary>
        /// Initializes a new instance of the <see cref="D2DColor"/> struct.
        /// </summary>
        /// <param name="r">The r.</param>
        /// <param name="g">The g.</param>
        /// <param name="b">The b.</param>
        /// <param name="a">a.</param>
        public D2DColor(float r, float g, float b, float a = 1.0f)
        {
            R = NormalizeColor(r);
            G = NormalizeColor(g);
            B = NormalizeColor(b);

            A = NormalizeColor(a);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="D2DColor"/> struct.
        /// </summary>
        /// <param name="r">The r.</param>
        /// <param name="g">The g.</param>
        /// <param name="b">The b.</param>
        /// <param name="a">a.</param>
        public D2DColor(int r, int g, int b, int a = 255)
        {
            R = NormalizeColor(r);
            G = NormalizeColor(g);
            B = NormalizeColor(b);

            A = NormalizeColor(a);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="D2DColor"/> struct.
        /// </summary>
        /// <param name="r">The r.</param>
        /// <param name="g">The g.</param>
        /// <param name="b">The b.</param>
        /// <param name="a">a.</param>
        public D2DColor(byte r, byte g, byte b, byte a = 255)
        {
            R = (float)r / 255.0f;
            G = (float)g / 255.0f;
            B = (float)b / 255.0f;

            A = (float)a / 255.0f;
        }

        private static float NormalizeColor(float color)
        {
            if (color < 0.0f) color *= -1.0f;

            if (color > 1.0f)
            {
                while (color > 255.0f) color /= 255.0f;

                return color / 255.0f;
            }

            return color;
        }

        private static float NormalizeColor(int color)
        {
            if (color < 0) color *= -1;

            while (color > 255) color /= 255;

            return (float)color / 255.0f;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="RawColor4"/> to <see cref="D2DColor"/>.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator D2DColor(RawColor4 color)
        {
            return new D2DColor(color.R, color.G, color.B, color.A);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="D2DColor"/> to <see cref="RawColor4"/>.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator RawColor4(D2DColor color)
        {
            return new RawColor4(color.R, color.G, color.B, color.A);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "{A=" + A + ", R=" + R + ", G=" + G + ", B=" + B + "}";
        }
    }
}