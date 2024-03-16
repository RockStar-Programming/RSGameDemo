
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

namespace Rockstar._Types
{
    // Defines the overall transformation origin for the game engine
    // NOTE:
    // Due to anchor calculations, UpperLeft mapping is far from "intuitive"
    public enum RSTransformationOrigin
    {
        UpperLeft,
        LowerLeft
    }

    public sealed class RSTransformation
    {
        // ********************************************************************************************
        // RSTransformation handles data and transformations 
        //
        // See _readme.txt
        // NOTE:
        // 

        // ********************************************************************************************
        // Constructors

        public static RSTransformation CreateWithData(params object[] data)
        {
            RSTransformation result = new RSTransformation();
            result.InitWithData(data);
            return result;
        }

        private RSTransformation()
        {
        }

        private void InitWithData(params object[] data)
        {
            if ((data.Length > 0) && (data[0] is RSTransformation transformation))
            {
                CopyFrom(transformation);
            }
            else
            {
                // initialise data
                //
                Position = (data.Length > 0) && (data[0] is SKPoint position) ? position : new SKPoint();
                Size = (data.Length > 1) && (data[1] is SKSize size) ? size : DEFAULT_SIZE;
                Scale = (data.Length > 2) && (data[2] is SKPoint scale) ? scale : DEFAULT_SCALE;
                Rotation = (data.Length > 3) && (data[3] is float rotation) ? rotation : 0.0f;
                Anchor = (data.Length > 4) && (data[4] is SKPoint anchor) ? anchor : DEFAULT_ANCHOR;
                Color = (data.Length > 5) && (data[5] is SKColor color) ? color : DEFAULT_COLOR;

                // always reset
                // 
                Visible = true;
                Altitude = 0;
                Origin = RSTransformationOrigin.LowerLeft;
            }
        }

        // ********************************************************************************************
        // Class Properties

        public static string POSITION = "Transformation.Position";
        public static string ALTITUDE = "Transformation.Altitude";
        public static string SIZE = "Transformation.Size";
        public static string ANCHOR = "Transformation.Anchor";
        public static string ROTATION = "Transformation.Rotation";
        public static string SCALE = "Transformation.Scale";
        public static string COLOR = "Transformation.Color";
        public static string ALPHA = "Transformation.Alpha";

        public static SKSize DEFAULT_SIZE = new SKSize(100, 100);
        public static SKPoint DEFAULT_SCALE = new SKPoint(1.0f, 1.0f);
        public static SKPoint DEFAULT_ANCHOR = new SKPoint(0.5f, 0.5f);
        public static SKColor DEFAULT_COLOR = SKColors.White;

        // ********************************************************************************************
        // Properties

        public bool Visible { get; set; }
        public SKPoint Position { get; set; }
        public float Altitude { get; set; }
        public SKSize Size { get; set; }
        public SKPoint Anchor { get; set; }
        public float Rotation { get; set; }
        public SKPoint Scale { get; set; }
        public SKColor Color { get; set; }
        public float Alpha { get { return GetAlpha(); } set { SetAlpha(value); } }
        public RSTransformationOrigin Origin { set; get; }
        public SKMatrix Matrix { get { return CalculateMatrix(); } }

        // ********************************************************************************************
        // Internal Data

        private SKMatrix _matrix;

        // ********************************************************************************************
        // Methods

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        private void CopyFrom(RSTransformation transformation)
        {
            Position = transformation.Position;
            Altitude = transformation.Altitude;
            Size = transformation.Size;
            Scale = transformation.Scale;
            Rotation = transformation.Rotation;
            Anchor = transformation.Anchor;
            Origin = transformation.Origin;
        }

        private SKMatrix CalculateMatrix()
        {
            SKPoint pos = Position;
            float rotation = Rotation;

            if (Origin == RSTransformationOrigin.LowerLeft) pos.Y = -pos.Y;
            _matrix = SKMatrix.CreateRotation((float)Math.PI * rotation / 180.0f, pos.X, pos.Y);
            _matrix = SKMatrix.Concat(_matrix, SKMatrix.CreateTranslation(pos.X, pos.Y));
            _matrix = SKMatrix.Concat(_matrix, SKMatrix.CreateScale(Scale.X, Scale.Y));
            
            return _matrix;
        }

        private float GetAlpha()
        {
            return (Color.Alpha / 255.0f).ClampNormalised();
        }

        private void SetAlpha(float alpha)
        {
            Color = new SKColor(
                Color.Red,
                Color.Green,
                Color.Blue,
                (255.0f * alpha).ClampByte());
        }

        // ********************************************************************************************

    }
}
