
using SkiaSharp;

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

namespace Rockstar._CoreMouseButton
{
    public enum RSMouseButton
    {
        Left,
        Middle,
        Right
    }

    public enum RSMouseEvent
    {
        OnPressed,
        OnMoved,
        OnReleased,
        OnAll            
    }

    public class RSCoreMouseButton
    {
        // ********************************************************************************************
        // RSUWPMouseButton handles event data for a single mouse button
        // 
        // Separate handlers can be set for button events OnPressed, OnMoved and OnReleased
        // If ButtonEvent.OnAll is used, same event handler can be set for all button events

        // ********************************************************************************************
        // Constructors

        public static RSCoreMouseButton Create()
        {
            return new RSCoreMouseButton();
        }

        private RSCoreMouseButton()
        {
            // create a dictionary holding an RSEvent for each available button event
            _eventList = new Dictionary<RSMouseEvent, RSEvent>
            {
                { RSMouseEvent.OnPressed, RSEvent.Create() },
                { RSMouseEvent.OnMoved, RSEvent.Create() },
                { RSMouseEvent.OnReleased, RSEvent.Create() }
            };

            _lastPosition = new SKPoint();
        }

        // ********************************************************************************************
        // Properties

        public bool Pressed { get { return _lastPressed; } }

        // ********************************************************************************************
        // Internal Data

        private Dictionary<RSMouseEvent, RSEvent> _eventList;
        private SKPoint _lastPosition;
        private bool _lastPressed;
        // movement threshold is added to reduce the number of times the movement handler is called
        // this is done to prevent repeated events for very small mouse movements
        private const float MOVEMENT_THRESHOLD = 2.0f;

        // ********************************************************************************************
        // Methods

        public void AddHandler(RSMouseEvent buttonEvent, RSEventHandler handler)
        {
            switch (buttonEvent)
            {
                case RSMouseEvent.OnPressed:
                case RSMouseEvent.OnMoved:
                case RSMouseEvent.OnReleased:
                    _eventList[buttonEvent].AddHandler(handler);
                    break;

                case RSMouseEvent.OnAll:
                    _eventList[RSMouseEvent.OnPressed].AddHandler(handler);
                    _eventList[RSMouseEvent.OnMoved].AddHandler(handler);
                    _eventList[RSMouseEvent.OnReleased].AddHandler(handler);
                    break;
            }
        }

        public void UpdateState(bool pressed, SKPoint position)
        {
            if (ButtonIsSteadyPassive(pressed) == true)
            {

            }
            if (ButtonHasBeenPressed(pressed) == true)
            {
                _lastPressed = true;
                _lastPosition = position;
                _eventList[RSMouseEvent.OnPressed].ExecuteHandler(this, RSEventArgs.Create(RSMouseEvent.OnPressed, position));
            }
            else if (ButtonIsSteadyActive(pressed) == true)
            {
                // do not execute a movement handler if movement is below movement threshold
                if (SKPoint.Distance(_lastPosition, position) >= MOVEMENT_THRESHOLD)
                {
                    _lastPosition = position;
                    _eventList[RSMouseEvent.OnMoved].ExecuteHandler(this, RSEventArgs.Create(RSMouseEvent.OnMoved, position));
                }
            }
            else if (ButtonHasBeenReleased(pressed) == true)
            {
                _lastPressed = false;
                _lastPosition = position;
                _eventList[RSMouseEvent.OnReleased].ExecuteHandler(this, RSEventArgs.Create(RSMouseEvent.OnReleased, position));
            }
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // These methods are added to improve readability in UpdateState logic

        private bool ButtonIsSteadyPassive(bool pressed)
        {
            return _lastPressed == false && pressed == false;
        }

        private bool ButtonHasBeenPressed(bool pressed)
        {
            return _lastPressed == false && pressed == true;
        }

        private bool ButtonIsSteadyActive(bool pressed)
        {
            return _lastPressed == true && pressed == true;
        }

        private bool ButtonHasBeenReleased(bool pressed)
        {
            return _lastPressed == true && pressed == false;
        }

        // ********************************************************************************************
    }
}
