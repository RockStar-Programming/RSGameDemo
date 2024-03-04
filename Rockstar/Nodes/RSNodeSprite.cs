
using SkiaSharp;

using Rockstar._CoreFile;
using Rockstar._RenderSurface;

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
    public class RSNodeSprite :RSNode
    {
        // ********************************************************************************************
        // Implements bitmap based nodes
        // 

        // ********************************************************************************************
        // Constructors

        public static RSNodeSprite CreateWithFile(SKPoint position, string filePath)
        { 
            RSNodeSprite result = new RSNodeSprite(position, filePath);

            return result;
        }

        public RSNodeSprite(SKPoint position, string filePath) 
        { 
            _bitmap = RSCoreFile.ReadAsBitmap(filePath);
            InitWithData(position, new SKSize(_bitmap.Width, _bitmap.Height));
        }


        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        // ********************************************************************************************
        // Internal Data

        private SKBitmap _bitmap;

        // ********************************************************************************************
        // Methods

        public override bool PointInside(SKPoint screenPosition)
        {
            return PointInsizeRectangle(screenPosition);
        }

        public override void Render(RSRenderSurface surface)
        {
            SKPoint upperLeft = new SKPoint((float)-_transformation.Size.Width * _transformation.Anchor.X, (float)-_transformation.Size.Height * (1.0f - _transformation.Anchor.Y));
            surface.DrawBitmap(upperLeft.X, upperLeft.Y, _bitmap);
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }
}
