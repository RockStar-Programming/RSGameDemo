
using SkiaSharp;
#if WINDOWS
using Platform._Windows;
#elif OSX
using Platform._OSX;
#endif

using Rockstar._MouseButton;
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
#if WINDOWS
    public class RSMouse : RSMouseWindows
#elif OSX
    public class RSCoreMouse : RSMouseOSX
#endif
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

        public static RSMouse Create(object gameLock)
        { 
            return new RSMouse(gameLock);
        }

        private RSMouse(object gameLock) : base()
        {
            _gameLock = gameLock;
            _leftButton = RSMouseButton.Create();
            _middleButton = RSMouseButton.Create();
            _rightButton = RSMouseButton.Create();

            LeftMouseEvent.AddHandler(OnLeftMouseButtonHandler);
            MiddleMouseEvent.AddHandler(OnMiddleMouseButtonHandler);
            RightMouseEvent.AddHandler(OnRightMouseButtonHandler);
        }

        // ********************************************************************************************
        // Properties

        // ********************************************************************************************
        // Internal Data

        private object _gameLock;
        private RSMouseButton _leftButton;
        private RSMouseButton _middleButton;
        private RSMouseButton _rightButton;

        // ********************************************************************************************
        // Methods

        public void AddHandler(RSMouseButtonType button, RSMouseEvent buttonEvent, RSEventHandler handler)
        {
            switch (button)
            {
                case RSMouseButtonType.Left:
                    _leftButton.AddHandler(buttonEvent, handler);
                    break;
                case RSMouseButtonType.Middle:
                    _middleButton.AddHandler(buttonEvent, handler);
                    break;
                case RSMouseButtonType.Right:
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
