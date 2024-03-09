using Rockstar.Types;
using System;
using System.Collections.Generic;
using System.Reflection;

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

namespace Rockstar._ActionProperty
{
    public enum RSLerp
    {
        Linear,             // linear lerp
        Exponential,        // fast rise
        InvExponential,     // slow rise
        Elastic,            // 
        Bouncy,             
        Ringing             
    }

    public partial class RSActionProperty
    {
        // ********************************************************************************************
        // RSActionProperty handles actions on RSNodes properties
        // supported properties are float, RSVector2, RSSize
        // NOTE, 
        
        // Changing properties
        // node             the node to perform the action on
        // propertyName     the name of the property 
        // value            the valu to lerp the property to
        // duration         action duration in seconds
        // lerp             lerp transition
        // handler          the handler to call when automation completes

        // ********************************************************************************************
        // Constructors

        protected RSActionProperty InitProperty(
            RSNode node, 
            string propertyName, 
            object value, 
            float duration, 
            RSLerp lerp, 
            RSEvent.Handler handler)
        {
            List<string> propertyList = new List<string>(propertyName.Split('.'));
            _property = node;
            while ((propertyList.Count > 0) && (_property != null))
            {
                _info = _property.GetType().GetProperty(propertyList[0]);
                if ((propertyList.Count > 1) && (_info != null))
                {
                    _property = _info.GetValue(_property);
                }
                propertyList.RemoveAt(0);
            }
            if ((_property != null) && (_info != null))
            {
                AssignLerp(_info.GetValue(_property), value);
            }
            _duration = (long)(duration * 1000);
            _lerp = lerp;
            _runTime = 0;
            _completed = false;

            return this;
        }

        // ********************************************************************************************
        // Properties

        public bool Completed { get { return _completed; } }

        // ********************************************************************************************
        // Internal Data

        private object _property;
        private PropertyInfo _info;

        private RSLerp _lerp;
        private object _lerpFrom;
        private object _lerpTo;

        private float _duration;
        private float _runTime;
        private bool _completed;

        // ********************************************************************************************
        // Methods

        public void Update(float interval)
        {
            if (_completed == false)
            {
                // calculate lerp progress
                float linearProgress = (float)_runTime / (float)_duration;
                float lerpProgress = CalculateLerpProgress(_lerp, linearProgress);

                // advance time
                _runTime += interval;
                if (_runTime >= _duration)
                {
                    _completed = true;
                    lerpProgress = 1.0f;
                }
                
                // property is set here
                Lerp(lerpProgress);

                // check for calling completion handler
                if (_completed == true)
                {
                    
                }
            }
        }

        // Replays the action
        // If action moved from x1,y1 to x2,y2, this movement will be replayed
        public void Replay()
        {
            _completed = false;
            _runTime = 0;
        }

        // Repeat the animation
        // The action will be repeated from the current setting
        public void Repeat()
        {
            AdjustLerp(_lerpFrom, _lerpTo);
            _completed = false;
            _runTime = 0;
        }


        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        private void AssignLerp(object lerpFrom, object lerpTo)
        {
            if (lerpFrom is float)
            {
                _lerpFrom = (float)lerpFrom;
                _lerpTo = (float)lerpTo;
            }
            else if (lerpFrom is RSVector2)
            {
                _lerpFrom = new RSVector2((RSVector2)lerpFrom);
                _lerpTo = new RSVector2((RSVector2)lerpTo);
            }
            else if (lerpFrom is RSSize)
            {
                //_lerpFrom = (RSSize)from;
                //_lerpTo = (RSSize)value;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void AdjustLerp(object lerpFrom, object lerpTo)
        {
            if (lerpFrom is float)
            {
                float difference = (float)_lerpTo - (float)_lerpFrom;
                _lerpFrom = (float)lerpFrom + difference;
                _lerpTo = (float)lerpTo + difference;
            }
            else if (lerpFrom is RSVector2)
            {
                _lerpFrom = new RSVector2((RSVector2)lerpFrom);
                _lerpTo = new RSVector2((RSVector2)lerpTo);
            }
            else if (lerpFrom is RSSize)
            {
                //_lerpFrom = (RSSize)from;
                //_lerpTo = (RSSize)value;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        protected class OmegaZeta
        {
            public float OmegaN;
            public float Zeta;
            public bool SlowStart;
            public OmegaZeta(float omegaN, float zeta, bool slowStart)
            {
                OmegaN = omegaN;
                Zeta = zeta;
                SlowStart = slowStart;
            }
        }

        // the bouncy setups
        private OmegaZeta EXPONENTIAL =     new OmegaZeta(  8.0f, 0.95f, false);
        private OmegaZeta BOUNCY =          new OmegaZeta( 12.0f, 0.60f, true);
        private OmegaZeta ELASTIC =         new OmegaZeta( 15.0f, 0.25f, true);
        private OmegaZeta RINGING =         new OmegaZeta( 30.0f, 0.15f, true);

        private float CalculateLerpProgress(RSLerp lerp, float linearProgress)
        {
            float result = linearProgress;

            switch (lerp)
            {
                case RSLerp.Exponential:
                    result = CalculateBounce(linearProgress, EXPONENTIAL);
                    break;
                case RSLerp.InvExponential:  
                    result = 1.0f - CalculateBounce(1.0f - linearProgress, EXPONENTIAL);
                    break;
                case RSLerp.Bouncy:
                    result = CalculateBounce(linearProgress, BOUNCY);
                    break;
                case RSLerp.Elastic:
                    result = CalculateBounce(linearProgress, ELASTIC);
                    break;
                case RSLerp.Ringing:
                    result = CalculateBounce(linearProgress, RINGING);
                    break;
            }

            return result;
        }

        protected float CalculateBounce(float progress, OmegaZeta setup)
        {
            // math calculates a normalised step response (Thanks chatGPT)
            if (setup.SlowStart == true) progress = progress * progress;
            double zeta2 = setup.Zeta * setup.Zeta;
            double omegaD = setup.OmegaN * (float)Math.Sqrt(1.0f - zeta2);
            double result = 1.0 - (Math.Exp(-setup.Zeta * setup.OmegaN * progress) / Math.Sqrt(1 - zeta2)) * Math.Sin(omegaD * progress + Math.Acos(setup.Zeta));

            return (float)result;
        }

        private void Lerp(float progress)
        {
            if (_lerpTo is float)
            {
                float value = (float)_lerpFrom + (((float)_lerpTo - (float)_lerpFrom) * progress);
                _info.SetValue(_property, value);
            }
            else if (_lerpTo is RSVector2)
            {
                RSVector2 vectorFrom = (RSVector2)_lerpFrom;
                RSVector2 vectorTo = (RSVector2)_lerpTo;
                RSVector2 vector = new RSVector2(
                    vectorFrom.X + ((vectorTo.X - vectorFrom.X) * progress),
                    vectorFrom.Y + ((vectorTo.Y - vectorFrom.Y) * progress));
                _info.SetValue(_property, vector);
            }
            else if (_lerpTo is RSSize)
            {
                //RSSize sizeFrom = (RSSize)_lerpFrom;
                //RSSize sizeTo = (RSSize)_lerpTo;
                //size.Width = sizeFrom.Width + ((sizeTo.Width - sizeFrom.Width) * progress);
                //size.Height = sizeFrom.Height + ((sizeTo.Height - sizeFrom.Height) * progress);
            }
        }

        // ********************************************************************************************
    }
}
