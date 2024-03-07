
using SkiaSharp;
using System.Numerics;
using Box2D.NetStandard.Dynamics.World;
using Box2D.NetStandard.Dynamics.Bodies;
using Box2D.NetStandard.Dynamics.Fixtures;
using Box2D.NetStandard.Collision.Shapes;

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

namespace Rockstar._Physics
{
    public enum RSPhysicsWorldType
    {
        Open,
        OpenBox,
        ClosedBox
    }

    public class RSPhysics
    {
        // ********************************************************************************************
        // Encapsulates the physics world
        //
        // IMPORTANT:
        // Nodes added to physics, will have their anchor forced set to (0.5, 0.5)
        //
        // TODO:
        // When adding a node, create complex combined shapes it the node has children

        // ********************************************************************************************
        // Constructors

        public static RSPhysics CreateWithScene(SKPoint gravity, float pixelSizeInMetres)
        {
            return new RSPhysics(gravity, pixelSizeInMetres);    
        }

        // ********************************************************************************************

        private RSPhysics(SKPoint gravity, float pixelSizeInMetres) 
        {
            // set up Box2D world
            _world = new World(new Vector2(gravity.X, gravity.Y));
            if (pixelSizeInMetres <= 0) 
            {
                pixelSizeInMetres = DEFAULT_PIXELSIZE_IN_METRES;
            }
            _scale = 1.0f / pixelSizeInMetres;
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        // ********************************************************************************************
        // Internal Data

        private const float DEFAULT_PIXELSIZE_IN_METRES = 0.01f;
        private const float WORLD_EXPANSION = 100000.0f;
        private const float GROUND_THICKNESS = 50.0f;
        private const float DEFAULT_DENSITY = 1.0f;
        private const float DEFAULT_FRICTION = 0.1f;
        private const float DEFAULT_RESTITUTION = 0.9f;

        private const int VELOCITY_INTERACTIONS = 8;
        private const int POSITION_INTERACTIONS = 3;

        private World _world;
        private float _scale;
        private object _lockObject = new object();

        // ********************************************************************************************
        // Methods

        public void Update(long interval)
        {
            lock (_lockObject)
            {
                _world.Step((float)interval / 1000.0f, VELOCITY_INTERACTIONS, POSITION_INTERACTIONS);
            }

            Body body = _world.GetBodyList();
            while (body != null)
            {
                if (body.GetUserData<RSNode>() is RSNode node)
                {
                    // Update the game object's position and rotation to match the physics body
                    Vector2 position = body.GetPosition();
                    node.Transformation.Position = new SKPoint(position.X * _scale, position.Y * _scale);
                    node.Transformation.Rotation = -body.GetAngle() * 180.0f / (float)Math.PI;
                    node.Transformation.Anchor = new SKPoint(0.5f, 0.5f);
                }
                body = body.GetNext();
            }

        }

        public void Reset(RSNodeScene scene, RSPhysicsWorldType type = RSPhysicsWorldType.ClosedBox) 
        {
            // Safely iterate through and remove all bodies
            Body body = _world.GetBodyList();
            while (body != null)
            {
                Body nextBody = body.GetNext();
                _world.DestroyBody(body);
                body = nextBody;
            }

            float expansion = (type == RSPhysicsWorldType.Open) ? WORLD_EXPANSION : 0.0f;

            RSNodeSolid ground = RSNodeSolid.CreateRectangle(
                new SKPoint(scene.Transformation.Size.Width / 2.0f, -GROUND_THICKNESS / 2.0f),
                new SKSize(scene.Transformation.Size.Width + expansion, GROUND_THICKNESS),
                SKColors.Green);
            ground.Transformation.Visible = false;
            scene.AddChild(ground);
            AddStaticNode(ground);

            if ((type == RSPhysicsWorldType.OpenBox) || (type == RSPhysicsWorldType.ClosedBox))
            {
                RSNodeSolid leftWall = RSNodeSolid.CreateRectangle(
                    new SKPoint(-GROUND_THICKNESS / 2.0f, scene.Transformation.Size.Height / 2.0f),
                    new SKSize(GROUND_THICKNESS, scene.Transformation.Size.Height),
                    SKColors.Green);
                leftWall.Transformation.Visible = false;
                scene.AddChild(leftWall);
                AddStaticNode(leftWall);

                RSNodeSolid rightWall = RSNodeSolid.CreateRectangle(
                    new SKPoint(scene.Transformation.Size.Width + (GROUND_THICKNESS / 2.0f), scene.Transformation.Size.Height / 2.0f),
                    new SKSize(GROUND_THICKNESS, scene.Transformation.Size.Height),
                    SKColors.Green);
                rightWall.Transformation.Visible = false;
                scene.AddChild(rightWall);
                AddStaticNode(rightWall);
            }
            if (type == RSPhysicsWorldType.ClosedBox)
            {
                RSNodeSolid roof = RSNodeSolid.CreateRectangle(
                    new SKPoint(scene.Transformation.Size.Width / 2.0f, scene.Transformation.Size.Height + (GROUND_THICKNESS / 2.0f)),
                    new SKSize(scene.Transformation.Size.Width + expansion, GROUND_THICKNESS),
                    SKColors.Green);
                roof.Transformation.Visible = false;
                scene.AddChild(roof);
                AddStaticNode(roof);
            }
        }

        public FixtureDef AddDynamicNode(RSNode node)
        {
            return AddNode(
                node,
                BodyType.Dynamic,
                DEFAULT_DENSITY,
                DEFAULT_FRICTION,
                DEFAULT_RESTITUTION
                );
         }

        public FixtureDef AddStaticNode(RSNode node)
        {
            return AddNode(
                node,
                BodyType.Static,
                DEFAULT_DENSITY,
                DEFAULT_FRICTION,
                DEFAULT_RESTITUTION
                );
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        private FixtureDef AddNode(RSNode node, BodyType type, float density, float friction, float restitution)
        {
            lock (_lockObject)
            {
                BodyDef bodyDef = new BodyDef();
                bodyDef.type = type;
                bodyDef.position = new Vector2(node.Transformation.Position.X / _scale, node.Transformation.Position.Y / _scale);
                node.Transformation.Anchor = new SKPoint(0.5f, 0.5f);
                
                Body body = _world.CreateBody(bodyDef);
                body.SetUserData(node);
                FixtureDef fixture = new FixtureDef();

                if (node.IsRound == true)
                {
                    CircleShape circle = new CircleShape();
                    circle.Radius = node.Transformation.Size.Width / (2.0f * _scale);
                    fixture.shape = circle;
                }
                else
                {
                    PolygonShape polygon = new PolygonShape();
                    polygon.SetAsBox(node.Transformation.Size.Width / (2.0f * _scale), node.Transformation.Size.Height / (2.0f * _scale));
                    fixture.shape = polygon;
                }
                fixture.density = density;
                fixture.friction = friction;
                fixture.restitution = restitution;

                body.CreateFixture(fixture);

                return fixture;
            }
        }

        // ********************************************************************************************
    }
}
