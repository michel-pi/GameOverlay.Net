using System;
using System.Runtime.InteropServices;

using SharpDX.Mathematics.Interop;

namespace GameOverlay.Drawing
{
    /// <summary>
    /// Represents an ARGB (alpha, red, green, blue) Color.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Color
    {
        /// <summary>
        /// Returns a transparent Color.
        /// </summary>
        public static Color Transparent => new Color(0.0f, 0.0f, 0.0f, 0.0f);

        /// <summary>
        /// Returns a red Color.
        /// </summary>
        public static Color Red => new Color(1.0f, 0.0f, 0.0f);

        /// <summary>
        /// Returns a green Color.
        /// </summary>
        public static Color Green => new Color(0.0f, 1.0f, 0.0f);

        /// <summary>
        /// Returns a blue Color.
        /// </summary>
        public static Color Blue => new Color(0.0f, 0.0f, 1.0f);

        /// <summary>
        /// Gets the alpha component value of this Color.
        /// </summary>
        public float A;

        /// <summary>
        /// Gets the red component value of this Color.
        /// </summary>
        public float R;

        /// <summary>
        /// Gets the green component value of this Color.
        /// </summary>
        public float G;

        /// <summary>
        /// Gets the blue component value of this Color.
        /// </summary>
        public float B;

        /// <summary>
        /// Initializes a new Color using the specified components.
        /// </summary>
        /// <param name="r">The red component value of this Color.</param>
        /// <param name="g">The green component value of this Color.</param>
        /// <param name="b">The blue component value of this Color.</param>
        /// <param name="a">The alpha component value of this Color.</param>
        public Color(float r, float g, float b, float a = 1.0f)
        {
            R = NormalizeColor(r);
            G = NormalizeColor(g);
            B = NormalizeColor(b);

            A = NormalizeColor(a);
        }

        /// <summary>
        /// Initializes a new Color using the specified components.
        /// </summary>
        /// <param name="r">The red component value of this Color.</param>
        /// <param name="g">The green component value of this Color.</param>
        /// <param name="b">The blue component value of this Color.</param>
        /// <param name="a">The alpha component value of this Color.</param>
        public Color(int r, int g, int b, int a = 255)
        {
            R = NormalizeColor(r);
            G = NormalizeColor(g);
            B = NormalizeColor(b);

            A = NormalizeColor(a);
        }

        /// <summary>
        /// Initializes a new Color using the specified components.
        /// </summary>
        /// <param name="r">The red component value of this Color.</param>
        /// <param name="g">The green component value of this Color.</param>
        /// <param name="b">The blue component value of this Color.</param>
        /// <param name="a">The alpha component value of this Color.</param>
        public Color(byte r, byte g, byte b, byte a = 255)
        {
            R = r / 255.0f;
            G = g / 255.0f;
            B = b / 255.0f;

            A = a / 255.0f;
        }

        /// <summary>
        /// Initializes a new Color using the specified Color and the alpha value.
        /// </summary>
        /// <param name="color">A Color structure.</param>
        /// <param name="alpha">The alpha component of the Color.</param>
        public Color(Color color, float alpha = 1.0f)
        {
            R = color.R;
            G = color.G;
            B = color.B;

            A = NormalizeColor(alpha);
        }

        /// <summary>
        /// Initializes a new Color using the specified Color and the alpha value.
        /// </summary>
        /// <param name="color">A Color structure.</param>
        /// <param name="alpha">The alpha component of the Color.</param>
        public Color(Color color, int alpha = 255)
        {
            R = color.R;
            G = color.G;
            B = color.B;

            A = NormalizeColor(alpha);
        }

        /// <summary>
        /// Initializes a new Color using the specified Color and the alpha value.
        /// </summary>
        /// <param name="color">A Color structure.</param>
        /// <param name="alpha">The alpha component of the Color.</param>
        public Color(Color color, byte alpha = 255)
        {
            R = color.R;
            G = color.G;
            B = color.B;

            A = alpha;
        }

        /// <summary>
        /// Returns a value indicating whether this instance and a specified <see cref="T:System.Object" /> represent the same type and value.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true" /> if <paramref name="obj" /> is a Color and equal to this instance; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Color clr)
            {
                return clr.R == R
                    && clr.G == G
                    && clr.B == B
                    && clr.A == A;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a value indicating whether two specified instances of Color represent the same value.
        /// </summary>
        /// <param name="value">An object to compare to this instance.</param>
        /// <returns><see langword="true" /> if <paramref name="value" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
        public bool Equals(Color value)
        {
            return value.R == R
                && value.G == G
                && value.B == B
                && value.A == A;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return OverrideHelper.HashCodes(
                R.GetHashCode(),
                G.GetHashCode(),
                B.GetHashCode(),
                A.GetHashCode());
        }

        /// <summary>
        /// Converts this Color structure to a human-readable string.
        /// </summary>
        /// <returns>A string representation of this Color.</returns>
        public override string ToString()
        {
            return OverrideHelper.ToString(
                "R", R.ToString(),
                "G", G.ToString(),
                "B", B.ToString(),
                "A", A.ToString());
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

        /// <summary>
        /// Determines whether two specified instances are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns><see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> represent the same value; otherwise, <see langword="false" />.</returns>
        public static bool operator ==(Color left, Color right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Determines whether two specified instances are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns><see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> do not represent the same value; otherwise, <see langword="false" />.</returns>
        public static bool operator !=(Color left, Color right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Returns a value indicating whether two specified instances of Color represent the same value.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns> <see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, <see langword="false" />.</returns>
        public static bool Equals(Color left, Color right)
        {
            return left.Equals(right);
        }
    }
}
