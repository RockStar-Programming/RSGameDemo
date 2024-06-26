﻿
using SkiaSharp;

using Rockstar._Game;
using Rockstar._Nodes;
using Rockstar._Types;

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

namespace Rockstar._GameClock
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

        public static RSGameClock Create()
        {
            return new RSGameClock();
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        // ********************************************************************************************
        // Internal Data

        private RSNode _clock;
        private RSNode _clockFace;
        private RSNode _hourHand;
        private RSNode _minuteHand;
        private RSNode _secondHand;

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

        // The following structs are only intended for holding setupo data
        // This is done to improve readability
        //

        private enum ClockType
        {
            Quartz,             // Quartz drive with jumping second hand
            Mechanical,         // Mechanical BHP drive
            SpringDrive         // Seiko Spring Drive
        }

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

        private enum HourMarkerType
        {
            None,
            Circle,
            Rectangle,
            DoubleRectangle
        }

        private struct ClockFace
        {
            public float Radius;
            public float HourRadius;
            public float SecondRadius;
            public SKColor Color;
            public float SecondMarkerCount;
        }

        private struct ClockMarker
        {
            public SKSize Size;
            public bool Circular;
            public SKColor Color;
            public float Offset;
        }

        private struct ClockText
        {
            public string Name;
            public string Font;
            public float FontSize;
            public bool Bold;
            public SKColor Color;
            public SKPoint Position;
        }

        private struct ClockHand
        {
            public string Name;
            public float Length;
            public float Width;
            public float BaseDiameter;
            public SKColor Color;
            public float TailWidth;
            public float TailLength;
        }

        // ********************************************************************************************
        // These are all the data defining the functionality and design of the clock
        // 

        // defining the "mechanics" of the watch
        //
        private ClockType CLOCK_TYPE = ClockType.Mechanical;
        private BPH BEATS_PR_HOUR = BPH.OmegaSpeedMaster;
        private MinuteHand MINUTE_HAND_JUMPING = MinuteHand.NonJumping;

        // defining the clock face,
        // clock dots start at 12 o'clock
        //
        private readonly HourMarkerType[] DOT_TYPES =
        {
            HourMarkerType.DoubleRectangle,
            HourMarkerType.Circle,
            HourMarkerType.Circle,
            HourMarkerType.Rectangle,
            HourMarkerType.Circle,
            HourMarkerType.Circle,
            HourMarkerType.Rectangle,
            HourMarkerType.Circle,
            HourMarkerType.Circle,
            HourMarkerType.Rectangle,
            HourMarkerType.Circle,
            HourMarkerType.Circle
        };

        private static ClockFace CLOCK_FACE = new ClockFace()
        {
            Radius = 240,
            HourRadius = 200,
            SecondRadius = 228,
            SecondMarkerCount = 180,
            Color = new SKColor(128, 150, 235, 32)
        };

        private static ClockMarker SECOND_MARKER = new ClockMarker()
        {
            Size = new SKSize(2, 12),
            Circular = false,
            Color = SKColors.LightGray
        };

        private static ClockMarker HOUR_DOT_MARKER = new ClockMarker()
        {
            Size = new SKSize(20, 20),
            Circular = true,
            Color = SKColors.DarkGray
        };

        private static ClockMarker HOUR_RECTANGLE_MARKER = new ClockMarker()
        {
            Size = new SKSize(14, 28),
            Circular = false,
            Color = SKColors.DarkGray
        };

        private static ClockMarker HOUR_DOUBLE_RECTANGLE_MARKER = new ClockMarker()
        {
            Size = new SKSize(14, 28),
            Circular = false,
            Color = SKColors.DarkGray,
            Offset = 10
        };

        // defining the text on the face
        //
        private static ClockText MANUFACTURER_STRING = new ClockText()
        {
            Position = new SKPoint(0, 100),
            Name = "RockstaR",
            Font = "Arial Rounded MT",
            FontSize = 24,
            Color = SKColors.White,
            Bold = false
        };

        private static ClockText MODEL_STRING = new ClockText()
        {
            Position = new SKPoint(0, -75),
            Name = "PROGRAMMER",
            Font = "Sans Serif Collection",
            FontSize = 16,
            Color = SKColors.White,
            Bold = false
        };

        private static ClockText TYPE_STRING = new ClockText()
        {
            Position = new SKPoint(0, -90),
            Name = "QUARTZ,CERTIFIED {0} BPH,SPRING DRIVE",
            Font = "Sans Serif Collection",
            FontSize = 10,
            Color = SKColors.Red,
            Bold = true
        };

        // defining the clock hands
        //
        private static ClockHand HOUR_HAND = new ClockHand()
        {
            Name = "hour_hand",
            Length = 130,
            Width = 24,
            BaseDiameter = 40,
            Color = SKColors.DarkGray
        };

        private static ClockHand MINUTE_HAND = new ClockHand()
        {
            Name = "minute_hand",
            Length = 200,
            Width = 16,
            BaseDiameter = 32,
            Color = SKColors.LightGray
        };

        private static ClockHand SECOND_HAND = new ClockHand()
        {
            Name = "second_hand",
            Length = 218,
            Width = 4,
            BaseDiameter = 18,
            Color = SKColors.Red,
            TailWidth = 8,
            TailLength = 32
        };

        // ********************************************************************************************
        // Methods

        // Builds the clock
        //
        public override void Initialize(SKSize size)
        {
            // base node
            _clock = RSNode.Create().SetPosition(400, 300);
            _scene.AddChild(_clock);

            // clock face
            _clockFace = CreateClockFace();
            _clock.AddChild(_clockFace);

            // hands
            _hourHand = CreateClockHand(HOUR_HAND);
            _clock.AddChild(_hourHand);

            _minuteHand = CreateClockHand(MINUTE_HAND);
            _clock.AddChild(_minuteHand);

            _secondHand = CreateClockHand(SECOND_HAND);
            _clock.AddChild(_secondHand);

            UpdateHands();
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

        // ********************************************************************************************
        // Internal Methods

        // Creates the clocks basic face
        //
        private RSNode CreateClockFace()
        {
            // create the base node to place hand elements on
            //
            RSNode result = RSNode.Create();

            // create face
            //
            RSNodeSolid face = RSNodeSolid.CreateEllipse(new SKSize(CLOCK_FACE.Radius * 2, CLOCK_FACE.Radius * 2), CLOCK_FACE.Color);
            face.Name = "face";
            result.AddChild(face);

            // create hour markers
            //
            double rotation = 0;
            double rotationStep = FULL_ROTATION / DOT_TYPES.Length;
            foreach (HourMarkerType type in DOT_TYPES)
            {
                result.AddChild(CreateHourClockMarker(type, new SKPoint(0, CLOCK_FACE.HourRadius), rotation));
                rotation += rotationStep;
            }

            // create second markers
            //
            rotation = 0;
            rotationStep = FULL_ROTATION / CLOCK_FACE.SecondMarkerCount;
            for (int index = 0; index < CLOCK_FACE.SecondMarkerCount; index++)
            {
                result.AddChild(CreateGenericClockMarker(SECOND_MARKER, new SKPoint(0, CLOCK_FACE.SecondRadius), rotation));
                rotation += rotationStep;
            }

            // create face labels
            //
            result.AddChild(CreateText(MANUFACTURER_STRING));
            result.AddChild(CreateText(MODEL_STRING));
            result.AddChild(CreateText(TYPE_STRING));

            return result;
        }

        // Creates an hour marker
        //
        private RSNode CreateHourClockMarker(HourMarkerType type, SKPoint position, double rotation)
        {
            RSNode result;

            switch (type)
            {
                case HourMarkerType.None:
                    result = RSNode.Create();
                    break;
                case HourMarkerType.Rectangle:
                    result = CreateGenericClockMarker(HOUR_RECTANGLE_MARKER, position, rotation);
                    break;
                case HourMarkerType.DoubleRectangle:
                    result = CreateGenericClockMarker(HOUR_DOUBLE_RECTANGLE_MARKER, position, rotation);
                    break;
                case HourMarkerType.Circle:
                default:
                    result = CreateGenericClockMarker(HOUR_DOT_MARKER, position, rotation);
                    break;
            }

            return result;
        }

        // Creates a generic marker
        //
        private RSNode CreateGenericClockMarker(ClockMarker marker, SKPoint position, double rotation)
        {
            RSNode result = RSNode.Create();

            RSNode node;
            SKPoint nodePosition = new SKPoint(position.X - marker.Offset, position.Y).Rotate((float)rotation);
            if (marker.Circular == true)
            {
                node = RSNodeSolid.CreateEllipse(marker.Size, marker.Color).SetPosition(nodePosition);
            }
            else
            {
                node = RSNodeSolid.CreateRectangle(marker.Size, marker.Color).SetPosition(nodePosition);
            }
            node.Transformation.Rotation = (float)rotation;
            result.AddChild(node);

            if (marker.Offset > 0)
            {
                nodePosition = new SKPoint(position.X + marker.Offset, position.Y).Rotate((float)rotation);
                if (marker.Circular == true)
                {
                    node = RSNodeSolid.CreateEllipse(marker.Size, marker.Color).SetPosition(nodePosition);
                }
                else
                {
                    node = RSNodeSolid.CreateRectangle(marker.Size, marker.Color).SetPosition(nodePosition);
                }
                node.Transformation.Rotation = (float)rotation;
                result.AddChild(node);
            }

            result.Name = "marker";
            return result;
        }

        // Creates a clock hand
        //
        private RSNode CreateClockHand(ClockHand hand)
        {
            // create the base node to place hand elements on
            RSNode result = RSNode.Create();

            // create the hand base with requested size and color
            // hand will rotate around anchor point, so this is placed center bottom
            //
            RSNodeSolid handBody = RSNodeSolid.CreateRectangle(new SKSize(hand.Width, hand.Length), hand.Color);
            handBody.Name = hand.Name + ".body";
            handBody.Transformation.Anchor = new SKPoint(0.5f, 0.0f);
            result.AddChild(handBody);

            // create a top rounded point and add it to the hand base at the top
            //
            RSNodeSolid handTop = RSNodeSolid.CreateEllipse(new SKSize(hand.Width, hand.Width), hand.Color).SetPosition(0, hand.Length);
            handTop.Name = hand.Name + ".top";
            result.AddChild(handTop);

            // create a tail if any defined
            // 
            if (hand.TailWidth > 0)
            {
                RSNodeSolid tail = RSNodeSolid.CreateRectangle(new SKSize(hand.TailWidth, hand.TailLength), hand.Color);
                tail.Name = hand.Name + ".tail";
                tail.Transformation.Anchor = new SKPoint(0.5f, 1.0f);
                result.AddChild(tail);
            }

            // create a round base, and add it to the hand base at the bottom
            //
            RSNodeSolid handBase = RSNodeSolid.CreateEllipse(new SKSize(hand.BaseDiameter, hand.BaseDiameter), hand.Color);
            handBase.Name = hand.Name + ".base";
            result.AddChild(handBase);

            return result;
        }

        // Creates a clock text
        private RSNodeString CreateText(ClockText setup)
        {
            RSFont font = RSFont.CreateWithName(setup.Font, setup.FontSize);
            font.Bold = setup.Bold;

            string name = GetTypeName(setup.Name);
            RSNodeString result = RSNodeString.CreateString(name, font).SetPosition(setup.Position);
            result.Name = "text." + name;
            result.Transformation.Color = setup.Color;

            return result;
        }

        // Gets the type name in a comma separated string
        // 
        private string GetTypeName(string name)
        {
            string[] nameList = name.Split(',');
            // if name splits into at least 3, set name according to clock type
            if (nameList.Length >= 3)
            {
                switch (CLOCK_TYPE)
                {
                    case ClockType.Quartz: return nameList[0];
                    case ClockType.Mechanical: return string.Format(nameList[1], (int)BEATS_PR_HOUR);
                    case ClockType.SpringDrive: return nameList[2];
                }
            }
            return name;
        }

        // ********************************************************************************************
        // Updates the hands of the clock
        //
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

            while (hour > HOURS_PR_ROTATION) hour -= HOURS_PR_ROTATION;
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

            _hourHand.Transformation.Rotation = (float)hourAngle;
            _minuteHand.Transformation.Rotation = (float)minuteAngle;
            _secondHand.Transformation.Rotation = (float)secondAngle;
        }

        // ********************************************************************************************
    }
}
