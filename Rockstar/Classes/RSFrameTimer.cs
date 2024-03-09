
using System.Diagnostics;

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

namespace Rockstar._FrameTimer
{
    public sealed class RSFrameTimer
    {
        // ********************************************************************************************
        // RSGameTimer calculates the interval seconds between frames
        // To do so, call BeginFrame once at the start of each game loop
        //
        // It also calculates FPS over a specified seconds update interval
        // Drops in FPS can be emphasised by setting FPS_ADJUST_FOR_DEVIATION = true

        // ********************************************************************************************
        // Constructors

        public static RSFrameTimer Create()
        {
            return new RSFrameTimer();
        }

        private RSFrameTimer()
        {
            _timer = Stopwatch.StartNew();
            _lastElapsed = 0;
            _interval = 0;
            _fps = 0;
            _intervalBuffer = new List<float>();
            _timeSinceLastUpdate = 0;
        }

        // ********************************************************************************************
        // Class Properties

        // interval in S at which FPS is updated
        public static float FPS_UPDATE_INTERVAL = 0.5f;

        // if true, changes (drops) to FPS will be emphasised
        // this makes it easier to visually spot frame stutter
        public static bool FPS_ADJUST_FOR_DEVIATION = true;

        // ********************************************************************************************
        // Properties

        public float Interval { get { return _interval; } }
        public double FPS { get { return _fps; } }

        // ********************************************************************************************
        // Internal Data

        private Stopwatch _timer;
        private float _lastElapsed;
        private float _interval;
        private double _fps;
        private float _timeSinceLastUpdate;
        private List<float> _intervalBuffer;

        // ********************************************************************************************
        // Methods

        public void BeginFrame()
        {
            // calculate new frame interval in mS since last frame was started
            float elapsed = _timer.ElapsedMilliseconds / 1000.0f;
            _interval = elapsed - _lastElapsed;
            _lastElapsed = elapsed;

            // add interval to buffer
            _intervalBuffer.Add(_interval);

            // if fps interval expired, calculate new fps
            _timeSinceLastUpdate += _interval;
            if (_timeSinceLastUpdate >= FPS_UPDATE_INTERVAL)
            {
                _timeSinceLastUpdate = 0;

                // calculate average interval for the interval buffer
                double sum = 0;
                foreach (float interval in _intervalBuffer) sum += interval;
                double meanInterval = sum / _intervalBuffer.Count;

                // adjust mean interval for deviation
                // this will emphasise changes in fps
                if (FPS_ADJUST_FOR_DEVIATION == true)
                {
                    sum = 0;
                    double squareSum = 0;
                    foreach (long interval in _intervalBuffer)
                    {
                        squareSum += Math.Pow(interval - meanInterval, 2);
                        sum += Math.Abs(interval - meanInterval);
                    }
                    double deviation = Math.Sqrt(squareSum / _intervalBuffer.Count) - (sum / _intervalBuffer.Count);
                    meanInterval += deviation;
                }

                // calculate new fps
                _fps = (meanInterval > 0) ? 1000.0f / meanInterval : 0;
                _intervalBuffer.Clear();
            }
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // ********************************************************************************************
    }
}
