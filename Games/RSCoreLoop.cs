
using SkiaSharp;

using Rockstar._Game;
using Rockstar._GameClockDDP;
using Rockstar._GameClock;
using Rockstar._GameSpeedMaster;
using RSGameDemo._GameSnowGlobe;

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

namespace Rockstar._CoreLoop
{
    public class RSCoreLoop
    {
        // ********************************************************************************************
        // Main game loop
        //
        // Create and initialse game instance here

        // ********************************************************************************************
        // Constructors

        // RSGamePhysics _game;
        // RSGameClock _game;
        // RSGameClockDDP _game;
        // RSGameSpeedMaster _game;
        RSGameSnowGlobe _game;

        public RSCoreLoop()
        {
            _game = RSGameSnowGlobe.Create();
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        // ********************************************************************************************
        // Internal Data

        // ********************************************************************************************
        // Methods

        public void Initialize(SKSize size)
        {
            _game.Initialize(size);
        }

        public void Resize(SKSize size)
        {
            _game.Resize(size);
        }

        public void Update()
        {
            _game.FrameTimer.BeginFrame();

            _game.UpdateNodes(_game.FrameTimer.Interval);
            _game.Update(_game.FrameTimer.Interval);
        }

        public void Render(SKCanvas canvas)
        {
            _game.Render(canvas);
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }
}
