﻿
using SkiaSharp;

using Rockstar._RenderSurface;

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

namespace Rockstar._Nodes
{
    public class RSNodeSolid : RSNode
    {
        // ********************************************************************************************
        // Simple colored solids
        //
        // Inherits RSNode, and implements rendering of solid objects

        // ********************************************************************************************
        // Constructors

        public static RSNodeSolid CreateRectangle(SKSize size, SKColor color)
        {
            RSNodeSolid result = new RSNodeSolid(SolidType.Rectangle);
            result.InitWithData(SKPoint.Empty, size);
            result.Transformation.Color = color;
            return result;
        }

        public static RSNodeSolid CreateEllipse(SKSize size, SKColor color)
        {
            RSNodeSolid result = new RSNodeSolid(SolidType.Ellipse);
            result.InitWithData(SKPoint.Empty, size);
            result.Transformation.Color = color;
            result._isRound = true;
            return result;
        }

        // ********************************************************************************************

        private RSNodeSolid(SolidType type)
        {
            _type = type;
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        // ********************************************************************************************
        // Internal Data

        private enum SolidType
        {
            Rectangle,
            Ellipse
        }

        private SolidType _type;

        // ********************************************************************************************
        // Methods

        public override bool PointInside(SKPoint screenPosition)
        {
            if (_touchMode != RSNodeTouchMode.Accurate) return base.PointInside(screenPosition);

            switch (_type)
            {
                case SolidType.Rectangle:
                    return PointInsizeRectangle(screenPosition);
                case SolidType.Ellipse:
                    return PointInsizeEllipse(screenPosition);
                default:
                    return false;
            }
        }

        public override void Render(RSRenderSurface surface)
        {
            base.Render(surface);

            switch (_type)
            {
                case SolidType.Rectangle:
                    SKPoint upperLeft = new SKPoint(
                        -_transformation.Size.Width * _transformation.Anchor.X, 
                        -_transformation.Size.Height * (1.0f - _transformation.Anchor.Y));
                    surface.DrawRectangle(upperLeft, _transformation.Size, _transformation.Color);
                    break;
                case SolidType.Ellipse:
                    SKPoint position = new SKPoint(
                        (0.5f - _transformation.Anchor.X) * _transformation.Size.Width, 
                        (_transformation.Anchor.Y - 0.5f) * _transformation.Size.Height);
                    surface.DrawEllipse(position, _transformation.Size, _transformation.Color);
                    break;
                default:
                    break;
            }
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }
}
