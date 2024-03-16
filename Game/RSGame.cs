using SkiaSharp;

using Rockstar._SpriteFrame;
using Rockstar._CodecJson;
using Rockstar._CoreGame;
using Rockstar._Dictionary;
using Rockstar._Event;
using Rockstar._Nodes;
using Rockstar._CoreMouseButton;
using Rockstar._NodeList;
using Rockstar._Action;
using Rockstar._RenderSurface;
using Rockstar._Lerp;

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

//
// IMPORTANT
//
// This is where all my test code goes, so do not expect this to be super clean at all times
//


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


        float _catTimer = 0;

        RSNodeSprite? _animal;
        RSNodeSurface? _surface;
        RSNodeSurface? _motionCanvas;
        RSNodeString? _loadScene;

        // ********************************************************************************************
        // Methods

        public override void Initialise(SKSize size)
        {
            LoadScene(size);

            _mouse.AddHandler(RSMouseButton.Left, RSMouseEvent.OnPressed, OnLeftMouseEvent);
            _mouse.AddHandler(RSMouseButton.Left, RSMouseEvent.OnReleased, OnLeftMouseEvent);

            _mouse.AddHandler(RSMouseButton.Right, RSMouseEvent.OnAll, OnRightMouseEvent);

            _mouse.AddHandler(RSMouseButton.Left, RSMouseEvent.OnAll, OnLeftMouseAddPhysics);
            _mouse.AddHandler(RSMouseButton.Right, RSMouseEvent.OnAll, OnRightMouseAddPhysics);
        }

        public override void Resize(SKSize size)
        {

        }

        public override void Update(float interval)
        {
            _catTimer += interval;
            if (_catTimer > 0.1f)
            {
                if (_animal != null) _animal.SetCurrentFrame(_animal.CurrentFrame + 1);
                _catTimer -= 0.1f;
            }
        }

        // ********************************************************************************************
        // Event Handlers

        public void OnLeftMouseEvent(object sender, RSEventArgs argument)
        {

            if (argument.Data is SKPoint position)
            {
                if (argument.Type is RSMouseEvent.OnPressed)
                {
                    _surface.RunAction("test");

                    RSNodeList nodeList = _scene.GetHitList(position);
                    if (nodeList.Contains(_loadScene) == true)
                    {
                        LoadScene(_scene.Transformation.Size);
                    }
                }
                else 
                {
                }
            }
        }

        public void OnRightMouseEvent(object sender, RSEventArgs argument)
        {
            if (argument.Data is SKPoint position)
            {
                if (argument.Type is RSMouseEvent.OnReleased)
                {
                    _debugNodeList.Clear();
                }
                else
                {
                    _surface.RunAction("color_test");
                    _debugNodeList = _scene.GetHitList(position);
                }
            }
        }

        public void OnRightMouseAddPhysics(object sender, RSEventArgs argument)
        {
            if (argument.Data is SKPoint position)
            {
                position = _scene.LocalPosition(position);

                RSNodeSolid solid = RSNodeSolid.CreateEllipse(position, new SKSize(18, 18), SKColors.Cyan);
                _motionCanvas.AddChild(solid);
                _physics.AddDynamicNode(solid, 5.0f, 1.0f, 0.3f, 0.9f);
            }    
        }

        public void OnLeftMouseAddPhysics(object sender, RSEventArgs argument)
        {
            if (argument.Data is SKPoint position)
            {
                position = _scene.LocalPosition(position);

                RSNodeSolid solid = RSNodeSolid.CreateRectangle(position, new SKSize(18, 18), SKColors.LightGreen);
                _scene.AddChild(solid);
                _physics.AddDynamicNode(solid, 15.0f, 5.0f, 0.5f, 0.1f);
            }
        }

        // ********************************************************************************************
        // Internal Methods

        private void LoadScene(SKSize size)
        {
            RSNode.RemoveChildren(_scene);

            _scene.Transformation.Size = size;
            _scene.Transformation.Color = SKColors.DarkGreen;
            _physics.Reset(_scene);

            // create an off screen render canvas to make motion streaks on
            // because alpha in this case is everything, the surface is created without pre-multiplied alpha
            // this allows for slightly better blending
            //
            _motionCanvas = RSNodeSurface.CreateWithSize(SKPoint.Empty, _scene.Transformation.Size);
            _motionCanvas.Transformation.Anchor = SKPoint.Empty;
            //_motionCanvas.Transformation.Color = SKColors.White;
            _motionCanvas.AlphaDecay = 25;
            _motionCanvas.Transformation.Altitude = -10;
            _scene.AddChild(_motionCanvas);

            _surface = RSNodeSurface.CreateWithSize(new SKPoint(400, 200), new SKSize(280, 350));
            _surface.Transformation.Anchor = new SKPoint(0.5f, 0.5f);
            _surface.Transformation.Scale = new SKPoint(0.8f, 0.8f);
            _surface.Transformation.Altitude = 10;
            //_surface.Transformation.Color = SKColors.White;
            _surface.ClearColor = SKColors.Red;
            _surface.AlphaDecay = 60;
            _scene.AddChild(_surface);

            _surface.Sequence().MoveTo(new SKPoint(-100, 200)).MoveBy(new SKPoint(1000, 0), 5.0f).MoveTo(new SKPoint(400, 200)).SaveAs("test");
            _surface.Sequence().ScaleTo(new SKPoint(0.5f, 0.5f), 5.0f).ScaleTo(new SKPoint(0.8f, 0.8f)).SaveAs("test");
            //_surface.Sequence().RotateTo(0, 2.5f).RotateBy(3600, 2.5f).SaveAs("test");
            _surface.Sequence().AlphaTo(0.0f, 2.5f).AlphaTo(1.0f, 2.5f).SaveAs("test");

            //_surface.Sequence().SizeTo(new SKSize(280, 100), 0.5f).SizeTo(new SKSize(280, 350), 0.5f).SaveAs("color_test");
            //_surface.Sequence().ColorTo(SKColors.Red, 0.5f).ColorTo(SKColors.White, 0.5f).SaveAs("color_test");


            //_physics.AddStaticNode(_surface);

            // _animal = RSNodeSprite.CreateWithFileAndJson(new SKPoint(0, -150), "Assets/animals.png/blue_cat");
            // _animal = RSNodeSprite.CreateWithFileAndJson(new SKPoint(0, -150), "Assets/animals.png/red_dog");
            _animal = RSNodeSprite.CreateWithFileAndJson(new SKPoint(0, -150), "Assets/animals.png/brown_monkey_walk");
            _animal.Transformation.Anchor = new SKPoint(0.5f, 0.0f);
            _surface.AddChild(_animal);

            _loadScene = RSNodeString.CreateString(new SKPoint(50, 50), "Reload Scene", RSFont.Create());
            _scene.AddChild(_loadScene);
        }


        // ********************************************************************************************
    }
}
