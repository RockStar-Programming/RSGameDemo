
using SkiaSharp;
using Platform._Windows;

using Rockstar._CoreMouseButton;
using Rockstar._Event;

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

namespace Rockstar._CoreMouse
{
    public class RSCoreMouse : RSWinMouse
    {
        // ********************************************************************************************
        // RSCoreMouse supports basic mouse handling 
        // Class should inherit from platform specific base class
        // Events are generated for
        // - Mouse button pressed
        // - Mouse moved
        // _ Mouse button released

        // ********************************************************************************************
        // Constructors

        public static RSCoreMouse Create(object gameLock)
        { 
            return new RSCoreMouse(gameLock);
        }

        private RSCoreMouse(object gameLock) : base()
        {
            _gameLock = gameLock;
            _leftButton = RSCoreMouseButton.Create();
            _middleButton = RSCoreMouseButton.Create();
            _rightButton = RSCoreMouseButton.Create();

            LeftMouseEvent.AddHandler(OnLeftMouseButtonHandler);
            MiddleMouseEvent.AddHandler(OnMiddleMouseButtonHandler);
            RightMouseEvent.AddHandler(OnRightMouseButtonHandler);
        }

        // ********************************************************************************************
        // Properties

        // ********************************************************************************************
        // Internal Data

        private object _gameLock;
        private RSCoreMouseButton _leftButton;
        private RSCoreMouseButton _middleButton;
        private RSCoreMouseButton _rightButton;

        // ********************************************************************************************
        // Methods

        public void AddHandler(RSMouseButton button, RSMouseEvent buttonEvent, RSEvent.Handler handler)
        {
            switch (button)
            {
                case RSMouseButton.Left:
                    _leftButton.AddHandler(buttonEvent, handler);
                    break;
                case RSMouseButton.Middle:
                    _middleButton.AddHandler(buttonEvent, handler);
                    break;
                case RSMouseButton.Right:
                    _rightButton.AddHandler(buttonEvent, handler);
                    break;
                default:
                    break;
            }
        }

        // ********************************************************************************************
        // Event Handlers

        private void OnLeftMouseButtonHandler(object sender, RSEventArgs argument)
        {
            lock (_gameLock)
            {
                if (argument.Data is SKPoint position)
                {
                    bool buttonPressed = (argument.Type is RSMouseEvent.OnReleased) ? false : true;
                    _leftButton.UpdateState(buttonPressed, position);
                }
            }
        }

        private void OnMiddleMouseButtonHandler(object sender, RSEventArgs argument)
        {
            lock (_gameLock)
            {
                if (argument.Data is SKPoint position)
                {
                    bool buttonPressed = (argument.Type is RSMouseEvent.OnReleased) ? false : true;
                    _middleButton.UpdateState(buttonPressed, position);
                }
            }
        }

        private void OnRightMouseButtonHandler(object sender, RSEventArgs argument)
        {
            lock (_gameLock)
            {
                if (argument.Data is SKPoint position)
                {
                    bool buttonPressed = (argument.Type is RSMouseEvent.OnReleased) ? false : true;
                    _rightButton.UpdateState(buttonPressed, position);
                }
            }
        }

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }
}
