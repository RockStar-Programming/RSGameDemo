using System.Numerics;

using Rockstar.Types;
using Rockstar.UWPCanvas;

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
    public class RSNodeScene : RSNode
    {
        // ********************************************************************************************
        // Brief Class Description

        // ********************************************************************************************
        // Constructors

        public static RSNodeScene CreateWithSize(RSSize size, RSSceneOrigin origin)
        { 
            return new RSNodeScene(size, origin); 
        }

        private RSNodeScene(RSSize size, RSSceneOrigin origin) 
        { 
            InitWithData(new RSVector2(), size);
            _origin = origin;
        }

        // ********************************************************************************************
        // Properties

        // ********************************************************************************************
        // Internal Data

        private RSSceneOrigin _origin;

        // ********************************************************************************************
        // Methods

        public override void Render(RSUWPCanvas canvas)
        {
            // sets render origin for transformations
            _transformation.Position = (_origin == RSSceneOrigin.UpperLeft) ? new RSVector2(0, 0) : new RSVector2(0, -_transformation.Size.Height);
            RSTransformation.Origin = _origin;

            // RSNodeScene has no visible representation

            // render all children
            foreach (RSNode node in _children)
            {
                // Apply the scene transformation
                canvas.InitialiseTransformation(_transformation.GetTransform());

                RenderAllNodes(node, canvas);
            }

            // reset transformation
            canvas.InitialiseTransformation(_transformation.GetTransform());
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        private void RenderAllNodes(RSNode node, RSUWPCanvas canvas)
        {
            // render node
            node.Render(canvas);

            // render all node children
            foreach (RSNode child in node.Children)
            {
                Matrix3x2 transform = canvas.Transformation;

                RenderAllNodes(child, canvas);

                canvas.InitialiseTransformation(transform);
            }
        }

        // ********************************************************************************************
    }
}
