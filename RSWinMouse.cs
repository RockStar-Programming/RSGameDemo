﻿
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

using Rockstar._CoreMouseButton;
using Rockstar._Event;
using System.Numerics;

namespace Rockstar._WinMouse
{
    public class RSWinMouse : IMessageFilter
    {
        // ********************************************************************************************
        // RSBaseMouse supports basic mouse handling 
        // Events are generated for
        // - Mouse button pressed
        // - Mouse moved
        // _ Mouse button released

        // ********************************************************************************************
        // Constructors

        public RSWinMouse() 
        {
            Application.AddMessageFilter(this);

            _leftMouseEvent = RSEvent.Create();
            _middleMouseEvent = RSEvent.Create();
            _rightMouseEvent = RSEvent.Create();
        }

        // ********************************************************************************************
        // Properties

        protected RSEvent LeftMouseEvent { get { return _leftMouseEvent; } }
        protected RSEvent MiddleMouseEvent { get { return _middleMouseEvent; } }
        protected RSEvent RightMouseEvent { get { return _rightMouseEvent; } }

        // ********************************************************************************************
        // Internal Data

        private const int WM_MOUSEMOVE = 0x0200;

        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;

        private const int WM_MBUTTONDOWN = 0x0207;
        private const int WM_MBUTTONUP = 0x0208;

        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_RBUTTONUP = 0x0205;

        private RSEvent _leftMouseEvent;
        private RSEvent _middleMouseEvent;
        private RSEvent _rightMouseEvent;

        private bool _leftButtonState = false;
        private bool _middleButtonState = false;
        private bool _rightButtonState = false;

        // ********************************************************************************************
        // Methods

        public bool PreFilterMessage(ref Message m)
        {
            Vector2 position;

            switch (m.Msg)
            {
                case WM_LBUTTONDOWN:
                    // Handle left button down globally
                    position = new Vector2(
                                    x: m.LParam.ToInt32() & 0xFFFF, // Low-order word
                                    y: m.LParam.ToInt32() >> 16 // High-order word
                                );
                    _leftButtonState = true;
                    _leftMouseEvent.ExecuteHandler(this, RSEventArgs.Create(RSMouseEvent.OnPressed, position));
                    break;
                case WM_LBUTTONUP:
                    // Handle left button down globally
                    position = new Vector2(
                                    x: m.LParam.ToInt32() & 0xFFFF, // Low-order word
                                    y: m.LParam.ToInt32() >> 16 // High-order word
                                );