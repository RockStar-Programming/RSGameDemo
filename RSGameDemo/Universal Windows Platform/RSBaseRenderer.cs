
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;

using Rockstar._BaseCanvas;
using Rockstar._FrameTimer;
            
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

namespace Rockstar._BaseRenderer
{
    public class RSBaseRenderer
    {
        // ********************************************************************************************
        // The Base Renderer for now only sets up a canvas for screen rendering
        //
        // This class will be changed at a later stage

        // ********************************************************************************************
        // Constructors

        public static RSBaseRenderer CreateWithWindowAndSize(CoreWindow window, Size size)
        {
            return new RSBaseRenderer(window, size);
        }

        private RSBaseRenderer(CoreWindow window, Size size)
        {
            _size = size;
            _timer = RSFrameTimer.Create();
            _canvas = RSBaseCanvas.Create(window, size, Colors.Black);
            Resize(_size);
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        public long FrameInterval { get { return _timer.Interval; } }
        public Size Size { get { return _size; } }
        public RSBaseCanvas Canvas { get { return _canvas; } }

        // ********************************************************************************************
        // Internal Data

        private Size _size;
        private RSFrameTimer _timer;
        private RSBaseCanvas _canvas;

        // ********************************************************************************************
        // Methods

        public void BeginFrame()
        {
            _timer.BeginFrame();
            _canvas.BeginFrame();
        }

        public void EndFrame()
        {
            RenderDebugInformation();
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
            string message = string.Format("Nodes:{0} @{1:00}x{2:00} - {3:0.0}fps", _canvas.NodeCount, _size.Width, _size.Height, _timer.FPS);
            _canvas.RenderDebugString(message);
        }

        // ********************************************************************************************
    }
}
