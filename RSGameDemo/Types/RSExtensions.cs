using Rockstar._Dictionary;
using Rockstar._Array;
using System;
using System.Numerics;

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
    public static class RSExtensions
    {
        // ********************************************************************************************
        // Vector2 Extensions

        // Rotations are clock-wise
        public static Vector2 Rotate(this Vector2 vector, float angle)
        {
            float phi = -angle * (float)Math.PI / 180.0f;
            float x = ((float)Math.Cos(phi) * vector.X) - ((float)Math.Sin(phi) * vector.Y);
            float y = ((float)Math.Sin(phi) * vector.X) + ((float)Math.Cos(phi) * vector.Y);
            return new Vector2(x, y);
        }

        // ********************************************************************************************

    }
}