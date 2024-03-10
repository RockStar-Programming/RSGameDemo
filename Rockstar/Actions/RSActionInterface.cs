
using SkiaSharp;

using Rockstar._Lerp;
using Rockstar._Nodes;
using Rockstar._Types;
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

        // ********************************************************************************************
        // Constructors

        public static RSAction MoveTo(this RSNode node, SKPoint position, float duration, RSLerpType type = RSLerpType.Linear)
        {
            RSAction action = new RSAction().InitAction(node, RSTransformation.POSITION, position, duration, type);
            RSActionManager.Add(node, action);
            return action;
        }

        public static RSAction MoveBy(this RSNode node, SKPoint movement, float duration, RSLerpType type = RSLerpType.Linear)
        {
            SKPoint position = node.Transformation.Position + movement;
            RSAction action = new RSAction().InitAction(node, RSTransformation.POSITION, position, duration, type);
            RSActionManager.Add(node, action);
            return action;
        }

        // ********************************************************************************************

        public static RSAction Repeat(this RSAction action, int repeat = -1)
        {
            action.SetRepeatCounter(repeat);
            return action;
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
