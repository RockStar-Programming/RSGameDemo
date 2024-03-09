
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
    public enum RSNodeSurfaceRenderMode
    {
        Automatic,
        Manual
    }

    public class RSNodeSurface : RSNode
    {
        // ********************************************************************************************
        // A node surface encapsulates off-screen rendering, but works 100% like any other node
        // With a few limitations
        // - Performance will be lower, due to the the off-screen render stage
        // - Children can (unlike any other parent node) NOT extend beyond the basic size of the node.
        //     If they do, they will be clipped
        // - Rendering can be controlled. It can either be:
        //     Automatic, ie done for each frame (default)
        //     Manual, ie only done on demand
        // 
        // A node surface with no children added, and a visible color, will look just like a rectangle
        // Placement of children works identical to any other node, except for the clipping
        // Node surfaces can contain other node surfaces
        // 
        // Node surfaces are used to achieve special effects when rendering. This could be:
        // - Uniform alpha blending on complex geometry (like ex. an animates character)
        // - Multi pass rendering
        // - Accumulative rendering
        // - Many more

        // ********************************************************************************************
        // Constructors

        public static RSNodeSurface CreateWithSize(SKPoint position, SKSize size, RSRenderSurfaceBlendMode blendMode, bool preMultiplyAlpha = true)
        { 
            return new RSNodeSurface(position, size, blendMode, preMultiplyAlpha);
        }

        // ********************************************************************************************

        private RSNodeSurface(SKPoint position, SKSize size, RSRenderSurfaceBlendMode blendMode, bool preMultiplyAlpha)
        {
            InitWithData(position, size);

            if (preMultiplyAlpha == true)
            {
                _bitmap = new SKBitmap((int)size.Width, (int)size.Height);
            }
            else
            {
                // create bitmap without pre-multiplied alpha
                // this makes blended operations look better, at a slight cost of speed
                //
                SKImageInfo imageInfo = new SKImageInfo((int)size.Width, (int)size.Height)
                {
                    ColorType = SKColorType.Rgba8888,
                    AlphaType = SKAlphaType.Unpremul // Specify non-premultiplied alpha
                };
                _bitmap = new SKBitmap(imageInfo);
            }

            _canvas = new SKCanvas(_bitmap);
            _color = SKColors.Transparent;
            _frame = RSSpriteFrame.Create(SKPoint.Empty, size);
            _renderMode = RSNodeSurfaceRenderMode.Automatic;
            _blendMode = blendMode;
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        public SKBitmap Bitmap { get { return _bitmap; } }
        public SKCanvas Canvas { get { return _canvas; } }
        public SKColor Color { get { return _color; } set { _color = value; } }
        public RSNodeSurfaceRenderMode RenderMode { get { return _renderMode; } }
        public RSRenderSurfaceBlendMode BlendMode { get { return _blendMode; } }

        // ********************************************************************************************
        // Internal Data

        private SKBitmap _bitmap;
        private SKCanvas _canvas;
        private SKColor _color;
        private RSSpriteFrame _frame;
        private RSNodeSurfaceRenderMode _renderMode;
        private RSRenderSurfaceBlendMode _blendMode;

        // ********************************************************************************************
        // Methods

        public override bool PointInside(SKPoint screenPosition)
        {
            if (_touchMode != RSNodeTouchMode.Accurate) return base.PointInside(screenPosition);

            // TODO: check alpha in image
            return PointInsizeRectangle(screenPosition);
        }

        public override void Render(RSRenderSurface surface)
        {
            SKPoint upperLeft = new SKPoint(
                 (-_transformation.Size.Width * _transformation.Anchor.X),
                 (-_transformation.Size.Height * (1.0f - _transformation.Anchor.Y)));

            surface.DrawBitmap(upperLeft, _frame, _bitmap);
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }
}
