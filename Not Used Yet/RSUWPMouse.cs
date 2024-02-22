using Rockstar.Types;
using Rockstar.UWPMouseButton;
using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.Input;

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

namespace Rockstar.UWPMouse
{
    public enum RSMouseButton
    {
        Left,
        Middle,
        Right
    }

    public class RSUWPMouse
    {
        // ********************************************************************************************
        // RSUWPMouse supports basic mouse handling 

        // ********************************************************************************************
        // Constructors

        public static RSUWPMouse CreateWithWindow(CoreWindow window)
        { 
            return new RSUWPMouse(window);
        }

        private RSUWPMouse(CoreWindow window) 
        {
            // create a dictionary holding an RSUWPMouseButton for each available button 
            _buttonList = new Dictionary<RSMouseButton, RSUWPMouseButton>
            {
                { RSMouseButton.Left, RSUWPMouseButton.Create() },
                { RSMouseButton.Middle, RSUWPMouseButton.Create() },
                { RSMouseButton.Right, RSUWPMouseButton.Create() }
            };

            window.PointerPressed += OnPointerChanged;
            window.PointerMoved += OnPointerChanged;
            window.PointerReleased += OnPointerChanged;
        }

        // ********************************************************************************************
        // Properties

        // ********************************************************************************************
        // Internal Data

        private Dictionary<RSMouseButton, RSUWPMouseButton> _buttonList;

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
            RSVector2 position = new RSVector2(point.Position.X, point.Position.Y);

            _buttonList[RSMouseButton.Left].UpdateState(point.Properties.IsLeftButtonPressed, position);
            _buttonList[RSMouseButton.Middle].UpdateState(point.Properties.IsMiddleButtonPressed, position);
            _buttonList[RSMouseButton.Right].UpdateState(point.Properties.IsRightButtonPressed, position);
        }

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }
}
