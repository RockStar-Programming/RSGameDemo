
using Rockstar._Action;
using Rockstar._ActionList;
using Rockstar._Nodes;
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

        private static List<RSActionList> _actionList = new List<RSActionList>();

        // ********************************************************************************************
        // Methods

        public static void Update(float interval)
        {
            // iterate backwards through the list, so that completed entries can be removed on the fly
            //
            for (int index = _actionList.Count - 1; index >= 0; index--) 
            {
                RSActionList list = _actionList[index];

                if (list.State == RSActionListState.Running)
                {
                    RSAction action = list.ActionList[list.Index];
                    action.Update(interval);

                    if (action.Completed == true)
                    {
                        list.StepToNextIndex();

                        // if StepToNextIndex resulted in Index == 0,
                        //   the list rolled round, and is completed
                        //
                        if (list.Index == 0)
                        {
                            if (list.Name != null)
                            {
                                // if its a named list, just stop it
                                list.Stop();
                            }
                            else 
                            {
                                list.DecrementRepeat();
                                // If repeat reached 0, list is done and removed
                                //
                                if (list.Repeat == 0)
                                {
                                    _actionList.RemoveAt(index);
                                }
                            }
                        }
                        else
                        {
                            // start next action
                            list.ActionList[list.Index].Start();
                        }
                    }
                }
            }
        }

        public static void Save(RSActionList list, string name)
        {
            RSActionList newList = RSActionList.CreateWithList(list, name);
            _actionList.Add(newList);
        }

        public static void RunAction(RSNode node, string name)
        {
            foreach (RSActionList list in _actionList)
            {
                if ((list.Name != null) && (list.Name == name))
                {
                    list.Start();
                }
            }
        }

        public static void Run(RSActionList list)
        {
            ;
        }

        public static void Repeat(RSActionList list, int repeat)
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
