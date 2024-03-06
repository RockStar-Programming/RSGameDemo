
using SkiaSharp;

using Rockstar._Dictionary;

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

namespace Rockstar._Types
{
    public class RSSpriteFrame
    {
        // ********************************************************************************************
        // A sprite frame defines an area within a bitmap
        //
        // See _readme.txt
        //
        // sprite frames currently only supports TexturePacker style  rotated sprites

        // ********************************************************************************************
        // Constructors

        public static RSSpriteFrame Create(SKPoint position, SKSize size)
        { 
            return new RSSpriteFrame(position, size);
        }

        public static RSSpriteFrame CreateFromTexturePacker(RSDictionary setup)
        {
            return new RSSpriteFrame(setup);
        }

        // ********************************************************************************************

        private RSSpriteFrame(SKPoint position, SKSize size)
        {
            Size = size;
            SheetRect = new SKRect(position.X, position.Y, position.X + size.Width, position.Y + size.Height);
            Offset = SKPoint.Empty;
        }

        private RSSpriteFrame(RSDictionary setup)
        {
            RSDictionary frame = setup.GetDictionary(TP_FRAME);
            RSDictionary data;

            Rotation = (setup.GetBool(TP_ROTATED, false) == true) ? 90 : 0;

            data = setup.GetDictionary(TP_SOURCE_SIZE);
            Size = new SKSize(data.GetFloat(TP_WIDTH, 0), data.GetFloat(TP_HEIGHT, 0));

            SKSize frameSize = new SKSize(frame.GetFloat(TP_WIDTH, 0), frame.GetFloat(TP_HEIGHT, 0));
            if (Rotation != 0)
            {
                (frameSize.Width, frameSize.Height) = (frameSize.Height, frameSize.Width);
            }

            SheetRect = new SKRect
            {
                Left = frame.GetFloat(TP_X, 0),
                Top = frame.GetFloat(TP_Y, 0),
                Right = frame.GetFloat(TP_X, 0) + frameSize.Width,
                Bottom = frame.GetFloat(TP_Y, 0) + frameSize.Height
            };

            data = setup.GetDictionary(TP_SPRITE_SOURCE_SIZE);
            Offset = new SKPoint(data.GetFloat(TP_X, 0), data.GetFloat(TP_Y, 0));
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties
        public SKRect SheetRect { get; }
        public SKSize Size { get; }
        public float Rotation { get; }
        public SKPoint Offset { get; }

        // ********************************************************************************************
        // Internal Data

        // Texture Packer encoding keys
        //
        public const string TP_FRAMES = "frames";
        public const string TP_FRAME = "frame";
        public const string TP_X = "x";
        public const string TP_Y = "y";
        public const string TP_WIDTH = "w";
        public const string TP_HEIGHT = "h";
        public const string TP_ROTATED = "rotated";
        public const string TP_SOURCE_SIZE = "sourcesize";
        public const string TP_SPRITE_SOURCE_SIZE = "spritesourcesize";

        // ********************************************************************************************
        // Methods

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods



        // ********************************************************************************************
    }
}
