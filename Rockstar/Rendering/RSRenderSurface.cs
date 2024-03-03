
using SkiaSharp;
using System.Numerics;

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

namespace Rockstar._RenderSurface
{
    public class RSRenderSurface
    {
        // ********************************************************************************************
        // Render surface encapsulates an SKCanvas for rendering 
        //
        // Any drawing and drawing support should be performed here
        // Do not access SKCanvas from other classes

        // ********************************************************************************************
        // Constructors

        public static RSRenderSurface Create(SKCanvas canvas, SKColor color, RSTransformationOrigin origin = RSTransformationOrigin.LowerLeft)
        {
            return new RSRenderSurface(canvas, color, origin);
        }

        private RSRenderSurface(SKCanvas canvas, SKColor color, RSTransformationOrigin origin)
        {
            _canvas = canvas;
            _size = new Size(_canvas.DeviceClipBounds.Width, _canvas.DeviceClipBounds.Height);
            _color = color;
            _origin = origin;
            _matrix = Matrix3x2.Identity;
            _antiAlias = true;
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        public SKCanvas Canvas { get { return _canvas; } }
        public Size Size { get { return _size; } }
        public RSTransformationOrigin Origin { get { return _origin; } }
        public Matrix3x2 Matrix { get { return _matrix; } }
        public bool AntiAlias { get { return _antiAlias; } }

        // ********************************************************************************************
        // Internal Data

        private SKCanvas _canvas;
        private Size _size;
        private SKColor _color;
        private RSTransformationOrigin _origin;
        private Matrix3x2 _matrix;
        private bool _antiAlias;

        // ********************************************************************************************
        // Methods

        public void SetMatrix(Matrix3x2 matrix)
        {
            _matrix = matrix;
        }

        public Matrix3x2 MultiplyMatrix(Matrix3x2 matrix)
        {
            _matrix = matrix * _matrix;
            return _matrix;
        }

        public void SetCanvasMatrix(Matrix3x2 transformation)
        {
            _canvas.SetMatrix(new SKMatrix(
                scaleX: transformation.M11, scaleY: transformation.M22,
                skewX: transformation.M21, skewY: transformation.M12,
                transX: transformation.M31, transY: transformation.M32,
                persp0: 0, persp1: 0, persp2: 1));
        }

        // ********************************************************************************************
        // Draw Methods

        public void DrawBox(float x, float y, float width, float height, SKColor color, float line)
        {
            SKPaint paint = new SKPaint
            {
                Color = color,
                IsAntialias = _antiAlias,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = line
            };
            _canvas.DrawRect(x, y, width, height, paint);
        }

        public void DrawRectangle(float x, float y, float width, float height, SKColor color)
        {
            SKPaint paint = new SKPaint
            {
                Color = color,
                IsAntialias = _antiAlias,
                Style = SKPaintStyle.Fill
            };
            _canvas.DrawRect(x, y, width, height, paint);
        }

        public void DrawEllipse(float x, float y, float width, float height, SKColor color)
        {
            SKPaint paint = new SKPaint
            {
                Color = color,
                IsAntialias = _antiAlias,
                Style = SKPaintStyle.Fill
            };
            _canvas.DrawOval(x, y, width / 2, height / 2, paint);
        }

        public void DrawText(float x, float y,  string text, SKPaint paint) 
        {
            _canvas.DrawText(text, x, y, paint);
        }

        // ********************************************************************************************

        public SKPaint GetTextPaint(RSFont font, SKColor color)
        {
            // create font style
            SKFontStyleWeight weight = (font.Bold == true) ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal;
            SKFontStyleSlant slant = (font.Italic == true) ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright;
            SKFontStyle style = new SKFontStyle(weight, SKFontStyleWidth.Normal, slant);

            // Create an SKTypeface object for the font
            SKTypeface typeface = SKTypeface.FromFamilyName(font.Name, style);

            // Create an SKPaint object to specify the drawing properties
            SKPaint paint = new SKPaint
            {
                Typeface = typeface,
                TextSize = font.Size,
                Color = color,
                TextAlign = SKTextAlign.Left,
                IsAntialias = true,
            };

            return paint;
        }

        public void Clear()
        { 
            _canvas.Clear(_color);
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }
}
