
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

namespace Rockstar._SpriteFrame
{
    public sealed class RSFont
    {
        // ********************************************************************************************
        // Basic font class
        //
        // See _readme.txt

        // ********************************************************************************************
        // Constructors

        public static RSFont Create()
        {
            return new RSFont(DEFAULT_FONT_NAME, DEFAULT_FONT_SIZE);
        }

        public static RSFont CreateWithName(string name, float size)
        {
            return new RSFont(name, size);
        }

        private RSFont(string name, float size)
        {
            Name = name;
            Size = size;
            Bold = false;
            Italic = false;
        }

        // ********************************************************************************************
        // Class Properties

        public static string DEFAULT_FONT_NAME = "Verdana";
        public static float DEFAULT_FONT_SIZE = 14;

        // ********************************************************************************************
        // Properties

        public string Name { get; set; }
        public float Size { get; set; }
        public bool Bold { get; set; }
        public bool Italic { get; set; }

        // ********************************************************************************************
        // Internal Data

        // ********************************************************************************************
        // Methods

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }
}
