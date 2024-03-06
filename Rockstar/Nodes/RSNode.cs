
using SkiaSharp;

using Rockstar._RenderSurface;
using Rockstar._Types;
using Rockstar._NodeList;

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
        None,           // touche disabled
        Simple,         // simple boundary box detection
        Accurate        // shape and pixel detection
    }

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
            throw new NotImplementedException();
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        public string Name { get { return _name; } set { SetName(value); } }
        public RSTransformation Transformation { get { return _transformation; } }
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
        protected RSTransformation _transformation;
        protected RSNode? _parent;
        protected List<RSNode> _children;
        protected SKMatrix _renderMatrix;
        protected RSNodeTouchMode _touchMode;

        private static long _nodeIndex = 0;

        // ********************************************************************************************
        // Methods

        // transforms the node into the current render surface
        //
        public void ApplySurfaceMatrix(RSRenderSurface surface)
        {
            // set the transformation origin for target surface
            _transformation.Origin = surface.Origin;
            _renderMatrix = surface.MultiplyMatrix(_transformation.Matrix);
        }

        // Override Render to draw the visual content of a node
        //
        public virtual void Render(RSRenderSurface surface)
        {
            // RSNode has no visible representation

        }

        // RenderDebug draws a simple frame around the node
        //
        public void RenderDebug(RSRenderSurface surface)
        {
            SKPoint upperLeft = new SKPoint(
                (float)-_transformation.Size.Width * _transformation.Anchor.X, 
                (float)-_transformation.Size.Height * (1.0f - _transformation.Anchor.Y));
            surface.DrawBox(upperLeft, _transformation.Size, DEBUG_COLOR, DEBUG_LINEWIDTH);
        }

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
        // Node positioning

        // convert a screen position to a node position
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
            foreach (RSNode node in _children)
            {
                result.AddRange(node.GetHitList(screenPosition));
            }

            return result;
        }

        // ********************************************************************************************
        // Handling Children

        public void AddChild(RSNode node)
        {
            if (node._parent == null)
            {
                node._parent = this;
                _children.Add(node);
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
