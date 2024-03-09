﻿using Box2D.NetStandard.Dynamics.Bodies;
using Box2D.NetStandard.Dynamics.Fixtures;
using Rockstar._Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Rockstar._PhysicsDef
{
    public class RSPhysicsDef
    {
        // ********************************************************************************************
        // The physics definition class defines and interacts with the node physics  
        //

        // ********************************************************************************************
        // Constructors

        public static RSPhysicsDef CreateWithNode(RSNode node, Body body, FixtureDef fixture, float breakEnergy)
        {
            return new RSPhysicsDef(node, body, fixture, breakEnergy);
        }

        // ********************************************************************************************

        private RSPhysicsDef(RSNode node, Body body, FixtureDef fixture, float breakEnergy)
        {
            _node = node;
            //_body = body;
            //_fixture = fixture;
            _breakEnergy = breakEnergy;
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        public RSNode Node { get { return _node; } }
        //public Body Body { get { return _body; } }
        //public FixtureDef Fixture { get { return _fixture; } }
        public float BreakEnergy { get { return _breakEnergy; } }

        // ********************************************************************************************
        // Internal Data

        private RSNode _node;
        //private Body _body;
        //private FixtureDef _fixture;
        private float _breakEnergy;

        // ********************************************************************************************
        // Methods

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }
}
