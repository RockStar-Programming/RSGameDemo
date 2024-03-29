using Rockstar._MouseButton;
using Rockstar._Event;
using Rockstar._Nodes;
using Rockstar._Physics;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Globalization;

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

namespace Rockstar._Touch
{
    public enum RSTouchEvent
    {
        None,
        Began,          
        Moved,          
        Ended,          
        Cancelled
    }

    public static class RSTouchManager
    {
        // ********************************************************************************************
        // Brief Class Description
        //
        //

        // ********************************************************************************************
        // Constructors

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        // ********************************************************************************************
        // Internal Data

        private static Dictionary<RSNode, RSEvent> _eventList = new Dictionary<RSNode, RSEvent>();
        private static List<RSNode> _activeNodes = new List<RSNode>();

        // ********************************************************************************************
        // Methods

        public static void Initialize()
        {
            _eventList.Clear();
            _activeNodes.Clear();
        }

        public static void AddEventHandler(RSNode node, RSEventHandler handler) 
        {
            if (_eventList.ContainsKey(node) == true)
            {
                // add handler to existing event
                RSEvent eventItem = _eventList[node];
                eventItem.AddHandler(handler);
            }
            else
            {
                // create new event
                _eventList.Add(node, RSEvent.CreateWithHandler(handler));
            }
        }

        // ********************************************************************************************
        // Event Handlers

        // Check if the current mouse event is inside and registered handlers
        //
        public static void MouseEvent(object sender, RSEventArgs argument)
        {
            if (argument.Data is SKPoint position)
            {
                if (argument.Type is RSMouseEvent.OnPressed)
                {
                    // touch begang
                    foreach (RSNode node in _eventList.Keys)
                    {
                        if (node.PointInside(position) == true)
                        {
                            // a touch started inside a node with an event handler

                            // if node is already active, something broke ... 
                            if (_activeNodes.Contains(node) == false)
                            {
                                _activeNodes.Add(node);
                            }
                            RSEvent eventItem = _eventList[node];
                            eventItem.ExecuteHandler(node, RSEventArgs.Create(RSTouchEvent.Began, position));
                        }
                    }
                }
                else if (argument.Type is RSMouseEvent.OnMoved)
                {
                    // touch was moved, check active nodes
                    foreach (RSNode node in _activeNodes)
                    {
                        RSEvent eventItem = _eventList[node];
                        eventItem.ExecuteHandler(node, RSEventArgs.Create(RSTouchEvent.Moved, position));
                    }
                }
                else
                {
                    // touch was liftet
                    // execute all active handlers and clear list
                    foreach (RSNode node in _activeNodes)
                    {
                        RSEvent eventItem = _eventList[node];
                        eventItem.ExecuteHandler(node, RSEventArgs.Create(RSTouchEvent.Ended, position));
                    }
                    _activeNodes.Clear();
                }
            }
        }

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }
}
