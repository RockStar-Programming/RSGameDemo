
using System.Reflection;

using Rockstar._Lerp;

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

namespace Rockstar._LerpProperty
{
    public partial class RSLerpProperty : RSLerp
    {
        // ********************************************************************************************
        // RSLerpProperty handles lerps on class properties
        // supported properties are float, SKPoint, SKSize, SKColor
        //

        // ********************************************************************************************
        // Constructors

        public new static RSLerpProperty Empty()
        {
            return new RSLerpProperty();
        }

        public new static RSLerpProperty Create(float duration, RSLerpType type)
        {
            return new RSLerpProperty(duration, type);
        }

        // ********************************************************************************************

        private RSLerpProperty() : base()
        {
        }

        private RSLerpProperty(float duration, RSLerpType type) : base(duration, type)
        {
            if ((_target == null) || (_info == null))
            {
                _valid = false;
            }
        }

        // ********************************************************************************************
        // Properties

        public object Target { get { return _target; } }
        public PropertyInfo Info { get { return _info; } }

        // ********************************************************************************************
        // Internal Data

        private object _target;
        private PropertyInfo _info;

        // ********************************************************************************************
        // Methods

        public void SetPropertyInfo(object target, PropertyInfo info)
        {
            _target = target;
            _info = info;
        }

        public override void Start(object lerpFrom, object lerpTo, bool relative)
        {
            if ((_info != null) && (_target != null))
            {
                base.Start(lerpFrom, lerpTo, relative);
                _info.SetValue(_target, _value);
            }
        }

        public override void Update(float interval)
        {
            base.Update(interval);
            if ((_valid == true) && (_info != null))
            {
                _info.SetValue(_target, _value);
            }
        }

        public override void Stop()
        {
            if ((_valid == true) && (_info != null))
            {
                base.Stop();
                _info.SetValue(_target, _value);
            }
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }
}
