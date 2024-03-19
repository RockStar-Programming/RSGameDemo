
using Rockstar._ActionTypes;
using Rockstar._Actions;

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

        private static List<RSAction> _runningActionList = new List<RSAction>();
        private static List<RSAction> _savedActionList = new List<RSAction>();

        // ********************************************************************************************
        // Methods

        public static void Update(float interval)
        {
            // iterate backwards through the list, so that completed entries can be removed on the fly
            //
            for (int index = _runningActionList.Count - 1; index >= 0; index--) 
            {
                RSAction list = _runningActionList[index];

                if (list.State == RSActionState.Running)
                {
                    RSActionBase action = list.ActionList[list.Index];
                    action.Update(interval);

                    if (action.Completed == true)
                    {
                        list.StepToNextIndex();

                        // if StepToNextIndex resulted in Index == 0,
                        //   the list rolled round, and is completed
                        //
                        if (list.Index == 0)
                        {
                            list.DecrementRepeat();
                            // If repeat reached 0, list is done and removed
                            //
                            if (list.Repeat == 0)
                            {
                                _runningActionList.RemoveAt(index);
                            }
                        }
                        else
                        {
                            // start next action
                            list.ActionList[list.Index].Start(list.Target);
                        }
                    }
                }
            }
        }

        public static void Save(RSAction list, string name)
        {
            list.SetName(name);
            _savedActionList.Add(list);
        }

        public static void RunAction(object target, string name)
        {
            foreach (RSAction action in _savedActionList)
            {
                if ((action.Name != null) && (action.Name == name))
                {
                    RSAction newAction = RSAction.CreateWithAction(target, action);
                    _runningActionList.Add(newAction);
                    newAction.Start();
                }
            }
        }

        public static void RunAction(RSAction list)
        {
            ;
        }

        public static void Repeat(RSAction list, int repeat)
        {
            ;
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }
}
