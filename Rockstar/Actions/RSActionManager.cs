
using Rockstar._Action;
using Rockstar._Nodes;

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

namespace Rockstar._ActionManager
{
    public class RSActionManager
    {
        // ********************************************************************************************
        // The action manager maintains a list of actions, and run any automations assigned
        // TBD: Automations are executed in render order

        // ********************************************************************************************
        // Constructors

        public static void Create()
        {

        }

        // ********************************************************************************************
        // Properties

        // ********************************************************************************************
        // Internal Data

        private static Dictionary<RSNode, List<RSAction>> _actionList = new Dictionary<RSNode, List<RSAction>>();

        // ********************************************************************************************
        // Methods

        public static void Update(float interval)
        {
            foreach (List<RSAction> list in _actionList.Values) 
            { 
                foreach(RSAction action in list)
                {
                    action.Update(interval);
                }
            }
        }

        public static void Add(RSNode node, RSAction action)
        {
            if (_actionList.ContainsKey(node) == false)
            {
                _actionList.Add(node, new List<RSAction> { action });
            }
            else
            {
                List<RSAction> list = _actionList[node];
                list.Add(action);
            }
            action.Start();
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }
}
