
using Windows.Foundation;
using Windows.UI.Core;

using Rockstar._BaseRenderer;
using Rockstar._Nodes;
using Rockstar._Types;
using Rockstar._BaseMouse;

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

namespace Rockstar._BaseGame
{
    public abstract class RSBaseGame
    {
        // ********************************************************************************************
        // Implements the basics of the game handler
        // Main tasks
        // - Control rendering
        // - Update game
        // - Render game 

        // ********************************************************************************************
        // Constructors

        protected RSBaseGame(CoreWindow window, Size size)
        {
            // Create the basic renderer
            _renderer = RSBaseRenderer.CreateWithWindowAndSize(window, size);

            // Create mouse instance
            _mouse = RSBaseMouse.CreateWithWindow(window);

            // Create the main scene
            // Default (0, 0) is in upper left corner
            // By settting scene origin to LowerLeft, (0, 0) is moved to lower left corner
            // This is often much more intuitive for games
            _scene = RSNodeScene.CreateWithSize(new Size((float)size.Width, (float)size.Height), RSSceneOrigin.LowerLeft);
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        // ********************************************************************************************
        // Internal Data

        protected RSBaseMouse _mouse;
        protected RSBaseRenderer _renderer;
        protected RSNodeScene _scene;

        // ********************************************************************************************
        // Abstract Methods to Implement in Game

        public abstract void Initialise();

        public abstract void Update(long interval);

        // ********************************************************************************************
        // Methods

        public void Resize(Size size)
        {
            _renderer.Resize(size);
        }

        public void Run()
        {
            _renderer.BeginFrame();

            // game logic goes here
            Update(_renderer.FrameInterval);

            // render
            _scene.Update(_renderer.FrameInterval);
            _scene.Render(_renderer.Canvas);

            // end frame
            _renderer.EndFrame();
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }

}



