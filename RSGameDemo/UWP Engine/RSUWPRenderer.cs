using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Rockstar.FrameTimer;
using Rockstar.Nodes;
using Rockstar.UWPCanvas;
using System.Numerics;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Core;

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

namespace Rockstar.UWPRenderer
{
    public class RSUWPRenderer
    {
        // ********************************************************************************************
        // Brief Class Description
        //
        //

        // ********************************************************************************************
        // Constructors

        public static RSUWPRenderer CreateWithWindowAndSize(CoreWindow window, Size size)
        { 
            return new RSUWPRenderer(window, size);
        }

        private RSUWPRenderer(CoreWindow window, Size size)
        {
            _size = size;
            _timer = RSFrameTimer.Create();
            _canvas = RSUWPCanvas.Create(window, size, Colors.Black);
            Resize(_size);
        }

        // ********************************************************************************************
        // Properties

        public long FrameInterval { get { return _timer.Interval; } }
        public Size Size { get { return _size; } }

        // ********************************************************************************************
        // Internal Data

        private Size _size;
        private RSFrameTimer _timer;
        private RSUWPCanvas _canvas;

        // ********************************************************************************************
        // Methods

        public void BeginFrame()
        {
            _timer.BeginFrame();
            _canvas.BeginFrame();
        }

        public void RenderScene(RSNodeScene scene)
        {
            _canvas.RenderScene(scene);

            RenderDebugInformation();
        }

        public void EndFrame()
        {
            _canvas.EndFrame();
        }

        public void Resize(Size size)
        {
            _canvas.Resize(size);
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        private void RenderDebugInformation()
        {
            string message = string.Format("{0:0.0}fps @{1:00}x{2:00} -", _timer.FPS, _size.Width, _size.Height);
            _canvas.RenderDebugString(message);
        }

        // ********************************************************************************************
    }
}
