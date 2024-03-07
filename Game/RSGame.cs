using SkiaSharp;

using Rockstar._Types;
using Rockstar._CodecJson;
using Rockstar._CoreGame;
using Rockstar._Dictionary;
using Rockstar._Event;
using Rockstar._Nodes;
using Rockstar._CoreMouseButton;
using Rockstar._NodeList;
using System.Diagnostics;

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

        RSNodeSprite? _cat;
        
        RSNodeSolid _ellipse;
        RSNodeSolid _square;
        RSNodeSurface _surface;
        RSNodeString _loadScene;

        // ********************************************************************************************
        // Methods

        public override void Initialise(SKSize size)
        {
            LoadScene(size);

            _mouse.AddHandler(RSMouseButton.Left, RSMouseEvent.OnPressed, OnLeftMouseEvent);
            _mouse.AddHandler(RSMouseButton.Left, RSMouseEvent.OnReleased, OnLeftMouseEvent);

            _mouse.AddHandler(RSMouseButton.Right, RSMouseEvent.OnAll, OnRightMouseEvent);

            _mouse.AddHandler(RSMouseButton.Left, RSMouseEvent.OnPressed, OnLeftMouseAddPhysics);
            _mouse.AddHandler(RSMouseButton.Right, RSMouseEvent.OnPressed, OnRightMouseAddPhysics);
        }

        public override void Resize(SKSize size)
        {

        }

        public override void Update(long interval)
        {
            // _ellipse.Transformation.Rotation += 0.1f;


        }

        // ********************************************************************************************
        // Event Handlers

        public void OnLeftMouseEvent(object sender, RSEventArgs argument)
        {
            _cat.SetCurrentFrame(_cat.CurrentFrame + 1);

            if (argument.Data is SKPoint position)
            {
                if (argument.Type is RSMouseEvent.OnPressed)
                {
                    RSNodeList nodeList = _scene.GetHitList(position);
                    if (nodeList.Contains(_loadScene) == true)
                    {
                        LoadScene(_scene.Transformation.Size);
                    }
                }
            }
        }

        public void OnRightMouseEvent(object sender, RSEventArgs argument)
        {
            _surface.Transformation.Rotation += 1;

            if (argument.Data is SKPoint position)
            {
                if (argument.Type is RSMouseEvent.OnReleased)
                {
                    _debugNodeList.Clear();
                }
                else
                {
                    _debugNodeList = _scene.GetHitList(position);
                }
            }
        }

        public void OnRightMouseAddPhysics(object sender, RSEventArgs argument)
        {
            if (argument.Data is SKPoint position)
            {
                position = _scene.LocalPosition(position);

                RSNodeSolid solid = RSNodeSolid.CreateEllipse(position, new SKSize(25, 25), SKColors.Cyan);
                _scene.AddChild(solid);
                _physics.AddDynamicNode(solid);
            }    
        }

        public void OnLeftMouseAddPhysics(object sender, RSEventArgs argument)
        {
            if (argument.Data is SKPoint position)
            {
                position = _scene.LocalPosition(position);

                RSNodeSolid solid = RSNodeSolid.CreateRectangle(position, new SKSize(25, 25), SKColors.Magenta);
                _scene.AddChild(solid);
                _physics.AddDynamicNode(solid);
            }
        }

        // ********************************************************************************************
        // Internal Methods

        private void LoadScene(SKSize size)
        {
            RSNode.RemoveChildren(_scene);

            _scene.Transformation.Size = size;
            _physics.Reset(_scene);

            // RSDictionary _setup = RSCodecJson.CreateDictionaryWithFilePath("Assets/SeikoSpringDrive.json");

            //RSNodeSolid node = RSNodeSolid.CreateRectangle(new SKPoint(100, 50), new SKSize(100, 60), SKColors.Red);
            //node.Transformation.Anchor = new SKPoint(0.5f, 0.0f);
            //// node.Transformation.Z = 10;
            //// node.Transformation.Rotation = 15;

            //RSNodeSolid leftEar = RSNodeSolid.CreateRectangle(new SKPoint(-50, 60), new SKSize(40, 40), SKColors.Green);
            //// leftEar.Transformation.Z = 15;
            //leftEar.Transformation.Rotation = -15;
            //leftEar.Transformation.Scale = new SKPoint(1.5f, 1.5f);
            //node.AddChild(leftEar);

            //RSNodeSolid rightEar = RSNodeSolid.CreateRectangle(new SKPoint(50, 60), new SKSize(40, 40), SKColors.Green);
            //// rightEar.Transformation.Anchor = new SKPoint();
            //rightEar.Transformation.Rotation = 15;
            //rightEar.Transformation.Scale = new SKPoint(0.5f, 0.5f);
            //node.AddChild(rightEar);

            //RSNodeString text = RSNodeString.CreateString(new SKPoint(0, 0), "Holy World", RSFont.Create());
            //// text.Transformation.Z = 20;
            //rightEar.AddChild(text);

            //_scene.AddChild(node);

            // _cat = RSNodeSprite.CreateWithFileAndSize(new SKPoint(400, 200), new SKSize(256, 219) ,"Assets/blue_cat_simple.png");
            _cat = RSNodeSprite.CreateWithFileAndJson(new SKPoint(500, 200), "Assets/blue_cat.png");
            // _cat.Transformation.Rotation = 45;
            // _cat.Transformation.Scale = new SKPoint(1.5f, 1.0f);
            _cat.Transformation.Anchor = new SKPoint(0.5f, 0.0f);
            _scene.AddChild(_cat);

            _cat.AddChild(RSNodeSolid.CreateEllipse(new SKPoint(0, 0), new SKSize(10, 10), SKColors.Yellow));

            RSNodeString text = RSNodeString.CreateString(new SKPoint(500, 420), "Click to Animate", RSFont.Create());
            _scene.AddChild(text);
            //

            _surface = RSNodeSurface.CreateWithSize(new SKPoint(200, 200), new SKSize(250, 250));
            _surface.Transformation.Anchor = new SKPoint(0.5f, 0.0f);
            _surface.Transformation.Rotation = -30;
            _surface.Color = new SKColor(255, 0, 0, 32);
            _scene.AddChild(_surface);
            _physics.AddStaticNode(_surface);


            _square = RSNodeSolid.CreateRectangle(new SKPoint(0, 50), new SKSize(100, 100), SKColors.Aqua);
            _square.Transformation.Rotation = 30;
            _surface.AddChild(_square);

            ////_ellipse = RSNodeSolid.CreateEllipse(SKPoint.Empty, new SKSize(50, 100), SKColors.Yellow);
            ////_surface.AddChild(_ellipse);
            ///

            _loadScene = RSNodeString.CreateString(new SKPoint(50, 50), "Load Scene", RSFont.Create());
            _scene.AddChild(_loadScene);
        }


        // ********************************************************************************************
    }
}
