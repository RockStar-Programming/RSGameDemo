﻿
using SkiaSharp;

using Rockstar._Lerp;
using Rockstar._Nodes;
using Rockstar._SpriteFrame;
using Rockstar._ActionList;
using System.Collections.Generic;
using Rockstar._ActionManager;

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

namespace Rockstar._Action
{
    public static class RSActionInterface
    {
        // ********************************************************************************************
        // This defines interfaces for the RSActionProperty class
        //
        // Ex: creates a MoveTo followed by a ScaleTo, and saves it (does not initially run)
        // "sequence" will be active until the actions are run, saved or repeated
        // node.Sequence().MoveTo(pos1).ScaleTo(scale1).SaveAs("test");
        // node.RunAction("test");
        //
        // Ex: simultaneously moves and scales the node. Runs instantly, and deletes action after completion
        // node.MoveTo(pos1).ScaleTo(scale1).Run();
        //
        // Ex: 
        // 

        // ********************************************************************************************
        // Action Extensions

        public static RSActionList MoveTo(this RSNode node, SKPoint position, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSAction action = new RSAction().InitAction(node, RSTransformation.POSITION, position, RSActionType.Absolute, duration, type);

            RSActionList list = node.ActionList().AddAction(action);
            _pendingActionList[node] = list;
            return list;
        }

        public static RSActionList MoveTo(this RSActionList list, SKPoint position, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSNode node = list.Node;
            RSAction action = new RSAction().InitAction(node, RSTransformation.POSITION, position, RSActionType.Absolute, duration, type);
            list.ActionList.Add(action);

            _pendingActionList[node] = list;
            return list;
        }

        public static RSActionList MoveBy(this RSNode node, SKPoint movement, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSAction action = new RSAction().InitAction(node, RSTransformation.POSITION, movement, RSActionType.Relative, duration, type);

            RSActionList list = node.ActionList().AddAction(action);
            _pendingActionList[node] = list;
            return list;
        }

        public static RSActionList MoveBy(this RSActionList list, SKPoint movement, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSNode node = list.Node;
            RSAction action = new RSAction().InitAction(node, RSTransformation.POSITION, movement, RSActionType.Relative, duration, type);
            list.ActionList.Add(action);

            _pendingActionList[node] = list;
            return list;
        }

        public static RSActionList ScaleTo(this RSNode node, SKPoint scale, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSAction action = new RSAction().InitAction(node, RSTransformation.SCALE, scale, RSActionType.Absolute, duration, type);

            RSActionList list = node.ActionList().AddAction(action);
            _pendingActionList[node] = list;
            return list;
        }

        public static RSActionList ScaleTo(this RSActionList list, SKPoint scale, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSNode node = list.Node;
            RSAction action = new RSAction().InitAction(node, RSTransformation.SCALE, scale, RSActionType.Absolute, duration, type);
            list.ActionList.Add(action);

            _pendingActionList[node] = list;
            return list;
        }

        public static RSActionList RotateTo(this RSNode node, float rotation, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSAction action = new RSAction().InitAction(node, RSTransformation.ROTATION, rotation, RSActionType.Absolute, duration, type);

            RSActionList list = node.ActionList().AddAction(action);
            _pendingActionList[node] = list;
            return list;
        }

        public static RSActionList RotateTo(this RSActionList list, float rotation, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSNode node = list.Node;
            RSAction action = new RSAction().InitAction(node, RSTransformation.ROTATION, rotation, RSActionType.Absolute, duration, type);
            list.ActionList.Add(action);

            _pendingActionList[node] = list;
            return list;
        }

        public static RSActionList RotateBy(this RSNode node, float angle, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSAction action = new RSAction().InitAction(node, RSTransformation.ROTATION, angle, RSActionType.Relative, duration, type);

            RSActionList list = node.ActionList().AddAction(action);
            _pendingActionList[node] = list;
            return list;
        }

        public static RSActionList RotateBy(this RSActionList list, float angle, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSNode node = list.Node;
            RSAction action = new RSAction().InitAction(node, RSTransformation.ROTATION, angle, RSActionType.Relative, duration, type);
            list.ActionList.Add(action);

            _pendingActionList[node] = list;
            return list;
        }

        public static RSActionList SizeTo(this RSNode node, SKSize size, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSAction action = new RSAction().InitAction(node, RSTransformation.SIZE, size, RSActionType.Absolute, duration, type);

            RSActionList list = node.ActionList().AddAction(action);
            _pendingActionList[node] = list;
            return list;
        }

        public static RSActionList SizeTo(this RSActionList list, SKSize size, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSNode node = list.Node;
            RSAction action = new RSAction().InitAction(node, RSTransformation.SIZE, size, RSActionType.Absolute, duration, type);
            list.ActionList.Add(action);

            _pendingActionList[node] = list;
            return list;
        }

        public static RSActionList AltitudeTo(this RSNode node, float altitude, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSAction action = new RSAction().InitAction(node, RSTransformation.ALTITUDE, altitude, RSActionType.Absolute, duration, type);

            RSActionList list = node.ActionList().AddAction(action);
            _pendingActionList[node] = list;
            return list;
        }

        public static RSActionList AltitudeTo(this RSActionList list, float altitude, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSNode node = list.Node;
            RSAction action = new RSAction().InitAction(node, RSTransformation.ALTITUDE, altitude, RSActionType.Absolute, duration, type);
            list.ActionList.Add(action);

            _pendingActionList[node] = list;
            return list;
        }

        public static RSActionList ColorTo(this RSNode node, SKColor color, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSAction action = new RSAction().InitAction(node, RSTransformation.COLOR, color, RSActionType.Absolute, duration, type);

            RSActionList list = node.ActionList().AddAction(action);
            _pendingActionList[node] = list;
            return list;
        }

        public static RSActionList ColorTo(this RSActionList list, SKColor color, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSNode node = list.Node;
            RSAction action = new RSAction().InitAction(node, RSTransformation.COLOR, color, RSActionType.Absolute, duration, type);
            list.ActionList.Add(action);

            _pendingActionList[node] = list;
            return list;
        }

        public static RSActionList AlphaTo(this RSNode node, float alpha, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSAction action = new RSAction().InitAction(node, RSTransformation.ALPHA, alpha, RSActionType.Absolute, duration, type);

            RSActionList list = node.ActionList().AddAction(action);
            _pendingActionList[node] = list;
            return list;
        }

        public static RSActionList AlphaTo(this RSActionList list, float alpha, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSNode node = list.Node;
            RSAction action = new RSAction().InitAction(node, RSTransformation.ALPHA, alpha, RSActionType.Absolute, duration, type);
            list.ActionList.Add(action);

            _pendingActionList[node] = list;
            return list;
        }

        // ********************************************************************************************
        // Node Extensions

        // runs all stored actions under the name
        public static void RunAction(this RSNode node, string name)
        {
            RSActionManager.RunAction(node, name);
        }

        // starts a sequence of actions
        public static RSActionList Sequence(this RSNode node) 
        {
            return RSActionList.CreateSequence(node);
        }

        // ********************************************************************************************
        // Final Action Extensions

        // Saves the pending batch of actions
        //   does not execute it
        public static void SaveAs(this RSActionList actionList, string name)
        {
            RSActionManager.Save(actionList, name);
            _pendingActionList.Remove(actionList.Node);
        }

        // saves the pending batch of actions
        //   runs is once and removes it
        public static void Run(this RSActionList actionList)
        {
            RSActionManager.Run(actionList);
            _pendingActionList.Remove(actionList.Node);
        }

        // saves the pending batch of actions
        //   runs is requested number of times (default forever)
        public static void Repeat(this RSActionList actionList, int repeat = 0)
        {
            RSActionManager.Repeat(actionList, repeat);
            _pendingActionList.Remove(actionList.Node);
        }

        // ********************************************************************************************
        // Properties

        // ********************************************************************************************
        // Internal Data

        private static Dictionary<RSNode, RSActionList> _pendingActionList = new Dictionary<RSNode, RSActionList>();

        // ********************************************************************************************
        // Methods

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // retrieves the action list for the node,
        //   otherwise returns a fresh one
        private static RSActionList ActionList(this RSNode node)
        {
            if (_pendingActionList.ContainsKey(node) == true)
            {
                return _pendingActionList[node];
            }

            // no actions were found on that node, create and return new one
            RSActionList newList = RSActionList.Create(node);
            _pendingActionList.Add(node, newList);
            return newList;
        }

        // ********************************************************************************************
    }
}
