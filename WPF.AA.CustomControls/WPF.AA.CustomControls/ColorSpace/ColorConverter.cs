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
        // https://stackoverflow.com/questions/359612/how-to-convert-rgb-color-to-hsv
        // https://bootcamp.uxdesign.cc/definitive-guide-to-wpf-colors-color-spaces-color-pickers-and-creating-your-own-colors-for-mere-f480935c6e94

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
                r = GetComponentHue(min, max, hue + 1.0f / 3.0f);
                g = GetComponentHue(min, max, hue);
                b = GetComponentHue(min, max, hue - 1.0f / 3.0f);
            }
            else // ensure greys are not converted to white
            {
                r = hsl.L;
                g = hsl.L;
                b = hsl.L;
            }

            return System.Drawing.Color.FromArgb(255, (byte)Math.Round(r * 255), (byte)Math.Round(g * 255), (byte)Math.Round(b * 255));
        }

        private static float GetComponentHue(float min, float max, float hue)
        {
            float value = min;

            if (hue < 0.0f) hue++;
            if (hue > 1.0f) hue--;
            
            if (hue * 6.0f < 1.0f)
                value = min + (max - min) * 6.0f * hue;
            else if (hue * 2.0f < 1.0f)
                value = max;
            else if (hue * 3.0f < 2.0f)
                value = min + (max - min) * (2.0f / 3.0f - hue) * 6.0f;

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

        public static CMY ConvertRgbToCmy(Color color)
        {
            return ConvertRgbToCmy(ConvertMediaColorToDrawingColor(color));
        }

        public static CMY ConvertRgbToCmy(System.Drawing.Color color)
        {
            return new CMY
            {
                C = (byte)(1 - color.R / 255),
                M = (byte)(1 - color.G / 255),
                Y = (byte)(1 - color.B / 255),
            };
        }

        public static CMYK ConvertRgbToCmyk(Color color)
        {
            return ConvertRgbToCmyk(ConvertMediaColorToDrawingColor(color));
        }

        public static CMYK ConvertRgbToCmyk(System.Drawing.Color color)
        {
            double r, g, b, c, m, y, k;

            r = color.R / 255.0;
            g = color.G / 255.0;
            b = color.B / 255.0;

            k = 1 - new List<double>() { r, g, b }.Max();
            c = (1 - r - k) / (1 - k);
            m = (1 - g - k) / (1 - k);
            y = (1 - b - k) / (1 - k);

            return new CMYK
            {
                C = (byte)Math.Round(c * 100),
                M = (byte)Math.Round(m * 100),
                Y = (byte)Math.Round(y * 100),
                K = (byte)Math.Round(k * 100)
            };
        }

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

        public static XYZ ConvertRgbToXyz(Color color)
        {
            return ConvertRgbToXyz(ConvertMediaColorToDrawingColor(color));
        }

        public static XYZ ConvertRgbToXyz(System.Drawing.Color color)
        {
            double[] modifiedRGB = { color.R / 255.0, color.G / 255.0, color.B / 255.0 };

            for (var x = 0; x < modifiedRGB.Length; x++)
            {
                modifiedRGB[x] = (modifiedRGB[x] > 0.04045) 
                    ? Math.Pow((modifiedRGB[x] + 0.055) / 1.055, 2.4) 
                    : modifiedRGB[x] / 12.92;

                modifiedRGB[x] *= 100;
            }

            // XYZ output will be a D65/2 standard illumnant
            return new XYZ
            {
                X = (float)(modifiedRGB[0] * 0.4124f + modifiedRGB[1] * 0.3576f + modifiedRGB[2] * 0.1805f),
                Y = (float)(modifiedRGB[0] * 0.2126f + modifiedRGB[1] * 0.7152f + modifiedRGB[2] * 0.0722f),
                Z = (float)(modifiedRGB[0] * 0.0193f + modifiedRGB[1] * 0.1192f + modifiedRGB[2] * 0.9505f)
            };
        }

        public static YXY ConvertRgbToYxy(Color color)
        {
            return ConvertRgbToYxy(ConvertMediaColorToDrawingColor(color));
        }

        public static YXY ConvertRgbToYxy(System.Drawing.Color color)
        {
            XYZ xyz = ConvertRgbToXyz(color);

            return new YXY
            {
                Y = xyz.Y,
                x = xyz.X / (xyz.X + xyz.Y + xyz.Z),
                y = xyz.Y / (xyz.X + xyz.Y + xyz.Z),
            };
        }

        #endregion

        #region XYZ

        #endregion

        #region YXY

        #endregion

        #endregion
    }
}
