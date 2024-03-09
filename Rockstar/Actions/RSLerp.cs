
using SkiaSharp;

using Rockstar._Types;
using Rockstar._Event;
using OpenTK.Graphics.OpenGL;

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

namespace Rockstar._Lerp
{
    public enum RSLerpType
    {
        Linear,             // linear lerp
        Exponential,        // fast rise
        InvExponential,     // slow rise
        Elastic,            // elastic step response
        Bouncy,             // bouncy step response
        Ringing,            // ringing step response
        FadeInOut           // sinus fade in and out    
    }

    public enum RSLerpState
    {
        Stopped,
        Running,
        Paused
    }

    public class RSLerp
    {
        // ********************************************************************************************
        // Lerps a value from A to B given a lerp type and a time
        //
        // Value types supported
        // - float
        // - SKPoint
        // - SKSize
        // - SKColor

        // ********************************************************************************************
        // Constructors

        public static RSLerp Create(object lerpFrom, object lerpTo, float duration, RSLerpType type, int repeat = 1)
        { 
            return new RSLerp(lerpFrom, lerpTo, duration, type);
        }

        // ********************************************************************************************

        protected RSLerp(object lerpFrom, object lerpTo, float duration, RSLerpType type, int repeat = 1) 
        { 
            _value = null;
            _lerpFrom = lerpFrom;
            _lerpTo = lerpTo;

            _invalid = true;
            if ((_lerpFrom is float) && (_lerpTo is float)) _invalid = false;
            if ((_lerpFrom is SKPoint) && (_lerpTo is SKPoint)) _invalid = false;
            if ((_lerpFrom is SKSize) && (_lerpTo is SKSize)) _invalid = false;
            if ((_lerpFrom is SKColor) && (_lerpTo is SKColor)) _invalid = false;

            _type = type;
            _duration = duration;
            _time = 0.0f;
            _completed = false;
            _state = RSLerpState.Stopped;
            RepeatCounter = repeat;
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        public object? Value { get { return _value; } }
        public RSLerpState State { get { return _state; } } 
        public int RepeatCounter { get; set; }

        // ********************************************************************************************
        // Internal Data

        protected object _lerpFrom;
        protected object _lerpTo;
        protected object? _value;
        protected RSLerpType _type;
        protected float _duration;
        protected float _time;
        protected bool _completed;
        protected bool _invalid;
        protected RSLerpState _state;
 
        // ********************************************************************************************
        // Methods

        public virtual void Update(float interval)
        { 
            if ((_state == RSLerpState.Running) && (_completed == false) && (_invalid == false)) 
            {
                // calculate lerp progress
                float linearProgress = (_time / _duration).ClampNormalised();
                float lerpProgress = CalculateLerpProgress(_type, linearProgress);

                // property is set here
                PerformLerp(lerpProgress);
                _time += interval;
            }
            if (_time >= _duration)
            {
                if (RepeatCounter > 0) RepeatCounter--;
                if (RepeatCounter == 0)
                {
                    Stop();
                } 
                else
                {
                    Start();
                }
            }
        }

        public virtual void Start()
        {
            _time = 0.0f;
            _value = _lerpFrom;
            _state = RSLerpState.Running;
        }

        public virtual void Stop()
        {
            _time = 0.0f;
            _value = _lerpTo;
            _completed = true;
            _state = RSLerpState.Stopped;
        }

        public virtual void Pause()
        {
            _state = RSLerpState.Paused;
        }

        public virtual void Resume()
        {
            _state = RSLerpState.Running;
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

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
        private OmegaZeta EXPONENTIAL = new OmegaZeta(8.0f, 0.95f, false);
        private OmegaZeta BOUNCY = new OmegaZeta(12.0f, 0.60f, true);
        private OmegaZeta ELASTIC = new OmegaZeta(15.0f, 0.25f, true);
        private OmegaZeta RINGING = new OmegaZeta(30.0f, 0.15f, true);

        private float CalculateLerpProgress(RSLerpType lerp, float linearProgress)
        {
            float result = linearProgress;

            switch (lerp)
            {
                case RSLerpType.Exponential:
                    result = CalculateBounce(linearProgress, EXPONENTIAL);
                    break;
                case RSLerpType.InvExponential:
                    result = 1.0f - CalculateBounce(1.0f - linearProgress, EXPONENTIAL);
                    break;
                case RSLerpType.Bouncy:
                    result = CalculateBounce(linearProgress, BOUNCY);
                    break;
                case RSLerpType.Elastic:
                    result = CalculateBounce(linearProgress, ELASTIC);
                    break;
                case RSLerpType.Ringing:
                    result = CalculateBounce(linearProgress, RINGING);
                    break;
                case RSLerpType.FadeInOut:
                    result = CalculateSinus(linearProgress, 0.0f, (float)Math.PI, 0.0f, 1.0f);
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

        protected float CalculateSinus(float progress, float startAngle, float endAngle, float offset, float gain)
        {
            float angle = startAngle + ((endAngle - startAngle) * progress);
            float result = ((float)Math.Sin(angle) * gain) + offset;
            return result;
        }

        private void PerformLerp(float progress)
        {
            if (_lerpTo is float)
            {
                _value = (float)_lerpFrom + (((float)_lerpTo - (float)_lerpFrom) * progress);
            }
            else if (_lerpTo is SKPoint)
            {
                SKPoint vectorFrom = (SKPoint)_lerpFrom;
                SKPoint vectorTo = (SKPoint)_lerpTo;
                _value = new SKPoint(
                    vectorFrom.X + ((vectorTo.X - vectorFrom.X) * progress),
                    vectorFrom.Y + ((vectorTo.Y - vectorFrom.Y) * progress));
            }
            else if (_lerpTo is SKSize)
            {
                SKSize sizeFrom = (SKSize)_lerpFrom;
                SKSize sizeTo = (SKSize)_lerpTo;
                _value = new SKSize(
                    sizeFrom.Width + ((sizeTo.Width - sizeFrom.Width) * progress),
                    sizeFrom.Height + ((sizeTo.Height - sizeFrom.Height) * progress));
            }
            else if (_lerpTo is SKColor)
            {
                SKColor sizeFrom = (SKColor)_lerpFrom;
                SKColor sizeTo = (SKColor)_lerpTo;
                _value = new SKColor(
                    (sizeFrom.Red + ((sizeTo.Red - sizeFrom.Red) * progress)).ClampByte(),
                    (sizeFrom.Green + ((sizeTo.Green - sizeFrom.Green) * progress)).ClampByte(),
                    (sizeFrom.Blue + ((sizeTo.Blue - sizeFrom.Blue) * progress)).ClampByte(),
                    (sizeFrom.Alpha + ((sizeTo.Alpha - sizeFrom.Alpha) * progress)).ClampByte());
            }
        }

        // ********************************************************************************************
    }
}
