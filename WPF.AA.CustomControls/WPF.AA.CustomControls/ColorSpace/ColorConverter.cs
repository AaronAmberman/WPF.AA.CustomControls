using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace WPF.AA.CustomControls.ColorSpace
{
    /// <summary>Helps convert colors between different color spaces.</summary>
    public static class ColorConverter
    {
        // https://www.easyrgb.com/en/math.php#text20
        // https://github.com/xceedsoftware/wpftoolkit/blob/master/ExtendedWPFToolkitSolution/Src/Xceed.Wpf.Toolkit/Core/Utilities/ColorUtilities.cs
        // https://stackoverflow.com/questions/1335426/is-there-a-built-in-c-net-system-api-for-hsv-to-rgb
        // https://stackoverflow.com/questions/359612/how-to-convert-rgb-color-to-hsv

        #region Methods

        #region Convert Between Media.Color and Drawing.Color

        public static Color ConvertDrawingColorToMediaColor(System.Drawing.Color color)
        {
            return new Color
            {
                A = color.A,
                B = color.B,
                G = color.G,
                R = color.R
            };
        }

        public static System.Drawing.Color ConvertMediaColorToDrawingColor(Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        #endregion

        #region CMY

        #endregion

        #region CMYK

        #endregion

        #region HSL

        public static Color ConvertHslToRgb(HSL hsl)
        {
            return ConvertDrawingColorToMediaColor(ConvertHslToRgbDrawing(hsl));
        }

        public static System.Drawing.Color ConvertHslToRgbDrawing(HSL hsl)
        {
            float hue, r, g, b, max, min;

            hue = hsl.H / 360.0f;
            max = (hsl.L < 0.5) ? hsl.L * (1.0f + hsl.S) : hsl.L + hsl.S - (hsl.L * hsl.S);
            min = 2.0f * hsl.L - max;

            if (hsl.L == 0)  // 0 is always be black
            {
                r = 0;
                g = 0;
                b = 0;
            }
            else if (hsl.S != 0)
            {
                r = GetHue(min, max, hue + 1.0f / 3.0f);
                g = GetHue(min, max, hue);
                b = GetHue(min, max, hue - 1.0f / 3.0f);
            }
            else // ensure greys are not converted to white
            {
                r = hsl.L;
                g = hsl.L;
                b = hsl.L;
            }

            return System.Drawing.Color.FromArgb(255, (byte)Math.Round(r * 255), (byte)Math.Round(g * 255), (byte)Math.Round(b * 255));
        }

        private static float GetHue(float p, float q, float t)
        {
            float value = p;

            if (t < 0.0f) t++;
            if (t > 1.0f) t--;
            
            if (t * 6.0f < 1.0f)
                value = p + (q - p) * 6.0f * t;
            else if (t * 2.0f < 1.0f)
                value = q;
            else if (t * 3.0f < 2.0f)
                value = p + (q - p) * (2.0f / 3.0f - t) * 6.0f;

            return value;
        }

        #endregion

        #region HSV

        public static Color ConvertHsvToRgb(HSV hsv)
        {
            return ConvertDrawingColorToMediaColor(ConvertHsvToRgbDrawing(hsv));
        }

        public static System.Drawing.Color ConvertHsvToRgbDrawing(HSV hsv)
        {
            if (hsv.S == 0)
                return System.Drawing.Color.FromArgb(255, (byte)hsv.V, (byte)hsv.V, (byte)hsv.V);
            else if (hsv.V < 0)
                return System.Drawing.Color.FromArgb(255, 0, 0, 0);

            int hi = Convert.ToInt32(Math.Floor(hsv.H / 60)) % 6;
            float f = hsv.H / 60 - (float)Math.Floor(hsv.H / 60);

            float value = hsv.V * 255;

            byte v = Convert.ToByte(value);
            byte p = Convert.ToByte(value * (1 - hsv.S));
            byte q = Convert.ToByte(value * (1 - f * hsv.S));
            byte t = Convert.ToByte(value * (1 - (1 - f) * hsv.S));

            if (hi == 0)
                return System.Drawing.Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return System.Drawing.Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return System.Drawing.Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return System.Drawing.Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return System.Drawing.Color.FromArgb(255, t, p, v);
            else
                return System.Drawing.Color.FromArgb(255, v, p, q);
        }

        #endregion

        #region RGB

        public static HSL ConvertRgbToHsl(Color color) 
        {
            return ConvertRgbToHsl(ConvertMediaColorToDrawingColor(color));
        }

        public static HSL ConvertRgbToHsl(System.Drawing.Color color)
        {
            return new HSL
            {
                H = color.GetHue(),
                S = color.GetSaturation(),
                L = color.GetBrightness()
            };
        }

        public static HSV ConvertRgbToHsv(Color color)
        {
            return ConvertRgbToHsv(ConvertMediaColorToDrawingColor(color));
        }

        public static HSV ConvertRgbToHsv(System.Drawing.Color color)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            float hue = color.GetHue();
            float saturation = (max == 0) ? 0 : 1f - (1f * min / max);
            float value = max / 255f;

            return new HSV
            {
                H = hue,
                S = saturation,
                V = value
            };
        }

        #endregion

        #region XYX

        #endregion

        #region YXY

        #endregion

        #endregion
    }
}
