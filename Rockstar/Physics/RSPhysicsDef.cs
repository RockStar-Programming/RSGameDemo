using Box2D.NetStandard.Dynamics.Bodies;
using SkiaSharp;

using Rockstar._Nodes;
using System.Drawing.Printing;

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

namespace Rockstar._PhysicsDef
{
    public class RSPhysicsDef
    {
        // ********************************************************************************************
        // The physics definition class defines and interacts with the node physics  
        //

        // ********************************************************************************************
        // Constructors

        public static RSPhysicsDef CreateWithNode(RSNode node, Body body, float breakEnergy)
        {
            return new RSPhysicsDef(node, body, breakEnergy);
        }

        // ********************************************************************************************

        private RSPhysicsDef(RSNode node, Body body, float breakEnergy)
        {
            _node = node;
            _color = _node.Transformation.Color;
            _body = body;
            _breakEnergy = breakEnergy;
            _bufferPointer = 0;
            _staticEnergyBuffer = new float[ENERGY_BUFFER_SIZE];
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        public RSNode Node { get { return _node; } }
        public float BreakEnergy { get { return _breakEnergy; } }
        public float StaticEnergy { get { return GetStaticEnergy(); } }
        public SKColor Color { get { return _color; } } 
        public float LinearVelocity { get { return _body.GetLinearVelocity().Length(); } }

        // ********************************************************************************************
        // Internal Data

        private const int ENERGY_BUFFER_SIZE = 32;

        private RSNode _node;
        private Body _body;
        private float _breakEnergy;
        private SKColor _color;
        private int _bufferPointer = 0;
        private float[] _staticEnergyBuffer;

        // ********************************************************************************************
        // Methods

        public void SaveImpuseEnergy(float load)
        {
            _staticEnergyBuffer[_bufferPointer] = load;
            _bufferPointer = (_bufferPointer + 1) % ENERGY_BUFFER_SIZE;
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        private float GetStaticEnergy()
        {
            return _staticEnergyBuffer.Sum() / ENERGY_BUFFER_SIZE;
        }

        // ********************************************************************************************
    }
}
