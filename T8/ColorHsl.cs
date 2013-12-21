using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Plot3D
{
    /// <summary>Represents an AHSL color.</summary>
    [Serializable, StructLayout(LayoutKind.Sequential), DebuggerDisplay("\\{AHSL = ({A}, {H}, {S}, {L})\\}")]
    public struct ColorHsl : IEquatable<Color>
    {
        /// <summary>Initializes a new instance of the <see cref="T:LukeSw.Drawing.ColorHsl"/> structure
        /// from the specified double values.</summary>
        /// <param name="alpha">The alpha component value. Valid values are 0 through 1.</param>
        /// <param name="hue">The hue component value. Valid values are 0 through 360.</param>
        /// <param name="saturation">The saturation component value. Valid values are 0 through 1.</param>
        /// <param name="lightness">The lightness component value. Valid values are 0 through 1.</param>
        public ColorHsl(double alpha, double hue, double saturation, double lightness)
        {
            ColorRgb.Checkdouble(alpha, "alpha");
            ColorRgb.Checkdouble(hue, "hue", 0.0, 360.0);
            if (hue == 360.0)
            {
                hue = 0.0;
            }
            ColorRgb.Checkdouble(saturation, "saturation");
            ColorRgb.Checkdouble(lightness, "lightness");
            this.A = alpha;
            this.H = hue;
            this.S = saturation;
            this.L = lightness;
        }

        /// <summary>Initializes a new instance of the <see cref="T:LukeSw.Drawing.ColorHsl"/> structure
        /// from the specified double values. The alpha value is implicitly 1 (fully opaque).</summary>
        /// <param name="hue">The hue component value. Valid values are 0 through 360.</param>
        /// <param name="saturation">The saturation component value. Valid values are 0 through 1.</param>
        /// <param name="lightness">The lightness component value. Valid values are 0 through 1.</param>
        public ColorHsl(double hue, double saturation, double lightness)
            : this(1.0, hue, saturation, lightness) { }

        /// <summary>Gets the alpha component value.</summary>
        public readonly double A;
        /// <summary>Gets the hue component value.</summary>
        public readonly double H;
        /// <summary>Gets the saturation component value.</summary>
        public readonly double S;
        /// <summary>Gets the lightness component value.</summary>
        public readonly double L;

        /// <summary>Converts this <see cref="T:LukeSw.Drawing.ColorHsl" /> structure to a human-readable string.</summary>
        /// <returns>String that consists of the AHSL component names and their values.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x20);
            builder.Append(GetType().Name);
            builder.Append(" [");
            builder.Append("A=");
            builder.Append(this.A);
            builder.Append(", H=");
            builder.Append(this.H);
            builder.Append(", S=");
            builder.Append(this.S);
            builder.Append(", L=");
            builder.Append(this.L);
            builder.Append("]");
            return builder.ToString();
        }

        public override bool Equals(object obj)
        {
            return ToColorRgb().Equals(obj);
        }

        public bool Equals(Color other)
        {
            return ToColorRgb() == ColorRgb.FromColor(other);
        }

        public override int GetHashCode()
        {
            return A.GetHashCode() ^ H.GetHashCode() ^ S.GetHashCode() ^ L.GetHashCode();
        }
        public Color ToColor()
        {
            return ColorRgb.FromColor(this).ToColor();
        }

        public ColorRgb ToColorRgb()
        {
            return ColorRgb.FromColor(this);
        }

        public ColorHsv ToColorHsv()
        {
            return ColorRgb.FromColor(this).ToColorHsv();
        }

        public static ColorHsl FromColor(Color color)
        {
            return ColorRgb.FromColor(color).ToColorHsl();
        }
    }
}
