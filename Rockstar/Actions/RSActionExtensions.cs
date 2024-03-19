
using SkiaSharp;

using Rockstar._Lerp;
using Rockstar._Types;
using Rockstar._ActionManager;
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
    public static class RSActionExtensions
    {
        // ********************************************************************************************
        // This defines interfaces for the RSActions class
        //
        // Ex: creates a MoveTo followed by a ScaleTo, and saves it (does not initially run)
        // "sequence" will be active until the actions are run, saved or repeated
        // target.CreateSequence().MoveTo(pos1).ScaleTo(scale1).SaveAs("test");
        // target.RunAction("test");
        //
        // Ex: simultaneously moves and scales the target. Runs instantly, and deletes newAction after completion
        // target.MoveTo(pos1).ScaleTo(scale1).Run();
        //
        // Ex: 
        // 

        // ********************************************************************************************
        // Action Extensions

        public static RSAction MoveTo<T>(this T target, float x, float y, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear) where T : class
        {
            RSActionProperty newAction = new RSActionProperty().InitAction(RSTransformation.POSITION, new SKPoint(x, y), RSActionMode.Absolute, duration, type);
            return RSAction.Create(target, newAction);
        }

        public static RSAction MoveTo(this RSAction action, float x, float y, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSActionProperty newAction = new RSActionProperty().InitAction(RSTransformation.POSITION, new SKPoint(x, y), RSActionMode.Absolute, duration, type);
            action.ActionList.Add(newAction);
            return action;
        }

        public static RSAction MoveBy<T>(this T target, float x, float y, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear) where T : class
        {
            RSActionProperty newAction = new RSActionProperty().InitAction(RSTransformation.POSITION, new SKPoint(x, y), RSActionMode.Relative, duration, type);
            return RSAction.Create(target, newAction);
        }

        public static RSAction MoveBy(this RSAction action, float x, float y, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSActionProperty newAction = new RSActionProperty().InitAction(RSTransformation.POSITION, new SKPoint(x, y), RSActionMode.Relative, duration, type);
            action.ActionList.Add(newAction);
            return action;
        }

        public static RSAction ScaleTo<T>(this T target, float scaleX, float scaleY, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear) where T : class
        {
            RSActionProperty newAction = new RSActionProperty().InitAction(RSTransformation.SCALE, new SKPoint(scaleX, scaleY), RSActionMode.Absolute, duration, type);
            return RSAction.Create(target, newAction);
        }

        public static RSAction ScaleTo(this RSAction action, float scaleX, float scaleY, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSActionProperty newAction = new RSActionProperty().InitAction(RSTransformation.SCALE, new SKPoint(scaleX, scaleY), RSActionMode.Absolute, duration, type);
            action.ActionList.Add(newAction);
            return action;
        }

        public static RSAction RotateTo<T>(this T target, float rotation, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear) where T : class
        {
            RSActionProperty newAction = new RSActionProperty().InitAction(RSTransformation.ROTATION, rotation, RSActionMode.Absolute, duration, type);
            return RSAction.Create(target, newAction);
        }

        public static RSAction RotateTo(this RSAction action, float rotation, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSActionProperty newAction = new RSActionProperty().InitAction(RSTransformation.ROTATION, rotation, RSActionMode.Absolute, duration, type);
            action.ActionList.Add(newAction);
            return action;
        }

        public static RSAction RotateBy<T>(this T target, float angle, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear) where T : class
        {
            RSActionProperty newAction = new RSActionProperty().InitAction(RSTransformation.ROTATION, angle, RSActionMode.Relative, duration, type);
            return RSAction.Create(target, newAction);
        }

        public static RSAction RotateBy(this RSAction action, float angle, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSActionProperty newAction = new RSActionProperty().InitAction(RSTransformation.ROTATION, angle, RSActionMode.Relative, duration, type);
            action.ActionList.Add(newAction);
            return action;
        }

        public static RSAction SizeTo<T>(this T target, float width, float height, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear) where T : class
        {
            RSActionProperty newAction = new RSActionProperty().InitAction(RSTransformation.SIZE, new SKSize(width, height), RSActionMode.Absolute, duration, type);
            return RSAction.Create(target, newAction);
        }

        public static RSAction SizeTo(this RSAction action, float width, float height, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSActionProperty newAction = new RSActionProperty().InitAction(RSTransformation.SIZE, new SKSize(width, height), RSActionMode.Absolute, duration, type);
            action.ActionList.Add(newAction);
            return action;
        }

        public static RSAction AltitudeTo<T>(this T target, float altitude, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear) where T : class
        {
            RSActionProperty newAction = new RSActionProperty().InitAction(RSTransformation.ALTITUDE, altitude, RSActionMode.Absolute, duration, type);
            return RSAction.Create(target, newAction);
        }

        public static RSAction AltitudeTo(this RSAction action, float altitude, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSActionProperty newAction = new RSActionProperty().InitAction(RSTransformation.ALTITUDE, altitude, RSActionMode.Absolute, duration, type);
            action.ActionList.Add(newAction);
            return action;
        }

        public static RSAction ColorTo<T>(this T target, SKColor color, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear) where T : class
        {
            RSActionProperty newAction = new RSActionProperty().InitAction(RSTransformation.COLOR, color, RSActionMode.Absolute, duration, type);
            return RSAction.Create(target, newAction);
        }

        public static RSAction ColorTo(this RSAction action, SKColor color, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSActionProperty newAction = new RSActionProperty().InitAction(RSTransformation.COLOR, color, RSActionMode.Absolute, duration, type);
            action.ActionList.Add(newAction);
            return action;
        }

        public static RSAction AlphaTo<T>(this T target, float alpha, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear) where T : class
        {
            RSActionProperty newAction = new RSActionProperty().InitAction(RSTransformation.ALPHA, alpha, RSActionMode.Absolute, duration, type);
            return RSAction.Create(target, newAction);
        }

        public static RSAction AlphaTo(this RSAction action, float alpha, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSActionProperty newAction = new RSActionProperty().InitAction(RSTransformation.ALPHA, alpha, RSActionMode.Absolute, duration, type);
            action.ActionList.Add(newAction);
            return action;
        }

        // ********************************************************************************************
        // Target Extensions (First extension on a node)

        // runs all stored actions under the name
        public static void RunAction<T>(this T target, string name) where T : class
        {
            RSActionManager.RunAction(target, name);
        }

        // starts a sequence of actions
        public static RSAction Sequence<T>(this T target) where T : class
        {
            return RSAction.CreateSequence(target);
        }

        // ********************************************************************************************
        // Final Action Extensions (RSActionManager calls)

        // Saves the pending batch of actions
        //   does not execute it
        public static void SaveAs(this RSAction action, string name)
        {
            RSActionManager.Save(action, name);
        }

        // saves the pending batch of actions
        //   runs is once and removes it
        public static void Run(this RSAction action)
        {
            RSActionManager.RunAction(action);
        }

        // saves the pending batch of actions
        //   runs is requested number of times (default forever)
        public static void Repeat(this RSAction action, int repeat = 0)
        {
            RSActionManager.Repeat(action, repeat);
        }

        // ********************************************************************************************
        // Properties

        // ********************************************************************************************
        // Internal Data

        // ********************************************************************************************
        // Methods

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }
}
