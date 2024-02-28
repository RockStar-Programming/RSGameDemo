
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.Graphics.Display;
using Windows.UI.Text;

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

namespace Rockstar._BaseCanvas
{
    public class RSBaseCanvas
    {
        // ********************************************************************************************
        // RSBaseCanvas encapsulates a drawing canvas
        // All RSNode based classes uses this for rendering

        // ********************************************************************************************
        // Constructors

        public static RSBaseCanvas Create(CoreWindow window, Size size, Color color)
        {
            return new RSBaseCanvas(window, size, color);
        }

        private RSBaseCanvas(CoreWindow window, Size size, Color color)
        {
            _size = size;
            _color = color;
            float dpi = DisplayInformation.GetForCurrentView().LogicalDpi;
            _swapChain = CanvasSwapChain.CreateForCoreWindow(new CanvasDevice(), window, dpi);
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        public Matrix3x2 Transformation { get { return _session.Transform; } }
        public long NodeCount { get { return _nodeCount; } }

        // ********************************************************************************************
        // Internal Data

        private Size _size;
        private Color _color;
        private CanvasSwapChain _swapChain;
        private CanvasDrawingSession _session;
        private long _nodeCount;

        // ********************************************************************************************
        // Methods

        public void BeginFrame()
        {
            _session = _swapChain.CreateDrawingSession(_color);
            _nodeCount = 0;
        }

        public void EndFrame()
        {
            _session.Dispose();
            _swapChain.Present();
        }

        public void Resize(Size size)
        {
            float dpi = DisplayInformation.GetForCurrentView().LogicalDpi;
            _swapChain.ResizeBuffers((float)_size.Width, (float)_size.Height, dpi);
        }

        // ********************************************************************************************
        // Canvas transformation

        public void InitialiseTransformation(Matrix3x2 transformation)
        {
            _session.Transform = transformation;
        }

        public void AddTransformation(Matrix3x2 transformation)
        {
            _session.Transform = transformation * _session.Transform;
        }

        // ********************************************************************************************
        // Render Functions

        public void RenderRectangle(float x, float y, float width, float height, Color color)
        {
            _session.FillRectangle(x, y, width, height, color);
            _nodeCount++;
        }

        public void RenderEllipse(float x, float y, float width, float height, Color color)
        {
            _session.FillEllipse(x, y, width / 2, height / 2, color);
            _nodeCount++;
        }

        public void RenderText(float x, float y, string text, RSFont font, Color color)
        {
            CanvasTextFormat format = CreateCanvasTextFormat(text, font);

            _session.DrawText(text, x, y, color, format);
            _nodeCount++;
        }

        // ********************************************************************************************
        // Misc methods

        public Size CalculateStringSize(string text, RSFont font)
        {
            CanvasTextFormat format = CreateCanvasTextFormat(text, font);
            CanvasTextLayout textLayout = new CanvasTextLayout(_session, text, format, 0.0f, 0.0f);
            return new Size(textLayout.DrawBounds.Width, textLayout.DrawBounds.Height);
        }

        public void RenderDebugString(string message)
        {
            // reset transformation
            InitialiseTransformation(Matrix3x2.Identity);

            _session.DrawText(message, _swapChain.Size.ToVector2(), Colors.White,
                new CanvasTextFormat()
                {
                    FontSize = 16,
                    HorizontalAlignment = CanvasHorizontalAlignment.Right,
                    VerticalAlignment = CanvasVerticalAlignment.Bottom
                });
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        private CanvasTextFormat CreateCanvasTextFormat(string text, RSFont font)
        {
            CanvasTextFormat result = new CanvasTextFormat();

            // as font is a struct, member might be null
            if (font.Name == null) font.Name = RSFont.DEFAULT_FONT_NAME;
            if (font.Size == 0) font.Size = RSFont.DEFAULT_FONT_SIZE; ;

            result.FontSize = font.Size;
            result.FontWeight = (font.Bold == true) ? FontWeights.Bold : FontWeights.Normal;
            result.FontStyle = (font.Italic == true) ? FontStyle.Italic : FontStyle.Normal;
            result.FontFamily = font.Name;
            result.HorizontalAlignment = CanvasHorizontalAlignment.Left;
            result.VerticalAlignment = CanvasVerticalAlignment.Top;
            result.WordWrapping = CanvasWordWrapping.NoWrap;

            return result;
        }

        // ********************************************************************************************
    }
}
