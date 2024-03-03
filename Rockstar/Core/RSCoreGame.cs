
using SkiaSharp;

using Rockstar._FrameTimer;
using Rockstar._NodeList;
using Rockstar._Renderer;
using Rockstar._RenderSurface;
using Rockstar._CoreMouse;
using Rockstar._NodeList;

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

namespace Rockstar._CoreGame
{
    public abstract class RSCoreGame
    {
        // ********************************************************************************************
        // Game instances should inherit from this class
        //

        // ********************************************************************************************
        // Constructors

        protected RSCoreGame()
        {
            _renderer = RSRenderer.Create();
            _mouse = RSCoreMouse.Create();
            _scene = RSNodeScene.CreateScene();
            _frameTimer = RSFrameTimer.Create();
            _debugNodeList = RSNodeList.Create();
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        public RSFrameTimer FrameTimer { get { return _frameTimer; } }

        // ********************************************************************************************
        // Internal Data

        private const bool RENDER_DEBUG_INFORMATION = true;
        protected RSRenderer _renderer;
        protected RSCoreMouse _mouse;
        protected RSNodeScene _scene;
        protected RSFrameTimer _frameTimer;
        protected RSNodeList _debugNodeList;

        // ********************************************************************************************
        // Methods to implement / override

        public abstract void Initialise(Size size);

        public abstract void Resize(Size size);

        public abstract void Update(long interval);

        // ********************************************************************************************
        // Methods

        public void UpdateNodes(long interval)
        { 
            if (_scene != null) 
            {
                UpdateNodeTree(_scene, interval);
            }
        }

        public void Render(SKCanvas canvas)
        {
            RSRenderSurface surface = RSRenderSurface.Create(canvas, SKColors.Gray);

            surface.Clear();

            if (_scene != null)
            {
                int nodeCount = _renderer.RenderNodeTree(_scene, surface);

                if (_debugNodeList != null)
                {
                    _renderer.RenderNodeList(_debugNodeList, surface, true);
                }

                if (RENDER_DEBUG_INFORMATION == true)
                {
                    _renderer.RenderDebugString(nodeCount, _frameTimer.FPS, surface);
                }

            }

        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        private void UpdateNodeTree(RSNode node, long interval)
        {
            node.Update(interval);
            foreach (RSNode child in node.Children)
            { 
                UpdateNodeTree(child, interval);
            }
        }

        // ********************************************************************************************

    }
}
