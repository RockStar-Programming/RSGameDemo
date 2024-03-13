
using SkiaSharp;

using Rockstar._SpriteFrame;
using Rockstar._Dictionary;
using Rockstar._CodecJson;
using Rockstar._Array;
using Rockstar._Event;
using Rockstar._CoreMouseButton;
using Rockstar._CoreGame;
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

namespace Rockstar._GameClockDD
{
    public class RSGameClockDD : RSCoreGame
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
        // RSGameClockDD is the data-driven version, loading watch setups from JSON files

        // ********************************************************************************************
        // Constructors

        public static RSGameClockDD Create()
        {
            return new RSGameClockDD();
        }

        private RSGameClockDD()
        {
            // select which watch to load
            //
            // _setup = RSCodecJson.CreateDictionaryWithFilePath("Assets/Quartz.json");
            // _setup = RSCodecJson.CreateDictionaryWithFilePath("Assets/VintageHamilton.json");
            // _setup = RSCodecJson.CreateDictionaryWithFilePath("Assets/OmegaSpeedMaster.json");
            // _setup = RSCodecJson.CreateDictionaryWithFilePath("Assets/RolexSubmariner.json");
            // _setup = RSCodecJson.CreateDictionaryWithFilePath("Assets/ZenithElPrimero.json");
            _setup = RSCodecJson.CreateDictionaryWithFilePath("Assets/SeikoSpringDrive.json");

            _mouse.AddHandler(RSMouseButton.Left, RSMouseEvent.OnAll, OnLeftMouseHandler);
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        // ********************************************************************************************
        // Internal Data

        private RSDictionary _setup;

        private ClockType _clockType;
        private double _bph;
        private MinuteHand _jumpingMinute;

        private RSNode? _clock;
        private RSNode? _clockFace;
        private RSNode? _clockDial;
        private RSNode? _hourHand;
        private RSNode? _minuteHand;
        private RSNode? _secondHand;

        // ********************************************************************************************
        // Enums used in setup json

        private enum ClockType
        {
            Quartz,             // Quartz drive with jumping second hand
            Mechanical,         // Mechanical BHP drive
            SpringDrive         // Seiko Spring Drive
        }

        private enum MinuteHand
        {
            NonJumping,
            Jumping             
        }

        private enum MarkerType
        {
            None,
            Ellipse,
            Rectangle,
            DoubleRectangle
        }

        // ********************************************************************************************
        // Constants (do not alter for now)
        // Used in accurate angle calculations, so made doubles to avoid a lot of casting

        private const double FULL_ROTATION = 360;
        private const double HOURS_PR_ROTATION = 12;
        private const double MINUTES_PR_HOUR = 60;
        private const double SECONDS_PR_MINUTE = 60;
        private const double MILLISECONDS_PR_SECOND = 1000;

        // ********************************************************************************************
        // Fully Data Driven Programming
        // 

        // ********************************************************************************************
        // Methods

        // Builds the clock
        //
        public override void Initialise(SKSize size)
        {
            // load watch base "mechanics"
            _clockType = _setup.GetObject("clock/type", "Quartz").ToEnum<ClockType>();
            _bph = _setup.GetLong("clock/beats_pr_hour", 14400);
            _jumpingMinute = _setup.GetObject("clock/jumping_minute", "NonJumping").ToEnum<MinuteHand>();

            // base clock node
            _clock = RSNode.CreateWithPosition(new SKPoint(size.Width / 2,size.Height / 2));
            _scene.AddChild(_clock);

            // clock face
            _clockFace = CreateFaceNode(RSDictionary.CreateWithObject(_setup.GetObject("face")));
            _clock.AddChild(_clockFace);

            // clock dial
            _clockDial = CreateDialNode(RSDictionary.CreateWithObject(_setup.GetObject("dial")));
            _clock.AddChild(_clockDial);

            // hands
            _hourHand = CreateHandNode(RSDictionary.CreateWithObject(_setup.GetObject("hands/hour")));
            _clock.AddChild(_hourHand);

            _minuteHand = CreateHandNode(RSDictionary.CreateWithObject(_setup.GetObject("hands/minute")));
            _clock.AddChild(_minuteHand);

            _secondHand = CreateHandNode(RSDictionary.CreateWithObject(_setup.GetObject("hands/second")));
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

        public void OnLeftMouseHandler(object sender, RSEventArgs args)
        {
            if (args.Data is SKPoint position)
            {
                if (args.Type is RSMouseEvent.OnReleased)
                {
                    _debugNodeList.Clear();
                }
                else
                {
                    _debugNodeList = _clock.GetHitList(position);
                }
            }
        }

        // ********************************************************************************************
        // Internal Methods

        // Creates the clock
        //
        private RSNode CreateFaceNode(RSDictionary setup)
        {
            RSNode result = RSNode.Create();

            // create basic dial
            // load dial setup
            float radius = setup.GetFloat("radius", 120);
            SKColor color = setup.GetArray("color").ToColor();
            RSNodeSolid dial = RSNodeSolid.CreateEllipse(new SKPoint(), new SKSize(radius * 2, radius * 2), color);
            result.AddChild(dial);

            return result;
        }

        // Creates the clocks basic dial
        //
        private RSNode CreateDialNode(RSDictionary setup)
        {
            // create the base node to place dial elements on
            //
            RSNode result = RSNode.Create();
            float radius;
            SKColor color;

            // create basic dial
            // load dial setup
            radius = setup.GetFloat("radius", 120);
            color = setup.GetArray("color").ToColor();
            RSNodeSolid dial = RSNodeSolid.CreateEllipse(new SKPoint(), new SKSize(radius * 2, radius * 2), color);
            result.AddChild(dial);

            // create hour markers
            //
            result.AddChild(CreateMarkerNode(RSDictionary.CreateWithObject(setup.GetObject("hourmarkers"))));

            // create second markers
            //
            result.AddChild(CreateMarkerNode(RSDictionary.CreateWithObject(setup.GetObject("secondmarkers"))));

            // create dial labels
            //
            RSArray labelList = RSArray.CreateWithObject(setup.GetObject("labels"));
            foreach (object label in labelList)
            {
                RSDictionary labelSetup = RSDictionary.CreateWithObject(label);
                result.AddChild(CreateLabelNode(labelSetup));
            }

            return result;
        }

        private RSNode CreateMarkerNode(RSDictionary setup)
        {
            RSNode result = RSNode.Create();

            // load marker setup data
            float radius = setup.GetFloat("radius", 110);
            SKColor color = setup.GetArray("color").ToColor();
            RSArray typeList = RSArray.CreateWithObject(setup.GetObject("type"));
            long markerCount = setup.GetLong("count", 0);
            SKSize rectangleSize = setup.GetArray("rectangle_size").ToSize();
            SKSize ellipseSize = setup.GetArray("ellipse_size").ToSize();
            float offset = setup.GetFloat("offset", 0);

            // iterate defined markers
            float rotation = 0;
            float rotationStep = (float)FULL_ROTATION / markerCount;
            for (int index = 0; index < markerCount; index++)
            {
                if (typeList.Count > 0)
                {
                    // if not enough types in typeList, repeat the pattern
                    MarkerType type = typeList.GetObject(index % typeList.Count).ToEnum<MarkerType>();

                    switch (type)
                    {
                        case MarkerType.None:
                            break;
                        case MarkerType.Rectangle:

                            SKPoint rectanglePosition = new SKPoint(0, radius).Rotate(rotation);
                            RSNodeSolid rectangle = RSNodeSolid.CreateRectangle(rectanglePosition, rectangleSize, color);
                            rectangle.Transformation.Rotation = rotation;
                            result.AddChild(rectangle);
                            
                            break;
                        case MarkerType.DoubleRectangle:
                   
                            SKPoint leftPosition = new SKPoint(-offset, radius).Rotate(rotation);
                            RSNodeSolid leftRectangle = RSNodeSolid.CreateRectangle(leftPosition, rectangleSize, color);
                            leftRectangle.Transformation.Rotation = rotation;
                            result.AddChild(leftRectangle);

                            SKPoint rightPosition = new SKPoint(offset, radius).Rotate(rotation);
                            RSNodeSolid rightRectangle = RSNodeSolid.CreateRectangle(rightPosition, rectangleSize, color);
                            rightRectangle.Transformation.Rotation = rotation;
                            result.AddChild(rightRectangle);

                            break;
                        case MarkerType.Ellipse:
                        default:

                            SKPoint ellipsePosition = new SKPoint(0, radius).Rotate(rotation);
                            RSNode ellipse = RSNodeSolid.CreateEllipse(ellipsePosition, ellipseSize, color);
                            ellipse.Transformation.Rotation = rotation;
                            result.AddChild(ellipse);
                            
                            break;
                    }
                }
                rotation += rotationStep;
            }

            return result;
        }

        // Creates a clock text
        //
        private RSNodeString CreateLabelNode(RSDictionary setup)
        {
            // load label setup data
            string text = setup.GetString("text", "");
            SKPoint position = setup.GetArray("position").ToVector2();
            RSFont font = setup.GetArray("font").ToFont();
            SKColor color = setup.GetArray("color").ToColor();

            // create label
            RSNodeString result = RSNodeString.CreateString(position, text, font);
            result.Transformation.Color = color;

            return result;
        }

        // Creates a clock hand
        //
        private RSNode CreateHandNode(RSDictionary setup)
        {
            // create the base node to place hand elements on
            RSNode result = RSNode.Create();

            // load hand setup data
            SKSize size = setup.GetArray("size").ToSize();
            float handBase = setup.GetFloat("base", 0);
            SKColor color = setup.GetArray("color").ToColor();
            SKSize tail = setup.GetArray("tail").ToSize();

            // create the hand base with requested rectangleSize and color
            // hand will rotate around anchor point, so this is placed center bottom
            //
            RSNodeSolid handBody = RSNodeSolid.CreateRectangle(new SKPoint(0, 0), new SKSize(size.Width, size.Height), color);
            handBody.Transformation.Anchor = new SKPoint(0.5f, 0.0f);
            result.AddChild(handBody);

            // create a top rounded point and add it to the hand base at the top
            //
            RSNodeSolid handTop = RSNodeSolid.CreateEllipse(new SKPoint(0, (float)size.Height), new SKSize(size.Width, size.Width), color);
            result.AddChild(handTop);

            // create a tail if any defined
            // 
            if (tail.Width > 0)
            {
                RSNodeSolid tailNode = RSNodeSolid.CreateRectangle(new SKPoint(0, 0), new SKSize(tail.Width, tail.Height), color);
                tailNode.Transformation.Anchor = new SKPoint(0.5f, 1.0f);
                result.AddChild(tailNode);
            }

            // create a round base if any defined
            //
            if (handBase > 0)
            {
                RSNodeSolid baseNode = RSNodeSolid.CreateEllipse(new SKPoint(0, 0), new SKSize(handBase, handBase), color);
                result.AddChild(baseNode);
            }

            return result;
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
            if (_jumpingMinute != MinuteHand.Jumping)
            {
                // dont adjust minute hand if jumping minutes
                minuteAngle += anglePrMinute * (second / SECONDS_PR_MINUTE);
            }

            double secondAngle = second * anglePrSecond;
            switch (_clockType)
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
