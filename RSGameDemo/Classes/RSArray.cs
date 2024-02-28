
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;

using Rockstar._Types;

// ****************************************************************************************************
// Copyright(c) 2024 Lars B. Amundsen
//
// Permission is hereby granted, free of charge, to any person obtaining alpha copy of this software
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

namespace Rockstar._Array
{
    public class RSArray : IEnumerable<object>
    {
        // ********************************************************************************************
        // RSArray wraps an List<object>
        //
        // Main functionality is to:
        // - Predictable operation and no crashes
        // - Implement meaningful constructors
        // - Implement convenience getters
        //
        // Apart from that, RSArray implements a series of ToXxxx methods, converting the array into
        // commonly used structures and classes

        // ********************************************************************************************
        // Constructors

        public static RSArray Create() 
        { 
            return new RSArray();
        }

        public static RSArray CreateWithList(List<object> list)
        { 
            return new RSArray(list);
        }

        public static RSArray CreateWithArray(RSArray array)
        {
            return new RSArray(array.Content);
        }

        public static RSArray CreateWithObject(object value)
        {
            if (value is RSArray result) return CreateWithArray(result);
            return RSArray.Create();
        }

        public RSArray(List<object> list = null)
        {
            _content = new List<object>();
            if (list != null)
            {
                foreach (object item in list)
                {
                    _content.Add(item);
                }
            }
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        public List<object> Content { get { return _content; } }
        public int Count { get { return _content.Count; } }

        // ********************************************************************************************
        // Internal Data

        private List<object> _content;

        // ********************************************************************************************
        // Methods 

        // ********************************************************************************************
        // Methods Getters (convenience wrappers)

        public object GetObject(int index, object fallback = null)
        {
            if (GetEntry(index) is object result) return result;
            return fallback;
        }

        public bool GetBool(int index, bool fallback)
        {
            if (GetEntry(index) is bool result) return result;
            return fallback;
        }

        public long GetLong(int index, long fallback)
        {
            if (GetEntry(index) is long result) return result;
            return fallback;
        }

        public float GetFloat(int index, float fallback)
        {
            return (float)GetDouble(index, fallback);
        }

        public double GetDouble(int index, double fallback)
        {
            object result = GetEntry(index);
            if (result is long) return (long)result;
            if (result is double) return (double)result;
            return fallback;
        }

        public string GetString(int index, string fallback)
        {
            if (GetEntry(index) is string result) return result;
            return fallback;
        }

        public RSArray GetArray(int index, RSArray fallback = null)
        {
            if (GetEntry(index) is RSArray result) return result;
            if (fallback == null) return RSArray.Create();
            return fallback;
        }

        // ********************************************************************************************
        // Methods Converting

        // Converts an array to a color
        public Color ToColor()
        {
            byte red = 255, green, blue, alpha = 255;

            // 0 values = full white 
            // 1 value = greyscale, full alpha
            // 2 values = greyscale, alpha
            // 3 values = RGB, full alpha
            // 4 or more = RGBA
            if (Count >= 1) red = (byte)GetLong(0, 255);
            if (Count >= 2) green = (byte)GetLong(1, 255); else green = red;
            if (Count >= 3) blue = (byte)GetLong(2, 255); else blue = red;
            if (Count >= 4) alpha = (byte)GetLong(3, 255);
            if (Count == 2)
            {
                alpha = green; 
                green = red;
            }

            return Color.FromArgb(alpha, red, green, blue);
        }

        public Size ToSize()
        {
            float x = 0, y;

            // 0 values = Size(0, 0); 
            // 1 value = Size(value, value)
            // 2 values = Size(value1, value2)
            if (Count >= 1) x = GetFloat(0, 0);
            if (Count >= 2) y = GetFloat(1, 0); else y = x;

            return new Size(x, y);
        }

        public Vector2 ToVector2()
        {
            float x = 0, y;

            // 0 values = Vector2(0, 0); 
            // 1 value = Vector2(value, value)
            // 2 values = Vector2(value1, value2)
            if (Count >= 1) x = GetFloat(0, 0);
            if (Count >= 2) y = GetFloat(1, 0); else y = x;

            return new Vector2(x, y);
        }

        public RSFont ToFont()
        {
            RSFont result = RSFont.Create();

            // value 0 = font name
            // value 1 = size
            // value 2 = bold
            // value 3 = italic
            if (Count >= 1) result.Name = GetString(0, result.Name);
            if (Count >= 2) result.Size = GetFloat(1, result.Size);
            if (Count >= 3) result.Bold = GetBool(2, result.Bold);
            if (Count >= 4) result.Italic = GetBool(3, result.Italic);

            return result;
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // GetEntry is the only methods used to retrieve data from the underlying _content
        //
        private object GetEntry(int index)
        {
            try
            {
                return _content[index];
            }
            catch (Exception)
            {
            }

            // if application gets here, an exception was thrown
            return null;
        }

        // ********************************************************************************************
        // IEnumerator Implementation

        public IEnumerator<object> GetEnumerator()
        {
            return ((IEnumerable<object>)_content).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_content).GetEnumerator();
        }

        // ********************************************************************************************
    }
}
