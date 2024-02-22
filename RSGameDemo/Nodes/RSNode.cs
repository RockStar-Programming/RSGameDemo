﻿using System.Collections.Generic;

using Rockstar.Types;
using Rockstar.UWPCanvas;

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

namespace Rockstar.Nodes
{
    public class RSNode
    {
        // ********************************************************************************************
        // RSNode encapsulates the simplest visual screen object which is a colored rectangle

        // ********************************************************************************************
        // Constructors

        public static RSNode Create()
        {
            RSNode result = new RSNode();
            result.InitWithData();
            return result;
        }

        public static RSNode CreateWithPosition(RSVector2 position)
        {
            RSNode result = new RSNode();
            result.InitWithData(position);
            return result;
        }

        public static RSNode CreateWithNode(RSNode node)
        {
            RSNode result = new RSNode();
            result.InitWithNode(node);
            return result;
        }

        protected RSNode()
        { 
        }

        protected void InitWithNode(RSNode node)
        {
            if (node is RSNode)
            {
                InitWithData(node.Transformation);
            }
            else
            {
                InitWithData();
            }
        }

        protected void InitWithData(params object[] data)
        {
            // initialise data 
            _transformation = RSTransformation.CreateWithData(data);

            // data which always is reset
            _children = new List<RSNode>();
        }

        // ********************************************************************************************
        // Properties

        public RSTransformation Transformation { get { return _transformation; } }
        public RSNode Parent { get { return _parent; } }
        public List<RSNode> Children { get { return _children; } }

        // ********************************************************************************************
        // Internal Data

        protected const int DATA_COUNT_MAX = 3;

        protected RSTransformation _transformation;
        protected RSNode _parent;
        protected List<RSNode> _children;

        // ********************************************************************************************
        // Methods

        // Override Render to draw the visual content of a node
        public virtual void Render(RSUWPCanvas canvas)
        {
            canvas.AddTransformation(_transformation.GetTransform());

            // RSNode has no visible representation

        }

        public void AddChild(RSNode node)
        {
            if (_children.Contains(node) == false) 
            {
                node._parent = this;
                _children.Add(node);
            }
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // Override Update to perform visual updates to a node
        // IMPORTANT:
        // This is not intended for any kind of game mechanics
        // Should be used only to update visual apperance of a node
        // Example could be for sprite animations, using a series of images
        protected virtual void Update(long interval)
        {

        }

        // ********************************************************************************************
    }
}
