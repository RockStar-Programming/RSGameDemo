
using Rockstar._ActionTypes;

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

namespace Rockstar._Actions
{
    public enum RSActionState
    {
        Stopped,
        Running,
        Paused
    }

    public class RSAction
    {
        // ********************************************************************************************
        // An action can consist of one or more individual action types
        // 

        // ********************************************************************************************
        // Constructors

        // ActionList constructors

        public static RSAction Create()
        {
            return new RSAction(new object(), null, false, REPEAT_ONCE);
        }

        public static RSAction CreateSequence()
        {
            return new RSAction(new object(), null, true, REPEAT_ONCE);
        }

        public static RSAction Create(object target, RSActionBase? action = null)
        {
            return new RSAction(target, action, false, REPEAT_ONCE);
        }

        public static RSAction CreateSequence(object target, RSActionBase? action = null)
        {
            return new RSAction(target, action, true, REPEAT_ONCE);
        }

        public static RSAction CreateWithAction(object target, RSAction action)
        {
            return new RSAction(target, action);
        }

        // ********************************************************************************************

        private RSAction(object target, RSActionBase? action, bool isSequence, int repeat) 
        {
            _target = target;
            _name = "";
            _index = 0;
            _isSequence = isSequence;
            _state = RSActionState.Stopped;
            _repeat = repeat;
            _actionList = new List<RSActionBase>();
            if (action != null)
            {
                _actionList.Add(action);
            }
        }

        private RSAction(object target, RSAction action)
        {
            _target = target;
            _name = action.Name;
            _index = 0;
            _isSequence = action.IsSequence;
            _state = RSActionState.Stopped;
            _repeat = action.Repeat;

            _actionList = new List<RSActionBase>();
            foreach (RSActionBase storedAction in action.ActionList)
            {
                _actionList.Add(storedAction);
            }
        }


        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        public const float INSTANT = 0.0f;

        public const int REPEAT_ONCE = 1;
        public RSActionState State { get { return _state; } }
        public object Target { get { return _target; } }
        public string? Name { get { return _name; } }
        public bool IsSequence { get { return _isSequence; } }
        public int Index {  get { return _index; } }
        public int Repeat { get { return _repeat; } }
        public List<RSActionBase> ActionList { get { return _actionList; } }

        // ********************************************************************************************
        // Internal Data

        private RSActionState _state;
        private object _target;
        private string? _name;
        private bool _isSequence;
        private int _index;
        private int _repeat;
        private List<RSActionBase> _actionList;

        // ********************************************************************************************
        // Methods

        public void SetName(string name)
        {
            _name = name;
        }

        public RSAction AddAction(RSActionBase action) 
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
            _actionList[_index].Start(_target);
            _state = RSActionState.Running;
        }

        public void Stop()
        {
            _index = 0;
            _state = RSActionState.Stopped;
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }
}
