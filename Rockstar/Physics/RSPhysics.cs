﻿
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
using Rockstar._PhysicsDef;
using Rockstar._Types;

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
            _worldScale = 1.0f / pixelSizeInMetres;
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

        public const bool SHOW_WORLD = false;
        public const float ENERGY_INFINITE = float.MaxValue;

        private const bool SHOW_LOAD = true;
        private const float SHOW_LOAD_MAGNIFICATION = 3.0f; // defines how prominent the red load color is

        private const float DEFAULT_PIXELSIZE_IN_METRES = 0.01f;
        private const float WORLD_EXPANSION = 100000.0f;
        private const float GROUND_THICKNESS = 50.0f;
        private const float DEFAULT_DENSITY = 1.0f;
        private const float DEFAULT_FRICTION = 0.1f;
        private const float DEFAULT_RESTITUTION = 0.1f;


        // IMPORTANT
        // The gain settings for impulse and kinetic energy, sets the relationship between these
        // They should not be changed, to set the overall "breakability" of a body
        // For that, use the breakForce
        //
        // In stead they should be adjusted to you actual game needs
        // impulse energy is used for when bodies are "under pressure"
        // kinetic energy is used for then bodies collide "at speed"
        //
        // 1) if you feel the body overall breaks too easily
        //    - increase the breakEnergy 
        // 2) if you feel bodies break corretly when colliding, but are too hard to squeeze
        //    - increase IMPULSE_ENERGY_GAIN
        // 3) if you feel bodies break correctly under presser, but breaks too easily when colliding
        //    - increase KINETIC_ENERGY_GAIN
        //
        // Density also matters, as more dense bodies will release more kinetic energy
        // All in all, it is a bit of a "cut and try" matter what suits your needs
        //
        // Known issues
        // - A more intuitive interface for this would be nice
        // - Box2D bodies deeply overlapping, aught to result in high impulse forces. It sometimes does not
        //   This sometimes results in breakable bodies being squeezed into other bodies without breaking
        //   Problem is minimal, and most often occur when new bodies are added to already cramped positions
        //
        private const float IMPULSE_ENERGY_GAIN = 7.50f;
        private const float KINETIC_ENERGY_GAIN = 0.25f;

        // basic step settings for Box2D
        //
        private const int VELOCITY_INTERACTIONS = 8;
        private const int POSITION_INTERACTIONS = 3;

        private World _world;
        private float _worldScale;

        private object _physicsLock;
        private List<Body> _bodyKillList;
        private Dictionary<Body, float> _contactList;

        // ********************************************************************************************
        // Methods

        public void Update(RSNodeScene scene, float interval)
        {
            lock (_physicsLock)
            {
                _contactList.Clear();

                // main world physics step
                //
                _world.Step(interval, VELOCITY_INTERACTIONS, POSITION_INTERACTIONS);

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
                    // Update the game RSNode's position and rotation to match the physics body
                    Vector2 position = body.GetPosition();
                    physics.Node.Transformation.Position = new SKPoint(position.X * _worldScale, position.Y * _worldScale);
                    if (physics.FixedRotation >= 0)
                    {
                        physics.Node.Transformation.Rotation = physics.FixedRotation;
                    }
                    else 
                    {
                        physics.Node.Transformation.Rotation = body.GetAngle().ToRotation();
                    }
                    physics.Node.Transformation.Anchor = new SKPoint(0.5f, 0.5f);

                    if (SHOW_LOAD == true)
                    {
                        float load = SHOW_LOAD_MAGNIFICATION * (physics.StaticEnergy / physics.BreakEnergy).ClampNormalised();
                        byte red = (physics.Color.Red + (load * (255.0f - physics.Color.Red))).ClampByte();
                        byte green = (physics.Color.Green - (load * physics.Color.Green)).ClampByte();
                        byte blue = (physics.Color.Blue - (load * physics.Color.Blue)).ClampByte();

                        physics.Node.Transformation.Color = new SKColor(red, green, blue, physics.Node.Transformation.Color.Alpha);
                    }
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
                    new SKSize(scene.Transformation.Size.Width + expansion, GROUND_THICKNESS + 2), SKColors.Green)
                    .SetPosition(scene.Transformation.Size.Width / 2.0f, -GROUND_THICKNESS / 2.0f)
                    .SetVisible(SHOW_WORLD);
                
                scene.AddChild(ground);
                AddStaticNode(ground);

                if ((type == RSPhysicsWorldType.OpenBox) || (type == RSPhysicsWorldType.ClosedBox))
                {
                    RSNodeSolid leftWall = RSNodeSolid.CreateRectangle(
                        new SKSize(GROUND_THICKNESS + 2, scene.Transformation.Size.Height), SKColors.Green)
                        .SetPosition(-GROUND_THICKNESS / 2.0f, scene.Transformation.Size.Height / 2.0f)
                        .SetVisible(SHOW_WORLD);
                
                    scene.AddChild(leftWall);
                    AddStaticNode(leftWall);

                    RSNodeSolid rightWall = RSNodeSolid.CreateRectangle(
                        new SKSize(GROUND_THICKNESS + 2, scene.Transformation.Size.Height), SKColors.Green)
                        .SetPosition(scene.Transformation.Size.Width + (GROUND_THICKNESS / 2.0f), scene.Transformation.Size.Height / 2.0f)
                        .SetVisible(SHOW_WORLD);

                    scene.AddChild(rightWall);
                    AddStaticNode(rightWall);
                }
                if (type == RSPhysicsWorldType.ClosedBox)
                {
                    RSNodeSolid roof = RSNodeSolid.CreateRectangle(
                        new SKSize(scene.Transformation.Size.Width + expansion, GROUND_THICKNESS + 2), SKColors.Green)
                        .SetPosition(scene.Transformation.Size.Width / 2.0f, scene.Transformation.Size.Height + (GROUND_THICKNESS / 2.0f))
                        .SetVisible(SHOW_WORLD);

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

        public void ApplyForceToGroup(SKPoint force, byte group)
        {
            lock (_physicsLock)
            {
                Body body = _world.GetBodyList();
                while (body != null)
                {
                    if ((body.GetUserData<RSPhysicsDef>() is RSPhysicsDef physics) && (physics.Group == group))   
                    {
                        body.ApplyForceToCenter(new Vector2(force.X, force.Y));
                    }
                    body = body.GetNext();
                }
            }
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
                bodyDef.position = new Vector2(node.Transformation.Position.X / _worldScale, node.Transformation.Position.Y / _worldScale);
                bodyDef.angle = node.Transformation.Rotation.ToRadians();

                // anchor of nodes added to physics, is forced to center
                //
                node.Transformation.Anchor = new SKPoint(0.5f, 0.5f);

                Body body = _world.CreateBody(bodyDef);
                FixtureDef fixture = new FixtureDef();

                if (node.IsRound == true)
                {
                    CircleShape circle = new CircleShape();
                    circle.Radius = node.Transformation.Size.Width * node.Transformation.Scale.X / (2.0f * _worldScale);
                    fixture.shape = circle;
                }
                else
                {
                    PolygonShape polygon = new PolygonShape();
                    polygon.SetAsBox(
                        node.Transformation.Size.Width * node.Transformation.Scale.X / (2.0f * _worldScale), 
                        node.Transformation.Size.Height * node.Transformation.Scale.Y / (2.0f * _worldScale));
                    fixture.shape = polygon;
                }

                float breakEnergy = ((data.Length >= 1) && (data[0] is float value)) ? value : ENERGY_INFINITE;

                fixture.density = ((data.Length >= 2) && (data[1] is float density)) ? density : DEFAULT_DENSITY; ;
                fixture.friction = ((data.Length >= 3) && (data[2] is float friction)) ? friction : DEFAULT_FRICTION;
                fixture.restitution = ((data.Length >= 4) && (data[3] is float restitution)) ? restitution : DEFAULT_RESTITUTION;

                body.CreateFixture(fixture);

                RSPhysicsDef physics = RSPhysicsDef.CreateWithNode(node, body, breakEnergy);
                body.SetUserData(physics);

                return physics;
            }
        }

        // ********************************************************************************************
        // Contact Callbacks

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

            RSPhysicsDef physicsA = bodyA.GetUserData<RSPhysicsDef>();
            RSPhysicsDef physicsB = bodyB.GetUserData<RSPhysicsDef>();

            Vector2 positionA = bodyA.GetPosition();
            Vector2 positionB = bodyB.GetPosition();

            // check for directional collision
            bool acceptCollision = true;
            if (physicsA.CollisionType != RSCollisionType.Normal)
            {
                if ((physicsA.CollisionType == RSCollisionType.Above) && (positionB.Y < positionA.Y)) acceptCollision = false;
                if ((physicsA.CollisionType == RSCollisionType.Below) && (positionB.Y > positionA.Y)) acceptCollision = false;
            }
            if (physicsB.CollisionType != RSCollisionType.Normal)
            {
                if ((physicsB.CollisionType == RSCollisionType.Above) && (positionA.Y < positionB.Y)) acceptCollision = false;
                if ((physicsB.CollisionType == RSCollisionType.Below) && (positionA.Y > positionB.Y)) acceptCollision = false;
            }

            if (acceptCollision == true)
            {
                // if half the energy is above breaking energy, store it
                if ((energy / 2.0f) >= physicsA.BreakEnergy)
                {
                    _contactList[bodyA] = energy;
                }

                if ((energy / 2.0f) >= physicsB.BreakEnergy)
                {
                    _contactList[bodyB] = energy;
                }
            }

            contact.Enabled = acceptCollision;
        }

        public override void PostSolve(in Contact contact, in ContactImpulse impulse)
        {
            var bodyA = contact.FixtureA.Body;
            var bodyB = contact.FixtureB.Body;

            float initialEnergy;
            float impulseEnergy = IMPULSE_ENERGY_GAIN * (impulse.normalImpulses[0] + impulse.normalImpulses[1]);

            // calculate energy of both bodies
            float energy = KINETIC_ENERGY_GAIN * (0.5f * bodyA.GetMass() * bodyA.GetLinearVelocity().LengthSquared()) + (0.5f * bodyB.GetMass() * bodyB.GetLinearVelocity().LengthSquared());

            _contactList.TryGetValue(bodyA, out initialEnergy);
            {
                // The energy lost in A, is half of the total energy lost
                // Now, this is not correct if one of the bodies are bouncy bouncy, but this is not the real world ...
                //
                float collisionEnergyA = Math.Abs(initialEnergy - energy) / 2.0f;
                
                // if energy is above breaking energy, the body is added to the kill list
                RSPhysicsDef physicsA = bodyA.GetUserData<RSPhysicsDef>();
                physicsA.SaveImpuseEnergy(impulseEnergy);

                if ((collisionEnergyA + impulseEnergy + physicsA.StaticEnergy > physicsA.BreakEnergy) && (_bodyKillList.Contains(bodyA) == false))
                    _bodyKillList.Add(bodyA);

                // contact always removed
                _contactList.Remove(bodyA);
            }

            _contactList.TryGetValue(bodyB, out initialEnergy);
            {
                float collisionEnergyB = Math.Abs(initialEnergy - energy) / 2.0f;
                RSPhysicsDef physicsB = bodyB.GetUserData<RSPhysicsDef>();
                physicsB.SaveImpuseEnergy(impulseEnergy);
                if ((collisionEnergyB + impulseEnergy + physicsB.StaticEnergy > physicsB.BreakEnergy) && (_bodyKillList.Contains(bodyB) == false))
                    _bodyKillList.Add(bodyB);
                _contactList.Remove(bodyB);
            }
        }

        // ********************************************************************************************
    }
}
