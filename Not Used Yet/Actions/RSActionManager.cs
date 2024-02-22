﻿using System.Collections.Generic;

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

namespace Rockstar.ActionManager
{
    public class RSActionManager
    {
        // ********************************************************************************************
        // The automation manager maintains a list of nodes, and run any automations assigned
        // TBD: Automations are executed in render order

        // ********************************************************************************************
        // Constructors

        public static RSActionManager Create()
        {
            return new RSActionManager();
        }

        private RSActionManager() 
        {
            _eventList = new Dictionary<RSNode, List<RSAction>>();
        }

        // ********************************************************************************************
        // Properties

        // ********************************************************************************************
        // Internal Data

        private Dictionary<RSNode, List<RSAction>> _eventList;

        // ********************************************************************************************
        // Methods

        public void AddAction(RSNode node, RSAction nodeEvent)
        { 

        }

        public void RemoveAction(RSNode node, RSAction nodeEvent) 
        { 

        }

        public void RemoveAllActions(RSNode node) 
        {
            
        }

        public void RemoveAllActions() 
        {
            
        }

        public void Update(long interval)
        {

        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }
}
