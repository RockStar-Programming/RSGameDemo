
using SkiaSharp;

using Rockstar._Lerp;
using Rockstar._Types;
using Rockstar._ActionManager;
using Rockstar._Operations;

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
            RSOperationProperty operation = new RSOperationProperty().Init(RSTransformation.POSITION, new SKPoint(x, y), RSOperationMode.Absolute, duration, type);
            return RSAction.Create(target, operation);
        }

        public static RSAction MoveTo(this RSAction action, float x, float y, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSOperationProperty operation = new RSOperationProperty().Init(RSTransformation.POSITION, new SKPoint(x, y), RSOperationMode.Absolute, duration, type);
            action.OperationList.Add(operation);
            return action;
        }

        public static RSAction MoveBy<T>(this T target, float x, float y, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear) where T : class
        {
            RSOperationProperty operation = new RSOperationProperty().Init(RSTransformation.POSITION, new SKPoint(x, y), RSOperationMode.Relative, duration, type);
            return RSAction.Create(target, operation);
        }

        public static RSAction MoveBy(this RSAction action, float x, float y, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSOperationProperty operation = new RSOperationProperty().Init(RSTransformation.POSITION, new SKPoint(x, y), RSOperationMode.Relative, duration, type);
            action.OperationList.Add(operation);
            return action;
        }

        public static RSAction ScaleTo<T>(this T target, float scaleX, float scaleY, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear) where T : class
        {
            RSOperationProperty operation = new RSOperationProperty().Init(RSTransformation.SCALE, new SKPoint(scaleX, scaleY), RSOperationMode.Absolute, duration, type);
            return RSAction.Create(target, operation);
        }

        public static RSAction ScaleTo(this RSAction action, float scaleX, float scaleY, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSOperationProperty operation = new RSOperationProperty().Init(RSTransformation.SCALE, new SKPoint(scaleX, scaleY), RSOperationMode.Absolute, duration, type);
            action.OperationList.Add(operation);
            return action;
        }

        public static RSAction RotateTo<T>(this T target, float rotation, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear) where T : class
        {
            RSOperationProperty operation = new RSOperationProperty().Init(RSTransformation.ROTATION, rotation, RSOperationMode.Absolute, duration, type);
            return RSAction.Create(target, operation);
        }

        public static RSAction RotateTo(this RSAction action, float rotation, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSOperationProperty operation = new RSOperationProperty().Init(RSTransformation.ROTATION, rotation, RSOperationMode.Absolute, duration, type);
            action.OperationList.Add(operation);
            return action;
        }

        public static RSAction RotateBy<T>(this T target, float angle, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear) where T : class
        {
            RSOperationProperty operation = new RSOperationProperty().Init(RSTransformation.ROTATION, angle, RSOperationMode.Relative, duration, type);
            return RSAction.Create(target, operation);
        }

        public static RSAction RotateBy(this RSAction action, float angle, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSOperationProperty operation = new RSOperationProperty().Init(RSTransformation.ROTATION, angle, RSOperationMode.Relative, duration, type);
            action.OperationList.Add(operation);
            return action;
        }

        public static RSAction SizeTo<T>(this T target, float width, float height, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear) where T : class
        {
            RSOperationProperty operation = new RSOperationProperty().Init(RSTransformation.SIZE, new SKSize(width, height), RSOperationMode.Absolute, duration, type);
            return RSAction.Create(target, operation);
        }

        public static RSAction SizeTo(this RSAction action, float width, float height, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSOperationProperty operation = new RSOperationProperty().Init(RSTransformation.SIZE, new SKSize(width, height), RSOperationMode.Absolute, duration, type);
            action.OperationList.Add(operation);
            return action;
        }

        public static RSAction AltitudeTo<T>(this T target, float altitude, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear) where T : class
        {
            RSOperationProperty operation = new RSOperationProperty().Init(RSTransformation.ALTITUDE, altitude, RSOperationMode.Absolute, duration, type);
            return RSAction.Create(target, operation);
        }

        public static RSAction AltitudeTo(this RSAction action, float altitude, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSOperationProperty operation = new RSOperationProperty().Init(RSTransformation.ALTITUDE, altitude, RSOperationMode.Absolute, duration, type);
            action.OperationList.Add(operation);
            return action;
        }

        public static RSAction ColorTo<T>(this T target, SKColor color, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear) where T : class
        {
            RSOperationProperty operation = new RSOperationProperty().Init(RSTransformation.COLOR, color, RSOperationMode.Absolute, duration, type);
            return RSAction.Create(target, operation);
        }

        public static RSAction ColorTo(this RSAction action, SKColor color, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSOperationProperty operation = new RSOperationProperty().Init(RSTransformation.COLOR, color, RSOperationMode.Absolute, duration, type);
            action.OperationList.Add(operation);
            return action;
        }

        public static RSAction AlphaTo<T>(this T target, float alpha, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear) where T : class
        {
            RSOperationProperty operation = new RSOperationProperty().Init(RSTransformation.ALPHA, alpha, RSOperationMode.Absolute, duration, type);
            return RSAction.Create(target, operation);
        }

        public static RSAction AlphaTo(this RSAction action, float alpha, float duration = RSAction.INSTANT, RSLerpType type = RSLerpType.Linear)
        {
            RSOperationProperty operation = new RSOperationProperty().Init(RSTransformation.ALPHA, alpha, RSOperationMode.Absolute, duration, type);
            action.OperationList.Add(operation);
            return action;
        }

        public static RSAction Delay<T>(this T target, float duration) where T : class
        {
            RSOperation operation = RSOperation.Create(duration);
            return RSAction.Create(target, operation);
        }

        public static RSAction Delay(this RSAction action, float duration)
        {
            RSOperation operation = RSOperation.Create(duration);
            action.OperationList.Add(operation);
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

        // runs the action once
        public static RSAction Run(this RSAction action)
        {
            RSActionManager.RunAction(action);
            return action;
        }

        // saves the pending batch of actions
        //   runs is requested number of times (default forever)
        public static void Repeat(this RSAction action, int repeat = 0)
        {
            RSActionManager.Repeat(action, repeat);
        }

        // ********************************************************************************************
        // misc

        public static int NumberOfRunningActions<T>(this T target) where T : class
        { 
            return RSActionManager.NumberOfRunningActions(target);
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
