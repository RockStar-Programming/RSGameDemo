using Rockstar._Actions;
using Rockstar._CodecJson;
using Rockstar._CoreGame;
using Rockstar._CoreMouseButton;
using Rockstar._Event;
using Rockstar._GameClockDDP;
using Rockstar._Nodes;
using Rockstar._Touch;
using Rockstar._Types;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Rockstar._GameSpeedMaster
{
    public class RSGameSpeedMaster : RSCoreGame
    {
        // ********************************************************************************************
        // Brief Class Description
        //
        //

        // ********************************************************************************************
        // Constructors

        public static RSGameSpeedMaster Create()
        {
            return new RSGameSpeedMaster();
        }

        private RSGameSpeedMaster()
        {
            // left mouse button simulates single touches
            //
            _mouse.AddHandler(RSMouseButton.Left, RSMouseEvent.OnAll, RSTouchManager.MouseEvent);

            // Load Omega SpeedMaster
            //

        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        // ********************************************************************************************
        // Internal Data

        private const double FULL_ROTATION = 360;
        private const double HOURS_PR_ROTATION = 12;
        private const double MINUTES_PR_HOUR = 60;
        private const double SECONDS_PR_MINUTE = 60;
        private const double MILLISECONDS_PR_SECOND = 1000;

        private readonly SKPoint CLOCK_POSITION = new SKPoint(400, 300);
        private readonly SKPoint LIGHT_POSITION = new SKPoint(-240, 240);
        private readonly SKPoint START_BUTTON_POSITION = new SKPoint(244, 108);
        private readonly SKPoint RESET_BUTTON_POSITION = new SKPoint(240, -118);
        private readonly SKPoint START_BUTTON_MOVEMENT = new SKPoint(-7, -4);
        private readonly SKPoint RESET_BUTTON_MOVEMENT = new SKPoint(-7, 4);
        private const float SHADOW_ALPHA = 0.8f;
        private const float DIAL_OFFSET = 83.0f;
        private const float CHRONO_MINUTE_MSROT = 1800000.0f;
        private const float CHRONO_HOUR_MSROT = 43200000.0f;
        private const float CHRONO_RESET_TIME = 0.25f;

        private const float SHADOW_LENGTH = 7.0f;
        private const float SHADOW_LENGTH_MINI_DIAL = 3.5f;
        // angle of highlights when sprites have no rotation
        private const float DEFAULT_HIGHLIGHT_ROTATION = 330.0f;

        private int _bph = 21600;

        private RSNodeSprite _startButton;
        private RSNodeSprite _resetButton;
        private RSNodeSprite _case;
        private RSNodeSprite _secondDialEdge;
        private RSNodeSprite _chronoHourDialEdge;
        private RSNodeSprite _chronoMinuteDialEdge;
        private RSNodeSprite _raise;
        private RSNodeSprite _dial;
        private RSNodeSprite _bezel;
        private RSNodeSprite _rim;

        private RSNodeSprite _secondHandShadow;
        private RSNodeSprite _secondHand;

        private RSNodeSprite _chronoHourHandShadow;
        private RSNodeSprite _chronoHourHand;

        private RSNodeSprite _chronoMinuteHandShadow;
        private RSNodeSprite _chronoMinuteHand;

        private RSNodeSprite _hourHandShadow;
        private RSNodeSprite _hourHand;

        private RSNodeSprite _minuteHandShadow;
        private RSNodeSprite _minuteHand;

        private RSNodeSprite _chronoSecondHandShadow;
        private RSNodeSprite _chronoSecondHand;

        private RSNodeSprite _hexalite;

        private RSNodeSprite  _light;

        private Stopwatch _chronoGraph;

        // ********************************************************************************************
        // Methods

        public override void Initialize(SKSize size)
        {
            _scene.Transformation.Color = SKColors.LightGray;

            _case = RSNodeSprite.CreateWithFileAndJson("Assets/speedmaster.png/case")
                .SetPosition(CLOCK_POSITION);
            _scene.AddChild(_case);

            _startButton = RSNodeSprite.CreateWithFileAndJson("Assets/speedmaster.png/top_button")
                .SetPosition(START_BUTTON_POSITION)
                .SetAltitude(-10)
                .AddTouchHandler(OnStartButtonTouched);
            _case.AddChild(_startButton);

            _resetButton = RSNodeSprite.CreateWithFileAndJson("Assets/speedmaster.png/bottom_button")
                .SetPosition(RESET_BUTTON_POSITION)
                .SetAltitude(-10)
                .AddTouchHandler(OnResetButtonTouched);
            _case.AddChild(_resetButton);

            _secondDialEdge = RSNodeSprite.CreateWithFileAndJson("Assets/speedmaster.png/mini_dial").SetPosition(-DIAL_OFFSET, 0);
            _case.AddChild(_secondDialEdge);

            _chronoHourDialEdge = RSNodeSprite.CreateWithFileAndJson("Assets/speedmaster.png/mini_dial").SetPosition(0, -DIAL_OFFSET);
            _case.AddChild(_chronoHourDialEdge);

            _chronoMinuteDialEdge = RSNodeSprite.CreateWithFileAndJson("Assets/speedmaster.png/mini_dial").SetPosition(DIAL_OFFSET, 0);
            _case.AddChild(_chronoMinuteDialEdge);

            _raise = RSNodeSprite.CreateWithFileAndJson("Assets/speedmaster.png/raise");
            _case.AddChild(_raise);

            _dial = RSNodeSprite.CreateWithFileAndJson("Assets/speedmaster.png/dial");
            _case.AddChild(_dial);

            _bezel = RSNodeSprite.CreateWithFileAndJson("Assets/speedmaster.png/bezel");
            _case.AddChild(_bezel);

            _rim = RSNodeSprite.CreateWithFileAndJson("Assets/speedmaster.png/rim");
            _case.AddChild(_rim);

            _secondHandShadow = RSNodeSprite.CreateWithFileAndJson("Assets/speedmaster.png/small_hand_shadow")
                .SetPosition(new SKPoint(-DIAL_OFFSET, 0))
                .SetAlpha(SHADOW_ALPHA);
            _case.AddChild(_secondHandShadow);

            _secondHand = RSNodeSprite.CreateWithFileAndJson("Assets/speedmaster.png/small_hand")
                .SetPosition(-DIAL_OFFSET, 0);
            _case.AddChild(_secondHand);

            _chronoHourHandShadow = RSNodeSprite.CreateWithFileAndJson("Assets/speedmaster.png/small_hand_shadow")
                .SetPosition(new SKPoint(0, -DIAL_OFFSET))
                .SetAlpha(SHADOW_ALPHA);
            _case.AddChild(_chronoHourHandShadow);

            _chronoHourHand = RSNodeSprite.CreateWithFileAndJson("Assets/speedmaster.png/small_hand")
                .SetPosition(0, -DIAL_OFFSET);
            _case.AddChild(_chronoHourHand);

            _chronoMinuteHandShadow = RSNodeSprite.CreateWithFileAndJson("Assets/speedmaster.png/small_hand_shadow")
                .SetPosition(new SKPoint(DIAL_OFFSET, 0))
                .SetAlpha(SHADOW_ALPHA);
            _case.AddChild(_chronoMinuteHandShadow);

            _chronoMinuteHand = RSNodeSprite.CreateWithFileAndJson("Assets/speedmaster.png/small_hand")
                .SetPosition(DIAL_OFFSET, 0);
            _case.AddChild(_chronoMinuteHand);

            _hourHandShadow = RSNodeSprite.CreateWithFileAndJson("Assets/speedmaster.png/hour_hand_shadow")
                .SetAlpha(SHADOW_ALPHA);
            _case.AddChild(_hourHandShadow);
            _hourHand = RSNodeSprite.CreateWithFileAndJson("Assets/speedmaster.png/hour_hand");
            _case.AddChild(_hourHand);

            _minuteHandShadow = RSNodeSprite.CreateWithFileAndJson("Assets/speedmaster.png/minute_hand_shadow")
                .SetAlpha(SHADOW_ALPHA);
            _case.AddChild(_minuteHandShadow);
            _minuteHand = RSNodeSprite.CreateWithFileAndJson("Assets/speedmaster.png/minute_hand");
            _case.AddChild(_minuteHand);

            _chronoSecondHandShadow = RSNodeSprite.CreateWithFileAndJson("Assets/speedmaster.png/second_hand_shadow")
                .SetAlpha(SHADOW_ALPHA);
            _case.AddChild(_chronoSecondHandShadow);
            _chronoSecondHand = RSNodeSprite.CreateWithFileAndJson("Assets/speedmaster.png/second_hand");
            _case.AddChild(_chronoSecondHand);

            _hexalite = RSNodeSprite.CreateWithFileAndJson("Assets/speedmaster.png/hexalite");
            _case.AddChild(_hexalite);

            _light = RSNodeSprite.CreateWithFileAndJson("Assets/speedmaster.png/light")
                .SetPosition(LIGHT_POSITION)
                .AddTouchHandler(OnLightTouched);
            _case.AddChild(_light);

            SetLightSource();

            _chronoGraph = new Stopwatch();
        }

        public override void Resize(SKSize size)
        {

        }

        public override void Update(float interval)
        {
            UpdateHands();
        }

        // ********************************************************************************************
        // Event Handlers

        private void OnStartButtonTouched(object sender, RSEventArgs argument)
        {
            if (argument.Type is RSTouchEvent.Began)
            {
                _startButton.SetPosition(START_BUTTON_POSITION + START_BUTTON_MOVEMENT);
                StartStopChronograph();
            }
            else if (argument.Type is RSTouchEvent.Ended)
            {
                _startButton.SetPosition(START_BUTTON_POSITION);
            }
        }

        private void OnResetButtonTouched(object sender, RSEventArgs argument)
        {
            if (argument.Type is RSTouchEvent.Began)
            {
                _resetButton.SetPosition(RESET_BUTTON_POSITION + RESET_BUTTON_MOVEMENT);
                ResetChronograph();
            }
            else if (argument.Type is RSTouchEvent.Ended)
            {
                _resetButton.SetPosition(RESET_BUTTON_POSITION);
            }
        }

        private void OnLightTouched(object sender, RSEventArgs argument)
        {
            if (argument.Data is SKPoint position)
            {
                position = _case.LocalPosition(position);
                _light.SetPosition(position);

                SetLightSource();
            }
        }

        // ********************************************************************************************
        // Internal Methods

        private void UpdateHands()
        {
            DateTime now = DateTime.Now;

            double hour = now.Hour;
            double minute = now.Minute;
            double second = now.Second;
            double milliSecond = now.Millisecond;
            double beatPrSecond = (double)_bph / (MINUTES_PR_HOUR * SECONDS_PR_MINUTE);

            double anglePrHour = FULL_ROTATION / HOURS_PR_ROTATION;
            double anglePrMinute = FULL_ROTATION / MINUTES_PR_HOUR;
            double anglePrSecond = FULL_ROTATION / SECONDS_PR_MINUTE;
            double anglePrMilliSecond = anglePrSecond / MILLISECONDS_PR_SECOND;
            double anglePrBeat = anglePrSecond / beatPrSecond;

            while (hour > HOURS_PR_ROTATION) hour -= HOURS_PR_ROTATION;
            double hourAngle = hour * anglePrHour;
            hourAngle += anglePrHour * (minute / MINUTES_PR_HOUR);
            hourAngle += anglePrSecond * (second / (SECONDS_PR_MINUTE * HOURS_PR_ROTATION));

            double minuteAngle = minute * anglePrMinute;
            minuteAngle += anglePrMinute * (second / SECONDS_PR_MINUTE);

            double secondAngle = second * anglePrSecond;
            // adjust second hand for beats
            double beat = Math.Round(milliSecond / (MILLISECONDS_PR_SECOND / beatPrSecond));
            secondAngle += anglePrBeat * beat;

            _hourHand.Transformation.Rotation = (float)hourAngle;
            _hourHandShadow.Transformation.Rotation = (float)hourAngle;

            _minuteHand.Transformation.Rotation = (float)minuteAngle;
            _minuteHandShadow.Transformation.Rotation = (float)minuteAngle;

            _secondHand.Transformation.Rotation = (float)secondAngle;
            _secondHandShadow.Transformation.Rotation = (float)secondAngle;

            // calculate chronograph
            if (_chronoGraph.IsRunning == true)
            {
                long time = _chronoGraph.ElapsedMilliseconds;

                secondAngle = (int)(time / 1000) * anglePrSecond;
                // adjust second hand for beats
                beat = Math.Round((time % MILLISECONDS_PR_SECOND) / (MILLISECONDS_PR_SECOND / beatPrSecond));
                secondAngle += anglePrBeat * beat;

                _chronoSecondHand.Transformation.Rotation = (float)secondAngle;
                _chronoSecondHandShadow.Transformation.Rotation = (float)secondAngle;

                minuteAngle = 360.0f * time / CHRONO_MINUTE_MSROT;
                _chronoMinuteHand.Transformation.Rotation = (float)minuteAngle;
                _chronoMinuteHandShadow.Transformation.Rotation = (float)minuteAngle;


                hourAngle = 360.0f * time / CHRONO_HOUR_MSROT;
                _chronoHourHand.Transformation.Rotation = (float)hourAngle;
                _chronoHourHandShadow.Transformation.Rotation = (float)hourAngle;
            }

        }

        private void SetLightSource()
        {
            float hightLightRotation = _light.Transformation.Position.Rotation() - DEFAULT_HIGHLIGHT_ROTATION;

            _raise.Transformation.Rotation = hightLightRotation;
            _rim.Transformation.Rotation = hightLightRotation;
            _secondDialEdge.Transformation.Rotation = hightLightRotation;
            _chronoHourDialEdge.Transformation.Rotation = hightLightRotation;
            _chronoMinuteDialEdge.Transformation.Rotation = hightLightRotation;
            _hexalite.Transformation.Rotation = hightLightRotation;

            SKPoint shadowOffset = new SKPoint(0, SHADOW_LENGTH).Rotate(hightLightRotation + 180.0f);

            _minuteHandShadow.SetPosition(shadowOffset);
            _hourHandShadow.SetPosition(shadowOffset);
            _chronoSecondHandShadow.SetPosition(shadowOffset);

            shadowOffset = new SKPoint(0, SHADOW_LENGTH_MINI_DIAL).Rotate(hightLightRotation + 180.0f);
            
            _secondHandShadow.SetPosition(new SKPoint(-DIAL_OFFSET, 0) + shadowOffset);
            _chronoHourHandShadow.SetPosition(new SKPoint(0, -DIAL_OFFSET) + shadowOffset);
            _chronoMinuteHandShadow.SetPosition(new SKPoint(DIAL_OFFSET, 0) + shadowOffset);
        }

        private void StartStopChronograph()
        {
            if (_chronoGraph.IsRunning == false)
            {
                _chronoGraph.Start();
            }
            else
            { 
                _chronoGraph.Stop();
            }
        }

        private void ResetChronograph()
        {
            if (_chronoGraph.IsRunning == false)
            {
                _chronoGraph.Reset();

                float rotation = _chronoSecondHand.Transformation.Rotation;
                rotation = (int)rotation % 360;
                _chronoSecondHand.Transformation.Rotation = rotation;
                _chronoSecondHandShadow.Transformation.Rotation = rotation;
                float time = (360.0f - rotation) / 360.0f * CHRONO_RESET_TIME;

                _chronoSecondHand.RotateTo(360.0f, time).Run();
                _chronoSecondHandShadow.RotateTo(360.0f, time).Run();

                rotation = _chronoMinuteHand.Transformation.Rotation;
                rotation = (int)rotation % 360;
                _chronoMinuteHand.Transformation.Rotation = rotation;
                _chronoMinuteHandShadow.Transformation.Rotation = rotation;
                time = (360.0f - rotation) / 360.0f * CHRONO_RESET_TIME;

                _chronoMinuteHand.RotateTo(360.0f, time).Run();
                _chronoMinuteHandShadow.RotateTo(360.0f, time).Run();

                rotation = _chronoHourHand.Transformation.Rotation;
                rotation = (int)rotation % 360;
                _chronoHourHand.Transformation.Rotation = rotation;
                _chronoHourHandShadow.Transformation.Rotation = rotation;
                time = (360.0f - rotation) / 360.0f * CHRONO_RESET_TIME;

                _chronoHourHand.RotateTo(360.0f, time).Run();
                _chronoHourHandShadow.RotateTo(360.0f, time).Run();
            }
        }

        // ********************************************************************************************
    }
}
