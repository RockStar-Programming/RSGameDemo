
using SkiaSharp;

using Rockstar._FrameTimer;
using Rockstar._Renderer;
using Rockstar._CoreMouse;
using Rockstar._Physics;
using Rockstar._ActionManager;
using Rockstar._Nodes;

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
            _gameLock = new object();
            _renderer = RSRenderer.Create();
            _mouse = RSCoreMouse.Create(_gameLock);
            _scene = RSNodeScene.CreateScene();
            _frameTimer = RSFrameTimer.Create();
            RSActionManager.Initialize();

            // Adding a bit of X gravity, prevents perfect stacking of objects
            //
            _physics = RSPhysics.CreateWithScene(new SKPoint(0.001f, -9.8f), 0.01f);
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        public RSFrameTimer FrameTimer { get { return _frameTimer; } }

        // ********************************************************************************************
        // Internal Data

        protected RSRenderer _renderer;
        protected RSCoreMouse _mouse;
        protected RSNodeScene _scene;
        protected RSFrameTimer _frameTimer;
        protected RSPhysics _physics;
        protected object _gameLock;

        // ********************************************************************************************
        // Methods to implement / override

        public abstract void Initialise(SKSize size);

        public abstract void Resize(SKSize size);

        public abstract void Update(float interval);

        // ********************************************************************************************
        // Methods

        public void UpdateNodes(float interval)
        {
            lock (_gameLock)
            {
                RSActionManager.Update(interval);

                if (_physics != null)
                {
                    _physics.Update(_scene, interval);
                }

                if (_scene != null)
                {
                    UpdateNodeTree(_scene, interval);
                }
            }
        }

        public void Render(SKCanvas canvas)
        {
            lock (_gameLock)
            {
                if (_scene != null)
                {
                    _renderer.RenderScene(_scene, canvas, (float)_frameTimer.FPS);
                }
            }
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        private void UpdateNodeTree(RSNode node, float interval)
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
