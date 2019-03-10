using System;
using System.Runtime.InteropServices;

using SharpDX.Mathematics.Interop;

namespace GameOverlay.Drawing
{
    /// <summary>
    /// Represents an ARGB (alpha, red, green, blue) color.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Color
    {
        /// <summary>
        /// Returns a transparent color.
        /// </summary>
        public static Color Transparent => new Color(0.0f, 0.0f, 0.0f, 0.0f);
        /// <summary>
        /// Returns a red color.
        /// </summary>
        public static Color Red => new Color(1.0f, 0.0f, 0.0f);
        /// <summary>
        /// Returns a green color.
        /// </summary>
        public static Color Green => new Color(0.0f, 1.0f, 0.0f);
        /// <summary>
        /// Returns a blue color.
        /// </summary>
        public static Color Blue => new Color(0.0f, 0.0f, 1.0f);

        /// <summary>
        /// Gets the alpha component value of this color.
        /// </summary>
        public float A;
        /// <summary>
        /// Gets the red component value of this color.
        /// </summary>
        public float R;
        /// <summary>
        /// Gets the green component value of this color.
        /// </summary>
        public float G;
        /// <summary>
        /// Gets the blue component value of this color.
        /// </summary>
        public float B;

        /// <summary>
        /// Initializes a new color using the specified components.
        /// </summary>
        /// <param name="r">The red component value of this color.</param>
        /// <param name="g">The green component value of this color.</param>
        /// <param name="b">The blue component value of this color.</param>
        /// <param name="a">The alpha component value of this color.</param>
        public Color(float r, float g, float b, float a = 1.0f)
        {
            R = NormalizeColor(r);
            G = NormalizeColor(g);
            B = NormalizeColor(b);

            A = NormalizeColor(a);
        }

        /// <summary>
        /// Initializes a new color using the specified components.
        /// </summary>
        /// <param name="r">The red component value of this color.</param>
        /// <param name="g">The green component value of this color.</param>
        /// <param name="b">The blue component value of this color.</param>
        /// <param name="a">The alpha component value of this color.</param>
        public Color(int r, int g, int b, int a = 255)
        {
            R = NormalizeColor(r);
            G = NormalizeColor(g);
            B = NormalizeColor(b);

            A = NormalizeColor(a);
        }

        /// <summary>
        /// Initializes a new color using the specified components.
        /// </summary>
        /// <param name="r">The red component value of this color.</param>
        /// <param name="g">The green component value of this color.</param>
        /// <param name="b">The blue component value of this color.</param>
        /// <param name="a">The alpha component value of this color.</param>
        public Color(byte r, byte g, byte b, byte a = 255)
        {
            R = r / 255.0f;
            G = g / 255.0f;
            B = b / 255.0f;

            A = a / 255.0f;
        }

        /// <summary>
        /// Converts this Color structure to a human-readable string.
        /// </summary>
        /// <returns>A string representation of this Color.</returns>
        public override string ToString()
        {
            return "{A=" + A + ", R=" + R + ", G=" + G + ", B=" + B + "}";
        }

        /// <summary>
        /// Gets the 32-bit ARGB value of this Color structure.
        /// </summary>
        /// <returns>The 32-bit ARGB value of this Color.</returns>
        public int ToARGB()
        {
            return ((int)(R * 255.0f) << 16 | (int)(G * 255.0f) << 8 | (int)(B * 255.0f) | (int)(A * 255.0f) << 24) & -1;
        }

        /// <summary>
        /// Creates a Color structure from a 32-bit ARGB value.
        /// </summary>
        /// <param name="value">A value specifying the 32-bit ARGB value.</param>
        /// <returns>The Color structure that this method creates.</returns>
        public static Color FromARGB(int value)
        {
            return new Color(
                value >> 16 & 255,
                value >> 8 & 255,
                value & 255,
                value >> 24 & 255);
        }

        private static float NormalizeColor(float color)
        {
            if (color < 0.0f) color *= -1.0f;

            if (color <= 1.0f) return color;

            while (color > 255.0f) color /= 255.0f;

            return color / 255.0f;
        }

        private static float NormalizeColor(int color)
        {
            if (color < 0) color *= -1;

            while (color > 255) color /= 255;

            return color / 255.0f;
        }
        
        /// <summary>
        /// Converts a SharpDX RawColor4 to a Color
        /// </summary>
        /// <param name="color">A RawColor4</param>
        public static implicit operator Color(RawColor4 color)
        {
            return new Color(color.R, color.G, color.B, color.A);
        }

        /// <summary>
        /// Converts a Color to a SharpDX RawColor4
        /// </summary>
        /// <param name="color"></param>
        public static implicit operator RawColor4(Color color)
        {
            return new RawColor4(color.R, color.G, color.B, color.A);
        }
    }
}
