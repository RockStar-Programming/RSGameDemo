using SkiaSharp;
using System.Numerics;

using Rockstar._NodeList;
using Rockstar._Types;
using Rockstar._RenderSurface;
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

        // ********************************************************************************************
        // Internal Data

        private const int RENDER_DEBUG_INSET = 5;

        // ********************************************************************************************
        // Methods

        public int RenderNodeTree(RSNode node, RSRenderSurface surface)
        {
            RSNodeList renderList = RSNodeList.Create();

            // reset surface transformation and transform the entire tree
            Matrix3x2 matrix = Matrix3x2.Identity;
            if (surface.Origin == RSTransformationOrigin.LowerLeft)
            {
                matrix = Matrix3x2.CreateTranslation(0, surface.Size.Height);
            }
            surface.SetMatrix(matrix);

            // transform the node tree to render
            TransformNodeTree(node, surface, renderList);

            // sort the render list for Z
            renderList.Sort();

            // node list is not transformed and sorted, ready to render
            RenderNodeList(renderList, surface);

            return renderList.Count;
        }

        public void RenderNodeList(RSNodeList renderList, RSRenderSurface surface, bool debugMode = false)
        {
            foreach (RSNode renderNode in renderList)
            {
                surface.SetCanvasMatrix(renderNode.RenderMatrix);
                if (debugMode == false)
                {
                    renderNode.Render(surface);
                }
                else 
                {
                    renderNode.RenderDebug(surface);
                }
            }
        }

        public void RenderDebugString(int nodeCount, double fps, RSRenderSurface surface)
        {
            SKMatrix matrix = surface.Canvas.TotalMatrix;
            surface.SetCanvasMatrix(Matrix3x2.Identity);

            if (fps > 999.9) fps = 999.9;
            string message = string.Format("Nodes:{0} @{1:00}x{2:00} - {3:0.0}fps", nodeCount, surface.Size.Width, surface.Size.Height, fps);

            SKPaint paint = surface.GetTextPaint(RSFont.Create(), SKColors.White);

            float width = paint.MeasureText(message);
            SKFontMetrics metrics = paint.FontMetrics;
            float height = metrics.Descent - metrics.Ascent;

            SKColor color = new SKColor(255, 255, 255, 64);

            surface.DrawRectangle(surface.Size.Width - width - (2 * RENDER_DEBUG_INSET), surface.Size.Height - height, width + (2 * RENDER_DEBUG_INSET), height, color);
            surface.DrawText(surface.Size.Width - width - RENDER_DEBUG_INSET, surface.Size.Height - metrics.Descent, message, paint);

            surface.Canvas.SetMatrix(matrix);
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        private void TransformNodeTree(RSNode node, RSRenderSurface surface, RSNodeList renderList)
        {
            // transform the node
            node.ApplySurfaceMatrix(surface);

            // add node to render list if visible
            if (node.Transformation.Visible == true)
            {
                renderList.Add(node);
            }

            // recursively iterate all children
            foreach (RSNode child in node.Children)
            {
                // store surface matrix before child transformations are added
                Matrix3x2 matrix = surface.Matrix;

                // transform children
                TransformNodeTree(child, surface, renderList);

                // restore surface matrix
                surface.SetMatrix(matrix);
            }
        }

        // ********************************************************************************************
    }
}
