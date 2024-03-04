
using SkiaSharp;
using System.Numerics;

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
                Position = (data.Length > 0) && (data[0] is Vector2 position) ? position : new Vector2();
                Size = (data.Length > 1) && (data[1] is SKSize size) ? size : DEFAULT_SIZE;
                Scale = (data.Length > 2) && (data[2] is Vector2 scale) ? scale : DEFAULT_SCALE;
                Rotation = (data.Length > 3) && (data[3] is float rotation) ? rotation : 0.0f;
                Anchor = (data.Length > 4) && (data[4] is Vector2 anchor) ? anchor : DEFAULT_ANCHOR;
                Color = (data.Length > 5) && (data[5] is SKColor color) ? color : SKColors.White;

                // always reset
                // 
                Visible = true;
                Z = 0;
                Origin = RSTransformationOrigin.LowerLeft;
            }
        }

        // ********************************************************************************************
        // Class Properties

        public static string POSITION = "Transformation.Position";
        public static string SIZE = "Transformation.Size";
        public static string ANCHOR = "Transformation.Anchor";
        public static string ROTATION = "Transformation.Rotation";
        public static string SCALE = "Transformation.Scale";

        public static SKSize DEFAULT_SIZE = new SKSize(100, 100);
        public static Vector2 DEFAULT_SCALE = new Vector2(1.0f, 1.0f);
        public static Vector2 DEFAULT_ANCHOR = new Vector2(0.5f, 0.5f);

        // ********************************************************************************************
        // Properties

        public bool Visible { get; set; }
        public Vector2 Position { get; set; }
        public float Z { get; set; }
        public SKSize Size { get; set; }
        public Vector2 Anchor { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; }
        public SKColor Color { get; set; }
        public RSTransformationOrigin Origin { set; get; }
        public Matrix3x2 Matrix { get { return CalculateMatrix(); } }

        // ********************************************************************************************
        // Internal Data

        private Matrix3x2 _matrix;

        // ********************************************************************************************
        // Methods

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        private void CopyFrom(RSTransformation transformation)
        {
            Position = transformation.Position;
            Z = transformation.Z;
            Size = transformation.Size;
            Scale = transformation.Scale;
            Rotation = transformation.Rotation;
            Anchor = transformation.Anchor;
            Origin = transformation.Origin;
        }

        private Matrix3x2 CalculateMatrix()
        {
            Vector2 pos = Position;
            float rotation = Rotation;

            if (Origin == RSTransformationOrigin.LowerLeft) pos.Y = -pos.Y;
            _matrix = Matrix3x2.CreateRotation((float)Math.PI * rotation / 180.0f, pos);
            _matrix = Matrix3x2.CreateTranslation(pos.X, pos.Y) * _matrix;
            _matrix = Matrix3x2.CreateScale(Scale.X, Scale.Y) * _matrix;
            return _matrix;
        }

        // ********************************************************************************************

    }
}
