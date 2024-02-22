using System;
using System.Numerics;
using Windows.Foundation;

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

namespace Rockstar.Types
{
    // ********************************************************************************************
    // Defines some basic assignable types
    //
    // IMPORTANT:
    // These MUST all be structs, so that direct assignment is possible

    public struct RSVector2
    {
        public float X { get; set; }
        public float Y { get; set; }

        public RSVector2(float value)
        {
            X = value;
            Y = value;
        }

        public RSVector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public RSVector2(double x, double y)
        {
            X = (float)x;
            Y = (float)y;
        }

        public RSVector2(RSVector2 vector)
        {
            X = vector.X;
            Y = vector.Y;
        }

        public Vector2 Vector2()
        {
            return new Vector2(X, Y);
        }

        public float Length()
        {
            return MathF.Sqrt((X * X) + (Y * Y));
        }

        public float DistanceTo(RSVector2 position)
        {
            float x = X - position.X;
            float y = Y - position.Y;

            return MathF.Sqrt((x * x) + (y * y));
        }

        public RSVector2 Add(RSVector2 vector)
        {
            return new RSVector2(X + vector.X, Y + vector.Y);
        }

        public RSVector2 Rotate(float angle)
        {
            float phi = -angle * (float)MathF.PI / 180.0f;
            float x = ((float)Math.Cos(phi) * X) - ((float)Math.Sin(phi) * Y);
            float y = ((float)Math.Sin(phi) * X) + ((float)Math.Cos(phi) * Y);
            return new RSVector2(x, y);
        }
    }

    public struct RSSize
    {
        public float Width { get; set; }
        public float Height { get; set; }

        public RSSize(float size)
        {
            Width = size;
            Height = size;
        }

        public RSSize(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public RSSize(double width, double height)
        {
            Width = (float)width;
            Height = (float)height;
        }

        public Size Size()
        {
            return new Size(Width, Height);
        }

    }
}
