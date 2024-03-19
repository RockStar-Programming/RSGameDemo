
using System.Reflection;

using Rockstar._Lerp;
using Rockstar._LerpProperty;

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

namespace Rockstar._Operations
{
    public class RSOperationProperty : RSOperation 
    {
        // ********************************************************************************************
        // Actions can be run either on variables directly, or on class properties
        //

        // ********************************************************************************************
        // Constructors

        public RSOperationProperty Init(string propertyName, object lerpValue, RSOperationMode mode, float duration, RSLerpType type)
        {
            _propertyName = propertyName;
            _lerp = RSLerpProperty.Create(duration, type);
            _lerpValue = lerpValue;
            _mode = mode;
            return this;
        }

        public RSOperationProperty()
        {
            _propertyName = "";
            _lerp = RSLerpProperty.Empty();
            _lerpValue = 0;
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        public RSLerpState State { get { return _lerp.State; } }

        // ********************************************************************************************
        // Internal Data

        private string _propertyName;
        private RSLerpProperty _lerp;
        private RSOperationMode _mode;
        private object _lerpValue;

        // ********************************************************************************************
        // Methods

        public override void Start(object target)
        {
            List<string> propertyList = new List<string>(_propertyName.Split('.'));
            object? property = target;
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

            if ((property != null) && (info != null))
            {
                _lerp.SetPropertyInfo(property, info);
            }

            if ((_lerp.Target != null) && (_lerp.Info != null))
            {
                object? lerpFrom = _lerp.Info.GetValue(_lerp.Target);
                if (lerpFrom != null)
                {
                    _lerp.Start(lerpFrom, _lerpValue, _mode == RSOperationMode.Relative);
                }
            }
        }

        public override void Update(float interval)
        {
            _lerp.Update(interval);
        }

        public override RSOperationProperty Copy()
        {
            RSOperationProperty result = new RSOperationProperty().Init(_propertyName, _lerpValue, _mode, _lerp.Duration, _lerp.Type);
            return result;
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        protected override bool GetCompleted()
        {
            return _lerp.Completed;
        }

        // ********************************************************************************************
    }
}
