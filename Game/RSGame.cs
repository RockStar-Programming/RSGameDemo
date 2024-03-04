using SkiaSharp;
using System.Numerics;

using Rockstar._Types;
using Rockstar._CodecJson;
using Rockstar._CoreGame;
using Rockstar._Dictionary;
using Rockstar._NodeList;
using Rockstar._Event;

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

        RSNodeAnimation _fox;

        // ********************************************************************************************
        // Methods

        public override void Initialise(SKSize size)
        {
            RSDictionary _setup = RSCodecJson.CreateDictionaryWithFilePath("Assets/SeikoSpringDrive.json");

            RSNodeSolid node = RSNodeSolid.CreateRectangle(new Vector2(100, 50), new SKSize(100, 60), SKColors.Red);
            node.Transformation.Anchor = new Vector2(0.5f, 0.0f);
            // node.Transformation.Z = 10;
            // node.Transformation.Rotation = 15;

            RSNodeSolid leftEar = RSNodeSolid.CreateRectangle(new Vector2(-50, 60), new SKSize(40, 40), SKColors.Green);
            // leftEar.Transformation.Z = 15;
            // leftEar.Transformation.Rotation = -15;
            node.AddChild(leftEar);

            RSNodeSolid rightEar = RSNodeSolid.CreateRectangle(new Vector2(50, 60), new SKSize(40, 40), SKColors.Green);
            // rightEar.Transformation.Anchor = new Vector2();
            // rightEar.Transformation.Rotation = 15;
            node.AddChild(rightEar);

            RSNodeString text = RSNodeString.CreateString(new Vector2(0, 0), "Holy World", RSFont.Create());
            // text.Transformation.Z = 20;
            rightEar.AddChild(text);

            _scene.AddChild(node);

            _fox = RSNodeAnimation.CreateWithFile(new Vector2(400, 300), new SKSize(256, 219) ,"Assets/blue_fox.png");
            _fox.AnimationInterval = 100;
            _scene.AddChild(_fox);

            _scene.AddChild(RSNodeString.CreateString(new Vector2(400, 180), "Click to Animate", RSFont.Create()));

            _mouse.AddHandler(_CoreMouseButton.RSMouseButton.Left, _CoreMouseButton.RSMouseEvent.OnPressed, OnLeftMouseEvent);

        }

        public override void Resize(SKSize size)
        {

        }

        public override void Update(long interval)
        {


        }

        // ********************************************************************************************
        // Event Handlers

        public void OnLeftMouseEvent(object sender, RSEventArgs argument)
        {
            //if (_fox.Running == true)
            //{
            //    _fox.StopAtFrame(2);
            //    // _fox.Stop();
            //}
            //else
            //{
            //    _fox.Start();
            //}
            _fox.Play(3, 2);
        }

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }
}
