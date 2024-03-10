
using SkiaSharp;

using Rockstar._NodeList;
using Rockstar._Types;
using Rockstar._RenderSurface;
using Rockstar._Nodes;

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

namespace Rockstar._Renderer
{
    public class RSRenderer
    {
        // ********************************************************************************************
        // RSRenderer renders a node tree to a drawing surface
        //
        // 1) All nodes are transformed
        //    - This causes node.RenderMatrix to be set
        //    - Visible nodes are added to render list
        // 2) Render list is sorted according to Z (lower Z rendered first)
        // 3) Render list is rendered to canvas
        // 4) RenderNodeTree is called resursively for off screen rendering 

        // ********************************************************************************************
        // Constructors

        public static RSRenderer Create() 
        { 
            return new RSRenderer(); 
        }

        private RSRenderer()
        { 
        }


        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        public int NodeCount { get { return _nodeCount; } }

        // ********************************************************************************************
        // Internal Data

        private const int RENDER_DEBUG_INSET = 5;
        private int _nodeCount;

        // ********************************************************************************************
        // Methods

        public void RenderBegin()
        {
            _nodeCount = 0;
        }

        public void RenderNodeTree(RSRenderSurface surface, RSNode node)
        {
            RSNodeList renderList = RSNodeList.Create();

            // reset surface transformation and transform the entire tree
            SKMatrix matrix = SKMatrix.Identity;
            if (surface.Origin == RSTransformationOrigin.LowerLeft)
            {
                matrix = SKMatrix.CreateTranslation(0, surface.Size.Height);
            }
            // apply anchor point
            SKPoint pos = new SKPoint(
                node.Transformation.Size.Width * node.Transformation.Anchor.X,
                -node.Transformation.Size.Height * node.Transformation.Anchor.Y);
            matrix = SKMatrix.Concat(matrix, SKMatrix.CreateTranslation(pos.X, pos.Y));

            // set initial matrix and clear render surface
            surface.SetMatrix(matrix);
            surface.Clear();

            // transform the node tree to render
            BuildRenderList(surface, node, renderList);

            // sort the render list for Z
            renderList.Sort();

            // node list is now transformed and sorted, ready to render
            RenderNodeList(surface, renderList);
            _nodeCount += renderList.Count;
        }

        public void RenderNodeList(RSRenderSurface surface, RSNodeList renderList)
        {
            foreach (RSNode renderNode in renderList)
            {
                surface.SetCanvasMatrix(renderNode.RenderMatrix);
                renderNode.Render(surface);
            }
        }

        public void RenderDebugNodeList(RSRenderSurface surface, RSNodeList renderList)
        {
            foreach (RSNode renderNode in renderList)
            {
                surface.SetCanvasMatrix(renderNode.RenderMatrix);
                renderNode.RenderDebug(surface);
            }
        }

        public void RenderDebugString(RSRenderSurface surface, int nodeCount, double fps)
        {
            SKMatrix matrix = surface.Canvas.TotalMatrix;
            surface.SetCanvasMatrix(SKMatrix.Identity);

            if (fps > 999.9) fps = 999.9;
            string message = string.Format("Nodes:{0} @{1:00}x{2:00} - {3:0.0}fps", nodeCount, surface.Size.Width, surface.Size.Height, fps);

            SKPaint paint = surface.GetTextPaint(RSFont.Create(), SKColors.White);

            float width = paint.MeasureText(message);
            SKFontMetrics metrics = paint.FontMetrics;
            float height = metrics.Descent - metrics.Ascent;

            SKColor color = new SKColor(0, 0, 0, 64);

            SKPoint position = new SKPoint(surface.Size.Width - width - (2 * RENDER_DEBUG_INSET), surface.Size.Height - height);
            SKSize size = new SKSize(width + (2 * RENDER_DEBUG_INSET), height);
            surface.DrawRectangle(position, size, color);

            position = new SKPoint(surface.Size.Width - width - RENDER_DEBUG_INSET, surface.Size.Height - metrics.Descent);
            surface.DrawText(position, message, paint);

            surface.Canvas.SetMatrix(matrix);
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // Builds a render list from a node tree
        // Called recursively for off screen rendering
        // Nodes are transformed into final position, but actual rendering is not done yet
        // The surface maxtrix is used for accumulative transformations
        // 
        private void BuildRenderList(RSRenderSurface surface, RSNode node, RSNodeList renderList)
        {
            bool ChildrenRendered = false;

            // If node is a surface, and canvas is different from the node surface canvas,
            //   children should be rendered off screen
            //
            if ((node is RSNodeSurface nodeSurface) && (surface.Canvas != nodeSurface.Canvas))
            {
                // Create off screen surface, and render the tree
                RSRenderSurface renderSurface = RSRenderSurface.CreateOffScreen(nodeSurface);
                renderSurface.Clear(nodeSurface.Bitmap);
                RenderNodeTree(renderSurface, nodeSurface);

                // children was just rendered, so drop them here
                ChildrenRendered = true;
            }

            // Dont transform and add nodes that are off screen node surfaces
            // This ensures that surfaces are only added to on-screen canvas
            //
            if ((surface.OffScreen == false) || (node is RSNodeSurface == false))
            {
                // Transform the node no matter if it is visible or not
                //   as children might depend on it
                node.ApplySurfaceMatrix(surface);

                // Add the node to the render list if visible
                if (node.Transformation.Visible == true)
                {
                    renderList.Add(node);
                }
            }

            // Render children if not already
            if (ChildrenRendered == false)
            {
                // Recursively iterate all children
                foreach (RSNode child in node.Children)
                {
                    // Store surface matrix before child transformations are added
                    SKMatrix matrix = surface.Matrix;

                    // Transform children
                    BuildRenderList(surface, child, renderList);

                    // Restore surface matrix
                    surface.SetMatrix(matrix);
                }
            }
        }

        // ********************************************************************************************
    }
}
