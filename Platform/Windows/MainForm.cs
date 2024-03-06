
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Diagnostics;

using Rockstar._CoreLoop;

namespace Rockstar
{
    public partial class MainForm : Form
    {
        private SKGLControl _controller;
        private Thread _gameThread;
        private bool _gameRunning;
        private RSCoreLoop _gameLoop;

        public MainForm()
        {
            Text = "RSGameDemo";
            ClientSize = new Size(800, 600);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            FormClosing += OnFormClosing;
            Resize += OnResize;

            _controller = new SKGLControl();
            _controller.Size = this.ClientSize;
            _controller.PaintSurface += OnPaintSurface;
            Controls.Add(_controller);

            _gameLoop = new RSCoreLoop();
            _gameLoop.Initialise(new SKSize(800, 600));

            _gameRunning = true;
            _gameThread = new Thread(() =>
            {
                // sets up a 60 FPS game loop
                Stopwatch stopwatch = Stopwatch.StartNew();
                long nextTick = stopwatch.ElapsedTicks;
                long ticksPerFrame = Stopwatch.Frequency / 60; // 60 FPS

                while (_gameRunning == true)
                {
                    while (stopwatch.ElapsedTicks < nextTick)
                    {
                        // Yield thread to prevent 100% CPU usage, but with minimum delay
                        Thread.Sleep(0);
                    }
                    nextTick += ticksPerFrame;

                    _gameLoop.Update();
                    _controller.Invalidate();
                }
            });

            _gameThread.Start();
        }

        private void OnResize(object? sender, EventArgs e)
        {
            //_controller.Size = Size;
            //_gameLoop.Resize(Size);
        }

        private void OnPaintSurface(object? sender, SKPaintGLSurfaceEventArgs e)
        {
            _gameLoop.Render(e.Surface.Canvas);
        }

        private void OnFormClosing(object? sender, FormClosingEventArgs e)
        {
            _gameRunning = false;
        }

    }
}