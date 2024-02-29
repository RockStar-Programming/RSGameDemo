
using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.Input;
using System.Numerics;

using Rockstar._BaseMouseButton;
using Rockstar._Event;
using System.Diagnostics;

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

namespace Rockstar._BaseMouse
{
    public enum RSMouseButton
    {
        Left,
        Middle,
        Right
    }

    public class RSBaseMouse
    {
        // ********************************************************************************************
        // RSBaseMouse supports basic mouse handling 
        // Events are generated for
        // - Mouse button pressed
        // - Mouse moved
        // _ Mouse button released

        // ********************************************************************************************
        // Constructors

        public static RSBaseMouse CreateWithWindow(CoreWindow window)
        { 
            return new RSBaseMouse(window);
        }

        private RSBaseMouse(CoreWindow window) 
        {
            // create a dictionary holding an RSUWPMouseButton for each available button 
            _buttonList = new Dictionary<RSMouseButton, RSBaseMouseButton>
            {
                { RSMouseButton.Left, RSBaseMouseButton.Create() },
                { RSMouseButton.Middle, RSBaseMouseButton.Create() },
                { RSMouseButton.Right, RSBaseMouseButton.Create() }
            };

            // enable windows mouse event handling
            window.PointerPressed += OnPointerChanged;
            window.PointerMoved += OnPointerChanged;
            window.PointerReleased += OnPointerChanged;
        }

        // ********************************************************************************************
        // Properties

        // ********************************************************************************************
        // Internal Data

        private Dictionary<RSMouseButton, RSBaseMouseButton> _buttonList;

        // ********************************************************************************************
        // Methods

        public void AddHandler(RSMouseButton button, RSMouseButtonEvent buttonEvent, RSEvent.Handler handler)
        {
            _buttonList[button].AddHandler(buttonEvent, handler);
        }
        
        // ********************************************************************************************
        // Event Handlers

        private void OnPointerChanged(CoreWindow sender, PointerEventArgs args)
        {
            PointerPoint point = args.CurrentPoint;
            Vector2 position = new Vector2((float)point.Position.X, (float)point.Position.Y);

            _buttonList[RSMouseButton.Left].UpdateState(point.Properties.IsLeftButtonPressed, position);
            _buttonList[RSMouseButton.Middle].UpdateState(point.Properties.IsMiddleButtonPressed, position);
            _buttonList[RSMouseButton.Right].UpdateState(point.Properties.IsRightButtonPressed, position);
        }

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }
}
