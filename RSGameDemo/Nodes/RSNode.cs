
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Rockstar._Array;
using Rockstar._BaseCanvas;
using Rockstar._Types;
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

namespace Rockstar._Nodes
{
    public class RSNode
    {
        // ********************************************************************************************
        // RSNode encapsulates an invisible node
        //
        // The main responsibilies for the node, is to
        // - Do the basic transformations
        // - Handle child nodes
        //
        // The node is not responsible for rendering its children

        // ********************************************************************************************
        // Constructors & cleanup

        public static RSNode Create()
        {
            RSNode result = new RSNode();
            result.InitWithData();
            return result;
        }

        public static RSNode CreateWithPosition(Vector2 position)
        {
            RSNode result = new RSNode();
            result.InitWithData(position);
            return result;
        }

        public static RSNode CreateWithNode(RSNode node)
        {
            RSNode result = new RSNode();
            result.InitWithNode(node);
            return result;
        }

        protected RSNode()
        {
        }

        protected void InitWithNode(RSNode node)
        {
            if (node is RSNode)
            {
                InitWithData(node.Transformation);
            }
            else
            {
                InitWithData();
            }
        }

        protected void InitWithData(params object[] data)
        {
            // initialise data 
            _transformation = RSTransformation.CreateWithData(data);

            // data which always is reset
            _children = new List<RSNode>();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        public RSTransformation Transformation { get { return _transformation; } }
        public RSNode Parent { get { return _parent; } }
        public List<RSNode> Children { get { return _children; } }

        // ********************************************************************************************
        // Internal Data

        protected const int DATA_COUNT_MAX = 3;
        protected const float INVALID_POSITION = -9999;
        protected readonly Color DEBUG_COLOR = Colors.Yellow;
        protected const float DEBUG_LINEWIDTH = 2.0f;

        protected RSTransformation _transformation;
        protected RSNode _parent;
        protected List<RSNode> _children;
        protected Matrix3x2 _outputMatrix;

        // ********************************************************************************************
        // Methods

        // Override Render to draw the visual content of a node
        // IMPORTANT:
        // When sub-classing, always call "base.Render", to ensure transformations are done
        //
        public virtual void Render(RSBaseCanvas canvas)
        {
            _outputMatrix = canvas.AddTransformation(_transformation.Matrix);

            // RSNode has no visible representation

        }

        // RenderDub can be called 
        public void RenderDebug(RSBaseCanvas canvas)
        {
            canvas.InitialiseTransformation(_outputMatrix);

            Vector2 upperLeft = new Vector2((float)-_transformation.Size.Width * _transformation.Anchor.X, (float)-_transformation.Size.Height * (1.0f - _transformation.Anchor.Y));
            canvas.RenderBox(upperLeft.X, upperLeft.Y, (float)_transformation.Size.Width, (float)_transformation.Size.Height, DEBUG_COLOR, DEBUG_LINEWIDTH);
        }

        // ********************************************************************************************
        // Node positioning

        // convert a screen position to a node position
        public Vector2 LocalPosition(Vector2 screenPosition)
        {
            Vector2 result = new Vector2();
            Matrix3x2 inverseMatrix;

            if (Matrix3x2.Invert(_outputMatrix, out inverseMatrix))
            {
                result = Vector2.Transform(screenPosition, inverseMatrix);
                
                // if origin is in lower left, Y axis is inverse
                if (RSTransformation.Origin == RSSceneOrigin.LowerLeft) result = new Vector2(result.X, -result.Y);
                
                // adjust for anchor
                result = new Vector2(result.X - (0.5f - _transformation.Anchor.X) * (float)_transformation.Size.Width, result.Y - (0.5f - _transformation.Anchor.Y) * (float)_transformation.Size.Height);
            }
            else
            {
                // Handle the case where the matrix is singular and doesn't have an inverse

            }
            return result;
        }

        // override this to perform point inside checks for specific nodes
        public virtual bool PointInsize(Vector2 screenPosition)
        {
            return false;
        }

        public bool PointInsizeRectangle(Vector2 screenPosition)
        {
            float width = (float)_transformation.Size.Width / 2.0f;
            float height = (float)_transformation.Size.Height / 2.0f;

            Vector2 position = LocalPosition(screenPosition);

            if ((position.X < -width) || (position.X > width)) return false;
            if ((position.Y < -height) || (position.Y > height)) return false;
            return true;
        }

        public bool PointInsizeEllipse(Vector2 screenPosition)
        {
            float width = (float)_transformation.Size.Width / 2.0f;
            float height = (float)_transformation.Size.Width / 2.0f;

            Vector2 position = LocalPosition(screenPosition);

            double value = (position.X * position.X) / (width * width) + (position.Y * position.Y) / (height * height);
            return (value <= 1.0);
        }

        public List<RSNode> GetHitList(Vector2 screenPosition) 
        {
            List<RSNode> hitList = new List<RSNode>();

            if (PointInsize(screenPosition) == true) hitList.Add(this);
            foreach (RSNode node in _children)
            {
                hitList.AddRange(node.GetHitList(screenPosition));
            }

            return hitList;
        }

        // ********************************************************************************************
        // Handling Children

        public void AddChild(RSNode node)
        {
            if (_children.Contains(node) == false)
            {
                node._parent = this;
                _children.Add(node);
            }
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // Override Update to perform visual updates to a node
        // IMPORTANT:
        // This is not intended for any kind of game mechanics
        // Should be used only to update visual apperance of a node descendant
        // Example could be for sprite animations, using a series of images
        //
        public virtual void Update(long interval)
        {
            ;
        }

        // ********************************************************************************************
    }
}
