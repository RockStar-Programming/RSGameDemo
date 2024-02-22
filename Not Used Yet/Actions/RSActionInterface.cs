using Rockstar.ActionProperty;
using Rockstar.Event;
using Rockstar.Node;
using Rockstar.Transformation;
using Rockstar.Types;
using System;

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

namespace Rockstar.ActionInterface
{
    public partial class RSAction
    {
        // ********************************************************************************************
        // This defines interfaces for the RSAction class

        // ********************************************************************************************
        // Constructors

        public static RSAction SetPosition(RSNode node, RSVector2 position, float duration, RSLerp lerp = RSLerp.Linear, RSEvent.Handler handler = null)    
        {
            return new RSAction().InitProperty(node, RSTransformation.POSITION, position, duration, lerp, handler);
        }

        public static RSAction PositionMove(RSNode node, RSVector2 distance, float duration, RSLerp lerp = RSLerp.Linear, RSEvent.Handler handler = null)
        {
            RSVector2 newPosition = node.Transformation.Position.Add(distance);
            return new RSAction().InitProperty(node, RSTransformation.POSITION, newPosition, duration, lerp, handler);
        }

        public static RSAction SetRotation(RSNode node, float angle, float duration, RSLerp lerp = RSLerp.Linear, RSEvent.Handler handler = null)
        {
            return new RSAction().InitProperty(node, RSTransformation.ROTATION, angle, duration, lerp, handler);
        }

        public static RSAction RotationMove(RSNode node, float angle, float duration, RSLerp lerp = RSLerp.Linear, RSEvent.Handler handler = null)
        {
            angle = node.Transformation.Rotation + angle;
            return new RSAction().InitProperty(node, RSTransformation.ROTATION, angle, duration, lerp, handler);
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
