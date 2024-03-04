
using SkiaSharp;

using Rockstar._CoreFile;
using Rockstar._RenderSurface;
using System.Security.Policy;
using Rockstar._SpriteSheet;

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

namespace Rockstar._NodeList
{
    public class RSNodeSprite : RSNode
    {
        // ********************************************************************************************
        // Implements bitmap based nodes
        // 

        // ********************************************************************************************
        // Constructors

        public static RSNodeSprite CreateWithFile(SKPoint position, string filePath)
        {
            return new RSNodeSprite(position, filePath);
        }

        public static RSNodeSprite CreateWithFileAndSize(SKPoint position, SKSize size,string filePath)
        {
            return new RSNodeSprite(position, size, filePath);
        }

        private RSNodeSprite(SKPoint position, string filePath)
        {
            _sheet = RSSpriteSheet.CreateFromFile(filePath);
            InitWithData(position, new SKSize(_sheet.Bitmap.Width, _sheet.Bitmap.Height));
            _currentFrame = 0;
        }

        private RSNodeSprite(SKPoint position, SKSize size, string filePath)
        {
            _sheet = RSSpriteSheet.CreateFromFileAndSize(filePath, size);
            InitWithData(position, size);
            _currentFrame = 0;
        }


        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        public RSSpriteSheet Sheet { get { return _sheet; } }
        public int CurrentFrame { get { return _currentFrame; } }

        // ********************************************************************************************
        // Internal Data

        private RSSpriteSheet _sheet;
        private int _currentFrame;

        // ********************************************************************************************
        // Methods

        public override bool PointInside(SKPoint screenPosition)
        {
            return PointInsizeRectangle(screenPosition);
        }

        public override void Render(RSRenderSurface surface)
        {
            SKPoint upperLeft = new SKPoint(
                -_transformation.Size.Width * _transformation.Anchor.X,
                -_transformation.Size.Height * (1.0f - _transformation.Anchor.Y));
            surface.DrawBitmap(upperLeft, _transformation.Size, _sheet.Frame(_currentFrame), _sheet.Bitmap);
        }

        public void SetCurrentFrame(int index)
        {
            _currentFrame = index % _sheet.FrameCount;
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }
}
