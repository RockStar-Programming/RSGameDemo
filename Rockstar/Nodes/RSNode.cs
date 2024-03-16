
using SkiaSharp;

using Rockstar._RenderSurface;
using Rockstar._Types;

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
    public enum RSNodeTouchMode
    {
        None,                   // hittest disabled
        Simple,                 // simple boundary box detection
        Accurate,               // shape and pixel detection
    }

    public class RSNode : IDisposable
    {
        // ********************************************************************************************
        // RSNode encapsulates an invisible node
        //
        // The main responsibilies for the node, is to
        // - Do the basic transformations
        // - Handle child nodes
        //
        // The node is not responsible for rendering its children
        //
        // The Level property orders nodes in levels
        // - Setting Level for a node, will also set all its children
        // - Adding a node will set Level to parent level
        // - Inserting a parent, will set parent Level to node Level
        //     When inserting, any children will not be affected
        // - Removing nodes does not affect levels

        // ********************************************************************************************
        // Constructors & cleanup

        public static RSNode Create()
        {
            RSNode result = new RSNode();
            result.InitWithData();
            return result;
        }

        public static RSNode CreateWithPosition(SKPoint position)
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

        // ********************************************************************************************
        
        protected RSNode()
        {
            _transformation = RSTransformation.CreateWithData();
            _transformation.Visible = false;

            // auto generated unique name
            _name = "node_" + _nodeIndex.ToString();
            _nodeIndex++;

            // data which always is reset
            _children = new List<RSNode>();
            _touchMode = RSNodeTouchMode.Accurate;
            _level = 0;
            _isRound = false;
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
        }

        public void Dispose()
        {

        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        public string Name { get { return _name; } set { SetName(value); } }
        public int Level { get { return _level; } }
        public RSTransformation Transformation { get { return _transformation; } }
        public bool IsRound { get { return _isRound; } }
        public RSNode? Parent { get { return _parent; } }
        public List<RSNode> Children { get { return _children; } }
        public SKMatrix RenderMatrix { get { return _renderMatrix; } }
        public RSNodeTouchMode TouchMode { get { return _touchMode; } set { SetTouchMode(value); } }

        // ********************************************************************************************
        // Internal Data

        protected const int DATA_COUNT_MAX = 3;
        protected const float INVALID_POSITION = -9999;
        protected readonly SKColor DEBUG_COLOR = SKColors.Yellow;
        protected const float DEBUG_LINEWIDTH = 2.0f;

        protected string _name;
        protected int _level;
        protected RSTransformation _transformation;
        protected bool _isRound;
        protected RSNode? _parent;
        protected List<RSNode> _children;
        protected SKMatrix _renderMatrix;
        protected RSNodeTouchMode _touchMode;

        private static long _nodeIndex = 0;

        // ********************************************************************************************
        // Class Methods

        public static void RemoveChildren(RSNode node)
        {
            RSNodeList nodeList = RSNode.ChildList(node);
            foreach (RSNode child in nodeList)
            {
                if (child._parent != null)
                {
                    child._parent._children.Remove(child);
                    child._parent = null;
                }
            }
            node._children.Clear();
        }

        public static RSNodeList ChildList(RSNode node)
        {
            RSNodeList result = RSNodeList.Create();

            foreach (RSNode child in node.Children)
            {
                result.AddRange(ChildList(child));
            }

            return result;
        }

        // Nodes can not be replaced if they have no parent, or if they have children
        public static void ReplaceNode(RSNode nodeToReplace, params RSNode[] nodeList)
        {
            if ((nodeToReplace._parent != null) && (nodeToReplace._children.Count == 0))
            {
                foreach (RSNode node in nodeList)
                {
                    nodeToReplace._parent.AddChild(node);
                }
                nodeToReplace._parent._children.Remove(nodeToReplace);
                nodeToReplace._parent = null;
            }
        }

        // ********************************************************************************************
        // Methods

        // transforms the nodeToRemove into the current render surface
        //
        public void ApplySurfaceMatrix(RSRenderSurface surface)
        {
            // set the transformation origin for target surface
            _transformation.Origin = surface.Origin;
            _renderMatrix = surface.MultiplyMatrix(_transformation.Matrix);
        }

        // Override Render to draw the visual content of a nodeToRemove
        //
        public virtual void Render(RSRenderSurface surface)
        {
            // RSNode has no visible representation

        }

        // RenderDebug draws a simple frame around the nodeToRemove
        //
        public void RenderDebug(RSRenderSurface surface)
        {
            SKPoint upperLeft = new SKPoint(
                (float)-_transformation.Size.Width * _transformation.Anchor.X, 
                (float)-_transformation.Size.Height * (1.0f - _transformation.Anchor.Y));
            surface.DrawBox(upperLeft, _transformation.Size, DEBUG_COLOR, DEBUG_LINEWIDTH);
        }

        // Override Update to perform visual updates to a nodeToRemove
        // IMPORTANT:
        // This is not intended for any kind of game mechanics
        // Should be used only to update visual apperance of a nodeToRemove descendant
        // Example could be for sprite animations, using a series of images
        //
        public virtual void Update(float interval)
        {

        }

        // ********************************************************************************************
        // Target positioning

        // convert a screen position to a nodeToRemove position
        public SKPoint LocalPosition(SKPoint screenPosition)
        {
            SKPoint result = new SKPoint();
            SKMatrix inverseMatrix;

            if (_renderMatrix.TryInvert(out inverseMatrix))
            {
                result = inverseMatrix.MapPoint(screenPosition);
                
                // if origin is in lower left, Y axis is inverse
                if (_transformation.Origin == RSTransformationOrigin.LowerLeft) result = new SKPoint(result.X, -result.Y);
                
                // adjust for anchor
                result = new SKPoint(result.X - (0.5f - _transformation.Anchor.X) * (float)_transformation.Size.Width, result.Y - (0.5f - _transformation.Anchor.Y) * (float)_transformation.Size.Height);
            }
            else
            {
                // Handle the case where the matrix is singular and doesn't have an inverse

            }
            return result;
        }

        // override this to perform point inside checks for specific nodes
        public virtual bool PointInside(SKPoint screenPosition)
        {
            if (_touchMode == RSNodeTouchMode.Simple) return PointInsizeRectangle(screenPosition);
            return false;
        }

        public bool PointInsizeRectangle(SKPoint screenPosition)
        {
            float halfWidth = (float)_transformation.Size.Width / 2.0f;
            float halfHeight = (float)_transformation.Size.Height / 2.0f;

            SKPoint nodePosition = LocalPosition(screenPosition);

            if ((nodePosition.X < -halfWidth) || (nodePosition.X > halfWidth)) return false;
            if ((nodePosition.Y < -halfHeight) || (nodePosition.Y > halfHeight)) return false;
            return true;
        }

        public bool PointInsizeEllipse(SKPoint screenPosition)
        {
            float halfWidth = (float)_transformation.Size.Width / 2.0f;
            float halfHeight = (float)_transformation.Size.Width / 2.0f;

            SKPoint nodePosition = LocalPosition(screenPosition);

            double value = 
                (nodePosition.X * nodePosition.X) / (halfWidth * halfWidth) + 
                (nodePosition.Y * nodePosition.Y) / (halfHeight * halfHeight);
            return (value <= 1.0);
        }

        public RSNodeList GetHitList(SKPoint screenPosition) 
        {
            RSNodeList result = RSNodeList.Create();

            if (PointInside(screenPosition) == true) result.Add(this);

            // dont hit test on nodes added to a surface
            if (this is RSNodeSurface == false)
            {
                foreach (RSNode node in _children)
                {
                    result.AddRange(node.GetHitList(screenPosition));
                }
            }

            return result;
        }

        // ********************************************************************************************
        // Handling Children

        public void AddChild(RSNode nodeToAdd)
        {
            if (nodeToAdd._parent == null)
            {
                nodeToAdd._parent = this;
                nodeToAdd._level = _level;
                _children.Add(nodeToAdd);
            }
        }

        public void RemoveChild(RSNode nodeToRemove) 
        {
            _children.Remove(nodeToRemove);
            nodeToRemove._parent = null;
        }

        // ********************************************************************************************

        public void SetLevel(int level)
        {
            _level = level;
            RSNodeList nodeList = ChildList(this);
            foreach (RSNode child in nodeList)
            {
                child._level = level;
            }
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        private void SetName(string name)
        {
            if (name != null) _name = name;
        }

        private void SetTouchMode(RSNodeTouchMode mode)
        {
            if (Enum.IsDefined(typeof(RSNodeTouchMode), mode))
            {
                _touchMode = mode;
            }
        }

        // ********************************************************************************************
    }
}
