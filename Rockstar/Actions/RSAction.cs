
using System.Reflection;

using Rockstar._Lerp;
using Rockstar._LerpProperty;
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

namespace Rockstar._Action
{
    public enum RSActionType
    {
        Absolute,
        Relative
    }

    public class RSAction
    {
        // ********************************************************************************************
        // Actions can be run either on variables directly, or on class properties
        //

        // ********************************************************************************************
        // Constructors

        public RSAction InitAction(RSNode node, string propertyName, object lerpValue, RSActionType actionType, float duration, RSLerpType lerpType)
        {
            List<string> propertyList = new List<string>(propertyName.Split('.'));
            object? property = node;
            PropertyInfo? info = null;
            while ((propertyList.Count > 0) && (property != null))
            {
                info = property.GetType().GetProperty(propertyList[0]);
                if ((propertyList.Count > 1) && (info != null))
                {
                    property = info.GetValue(property);
                }
                propertyList.RemoveAt(0);
            }
            _node = node;
            _lerp = RSLerpProperty.Create(property, info, duration, lerpType);
            _lerpValue = lerpValue;
            _actionType = actionType;
            return this;
        }

        public RSAction()
        {
            _node = RSNode.Create();
            _lerp = RSLerpProperty.Empty();
        }

        // ********************************************************************************************
        // Class Properties

        public const float INSTANT = 0.0f;

        // ********************************************************************************************
        // Properties

        public RSNode Node { get { return _node; } }
        public RSLerpState State { get { return _lerp.State; } }
        public bool Completed { get { return _lerp.Completed; } }

        // ********************************************************************************************
        // Internal Data

        private RSNode _node;
        private RSLerpProperty _lerp;
        private RSActionType _actionType;
        private object _lerpValue;

        // ********************************************************************************************
        // Methods

        public void Update(float interval)
        {
            _lerp.Update(interval);
        }

        public void Start()
        {
            if ((_lerp.Property != null) && (_lerp.Info != null))
            {
                object? lerpFrom = _lerp.Info.GetValue(_lerp.Property);
                if (lerpFrom != null)
                {
                    _lerp.Start(lerpFrom, _lerpValue, _actionType == RSActionType.Relative);
                }
            }
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }
}
