
using SkiaSharp;

// ****************************************************************************************************
// Copyright(c) 2024 Lars B. Amundsen
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software
// and associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies
// or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE
// AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ****************************************************************************************************

namespace Rockstar._Types
{
    public static class RSExtensions
    {
        // ********************************************************************************************
        // SKPoint Extensions

        // Rotations are clock-wise
        public static SKPoint Rotate(this SKPoint vector, float angle)
        {
            float phi = -angle * (float)Math.PI / 180.0f;
            float x = ((float)Math.Cos(phi) * vector.X) - ((float)Math.Sin(phi) * vector.Y);
            float y = ((float)Math.Sin(phi) * vector.X) + ((float)Math.Cos(phi) * vector.Y);
            return new SKPoint(x, y);
        }

        // ********************************************************************************************
        // RSNode Extensions

        public static bool ToBool(this object value)
        {
            if (value is bool result) return result;
            return false;
        }

        public static float ToFloat(this object value)
        {
            if (value is float result) return result;
            return 0.0f;
        }

        public static SKPoint ToPoint(this object value)
        {
            if (value is SKPoint result) return result;
            return SKPoint.Empty;
        }

        public static SKSize ToSize(this object value)
        {
            if (value is SKSize result) return result;
            return SKSize.Empty;
        }

        public static TEnum ToEnum<TEnum>(this object value) where TEnum : struct, Enum
        {
            if (value is string)
            {
                if (Enum.TryParse(value.ToString(), true, out TEnum result) == true) return result;
            }
            // Return default
            return default;
        }

        public static float ClampNormalised(this float value)
        {
            if (value < 0.0f) return 0.0f;
            if (value > 1.0f) return 1.0f;
            return value;
        }

        public static byte ClampByte(this float value)
        {
            if (value < 0.0f) return 0;
            if (value > 255.0f) return 255;
            return (byte)value;
        }

        // ********************************************************************************************
    }
}