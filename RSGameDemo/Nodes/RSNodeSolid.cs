using Rockstar.Types;
using Rockstar.UWPCanvas;
using Windows.UI;

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

namespace Rockstar.Nodes
{
    public class RSNodeSolid : RSNode
    {
        // ********************************************************************************************
        // Simple colored solids

        // ********************************************************************************************
        // Constructors

        public static RSNodeSolid CreateRectangle(RSVector2 position, RSSize size, Color color)
        {
            RSNodeSolid result = new RSNodeSolid(SolidType.Rectangle);
            result.InitWithData(position, size);
            result.Transformation.Color = color;
            return result;
        }

        public static RSNodeSolid CreateEllipse(RSVector2 position, RSSize size, Color color)
        {
            RSNodeSolid result = new RSNodeSolid(SolidType.Ellipse);
            result.InitWithData(position, size);
            result.Transformation.Color = color;
            return result;
        }

        private RSNodeSolid(SolidType type) 
        {
            _type = type;
        }

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

        public override void Render(RSUWPCanvas canvas)
        {
            base.Render(canvas);

            switch (_type)
            {
                case SolidType.Rectangle:
                    RSVector2 upperLeft = new RSVector2(-_transformation.Size.Width * _transformation.Anchor.X, -_transformation.Size.Height * (1.0f - _transformation.Anchor.Y));
                    canvas.RenderRectangle(upperLeft.X, upperLeft.Y, _transformation.Size.Width, _transformation.Size.Height, _transformation.Color);
                    break;
                case SolidType.Ellipse:
                    canvas.RenderEllipse(0, 0, _transformation.Size.Width, _transformation.Size.Height, _transformation.Color);
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
