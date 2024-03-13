
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
        // RSLerpProperty handles lerps on primarily RSNodes properties
        // supported properties are float, SKPoint, SKSize
        //
        // Changing properties
        // property         the class which property to operate on 
        // info             the actual property info
        // lerpFrom         the value to lerp the property from
        // lerpTo           the value to lerp the property to
        // duration         action duration in seconds
        // type             lerp type

        // ********************************************************************************************
        // Constructors

        public new static RSLerpProperty Empty()
        {
            return new RSLerpProperty();
        }

        public static RSLerpProperty Create(object? property, PropertyInfo? info, float duration, RSLerpType type)
        {
            return new RSLerpProperty(property, info, duration, type);
        }

        // ********************************************************************************************

        private RSLerpProperty() : base()
        {
        }

        private RSLerpProperty(object? property, PropertyInfo? info, float duration, RSLerpType type) : base(duration, type)
        {
            _property = property;
            _info = info;

            if ((_property == null) || (_info == null))
            {
                _invalid = true;
            }
        }

        // ********************************************************************************************
        // Properties

        public object? Property { get { return _property; } }
        public PropertyInfo? Info { get { return _info; } }

        // ********************************************************************************************
        // Internal Data

        private object? _property;
        private PropertyInfo? _info;

        // ********************************************************************************************
        // Methods

        public override void Start(object lerpFrom, object lerpTo, bool relative)
        {
            if ((_info != null) && (_property != null))
            {
                base.Start(lerpFrom, lerpTo, relative);
                _info.SetValue(_property, _value);
            }
        }

        public override void Update(float interval)
        {
            base.Update(interval);
            if ((_invalid == false) && (_info != null))
            {
                _info.SetValue(_property, _value);
            }
        }

        public override void Stop()
        {
            if ((_invalid == false) && (_info != null))
            {
                base.Stop();
                _info.SetValue(_property, _value);
            }
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }
}
