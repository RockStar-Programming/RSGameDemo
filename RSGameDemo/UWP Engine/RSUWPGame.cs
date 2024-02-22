using Windows.UI.Core;
using System.Diagnostics;
using Windows.Foundation;
using System.Reflection;
using System;
using Windows.Foundation.Collections;
using Rockstar.UWPRenderer;
using Rockstar.Types;
using Rockstar.Transformation;
using Rockstar.GameClock;
using Rockstar.Nodes;

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

namespace Rockstar.UWPGame
{
    public class RSUWPGame
    {
        // ********************************************************************************************
        // Brief Class Description
        //
        //

        // ********************************************************************************************
        // Constructors

        public static RSUWPGame CreateWithWindowAndSize(CoreWindow window, Size size) 
        {
            return new RSUWPGame(window, size);
        }

        private RSUWPGame(CoreWindow window, Size size) 
        {
            // Create the basic renderer
            _renderer = RSUWPRenderer.CreateWithWindowAndSize(window, size);

            // Create the main scene
            // Default (0, 0) is in upper left corner
            // By settting scene origin to LowerLeft, (0, 0) is moved to lower left corner
            // This is often much more intuitive for games
            _mainScene = RSNodeScene.CreateWithSize(new RSSize((float)size.Width, (float)size.Height), RSSceneOrigin.LowerLeft);

            // Create and initialise the instance of the game running
            _clock = RSGameClock.CreateWithScene(_mainScene);
            _clock.Initialise();
        }

        // ********************************************************************************************
        // Properties

        // ********************************************************************************************
        // Internal Data

        private RSUWPRenderer _renderer;
        private RSNodeScene _mainScene;

        private RSGameClock _clock;
        
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
            _clock.Update(_renderer.FrameInterval);

            // render
            _renderer.RenderScene(_mainScene);

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



