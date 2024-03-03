
using System.Numerics;
using SkiaSharp;

using Rockstar._RenderSurface;
using Rockstar._Types;

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

namespace Rockstar._NodeList
{
    internal class RSNodeString : RSNode
    {
        // ********************************************************************************************
        // Implementation of node based string
        //
        // IMPORTANT:
        // The Transformation.Size of the node is ALWAYS set to the actual render size, when the node is rendered 

        // ********************************************************************************************
        // Constructors

        public static RSNodeString CreateString(Vector2 position, string text, RSFont font)
        {
            return new RSNodeString(position, text, font);
        }

        protected RSNodeString(Vector2 position, string text, RSFont font)
        {
            _text = text;
            _font = font;
            InitWithData(position);
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        public string Text { get { return _text; } }
        public RSFont Font { get { return _font; } }

        // ********************************************************************************************
        // Internal Data

        private string _text;
        private RSFont _font;

        // ********************************************************************************************
        // Methods

        public override bool PointInside(Vector2 screenPosition)
        {
            return PointInsizeRectangle(screenPosition);
        }

        public override void Render(RSRenderSurface surface)
        {
            SKPaint paint = surface.GetTextPaint(_font, _transformation.Color); 

            // NOTE: Transformation.Size must be set prior to doing any calculations
            // Get the size of the text
            float width = paint.MeasureText(_text);
            SKFontMetrics metrics = paint.FontMetrics;
            float height = metrics.Descent - metrics.Ascent;
            _transformation.Size = new Size((int)width, (int)height);

            float offset = metrics.XHeight / 2;
            Vector2 center = new Vector2(-_transformation.Size.Width * _transformation.Anchor.X, ((float)_transformation.Size.Height * (_transformation.Anchor.Y - 0.5f)) + offset);

            surface.DrawText(center.X, center.Y, _text, paint);
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }
}
