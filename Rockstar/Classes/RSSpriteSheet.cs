
using SkiaSharp;

using Rockstar._CoreFile;
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

namespace Rockstar._SpriteSheet
{
    public class RSSpriteSheet
    {
        // ********************************************************************************************
        // The sprite sheet is the Rockstar implementation of a bitmap (the Sheet property)
        // 
        // It maintains a list of frames
        // these frames can be loaded in different ways
        //
        // 1) just from a file => creates one frame
        // 2) From a file and a size => automatically creates available frames
        // 3) From a file and a json => creates the frames from the json

        // ********************************************************************************************
        // Constructors

        public static RSSpriteSheet CreateFromFile(string filePath)
        { 
            return new RSSpriteSheet(filePath, SKSize.Empty);
        }

        public static RSSpriteSheet CreateFromFileAndSize(string filePath, SKSize size)
        {
            return new RSSpriteSheet(filePath, size);
        }

        public static RSSpriteSheet CreateFromFileAndJson(string filePath, string jsonPath)
        {
            return new RSSpriteSheet(filePath, jsonPath);
        }

        private RSSpriteSheet(string filePath)
        {
            _frameList = new List<RSSpriteFrame>();
            _bitmap = RSCoreFile.ReadAsBitmap(filePath);
        }

        private RSSpriteSheet(string filePath, SKSize size) : this(filePath)
        {
            if (size.IsEmpty == false)
            {
                // automatically create frames
                // frames will be added in rows from upper left corner
                // bitmap can be slightly oversized
                //
                SKPoint pos = SKPoint.Empty;
                while ((pos.Y + size.Height) <= _bitmap.Height)
                {
                    while ((pos.X + size.Width) <= _bitmap.Width)
                    {
                        RSSpriteFrame frame = RSSpriteFrame.Create(pos, size);
                        _frameList.Add(frame);
                        pos.X += size.Width;
                    }
                    pos.X = 0;
                    pos.Y += size.Height;
                }
            }

            // if no frames has been added, create a single frame of the entire bitmap
            if (_frameList.Count == 0)
            {
                size = new SKSize(_bitmap.Width, _bitmap.Height);
                RSSpriteFrame frame = RSSpriteFrame.Create(SKPoint.Empty, size);
                _frameList.Add(frame);
            }
        }

        private RSSpriteSheet(string filePath, string jsonPath) : this(filePath)
        {
            throw new NotImplementedException();
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        public int FrameCount { get { return _frameList.Count; } }
        public SKBitmap Bitmap { get { return _bitmap; } }

        // ********************************************************************************************
        // Internal Data

        private List<RSSpriteFrame> _frameList;
        private SKBitmap _bitmap;

        // ********************************************************************************************
        // Methods

        public RSSpriteFrame Frame(int index)
        {
            return _frameList[index % FrameCount];
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }
}
