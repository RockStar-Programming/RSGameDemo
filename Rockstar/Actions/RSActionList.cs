using Rockstar._Action;
using Rockstar._Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

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

namespace Rockstar._ActionList
{
    public enum RSActionListState
    {
        Stopped,
        Running,
        Paused
    }

    public class RSActionList
    {
        // ********************************************************************************************
        // An action list contains the node reference, and all actions associated with that node
        // 
        // Each entry in _actionList holds a List<RSAction>
        // These lists of actions are executed sequentially
        // All individual entries in the _actionList are executed simultaneously
        // 
        // _actionList[0]:action1               executed simultaneously with next lines
        // _actionList[1]:action2,action3       executes in isSequence 
        // _actionList[2]:action4               
        //
        // 

        // ********************************************************************************************
        // Constructors

        public static RSActionList Create(RSNode node, RSAction? action = null)
        {
            return new RSActionList(node, null, action, null, false, REPEAT_ONCE);
        }

        public static RSActionList CreateSequence(RSNode node)
        {
            return new RSActionList(node, null, null, null, true, REPEAT_ONCE);
        }

        public static RSActionList CreateWithList(RSActionList list, string name, int repeat = REPEAT_ONCE)
        {
            return new RSActionList(list.Node, name, null, list.ActionList, list.IsSequence, repeat);
        }

        // ********************************************************************************************

        private RSActionList(RSNode node, string? name, RSAction? action, List<RSAction>? list, bool isSequence, int repeat) 
        {
            _node = node;
            _name = name;
            _index = 0;
            _isSequence = isSequence;
            _state = RSActionListState.Stopped;
            _repeat = repeat;

            _actionList = new List<RSAction>();
            if (list != null)
            { 
                foreach (RSAction storedAction in list)
                {
                    _actionList.Add(storedAction);
                }
            }
            if (action != null)
            {
                _actionList.Add(action);
            }
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        public const int REPEAT_ONCE = 1;
        public RSActionListState State { get { return _state; } }
        public RSNode Node { get { return _node; } }
        public string? Name { get { return _name; } }
        public bool IsSequence { get { return _isSequence; } }
        public int Index {  get { return _index; } }
        public int Repeat { get { return _repeat; } }
        public List<RSAction> ActionList { get { return _actionList; } }

        // ********************************************************************************************
        // Internal Data

        private RSActionListState _state;
        private RSNode _node;
        private string? _name;
        private bool _isSequence;
        private int _index;
        private int _repeat;
        private List<RSAction> _actionList;

        // ********************************************************************************************
        // Methods

        public RSActionList AddAction(RSAction action) 
        {
            _actionList.Add(action);
            return this;
        }

        public void StepToNextIndex()
        {
            _index = (_index + 1) % _actionList.Count;
        }

        public void DecrementRepeat()
        {
            if (_repeat > 0)
            {
                _repeat--;
            }
        }

        public void Start()
        {
            _index = 0;
            _actionList[_index].Start();
            _state = RSActionListState.Running;
        }

        public void Stop()
        {
            _index = 0;
            _state = RSActionListState.Stopped;
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }
}
