
using SkiaSharp;
using System.Numerics;
using Box2D.NetStandard.Dynamics.World;
using Box2D.NetStandard.Dynamics.Bodies;
using Box2D.NetStandard.Dynamics.Fixtures;
using Box2D.NetStandard.Collision.Shapes;
using Box2D.NetStandard.Dynamics.World.Callbacks;
using Box2D.NetStandard.Dynamics.Contacts;
using Box2D.NetStandard.Collision;

using Rockstar._Nodes;
using System.Diagnostics;
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

namespace Rockstar._Physics
{
    public enum RSPhysicsWorldType
    {
        Open,
        OpenBox,
        ClosedBox
    }

    public class RSPhysics : ContactListener
    {
        // ********************************************************************************************
        // Encapsulates the physics world
        //
        // IMPORTANT:
        // Nodes added to physics, will have their anchor forced set to (0.5, 0.5)
        //
        // TODO:
        // When adding a node, create complex combined shapes it the node has children
        // Implement joints

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
            _world.SetContactListener(this);
            _bodyKillList = new List<Body>();
            _contactList = new Dictionary<Body, float>();
            _physicsLock = new object();
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
        private const float DEFAULT_RESTITUTION = 0.1f;

        private const float ENERGY_INFINITE = float.MaxValue;
        private const float IMPULSE_ENERGY_GAIN = 10.0f;
        private const float KINETIC_ENERGY_GAIN = 0.25f;

        private const int VELOCITY_INTERACTIONS = 4;
        private const int POSITION_INTERACTIONS = 2;

        private World _world;
        private float _scale;

        private object _physicsLock;
        private List<Body> _bodyKillList;
        private Dictionary<Body, float> _contactList;

        // ********************************************************************************************
        // Methods

        public void Update(RSNodeScene scene, long interval)
        {
            lock (_physicsLock)
            {
                _contactList.Clear();

                // main world physics step
                //
                _world.Step((float)interval / 1000.0f, VELOCITY_INTERACTIONS, POSITION_INTERACTIONS);

                // remove killed bodies
                //
                foreach (Body bodyToKill in _bodyKillList)
                {
                    RSPhysicsDef physics = bodyToKill.GetUserData<RSPhysicsDef>();
                    if (physics.Node.Parent != null)
                    {
                        physics.Node.Parent.RemoveChild(physics.Node);
                    }
                    _world.DestroyBody(bodyToKill);
                }
                _bodyKillList.Clear();
            }

            // update node positions after step by iterating linked list
            //
            Body body = _world.GetBodyList();
            while (body != null)
            {
                if (body.GetUserData<RSPhysicsDef>() is RSPhysicsDef physics)
                {
                    // Update the game object's position and rotation to match the physics body
                    Vector2 position = body.GetPosition();
                    physics.Node.Transformation.Position = new SKPoint(position.X * _scale, position.Y * _scale);
                    physics.Node.Transformation.Rotation = -body.GetAngle() * 180.0f / (float)Math.PI;
                    physics.Node.Transformation.Anchor = new SKPoint(0.5f, 0.5f);
                }
                body = body.GetNext();
            }

        }

        public void Reset(RSNodeScene scene, RSPhysicsWorldType type = RSPhysicsWorldType.ClosedBox)
        {
            lock (_physicsLock)
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
        }

        public RSPhysicsDef AddDynamicNode(RSNode node, float breakEnergy)
        {
            return AddNode(node, BodyType.Dynamic, breakEnergy);
        }

        public RSPhysicsDef AddDynamicNode(RSNode node, float breakEnergy, float density)
        {
            return AddNode(node, BodyType.Dynamic, breakEnergy, density);
        }

        public RSPhysicsDef AddDynamicNode(RSNode node, params object[] data)
        {
            return AddNode(node, BodyType.Dynamic, data);
        }

        public RSPhysicsDef AddStaticNode(RSNode node, float breakEnergy)
        {
            return AddNode(node, BodyType.Static, breakEnergy);
        }

        public RSPhysicsDef AddStaticNode(RSNode node, float breakEnergy, float density)
        {
            return AddNode(node, BodyType.Static, breakEnergy, density);
        }

        public RSPhysicsDef AddStaticNode(RSNode node, params object[] data)
        {
            return AddNode(node, BodyType.Static, data);
        }

        public void RemoveNode(RSNode node)
        {
            lock (_physicsLock)
            {
                // Safely iterate through all bodies
                Body body = _world.GetBodyList();
                while (body != null)
                {
                    Body nextBody = body.GetNext();
                    RSPhysicsDef physics = body.GetUserData<RSPhysicsDef>();
                    if (physics.Node == node)
                    {
                        _world.DestroyBody(body);
                        break;
                    }
                    body = nextBody;
                }
            }

        }

        public RSPhysicsDef? GetPhysics(RSNode node)
        {
            // Maybe maintain a <node, body> dictionary for faster access
            //
            // iterate linked list
            //
            Body body = _world.GetBodyList();
            while (body != null)
            {
                if (body.GetUserData<RSPhysicsDef>() is RSPhysicsDef physics)
                {
                    if (physics.Node == node) return physics;
                }
                body = body.GetNext();
            }
            return null;
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        private RSPhysicsDef AddNode(RSNode node, BodyType type, params object[] data)
        {
            lock (_physicsLock)
            {

                BodyDef bodyDef = new BodyDef();
                bodyDef.type = type;
                bodyDef.position = new Vector2(node.Transformation.Position.X / _scale, node.Transformation.Position.Y / _scale);
                node.Transformation.Anchor = new SKPoint(0.5f, 0.5f);
                
                Body body = _world.CreateBody(bodyDef);
                FixtureDef fixture = new FixtureDef();

                if (node.IsRound == true)
                {
                    CircleShape circle = new CircleShape();
                    circle.Radius = node.Transformation.Size.Width * node.Transformation.Scale.X / (2.0f * _scale);
                    fixture.shape = circle;
                }
                else
                {
                    PolygonShape polygon = new PolygonShape();
                    polygon.SetAsBox(
                        node.Transformation.Size.Width * node.Transformation.Scale.X / (2.0f * _scale), 
                        node.Transformation.Size.Height * node.Transformation.Scale.Y / (2.0f * _scale));
                    fixture.shape = polygon;
                }

                float breakEnergy = ((data.Length >= 1) && (data[0] is float value)) ? value : ENERGY_INFINITE;

                fixture.density = ((data.Length >= 2) && (data[1] is float density)) ? density : DEFAULT_DENSITY; ;
                fixture.friction = ((data.Length >= 3) && (data[2] is float friction)) ? friction : DEFAULT_FRICTION;
                fixture.restitution = ((data.Length >= 4) && (data[3] is float restitution)) ? restitution : DEFAULT_RESTITUTION;

                body.CreateFixture(fixture);

                RSPhysicsDef physics = RSPhysicsDef.CreateWithNode(node, body, fixture, breakEnergy);
                body.SetUserData(physics);

                return physics;
            }
        }

        public override void BeginContact(in Contact contact)
        {

        }

        public override void EndContact(in Contact contact)
        {

        }

        public override void PreSolve(in Contact contact, in Manifold oldManifold)
        {
            var bodyA = contact.FixtureA.Body;
            var bodyB = contact.FixtureB.Body;

            // calculate energy of both bodies
            float energy = KINETIC_ENERGY_GAIN * (0.5f * bodyA.GetMass() * bodyA.GetLinearVelocity().LengthSquared()) + (0.5f * bodyB.GetMass() * bodyB.GetLinearVelocity().LengthSquared());

            // if half the energy is above breaking energy, store it
            RSPhysicsDef physicsA = bodyA.GetUserData<RSPhysicsDef>();
            if ((energy / 2.0f) >= physicsA.BreakEnergy)
            {
                _contactList[bodyA] = energy;
            }

            RSPhysicsDef physicsB = bodyB.GetUserData<RSPhysicsDef>();
            if ((energy / 2.0f) >= physicsB.BreakEnergy)
            {
                _contactList[bodyB] = energy;
            }
        }

        public override void PostSolve(in Contact contact, in ContactImpulse impulse)
        {
            var bodyA = contact.FixtureA.Body;
            var bodyB = contact.FixtureB.Body;

            float initialEnergy = 0;
            float impulseEnergy = IMPULSE_ENERGY_GAIN * (impulse.normalImpulses[0] + impulse.normalImpulses[1]);

            // calculate energy of both bodies
            float energy = KINETIC_ENERGY_GAIN * (0.5f * bodyA.GetMass() * bodyA.GetLinearVelocity().LengthSquared()) + (0.5f * bodyB.GetMass() * bodyB.GetLinearVelocity().LengthSquared());

            _contactList.TryGetValue(bodyA, out initialEnergy);
            {
                // The energy lost in A, is half of the total energy lost
                // Now, this is not correct if one of the bodies are bouncy bouncy, but this is not the real world ...
                //
                float energyA = Math.Abs(initialEnergy - energy) / 2.0f;
                
                // if energy is above breaking energy, the body is added to the kill list
                RSPhysicsDef physicsA = bodyA.GetUserData<RSPhysicsDef>();

                if ((energyA + impulseEnergy > physicsA.BreakEnergy) && (_bodyKillList.Contains(bodyA) == false))
                    _bodyKillList.Add(bodyA);

                // contact always removed
                _contactList.Remove(bodyA);
            }

            _contactList.TryGetValue(bodyB, out initialEnergy);
            {
                float energyB = Math.Abs(initialEnergy - energy) / 2.0f;
                RSPhysicsDef physicsB = bodyB.GetUserData<RSPhysicsDef>();
                if ((energyB + impulseEnergy > physicsB.BreakEnergy) && (_bodyKillList.Contains(bodyB) == false))
                    _bodyKillList.Add(bodyB);
                _contactList.Remove(bodyB);
            }
        }

        // ********************************************************************************************
    }
}
