﻿using Rockstar._CoreFile;
using Rockstar._RenderSurface;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Rockstar._Nodes
{
    public class RSNodeAnimation : RSNode
    {
        // ********************************************************************************************
        // Implements bitmap based nodes
        // 

        // ********************************************************************************************
        // Constructors

        public static RSNodeAnimation CreateWithFile(Vector2 position, Size size, string filePath)
        {
            RSNodeAnimation result = new RSNodeAnimation(position, size, filePath);

            return result;
        }

        public RSNodeAnimation(Vector2 position, Size size, string filePath)
        {
            _bitmap = RSCoreFile.ReadAsBitmap(filePath);
            InitWithData(position, size);
            FrameCount = (int)(_bitmap.Width / size.Width) * (int)(_bitmap.Height / size.Height);
            _frame = 0;
            _frameStop = 0;
            _animationInterval = ANIMATION_INTERVAL_MIN;
            _animationTime = 0;
        }


        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        public int Frame { get { return _frame; } set { SetFrame(value); } }
        public int FrameCount { get; }
        public long AnimationInterval { get { return _animationInterval; } set { SetAnimationInternal(value); } }
        public bool Running { get { return (_frameStop == ANIMATION_RUNNING); } }

        // ********************************************************************************************
        // Internal Data

        private const int ANIMATION_INTERVAL_MIN = 50;
        private const int ANIMATION_RUNNING = -1;

        private SKBitmap _bitmap;
        private int _frame;
        private int _frameStop;
        private long _animationInterval;
        private long _animationTime;

        // ********************************************************************************************
        // Methods

        public override bool PointInside(Vector2 screenPosition)
        {
            return PointInsizeRectangle(screenPosition);
        }

        public override void Render(RSRenderSurface surface)
        {
            Vector2 upperLeft = new Vector2((float)-_transformation.Size.Width * _transformation.Anchor.X, (float)-_transformation.Size.Height * (1.0f - _transformation.Anchor.Y));
            surface.DrawBitmap(upperLeft.X, upperLeft.Y, _transformation.Size.Width, _transformation.Size.Height, _frame, _bitmap);
        }

        public override void Update(long interval)
        {
            if (Frame != _frameStop)
            {
                _animationTime += interval;
                if (_animationTime > _animationInterval)
                {
                    Frame = Frame + 1;
                    _animationTime -= _animationInterval;
                }
            }
        }

        public void Start()
        {
            _frameStop = ANIMATION_RUNNING;
            _animationTime = 0;
        }

        public void Stop() 
        {
            _frameStop = Frame;
        }

        public void StopAtFrame(int frame) 
        {
            _frameStop = frame % FrameCount;
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        private void SetFrame(int frame)
        {
            _frame = frame % FrameCount;
        }

        private void SetAnimationInternal(long interval)
        {
            if (interval < ANIMATION_INTERVAL_MIN) interval = ANIMATION_INTERVAL_MIN;
            _animationInterval = interval;
        }

        // ********************************************************************************************
    }
}
