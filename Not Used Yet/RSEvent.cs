using Rockstar.EventArgs;
using System.Collections.Generic;

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

namespace Rockstar.Event
{
	public class RSEvent
	{
        // ********************************************************************************************
        // RSEvent replaces the default event in C#. The reason for that is:
        // - Better control of which handlers have been added
        // - Automatic removal of invalid handlers
        // - No duplicate handlers
        // - 100% crash safe

        // ********************************************************************************************
        // Constructors

        public static RSEvent Create()
        { 
            return new RSEvent(); 
        }

        public static RSEvent CreateWithHandler(Handler handler)
        {
            RSEvent result = RSEvent.Create();
            result.AddHandler(handler);
            return result;
        }

        private RSEvent()
        {
            _handlerList = new List<Handler>();
        }

        // ********************************************************************************************
        // Properties

        public delegate void Handler(object sender, RSEventArgs argument);

        public bool Empty { get { return _handlerList.Count == 0; } }

        // ********************************************************************************************
        // Internal Data

        private List<Handler> _handlerList;

        // ********************************************************************************************
        // Methods

        public void AddHandler(Handler handler)
        {
            if (_handlerList.Contains(handler) == false)
            {
                _handlerList.Add(handler);
            }
        }

        public void RemoveHandler(Handler handler)
        {
            if (_handlerList.Contains(handler) == true)
            {
                _handlerList.Remove(handler);
            }
        }

        public void ClearHandlerList()
        {
            _handlerList.Clear();
        }

        public bool ExecuteHandler(object sender, RSEventArgs argument)
        {
            bool handlerExecuted = false;

            if (Empty == false)
            {
                // iterate backwards to be able to remove bad handlers on the fly
                for (int index = _handlerList.Count - 1; index >= 0; index--)
                {
                    Handler handler = _handlerList[index];

                    try
                    {
                        // handler is executed on the same thread
                        handler(sender, argument);
                        handlerExecuted = true;
                    }
                    catch
                    {
                        _handlerList.RemoveAt(index);
                    }
                }
            }

            return handlerExecuted;
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
	}
}

