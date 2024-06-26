﻿using SkiaSharp;

using Rockstar._Game;
using Rockstar._Event;
using Rockstar._Nodes;
using Rockstar._MouseButton;
using Rockstar._Types;
using Rockstar._Actions;
using Rockstar._Physics;
using Rockstar._PhysicsDef;

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
    internal class RSGamePhysics : RSGame
    {
        // ********************************************************************************************
        // Game template

        // ********************************************************************************************
        // Constructors

        public static RSGamePhysics Create()
        {
            return new RSGamePhysics();
        }

        // ********************************************************************************************

        private RSGamePhysics()
        {
            // Adding a bit of X gravity, prevents perfect stacking of objects
            //
            _physics = RSPhysics.CreateWithScene(new SKPoint(0.001f, -9.8f), 0.01f);
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        // ********************************************************************************************
        // Internal Data


        float _catTimer = 0;

        RSNodeSprite _cat;
        RSNodeSprite _animal;
        RSNodeSurface _surface;
        RSNodeSurface _motionCanvas;
        RSNodeString _loadScene;

        // ********************************************************************************************
        // Methods

        public override void Initialize(SKSize size)
        {
            LoadScene(size);

            _mouse.AddHandler(RSMouseButtonType.Left, RSMouseEvent.OnPressed, OnLeftMouseEvent);
            _mouse.AddHandler(RSMouseButtonType.Left, RSMouseEvent.OnReleased, OnLeftMouseEvent);

            _mouse.AddHandler(RSMouseButtonType.Right, RSMouseEvent.OnAll, OnRightMouseEvent);

            _mouse.AddHandler(RSMouseButtonType.Left, RSMouseEvent.OnAll, OnLeftMouseAddPhysics);
            _mouse.AddHandler(RSMouseButtonType.Right, RSMouseEvent.OnAll, OnRightMouseAddPhysics);
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
                if (_cat != null) _cat.SetCurrentFrame(_cat.CurrentFrame + 1);
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
                    //_surface.RunAction("test");

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
                    _renderer.DebugNodeList.Clear();
                }
                else
                {
                    //_cat.RunAction("test");
                    _renderer.AssignDebugNodeList(_scene.GetHitList(position));
                }
            }
        }

        public void OnRightMouseAddPhysics(object sender, RSEventArgs argument)
        {
            if (argument.Data is SKPoint position)
            {
                position = _scene.LocalPosition(position);

                RSNodeSolid solid = RSNodeSolid.CreateEllipse(new SKSize(18, 18), SKColors.Cyan).SetPosition(position);
                _motionCanvas.AddChild(solid);
                _physics.AddDynamicNode(solid, 5.0f, 1.0f, 0.3f, 0.9f);
            }    
        }

        public void OnLeftMouseAddPhysics(object sender, RSEventArgs argument)
        {
            if (argument.Data is SKPoint position)
            {
                position = _scene.LocalPosition(position);

                RSNodeSolid solid = RSNodeSolid.CreateRectangle(new SKSize(18, 18), SKColors.LightGreen).SetPosition(position);
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
            //
            _motionCanvas = RSNodeSurface.CreateWithSize(_scene.Transformation.Size);
            _motionCanvas.Transformation.Anchor = SKPoint.Empty;
            //_motionCanvas.Transformation.Color = SKColors.White;
            _motionCanvas.AlphaDecay = 25;
            _motionCanvas.Transformation.Altitude = -10;
            _scene.AddChild(_motionCanvas);

            // add some one way platforms
            RSNodeSolid platform = RSNodeSolid.CreateRectangle(new SKSize(200, 20), SKColors.White).SetPosition(200, 120);
            _physics.AddStaticNode(platform).SetCollisionType(RSCollisionType.Above);
            _scene.AddChild(platform);

            //_surface = RSNodeSurface.CreateWithSize(new SKSize(280, 350)).SetPosition(400, 200);
            //_surface.Transformation.Anchor = new SKPoint(0.5f, 0.5f);
            //_surface.Transformation.Scale = new SKPoint(0.8f, 0.8f);
            //_surface.Transformation.Altitude = 10;
            ////_surface.Transformation.Color = SKColors.White;
            //_surface.ClearColor = SKColors.Red;
            //_surface.AlphaDecay = 60;
            //_scene.AddChild(_surface);

            //// create 
            //_surface
            //    .Sequence()
            //    .MoveTo(-100, 200)
            //    .MoveBy(1000, 0, 5.0f)
            //    .MoveTo(400, 200)
            //    .Run()
            //    .SaveAs("test");

            //RSAction.CreateSequence()
            //    .ScaleTo(0.5f, 0.5f, 2.0f)
            //    .ScaleTo(0.8f, 0.8f)
            //    .SaveAs("test");

            //// create a generic action sequence
            //RSAction.Create().MoveTo(100, 100, 5.0f).SaveAs("test_2");

            //_animal = RSNodeSprite.CreateWithFileAndJson("Assets/animals.png/brown_monkey_walk").SetPosition(0, -150);
            //_animal.Transformation.Anchor = new SKPoint(0.5f, 0.0f);
            //_surface.AddChild(_animal);

            //_cat = RSNodeSprite.CreateWithFileAndJson("Assets/animals.png/blue_cat").SetPosition(0, 200);
            //_cat.Transformation.Scale = new SKPoint(0.5f, 0.5f);
            //_cat.Transformation.Anchor = new SKPoint(0.5f, -1.0f);
            //_scene.AddChild(_cat);

            _loadScene = RSNodeString.CreateString("Reload Scene", RSFont.Create()).SetPosition(50, 50);
            _scene.AddChild(_loadScene);
        }


        // ********************************************************************************************
    }
}
