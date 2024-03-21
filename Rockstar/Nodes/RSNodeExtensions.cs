
using SkiaSharp;

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
    public static class RSNodeExtensions
    {
        // ********************************************************************************************
        // Extensions for the RSNode class
        //

        // ********************************************************************************************
        // Constructors

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        // ********************************************************************************************
        // Internal Data

        // ********************************************************************************************
        // Methods

        public static T SetPosition<T>(this T node, SKPoint position) where T : RSNode
        {
            node.Transformation.Position = position;
            return node;
        }

        public static T SetPosition<T>(this T node, float x, float y) where T : RSNode
        {
            node.Transformation.Position = new SKPoint(x, y);
            return node;
        }

        public static T SetAnchor<T>(this T node, SKPoint anchor) where T : RSNode
        {
            node.Transformation.Anchor = anchor;
            return node;
        }

        public static T SetAnchor<T>(this T node, float anchor) where T : RSNode
        {
            node.Transformation.Anchor = new SKPoint(anchor, anchor);
            return node;
        }

        public static T SetAnchor<T>(this T node, float anchorX, float anchorY) where T : RSNode
        {
            node.Transformation.Anchor = new SKPoint(anchorX, anchorY);
            return node;
        }

        public static T SetScale<T>(this T node, SKPoint scale) where T : RSNode
        {
            node.Transformation.Scale = scale;
            return node;
        }

        public static T SetScale<T>(this T node, float scale) where T : RSNode
        {
            node.Transformation.Scale = new SKPoint(scale, scale);
            return node;
        }

        public static T SetScale<T>(this T node, float scaleX, float scaleY) where T : RSNode
        {
            node.Transformation.Scale = new SKPoint(scaleX, scaleY);
            return node;
        }

        public static T SetAlpha<T>(this T node, float alpha) where T : RSNode
        {
            node.Transformation.Alpha = alpha;
            return node;
        }

        public static T SetAltitude<T>(this T node, float altitude) where T : RSNode
        {
            node.Transformation.Altitude = altitude;
            return node;
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }
}
