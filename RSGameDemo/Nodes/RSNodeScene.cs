using Rockstar.EngineCanvas;
using Rockstar.Types;
using System.Numerics;
using Windows.Foundation;

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
        // RSNodeScene is the top level node for a scene
        // 

        // ********************************************************************************************
        // Constructors

        public static RSNodeScene CreateWithSize(Size size, RSSceneOrigin origin)
        {
            return new RSNodeScene(size, origin);
        }

        private RSNodeScene(Size size, RSSceneOrigin origin)
        {
            InitWithData(new Vector2(), size);
            _origin = origin;
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        // ********************************************************************************************
        // Internal Data

        private RSSceneOrigin _origin;

        // ********************************************************************************************
        // Methods

        public override void Render(RSEngineCanvas canvas)
        {
            // sets render origin for transformations
            _transformation.Position = (_origin == RSSceneOrigin.UpperLeft) ? new Vector2(0, 0) : new Vector2(0, (float)-_transformation.Size.Height);
            RSTransformation.Origin = _origin;

            // RSNodeScene has no visible representation

            // render all children
            foreach (RSNode node in _children)
            {
                // Apply the scene transformation
                canvas.InitialiseTransformation(_transformation.CreateTransformationMatrix());

                RenderAllNodes(node, canvas);
            }

            // reset transformation
            canvas.InitialiseTransformation(_transformation.CreateTransformationMatrix());
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        private void RenderAllNodes(RSNode node, RSEngineCanvas canvas)
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
