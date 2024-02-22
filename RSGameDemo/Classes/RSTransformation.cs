using Rockstar.Types;
using System;
using System.Numerics;
using Windows.UI;

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

namespace Rockstar.Transformation
{
    public enum RSSceneOrigin
    {
        UpperLeft,
        LowerLeft
    }

    public class RSTransformation
    {
        // ********************************************************************************************
        // RSTransformation handles data and transformations 
        //
        //

        // ********************************************************************************************
        // Constructors

        public static RSTransformation CreateWithData(params object[] data)
        {
            RSTransformation result = new RSTransformation();
            result.InitWithData(data);
            return result;
        }

        protected void InitWithData(params object[] data)
        {
            if ((data.Length > 0) && (data[0] is RSTransformation transformation))
            {
                CopyFrom(transformation);
            }
            else
            {
                // initialise data 
                Position = (data.Length > 0) && (data[0] is RSVector2 position) ? position : new RSVector2();
                Size = (data.Length > 1) && (data[1] is RSSize size) ? size : DEFAULT_SIZE;
                Scale = (data.Length > 2) && (data[2] is RSVector2 scale) ? scale : DEFAULT_SCALE;
                Rotation = (data.Length > 3) && (data[3] is float rotation) ? rotation : 0.0f;
                Anchor = (data.Length > 4) && (data[4] is RSVector2 anchor) ? anchor : DEFAULT_ANCHOR;
                Color = (data.Length > 5) && (data[5] is Color color) ? color : Colors.White;
            }
            _matrix = Matrix3x2.Identity;
        }

        // ********************************************************************************************
        // Properties

        public const string POSITION = "Transformation.Position";
        public const string SIZE = "Transformation.Size";
        public const string ANCHOR = "Transformation.Anchor";
        public const string ROTATION = "Transformation.Rotation";
        public const string SCALE = "Transformation.Scale";

        public static RSSceneOrigin Origin = RSSceneOrigin.UpperLeft;

        public readonly RSSize DEFAULT_SIZE = new RSSize(100, 100);
        public readonly RSVector2 DEFAULT_SCALE = new RSVector2(1.0f, 1.0f);
        public readonly RSVector2 DEFAULT_ANCHOR = new RSVector2(0.5f, 0.5f);

        public RSVector2 Position { get; set; }
        public RSSize Size { get; set; }
        public RSVector2 Anchor { get; set; }
        public float Rotation { get; set; }
        public RSVector2 Scale { get; set; }
        public Color Color { get; set; }
        public Matrix3x2 Matrix { get { return _matrix; } }

        // ********************************************************************************************
        // Internal Data

        private Matrix3x2 _matrix;

        // ********************************************************************************************
        // Methods

        public void CopyFrom(RSTransformation transformation)
        { 
            Position = transformation.Position;
            Size = transformation.Size;
            Scale = transformation.Scale;
            Rotation = transformation.Rotation;
            Anchor = transformation.Anchor;
        }

        public void CopyTo(RSTransformation transformation)
        {
            transformation.Position = Position;
            transformation.Size = Size;
            transformation.Scale = Scale;
            transformation.Rotation = Rotation;
            transformation.Anchor = Anchor;
        }

        public Matrix3x2 GetTransform()
        {
            RSVector2 pos = Position;
            float rotation = Rotation;

            if (Origin == RSSceneOrigin.LowerLeft) pos.Y = -pos.Y;
            _matrix = Matrix3x2.CreateRotation((float)Math.PI * rotation / 180.0f, pos.Vector2());
            _matrix = Matrix3x2.CreateTranslation(pos.X, pos.Y) * _matrix;
            _matrix = Matrix3x2.CreateScale(Scale.X, Scale.Y) * _matrix;
            return _matrix;
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        private RSTransformation()
        { 
        }

        // ********************************************************************************************

    }
}
