using SkiaSharp;
using System.Numerics;

using Rockstar._Types;
using Rockstar._CodecJson;
using Rockstar._CoreGame;
using Rockstar._Dictionary;
using Rockstar._Nodes;

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

namespace Rockstar._Game
{
    internal class RSGame : RSCoreGame
    {
        // ********************************************************************************************
        // Game template

        // ********************************************************************************************
        // Constructors

        public static RSGame Create()
        {
            return new RSGame();
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        // ********************************************************************************************
        // Internal Data

        // ********************************************************************************************
        // Methods

        public override void Initialise(Size size)
        {
            RSDictionary _setup = RSCodecJson.CreateDictionaryWithFilePath("Assets/SeikoSpringDrive.json");

            RSNodeSolid node = RSNodeSolid.CreateRectangle(new Vector2(100, 50), new Size(100, 60), SKColors.Red);
            node.Transformation.Anchor = new Vector2(0.5f, 0.0f);
            // node.Transformation.Z = 10;
            // node.Transformation.Rotation = 15;

            RSNodeSolid leftEar = RSNodeSolid.CreateRectangle(new Vector2(-50, 60), new Size(40, 40), SKColors.Green);
            // leftEar.Transformation.Z = 15;
            // leftEar.Transformation.Rotation = -15;
            node.AddChild(leftEar);

            RSNodeSolid rightEar = RSNodeSolid.CreateRectangle(new Vector2(50, 60), new Size(40, 40), SKColors.Green);
            // rightEar.Transformation.Anchor = new Vector2();
            // rightEar.Transformation.Rotation = 15;
            node.AddChild(rightEar);

            RSNodeString text = RSNodeString.CreateString("Holy World", new Vector2(0, 0), RSFont.Create());
            // text.Transformation.Z = 20;
            rightEar.AddChild(text);

            _scene.AddChild(node);




        }

        public override void Resize(Size size)
        {
        }

        public override void Update(long interval)
        {
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }
}
