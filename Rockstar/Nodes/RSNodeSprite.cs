
using SkiaSharp;

using Rockstar._RenderSurface;
using Rockstar._SpriteSheet;
using Rockstar._SpriteFrame;
using Rockstar._Types;
using Rockstar._Array;
using Box2D.NetStandard.Dynamics.World;

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
    public class RSNodeSprite : RSNode
    {
        // ********************************************************************************************
        // Implements bitmap based nodes
        // 
        // IMPORTANT:
        // For animations, sprite frames should always have the same size
        // if not, anything but anchor(0.5, 0.5) might result in jumpy animations

        // ********************************************************************************************
        // Constructors

        public static RSNodeSprite CreateWithFile(string filePath)
        {
            return new RSNodeSprite(filePath);
        }

        public static RSNodeSprite CreateWithFileAndSize(SKSize size,string filePath)
        {
            return new RSNodeSprite(size, filePath);
        }

        public static RSNodeSprite CreateWithFileAndJson(string filePath, string? jsonPath = null)
        {
            return new RSNodeSprite(filePath, jsonPath);
        }

        public static RSNodeSprite CreateWithParams(params object[] data)
        {
            if ((data.Length >= 1) && (data[0] is string fileName_0))
            {
                return CreateWithFileAndJson(fileName_0);
            }
            else if ((data.Length >= 2) && (data[0] is RSArray position) && (data[1] is string fileName_1))
            { 
                return CreateWithFileAndSize(position.ToSize(), fileName_1);
            }
            else if ((data.Length >= 2) && (data[0] is string fileName_2) && (data[1] is string filename_3))
            {
                return CreateWithFileAndJson(fileName_2, filename_3);
            }
            // oops
            return CreateWithFile("");
        }

        // ********************************************************************************************

        protected RSNodeSprite(string filePath)
        {
            _sheet = RSSpriteSheet.CreateFromFile(filePath);
            InitWithData(SKPoint.Empty, new SKSize(_sheet.Image.Width, _sheet.Image.Height));
            _currentFrame = 0;
        }

        protected RSNodeSprite(SKSize size, string filePath)
        {
            _sheet = RSSpriteSheet.CreateFromFileAndSize(filePath, size);
            InitWithData(SKPoint.Empty, size);
            _currentFrame = 0;
        }

        protected RSNodeSprite(string filePath, string? jsonPath)
        { 
            _sheet = RSSpriteSheet.CreateFromFileAndJson(filePath, jsonPath);
            InitWithData(SKPoint.Empty, _sheet.Frame(0).Size);
            _currentFrame = 0;
        }
        protected RSNodeSprite()
        { 
            throw new NotImplementedException();
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        public RSSpriteSheet Sheet { get { return _sheet; } }
        public int CurrentFrame { get { return _currentFrame; } }

        // ********************************************************************************************
        // Internal Data

        private byte ALPHA_THRESHOLD = 5;

        private RSSpriteSheet _sheet;
        private int _currentFrame;

        // ********************************************************************************************
        // Methods

        public override bool PointInside(SKPoint screenPosition)
        {
            if (_touchMode != RSNodeTouchMode.Accurate) return base.PointInside(screenPosition);

            return false;

            //// check if inside rectangle
            //if (PointInsizeRectangle(screenPosition) == false) return false;

            //RSSpriteFrame frame = _sheet.Frame(_currentFrame);

            //// calculate texture coordinate
            //SKPoint point = LocalPosition(screenPosition);
            //SKPoint textureCoordinate = new SKPoint(point.X + (frame.Size.Width / 2.0f), -point.Y + (frame.Size.Height / 2.0f));
            //textureCoordinate = textureCoordinate - frame.Offset;

            //if (frame.Rotation != 0)
            //{
            //    SKPoint rotationCenter = new SKPoint(frame.SheetRect.Width / 2.0f, frame.SheetRect.Width / 2.0f);

            //    textureCoordinate = textureCoordinate - rotationCenter;
            //    textureCoordinate = textureCoordinate.Rotate(-frame.Rotation);
            //    textureCoordinate = textureCoordinate + rotationCenter;
            //}

            //textureCoordinate = textureCoordinate + new SKPoint(frame.SheetRect.Left, frame.SheetRect.Top);
            //if ((int)textureCoordinate.X < 0) return false;
            //if ((int)textureCoordinate.X >= _sheet.Image.Width) return false;
            //if ((int)textureCoordinate.Y < 0) return false;
            //if ((int)textureCoordinate.Y >= _sheet.Image.Height) return false;

            //SKColor color = _sheet.Image.GetPixel((int)textureCoordinate.X, (int)textureCoordinate.Y);
            //return (color.Alpha > ALPHA_THRESHOLD);
        }

        public override void Render(RSRenderSurface surface)
        {
            RSSpriteFrame frame = _sheet.Frame(_currentFrame);

            SKPoint upperLeft = new SKPoint(
                 (-_transformation.Size.Width * _transformation.Anchor.X) + frame.Offset.X,
                 (-_transformation.Size.Height * (1.0f - _transformation.Anchor.Y)) + frame.Offset.Y);

            surface.DrawImage(upperLeft, frame, _sheet.Image, _transformation.Color);
        }

        public void SetCurrentFrame(int index)
        {
            _currentFrame = index % _sheet.FrameCount;
            // set content size to the currently selected frame
            _transformation.Size = _sheet.Frame(_currentFrame).Size;
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }
}
