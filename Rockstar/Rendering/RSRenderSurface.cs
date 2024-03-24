
using SkiaSharp;

using Rockstar._Types;
using Rockstar._SpriteFrame;

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
    public class RSRenderSurface : IDisposable
    {
        // ********************************************************************************************
        // Render surface encapsulates an SKCanvas for rendering 
        //
        // Any drawing and drawing support should be performed here
        // Do not access SKCanvas from other classes

        // ********************************************************************************************
        // Constructors

        public static RSRenderSurface Create(SKCanvas canvas, RSTransformationOrigin origin = RSTransformationOrigin.LowerLeft)
        {
            return new RSRenderSurface(canvas, origin, false);
        }

        public static RSRenderSurface CreateOffScreen(SKCanvas canvas, RSTransformationOrigin origin = RSTransformationOrigin.LowerLeft)
        {
            return new RSRenderSurface(canvas, origin, true);
        }

        // ********************************************************************************************

        private RSRenderSurface(SKCanvas canvas, RSTransformationOrigin origin, bool offScreen)
        {
            _canvas = canvas;
            _size = new SKSize(_canvas.DeviceClipBounds.Width, _canvas.DeviceClipBounds.Height);
            _origin = origin;
            _matrix = SKMatrix.Identity;
            _offScreen = offScreen;
            SetRenderQuality(SKFilterQuality.Low);
        }

        public void Dispose()
        {
            _canvas.Dispose();
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        public SKCanvas Canvas { get { return _canvas; } }
        public SKSize Size { get { return _size; } }
        public RSTransformationOrigin Origin { get { return _origin; } }
        public SKMatrix Matrix { get { return _matrix; } }
        public bool AntiAlias { get { return _antiAlias; } }
        public SKFilterQuality Quality { get { return _quality; } }
        public bool OffScreen { get { return _offScreen; } }

        // ********************************************************************************************
        // Internal Data

        private SKCanvas _canvas;
        private SKSize _size;
        private RSTransformationOrigin _origin;
        private SKMatrix _matrix;
        private bool _antiAlias;
        private SKFilterQuality _quality;
        private bool _offScreen;

        // ********************************************************************************************
        // Methods

        public void SetMatrix(SKMatrix matrix)
        {
            _matrix = matrix;
        }

        public SKMatrix MultiplyMatrix(SKMatrix matrix)
        {
            _matrix = SKMatrix.Concat(_matrix, matrix);
            return _matrix;
        }

        public void SetCanvasMatrix(SKMatrix transformation)
        {
            _canvas.SetMatrix(transformation);
        }

        // ********************************************************************************************
        // Draw Methods

        public void DrawBox(SKPoint position, SKSize size, SKColor color, float line)
        {
            SKPaint paint = new SKPaint
            {
                Color = color,
                FilterQuality = _quality,
                IsAntialias = _antiAlias,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = line
            };
            _canvas.DrawRect(position.X, position.Y, size.Width, size.Height, paint);
        }

        public void DrawRectangle(SKPoint position, SKSize size, SKColor color)
        {
            SKPaint paint = new SKPaint
            {
                Color = color,
                FilterQuality = _quality,
                IsAntialias = _antiAlias,
                Style = SKPaintStyle.Fill
            };
            _canvas.DrawRect(position.X, position.Y, size.Width, size.Height, paint);
        }

        public void DrawEllipse(SKPoint position, SKSize size, SKColor color)
        {
            SKPaint paint = new SKPaint
            {
                Color = color,
                FilterQuality = _quality,
                IsAntialias = _antiAlias,
                Style = SKPaintStyle.Fill
            };
            _canvas.DrawOval(position.X, position.Y, size.Width / 2, size.Height / 2, paint);
        }

        public void DrawText(SKPoint position,  string text, SKPaint paint) 
        {
            paint.FilterQuality = _quality;
            paint.IsAntialias = _antiAlias;
            _canvas.DrawText(text, position.X, position.Y, paint);
        }

        public void DrawBitmap(SKPoint position, RSSpriteFrame frame, SKBitmap bitmap, SKColor color)
        {
            SKPaint paint = new SKPaint
            {
                FilterQuality = _quality,
                IsAntialias = _antiAlias,
                ColorFilter = SKColorFilter.CreateBlendMode(
                    color, // Tint color
                    SKBlendMode.Modulate 
                )
            };

            SKSize renderSize = new SKSize(frame.SheetRect.Right - frame.SheetRect.Left, frame.SheetRect.Bottom - frame.SheetRect.Top);
            SKRect destination = new SKRect(position.X, position.Y, position.X + renderSize.Width, position.Y + renderSize.Height);

            if (frame.Rotation != 0)
            {
                // this calculates the rotation point on the screen
                SKPoint offset = new SKPoint(position.X + (frame.SheetRect.Width / 2), position.Y + (frame.SheetRect.Width / 2));

                // translate, rotate and translate back
                _canvas.Translate(offset.X, offset.Y);
                _canvas.RotateDegrees(-frame.Rotation);
                _canvas.Translate(-offset.X, -offset.Y);
            }

            _canvas.DrawBitmap(bitmap, frame.SheetRect, destination, paint);
        }

        public void DrawImage(SKPoint position, RSSpriteFrame frame, SKImage image, SKColor color)
        {
            SKPaint paint = new SKPaint
            {
                FilterQuality = _quality,
                IsAntialias = _antiAlias,
                ColorFilter = SKColorFilter.CreateBlendMode(
                    color, // Tint color
                    SKBlendMode.Modulate
                )
            };

            SKSize renderSize = new SKSize(frame.SheetRect.Right - frame.SheetRect.Left, frame.SheetRect.Bottom - frame.SheetRect.Top);
            SKRect destination = new SKRect(position.X, position.Y, position.X + renderSize.Width, position.Y + renderSize.Height);

            if (frame.Rotation != 0)
            {
                // this calculates the rotation point on the screen
                SKPoint offset = new SKPoint(position.X + (frame.SheetRect.Width / 2), position.Y + (frame.SheetRect.Width / 2));

                // translate, rotate and translate back
                _canvas.Translate(offset.X, offset.Y);
                _canvas.RotateDegrees(-frame.Rotation);
                _canvas.Translate(-offset.X, -offset.Y);
            }

            _canvas.DrawImage(image, frame.SheetRect, destination, paint);
        }

        // ********************************************************************************************

        public SKPaint GetTextPaint(RSFont font, SKColor color)
        {
            // create font style
            SKFontStyleWeight weight = (font.Bold == true) ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal;
            SKFontStyleSlant slant = (font.Italic == true) ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright;
            SKFontStyle style = new SKFontStyle(weight, SKFontStyleWidth.Normal, slant);

            // Create an SKTypeface RSNode for the font
            SKTypeface typeface = SKTypeface.FromFamilyName(font.Name, style);

            // Create an SKPaint RSNode to specify the drawing properties
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

        public void SetRenderQuality(SKFilterQuality quality)
        {
            _antiAlias = (quality != SKFilterQuality.None);
            _quality = quality;
        }

        public void Clear(SKColor color)
        {
            _canvas.Clear(color);
        }

        //
        // TODO:
        // When any alpha decay, ClearColor must be preserved
        //
        public void Clear(SKBitmap bitmap, SKColor color, Byte alphaDecay)
        {
            if (alphaDecay == 0) return;

            if (alphaDecay == 255)
            { 
                _canvas.Clear(color);
                return;
            }

            SKCanvas canvas = new SKCanvas(bitmap);

            //var colorPaint = new SKPaint
            //{
            //    Color = new SKColor(0, 0, 0, 255)
            //};
            //canvas.DrawRect(new SKRect(0, 0, bitmap.Width, bitmap.Height), colorPaint);

            // Create a paint RSNode for the operation
            var paint = new SKPaint
            {
                Color = SKColors.Black, // Color doesn’t matter, only the alphaClear channel will be used
                BlendMode = SKBlendMode.DstOut // This mode subtracts alphaClear from the destination
            };

            // Create a mask that represents the reduction in alphaClear
            SKBitmap alphaMask = new SKBitmap(bitmap.Width, bitmap.Height);
            using (var maskCanvas = new SKCanvas(alphaMask))
            {
                // Draw with the desired alphaClear reduction
                var maskPaint = new SKPaint
                {
                    Color = new SKColor(0, 0, 0, alphaDecay)
                };
                maskCanvas.DrawRect(new SKRect(0, 0, bitmap.Width, bitmap.Height), maskPaint);
            }

            // Apply the mask to the original canvas
            canvas.DrawBitmap(alphaMask, 0, 0, paint);

            // Dispose of the mask bitmap
            alphaMask.Dispose();
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }
}
