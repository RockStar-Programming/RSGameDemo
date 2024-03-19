
using Rockstar._Operations;
using Rockstar._Actions;
using System.Collections.Generic;
using System.Xml.Linq;

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
    public static class RSActionManager
    {
        // ********************************************************************************************
        // The action manager maintains a list of actions, and run any automations assigned
        // 

        // ********************************************************************************************
        // Constructors

        // ********************************************************************************************
        // Properties

        // ********************************************************************************************
        // Internal Data

        private static List<RSAction> _runningActionList = new List<RSAction>();
        private static List<RSAction> _savedActionList = new List<RSAction>();

        // ********************************************************************************************
        // Methods

        public static void Initialize()
        { 
        
        }

        public static void Update(float interval)
        {
            // iterate backwards through the list, so that completed entries can be removed on the fly
            //
            for (int index = _runningActionList.Count - 1; index >= 0; index--) 
            {
                RSAction action = _runningActionList[index];

                if (action.State == RSActionState.Running)
                {
                    if (action.IsSequence == true)
                    {
                        // execute operations sequentianlly
                        //
                        RSOperation operation = action.OperationList[action.Index];
                        operation.Update(interval);

                        if (operation.Completed == true)
                        {
                            action.StepToNextIndex();

                            // if StepToNextIndex resulted in Index == 0,
                            //   the list rolled round, and is completed
                            //
                            if (action.Index == 0)
                            {
                                action.DecrementRepeat();
                                // If repeat reached 0, list is done and removed
                                //
                                if (action.Repeat == 0)
                                {
                                    _runningActionList.RemoveAt(index);
                                }
                            }
                            else
                            {
                                // start next action
                                action.OperationList[action.Index].Start(action.Target);
                            }
                        }
                    }
                    else
                    { 
                        // execute operations simultaneously
                        //
                        bool removeAction = true;
                        
                        foreach (RSOperation operation in action.OperationList)
                        {
                            operation.Update(interval);
                            if (operation.Completed == false)
                            { 
                                removeAction = false;
                            }
                        }

                        if (removeAction == true)
                        {
                            _runningActionList.RemoveAt(index);
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

        public static void StopAction(object target, string name)
        {
            // iterate backwards through the list, so that entries can be removed on the fly
            //
            for (int index = _runningActionList.Count - 1; index >= 0; index--)
            {
                RSAction action = _runningActionList[index];

                if ((action.Target == target) && (action.Name == name))
                { 
                    action.Stop();
                    _runningActionList.RemoveAt(index);
                }
            }
        }

        public static void RunAction(object target, string name)
        {
            StopAction(target, name);

            foreach (RSAction action in _savedActionList)
            {
                if ((action.Name.Length > 0) && (action.Name == name))
                {
                    RSAction newAction = RSAction.CreateWithAction(target, action);
                    _runningActionList.Add(newAction);
                    newAction.Start();
                }
            }
        }

        public static void StopAction(RSAction action)
        {
            // iterate backwards through the list, so that entries can be removed on the fly
            //
            for (int index = _runningActionList.Count - 1; index >= 0; index--)
            {
                if (action == _runningActionList[index])
                {
                    action.Stop();
                    _runningActionList.RemoveAt(index);
                }
            }
        }

        public static void RunAction(RSAction action)
        {
            StopAction(action);

            _runningActionList.Add(action);
            action.Start();
        }

        public static void Repeat(RSAction action, int repeat)
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
