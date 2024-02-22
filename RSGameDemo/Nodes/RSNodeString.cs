
using Rockstar.EngineCanvas;
using Rockstar.Types;
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

namespace Rockstar.Nodes
{
    internal class RSNodeString : RSNode
    {
        // ********************************************************************************************
        // Implementation of node based string
        // IMPORTANT:
        // The Transformation.Size of the node is ALWAYS set to the actual render size, when the node is rendered 

        // ********************************************************************************************
        // Constructors

        public static RSNodeString CreateString(string text, Vector2 position, RSFont font)
        {
            return new RSNodeString(text, position, font);
        }

        private RSNodeString()
        {
        }

        protected RSNodeString(string text, Vector2 position, RSFont font)
        {
            _text = text;
            _font = font;
            InitWithData(position);
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        public RSFont Font { get; }

        // ********************************************************************************************
        // Internal Data

        private string _text;
        private RSFont _font;

        // ********************************************************************************************
        // Methods

        public override void Render(RSEngineCanvas canvas)
        {
            // NOTE: Size must be set prior to doing transformations in base node
            Transformation.Size = canvas.CalculateStringSize(_text, _font);

            base.Render(canvas);

            Vector2 upperLeft = new Vector2((float)-_transformation.Size.Width * _transformation.Anchor.X, (float)-_transformation.Size.Height * (1.0f - _transformation.Anchor.Y));
            canvas.RenderText(upperLeft.X, upperLeft.Y, _text, _font, Transformation.Color);
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }
}
