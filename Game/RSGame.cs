using SkiaSharp;

using Rockstar._Types;
using Rockstar._CodecJson;
using Rockstar._CoreGame;
using Rockstar._Dictionary;
using Rockstar._Event;
using Rockstar._Nodes;
using Rockstar._CoreMouseButton;
using Rockstar._NodeList;

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

        RSNodeSprite? _cat;
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

        public override void Update(long interval)
        {
            _catTimer += interval;
            if (_catTimer > 100)
            {
                _cat.SetCurrentFrame(_cat.CurrentFrame + 1);
                _catTimer -= 100;
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

                RSNodeSolid solid = RSNodeSolid.CreateEllipse(position, new SKSize(18, 18), SKColors.Cyan);
                _motionCanvas.AddChild(solid);
                _physics.AddDynamicNode(solid, 1.0f, 1.0f, 0.3f, 0.9f);
            }    
        }

        public void OnLeftMouseAddPhysics(object sender, RSEventArgs argument)
        {
            if (argument.Data is SKPoint position)
            {
                position = _scene.LocalPosition(position);

                RSNodeSolid solid = RSNodeSolid.CreateRectangle(position, new SKSize(18, 18), SKColors.Magenta);
                _scene.AddChild(solid);
                _physics.AddDynamicNode(solid, 50.0f, 25.0f, 0.5f, 0.1f);
            }
        }

        // ********************************************************************************************
        // Internal Methods

        private void LoadScene(SKSize size)
        {
            RSNode.RemoveChildren(_scene);

            _scene.Transformation.Size = size;
            _physics.Reset(_scene);

            // create an off screen render canvas to make motion streaks on
            // because alpha in this case is everything, the surface is created without pre-multiplied alpha
            // this allows for slightly better blending
            //
            _motionCanvas = RSNodeSurface.CreateWithSize(SKPoint.Empty, _scene.Transformation.Size, false);
            _motionCanvas.Transformation.Anchor = SKPoint.Empty;
            _motionCanvas.Color = new SKColor(32, 32, 32, 32);
            _scene.AddChild(_motionCanvas);

            _surface = RSNodeSurface.CreateWithSize(new SKPoint(200, 200), new SKSize(250, 250));
            _surface.Transformation.Anchor = new SKPoint(0.5f, 0.0f);
            _surface.Color = SKColors.AliceBlue;
            _scene.AddChild(_surface);
            _physics.AddStaticNode(_surface);

            _cat = RSNodeSprite.CreateWithFileAndJson(new SKPoint(0, -100), "Assets/blue_cat.png");
            _cat.Transformation.Anchor = new SKPoint(0.5f, 0.0f);
            _surface.AddChild(_cat);

            _loadScene = RSNodeString.CreateString(new SKPoint(50, 50), "Reload Scene", RSFont.Create());
            _scene.AddChild(_loadScene);
        }


        // ********************************************************************************************
    }
}
