using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
    public struct RSFont
    {
        // ********************************************************************************************
        // Basic font class
        // IMPORTANT:
        // RSFont MUST be a struct, so that direct assignment is possible

        // ********************************************************************************************
        // Constructors

        public static RSFont Create()
        {
            return new RSFont(FONT_NAME, FONT_SIZE);
        }

        public static RSFont Create(string name, float size)
        { 
            return new RSFont(name, size);
        }

        public RSFont(string name, float size)
        { 
            Name = name;
            Size = size;
            Bold = false;
            Italic = false;
        }

        // ********************************************************************************************
        // Properties

        public const string FONT_NAME = "Verdana";
        public const float FONT_SIZE = 16;

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
