using Rockstar._Actions;
using Rockstar._CodecJson;
using Rockstar._Game;
using Rockstar._MouseButton;
using Rockstar._Dictionary;
using Rockstar._Event;
using Rockstar._NodeFactory;
using Rockstar._Nodes;
using Rockstar._Physics;
using Rockstar._PhysicsDef;
using Rockstar._Random;
using Rockstar._Touch;
using Rockstar._Types;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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

namespace RSGameDemo._GameSnowGlobe
{
    public class RSGameSnowGlobe : RSGame
    {
        // ********************************************************************************************
        // Brief Class Description
        //
        //

        // ********************************************************************************************
        // Constructors

        public static RSGameSnowGlobe Create()
        {
            return new RSGameSnowGlobe();
        }

        // ********************************************************************************************

        private RSGameSnowGlobe() 
        {
            _physics = RSPhysics.CreateWithScene(new SKPoint(0.001f, -0.9f), 0.2f);
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        // ********************************************************************************************
        // Internal Data

        private readonly SKPoint SCREEN_POSITION = new SKPoint(400, 300);

        private readonly SKPoint MENU_POSITION = new SKPoint(90, 30);
        private const float MENU_SPACING = 30.0f;

        private const float GLOBE_START_ANGLE = 215.0f;
        private const float GLOBE_END_ANGLE = 145.0f;
        private const float GLOBE_RADIUS = 290.0f;
        private const int GLOBE_SEGMENT_COUNT = 30;
        private readonly SKSize GLOBE_SEGMENT_SIZE = new SKSize(50, 30);

        private readonly SKPoint GLOBE_FLOOR_POSITION = new SKPoint(400, 65);
        private readonly SKSize GLOBE_FLOOR_SIZE = new SKSize(350, 30);

        private const byte HEAVY_COLLISION_GROUP = 99;
        private const byte GLOBE_COLLISION_GROUP = 128;
        private const float LIGHT_FORCE_GAIN = 0.0025f;
        private const float HEAVY_FORCE_GAIN = 25.0f;

        private const int MAX_COLLISION_GROUPS = 4;

        RSNode _physicsNode;
        RSNodeString _seeMore;
        RSNodeString _reload;
        RSNodeString _giefMore;
        RSNodeString _showPhysics;
        RSNodeSprite _easyNow;
        SKPoint _leftMousePosition;
        int _collisionGroup = 0;

        // ********************************************************************************************
        // Methods

        public override void Initialize(SKSize size)
        {
            LoadScene(size);

            _mouse.AddHandler(RSMouseButtonType.Left, RSMouseEvent.OnPressed, OnLeftMouseClick);
            _mouse.AddHandler(RSMouseButtonType.Left, RSMouseEvent.OnMoved, OnLeftMouseAddForce);
            _mouse.AddHandler(RSMouseButtonType.Left, RSMouseEvent.OnReleased, OnLeftMouseAddForce);
        }

        public override void Resize(SKSize size)
        {

        }

        public override void Update(float interval)
        {

        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        private void OnLeftMouseClick(object sender, RSEventArgs argument)
        {
            if (argument.Data is SKPoint position)
            {
                if (argument.Type is RSMouseEvent.OnPressed)
                {
                    _leftMousePosition = _scene.LocalPosition(position);

                    RSNodeList nodeList = _scene.GetHitList(position);

                    if (nodeList.Contains(_reload) == true)
                    {
                        LoadScene(_scene.Transformation.Size);
                    }

                    if (nodeList.Contains(_giefMore) == true)
                    {
                        if (_scene.Children.Count < 1200)
                        {
                            CreateSnowFlakes();
                        }
                        else if (_easyNow.NumberOfRunningActions() == 0)
                        {
                            _easyNow
                                .Sequence()
                                .ScaleTo(1.0f, 1.0f, 1.0f, Rockstar._Lerp.RSLerpType.Ringing)
                                .Delay(2.0f) 
                                .ScaleTo(0.0f, 0.0f, 0.3f)
                                .Run();
                        }
                    }

                    if (nodeList.Contains(_showPhysics) == true)
                    {
                        _physicsNode.Transformation.Visible = !_physicsNode.Transformation.Visible;
                        foreach (RSNode node in _physicsNode.Children)
                            node.Transformation.Visible = !node.Transformation.Visible;
                    }

                    if (nodeList.Contains(_seeMore) == true)
                    {
                        try
                        {
                            Process.Start(new ProcessStartInfo("https://www.rockstarprogrammer.dk") { UseShellExecute = true });
                        }
                        catch
                        {

                        }

                    }
                }
            }
        }

        private void OnLeftMouseAddForce(object sender, RSEventArgs argument)
        {
            if (argument.Data is SKPoint position)
            {
                position = _scene.LocalPosition(position);

                Debug.WriteLine(position);

                position -= _leftMousePosition;

                _physics.ApplyForceToGroup(new SKPoint(position.X * HEAVY_FORCE_GAIN, position.Y * HEAVY_FORCE_GAIN), HEAVY_COLLISION_GROUP);
                _physics.ApplyForceToGroup(new SKPoint(position.X * LIGHT_FORCE_GAIN, position.Y * LIGHT_FORCE_GAIN), 1);
            }
        }

        private void LoadScene(SKSize size)
        {
            RSNode.RemoveChildren(_scene);
            _physics.Reset(_scene);

            RSDictionary setup = RSCodecJson.CreateDictionaryWithFilePath("Assets/snowglobe.json");
            _scene.InitializeNode(setup.GetDictionary("little_mermaid"));

            CreateGlobe();
            CreatePlatforms();
            CreateGlobeDisrupters();
            CreateSnowFlakes();
            CreateText();
        }

        private void CreateGlobe()
        {
            float circleRange = GLOBE_END_ANGLE - GLOBE_START_ANGLE;
            if (circleRange <= 0) circleRange += 360.0f;
            float angleStep = circleRange / GLOBE_SEGMENT_COUNT;
            float angle = GLOBE_START_ANGLE + (angleStep / 2.0f) - 90.0f;

            _physicsNode = RSNode.Create();
            _scene.AddChild( _physicsNode);

            // create collision sphere
            //
            for (int index = 0; index < GLOBE_SEGMENT_COUNT; index++)
            {
                SKPoint vector = angle.Vector();
                vector = SCREEN_POSITION + new SKPoint(vector.X * GLOBE_RADIUS, vector.Y * GLOBE_RADIUS);
                RSNodeSolid segment = RSNodeSolid.CreateRectangle(new SKSize(GLOBE_SEGMENT_SIZE.Width, GLOBE_SEGMENT_SIZE.Height), SKColors.Red)
                    .SetPosition(vector)
                    .SetRotation(90.0f + angle)
                    .SetVisible(RSPhysics.SHOW_WORLD);
                _physicsNode.AddChild(segment);

                _physics.AddStaticNode(segment, RSPhysics.ENERGY_INFINITE, 1, 0.0f).SetCollisionGroup(GLOBE_COLLISION_GROUP);

                angle += angleStep;
            }

            // create collision floor
            RSNodeSolid floor = RSNodeSolid.CreateRectangle(GLOBE_FLOOR_SIZE, SKColors.Red)
                .SetPosition(GLOBE_FLOOR_POSITION)
                .SetVisible(RSPhysics.SHOW_WORLD);
            _physicsNode.AddChild(floor);

            _physics.AddStaticNode(floor, RSPhysics.ENERGY_INFINITE, 1, 0.0f).SetCollisionGroup(GLOBE_COLLISION_GROUP);
        }

        private void CreatePlatforms()
        {
            SKPoint[] platformPos = new[]
            {
                new SKPoint(403, 438), new SKPoint(383, 430), new SKPoint(406, 410),
                new SKPoint(373, 349), new SKPoint(447, 335), new SKPoint(395, 215),
                new SKPoint(359, 205), new SKPoint(293, 155), new SKPoint(393, 171),
                new SKPoint(421, 183), new SKPoint(451, 150), new SKPoint(484, 140), 
            };
            SKSize[] platformSize = new[]
            {
                new SKSize(20, 5), new SKSize(40, 5), new SKSize(40, 5),
                new SKSize(30, 5), new SKSize(40, 5), new SKSize(50, 5),
                new SKSize(40, 5), new SKSize(50, 5), new SKSize(50, 5),
                new SKSize(20, 5), new SKSize(40, 5), new SKSize(20, 5), 
            };

            for (int index = 0; index < platformPos.Length; index++)
            {
                RSNodeSolid segment = RSNodeSolid.CreateRectangle(platformSize[index], SKColors.Red)
                   .SetPosition(platformPos[index])
                   .SetVisible(RSPhysics.SHOW_WORLD);
                _physicsNode.AddChild(segment);

                _physics.AddStaticNode(segment, RSPhysics.ENERGY_INFINITE, 1, 0.5f)
                    .SetCollisionType(RSCollisionType.Above);
            }
        }

        private void CreateGlobeDisrupters()
        {
            for (float angle = -170; angle <= -10; angle += 40)
            {
                SKPoint vector = angle.Vector();
                vector = SCREEN_POSITION + new SKPoint(vector.X * GLOBE_RADIUS, vector.Y * GLOBE_RADIUS);

                RSNodeSolid segment = RSNodeSolid.CreateRectangle(new SKSize(GLOBE_SEGMENT_SIZE.Height, GLOBE_SEGMENT_SIZE.Height), SKColors.Red)
                    .SetPosition(vector)
                    .SetRotation(45.0f + angle)
                    .SetVisible(RSPhysics.SHOW_WORLD);
                _physicsNode.AddChild(segment);

                _collisionGroup++;
                _collisionGroup %= MAX_COLLISION_GROUPS;

                _physics.AddStaticNode(segment, RSPhysics.ENERGY_INFINITE, 1, 0.0f)
                    .SetCollisionGroup((byte)_collisionGroup)
                    .SetCollisionType(RSCollisionType.Below);
            }
        }

        private void CreateSnowFlakes()
        {
            for (int count = 0; count < 200; count++)
            {
                float length = RSRandom.RandomNormalised();
                length = (float)Math.Sqrt(length) * 270;
                float angle = RSRandom.RandomRange(0, 360);

                SKPoint position = SCREEN_POSITION + new SKPoint(angle.Vector().X * length, angle.Vector().Y * length);
                if (position.Y < 100) position = new SKPoint(position.X, 100);

                float size = RSRandom.RandomRange(0.8f, 1.2f);

                RSNodeSprite flake = RSNodeSprite.CreateWithFileAndJson("Assets/mermaid.png/flake")
                    .SetPosition(position)
                    .SetScale(size * 0.3f, size * 0.1f)
                    .SetAlpha(0.85f);
                flake.SetCurrentFrame((int)RSRandom.RandomRange(0, 8));
                _scene.AddChild(flake);

                if ((count % 50) == 0)
                {
                    _physics.AddDynamicNode(flake, RSPhysics.ENERGY_INFINITE, 1000.0f, 0.1f, 0.60f)
                        .SetFixedRotation(0)
                        .SetCollisionGroup(HEAVY_COLLISION_GROUP);
                }
                else
                {
                    _physics.AddDynamicNode(flake, RSPhysics.ENERGY_INFINITE, 0.1f, 0.2f, 0.0f)
                        .SetFixedRotation(0)
                        .SetCollisionGroup((byte)_collisionGroup);

                    _collisionGroup++;
                    _collisionGroup %= MAX_COLLISION_GROUPS;
                }

                flake.SetScale(size * 0.38f);
            }
        }

        private void CreateText()
        {
            _giefMore = RSNodeString.CreateString("Give Me More Snow", RSFont.Create())
                .SetPosition(MENU_POSITION.X, MENU_POSITION.Y + (2 * MENU_SPACING))
                .SetScale(1.2f);
            _scene.AddChild(_giefMore);

            _reload = RSNodeString.CreateString("Reload", RSFont.Create())
                .SetPosition(MENU_POSITION.X, MENU_POSITION.Y + (1 * MENU_SPACING))
                .SetScale(0.8f);
            _scene.AddChild(_reload);

            _showPhysics = RSNodeString.CreateString("Show Physics", RSFont.Create())
                .SetPosition(MENU_POSITION.X, MENU_POSITION.Y + (0 * MENU_SPACING))
                .SetScale(0.8f);
            _scene.AddChild(_showPhysics);

            _seeMore = RSNodeString.CreateString("See more at https://www.rockstarprogrammer.dk", RSFont.Create())
                .SetPosition(SCREEN_POSITION.X, MENU_POSITION.Y + (1 * MENU_SPACING));
            _scene.AddChild(_seeMore);

            RSNodeString drag = RSNodeString.CreateString("Drag Left Mouse Inside Globe to Stir", RSFont.Create())
                .SetPosition(SCREEN_POSITION.X, MENU_POSITION.Y + (0 * MENU_SPACING))
                .SetScale(0.8f);
            _scene.AddChild(drag);

            _easyNow = RSNodeSprite.CreateWithFileAndJson("Assets/mermaid.png/easy_now")
                .SetPosition(SCREEN_POSITION)
                .SetScale(0)
                .SetAltitude(10);
            _scene.AddChild(_easyNow);
        }

        // ********************************************************************************************
    }
}
