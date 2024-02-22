using System;
using Windows.UI;

using Rockstar.Game;
using Rockstar.Nodes;
using Rockstar.Types;

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

namespace Rockstar.GameClock
{
    public class RSGameClock : RSGame
    {
        // ********************************************************************************************
        // RSGameClock implements a simple clock, capable of simulating three different watch types
        //
        // Quartz: The second hand ticks once every second
        // Mechanical: The second hand ticks with the internal beat of the watch (known as BPH) 
        // Spring Drive: The second hand moves smooth
        //
        // Apart from that, it supports jumping minute hand, sometimes found on expensive watches
        // 
        // Much of the watch functionality and apperance can be set up in the setup section
        // (basic Data Driven Programming)

        // ********************************************************************************************
        // Constructors

        public static RSGameClock CreateWithScene(RSNodeScene scene)
        {
            return new RSGameClock(scene);
        }

        private RSGameClock(RSNodeScene scene) : base(scene)
        { 
        }

        // ********************************************************************************************
        // Properties

        public enum ClockType
        {
            Quartz,             // Quartz drive with jumping second hand
            Mechanical,         // Mechanical BHP drive
            SpringDrive         // Seiko Spring Drive
        }

        // ********************************************************************************************
        // Internal Data

        private enum BPH
        {
            // Mechanical clocks only
            VintageHamilton = 14400,
            VintageRolex = 18000,
            OmegaSpeedMaster = 21600,
            RolexSubmariner = 28800,
            ZenithElPrimaro = 36000
        }

        private enum MinuteHand
        {
            NonJumping,
            Jumping
        }

        private enum DotType
        {
            None,
            Circle,
            Rectangle,
            DoubleRectangle
        }

        private RSNode _clockNode;
        private RSNode _clockFace;
        private RSNode _hourHandNode;
        private RSNode _minuteHandNode;
        private RSNode _secondHandNode;

        // ********************************************************************************************
        // Constants (do not alter for now)
        // Used in accurate angle calculations, so made doubles to avoid a lot of casting

        private const double FULL_ROTATION = 360;
        private const double HOURS_PR_ROTATION = 12;
        private const double MINUTES_PR_HOUR = 60;
        private const double SECONDS_PR_MINUTE = 60;
        private const double MILLISECONDS_PR_SECOND = 1000;

        // ********************************************************************************************
        // Simplest form of Data Driven Programming
        // Data can be changed to alter the apperance and functionality of the clock

        // defining the "mechanics" of the watch
        private ClockType CLOCK_TYPE = ClockType.Mechanical;
        private BPH BEATS_PR_HOUR = BPH.VintageHamilton;
        private MinuteHand MINUTE_HAND_JUMPING = MinuteHand.NonJumping;

        // defining the clock face,
        // clock dots start at 12 o'clock
        private readonly DotType[] DOT_TYPES = 
        {
            DotType.DoubleRectangle,
            DotType.Circle,
            DotType.Circle,
            DotType.Rectangle,
            DotType.Circle,
            DotType.Circle,
            DotType.Rectangle,
            DotType.Circle,
            DotType.Circle,
            DotType.Rectangle,
            DotType.Circle,
            DotType.Circle
        };

        private const float FACE_RADIUS = 240;
        private readonly Color FACE_COLOR = Color.FromArgb(32, 128, 150, 235);
        private const float FACE_HOUR_MARKER_RADIUS = 200;
        private readonly Color HOUR_MARKER_COLOR = Colors.DarkGray;

        private const float FACE_SECOND_MARKER_RADIUS = 228;
        private const float SECOND_MARKER_COUNT = 60;
        private readonly Color SECOND_MARKER_COLOR = Colors.LightGray;

        private const float HOUR_MARKER_DOT_DIAMETER = 20;
        private const float HOUR_MARKER_RECTANGLE_WIDTH = 14;
        private const float HOUR_MARKER_RECTANGLE_HEIGHT = 28;
        private const float HOUR_MARKER_RECTANGLE_OFFSET = 10;

        private const float SECOND_MARKER_RECTANGLE_WIDTH = 2;
        private const float SECOND_MARKER_RECTANGLE_HEIGHT = 12;

        private const string MANUFACTURER_NAME = "RockstaR";
        private const string MANUFACTURER_FONT = "Arial Rounded MT";
        private readonly Color MANUFACTURER_COLOR = Colors.White;
        private const float MANUFACTURER_POSITION = 100;
        private const float MANUFACTURER_FONT_SIZE = 24;

        private const string MODEL_NAME = "PROGRAMMER";
        private const string MODEL_FONT = "Sans Serif Collection";
        private readonly Color MODEL_COLOR = Colors.White;
        private const float MODEL_POSITION = -75;
        private const float MODEL_FONT_SIZE = 16;

        private const string TYPE_NAME_QUARTS = " QUARTZ ";
        private const string TYPE_NAME_MECHANICAL = "CERTIFIED {0} BPH";
        private const string TYPE_NAME_SPRING_DRIVE = "SPRING DRIVE";
        private const string TYPE_FONT = "Sans Serif Collection";
        private readonly Color TYPE_COLOR = Colors.Red;
        private const float TYPE_POSITION = -90;
        private const float TYPE_FONT_SIZE = 10;

        // defining the hands
        private const float HOUR_HAND_WIDTH = 24;
        private const float HOUR_HAND_LENGTH = 130;
        private const float HOUR_HAND_BASE = 40;
        private readonly Color HOUR_HAND_COLOR = Colors.DarkGray;

        private const float MINUTE_HAND_WIDTH = 16;
        private const float MINUTE_HAND_LENGTH = 200;
        private const float MINUTE_HAND_BASE = 32;
        private readonly Color MINUTE_HAND_COLOR = Colors.LightGray;

        private const float SECOND_HAND_WIDTH = 4;
        private const float SECOND_HAND_LENGTH = 218;
        private const float SECOND_HAND_BASE = 18;
        private readonly Color SECOND_HAND_COLOR = Colors.Red;

        private const float SECOND_TAIL_WIDTH = 8;
        private const float SECOND_TAIL_LENGTH = 32;

        private const float SECOND_DOT_WIDTH = 10;
        private readonly Color SECOND_DOT_COLOR = Color.FromArgb(255, 48, 48, 48);

        // ********************************************************************************************
        // Methods

        public override void Initialise()
        {
            base.Initialise();

            // base node
            _clockNode = RSNode.CreateWithPosition(new RSVector2(400, 300));
            _scene.AddChild(_clockNode);

            // clock face
            _clockFace = CreateClockFaceNode();
            _clockNode.AddChild(_clockFace);

            // hands
            _hourHandNode = CreateHandNode(HOUR_HAND_WIDTH, HOUR_HAND_LENGTH, HOUR_HAND_BASE, HOUR_HAND_COLOR);
            _clockNode.AddChild(_hourHandNode);

            _minuteHandNode = CreateHandNode(MINUTE_HAND_WIDTH, MINUTE_HAND_LENGTH, MINUTE_HAND_BASE, MINUTE_HAND_COLOR);
            _clockNode.AddChild(_minuteHandNode);

            _secondHandNode = CreateHandNode(SECOND_HAND_WIDTH, SECOND_HAND_LENGTH, SECOND_HAND_BASE, SECOND_HAND_COLOR);
            _clockNode.AddChild(_secondHandNode);

            // manually add a tail to the second hand
            RSNodeSolid tail = RSNodeSolid.CreateRectangle(new RSVector2(0, 0), new RSSize(SECOND_TAIL_WIDTH, SECOND_TAIL_LENGTH), SECOND_HAND_COLOR);
            tail.Transformation.Anchor = new RSVector2(0.5f, 1.0f);
            _secondHandNode.AddChild(tail);

            // manually add dark center dot to second hand
            RSNodeSolid center = RSNodeSolid.CreateEllipse(new RSVector2(0, 0), new RSSize(SECOND_DOT_WIDTH, SECOND_DOT_WIDTH), SECOND_DOT_COLOR);
            _secondHandNode.AddChild(center);

            UpdateHands();
        }

        public override void Update(long interval)
        {
            base.Update(interval);

            UpdateHands();
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        private RSNode CreateHandNode(float width, float length, float center, Color color)
        {
            // create the base node to place hand elements on
            RSNode result = RSNode.Create();

            // create the hand base with requested size and color
            // hand will rotate around anchor point, so this is placed center bottom
            //
            RSNodeSolid handBody = RSNodeSolid.CreateRectangle(new RSVector2(0, 0), new RSSize(width, length), color);
            handBody.Transformation.Anchor = new RSVector2(0.5f, 0.0f);
            result.AddChild(handBody);

            // create a top rounded point and add it to the hand base at the top
            //
            RSNodeSolid handTop = RSNodeSolid.CreateEllipse(new RSVector2(0, length), new RSSize(width, width), color);
            result.AddChild(handTop);

            // create a round base, and add it to the hand base at the bottom
            //
            RSNodeSolid handBase = RSNodeSolid.CreateEllipse(new RSVector2(0, 0), new RSSize(center, center), color);
            result.AddChild(handBase);

            return result;
        }

        private RSNode CreateClockFaceNode()
        {
            // create the base node to place hand elements on
            //
            RSNode result = RSNode.Create();

            // create face
            //
            RSNodeSolid face = RSNodeSolid.CreateEllipse(new RSVector2(), new RSSize(FACE_RADIUS * 2, FACE_RADIUS * 2), FACE_COLOR);
            result.AddChild(face);

            double rotation = 0;
            double rotationStep = FULL_ROTATION / DOT_TYPES.Length;
            RSSize size;

            // create hour markers
            //
            foreach (DotType type in DOT_TYPES) 
            {
                RSVector2 position = new RSVector2(0, FACE_HOUR_MARKER_RADIUS).Rotate((float)rotation);
                switch (type)
                {
                    case DotType.None:
                        break;
                    case DotType.Rectangle:
                        size = new RSSize(HOUR_MARKER_RECTANGLE_WIDTH, HOUR_MARKER_RECTANGLE_HEIGHT);
                        
                        RSNodeSolid rectangle = RSNodeSolid.CreateRectangle(position, size, HOUR_MARKER_COLOR);
                        rectangle.Transformation.Rotation = (float)rotation;
                        result.AddChild(rectangle);
                        
                        break;
                    case DotType.DoubleRectangle:
                        size = new RSSize(HOUR_MARKER_RECTANGLE_WIDTH, HOUR_MARKER_RECTANGLE_HEIGHT);
                        
                        RSVector2 positionLeft = new RSVector2(-HOUR_MARKER_RECTANGLE_OFFSET, FACE_HOUR_MARKER_RADIUS).Rotate((float)rotation);
                        RSNodeSolid rectangleLeft = RSNodeSolid.CreateRectangle(positionLeft, size, HOUR_MARKER_COLOR);
                        rectangleLeft.Transformation.Rotation = (float)rotation;
                        result.AddChild(rectangleLeft);

                        RSVector2 positionRight = new RSVector2(HOUR_MARKER_RECTANGLE_OFFSET, FACE_HOUR_MARKER_RADIUS).Rotate((float)rotation);
                        RSNodeSolid rectangleRight = RSNodeSolid.CreateRectangle(positionRight, size, HOUR_MARKER_COLOR);
                        rectangleRight.Transformation.Rotation = (float)rotation;
                        result.AddChild(rectangleRight);

                        break;
                    case DotType.Circle:
                    default:
                        RSNodeSolid dot = RSNodeSolid.CreateEllipse(position, new RSSize(HOUR_MARKER_DOT_DIAMETER), HOUR_MARKER_COLOR);
                        dot.Transformation.Rotation = (float)rotation;
                        result.AddChild(dot);
                        
                        break;
                }
                rotation += rotationStep;
            }

            // create second markers
            //
            rotation = 0;
            rotationStep = FULL_ROTATION / SECOND_MARKER_COUNT;
            size = new RSSize(SECOND_MARKER_RECTANGLE_WIDTH, SECOND_MARKER_RECTANGLE_HEIGHT);
            for (int index = 0; index < SECOND_MARKER_COUNT; index++)
            {
                RSVector2 position = new RSVector2(0, FACE_SECOND_MARKER_RADIUS).Rotate((float)rotation);

                RSNodeSolid rectangle = RSNodeSolid.CreateRectangle(position, size, SECOND_MARKER_COLOR);
                rectangle.Transformation.Rotation = (float)rotation;
                result.AddChild(rectangle);

                rotation += rotationStep;
            }

            // create face label
            RSFont font = new RSFont(MANUFACTURER_FONT, MANUFACTURER_FONT_SIZE);
            font.Bold = true;
            RSNodeString label = RSNodeString.CreateString(MANUFACTURER_NAME, new RSVector2(0, MANUFACTURER_POSITION), font);
            label.Transformation.Color = MANUFACTURER_COLOR;
            result.AddChild(label);

            font = new RSFont(MODEL_FONT, MODEL_FONT_SIZE);
            label = RSNodeString.CreateString(MODEL_NAME, new RSVector2(0, MODEL_POSITION), font);
            label.Transformation.Color = MODEL_COLOR;
            result.AddChild(label);

            font = new RSFont(TYPE_FONT, TYPE_FONT_SIZE);
            font.Bold= true;
            string text = "";
            switch (CLOCK_TYPE)
            {
                case ClockType.Quartz: 
                    text = TYPE_NAME_QUARTS; 
                    break;
                case ClockType.Mechanical:
                    text = string.Format(TYPE_NAME_MECHANICAL, (int)BEATS_PR_HOUR);
                    break;
                case ClockType.SpringDrive:
                    text = TYPE_NAME_SPRING_DRIVE;
                    break;
            }
            label = RSNodeString.CreateString(text, new RSVector2(0, TYPE_POSITION), font);
            label.Transformation.Color = TYPE_COLOR;
            result.AddChild(label);


            return result;
        }

        private void UpdateHands() 
        {
            DateTime now = DateTime.Now;

            double hour = now.Hour;
            double minute = now.Minute;
            double second = now.Second;
            double milliSecond = now.Millisecond;
            double beatPrSecond = (double)BEATS_PR_HOUR / (MINUTES_PR_HOUR * SECONDS_PR_MINUTE);

            double anglePrHour = FULL_ROTATION / HOURS_PR_ROTATION;
            double anglePrMinute = FULL_ROTATION / MINUTES_PR_HOUR;
            double anglePrSecond = FULL_ROTATION / SECONDS_PR_MINUTE;
            double anglePrMilliSecond = anglePrSecond / MILLISECONDS_PR_SECOND;
            double anglePrBeat = anglePrSecond / beatPrSecond;

            if (hour > HOURS_PR_ROTATION) hour -= HOURS_PR_ROTATION;
            double hourAngle = hour * anglePrHour;
            hourAngle += anglePrHour * (minute / MINUTES_PR_HOUR);
            hourAngle += anglePrSecond * (second / (SECONDS_PR_MINUTE * HOURS_PR_ROTATION));

            double minuteAngle = minute * anglePrMinute;
            if (MINUTE_HAND_JUMPING != MinuteHand.Jumping)
            {
                // dont adjust minute hand if jumping minutes
                minuteAngle += anglePrMinute * (second / SECONDS_PR_MINUTE);
            }

            double secondAngle = second * anglePrSecond;
            switch (CLOCK_TYPE)
            {
                case ClockType.Quartz:
                    // no second hand adjustment if quartz
                    break;
                case ClockType.Mechanical:
                    // adjust second hand for beats if mechanical watch
                    double beat = Math.Round(milliSecond / (MILLISECONDS_PR_SECOND / beatPrSecond));
                    secondAngle += anglePrBeat * beat;
                    break;
                case ClockType.SpringDrive:
                    // smooth second hand if spring drive watch
                    secondAngle += milliSecond * anglePrMilliSecond;
                    break;
            }

            _minuteHandNode.Transformation.Rotation = (float)minuteAngle;
            _hourHandNode.Transformation.Rotation = (float)hourAngle;
            _secondHandNode.Transformation.Rotation = (float)secondAngle;
        }

        // ********************************************************************************************
    }
}
