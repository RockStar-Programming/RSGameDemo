
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;

using Rockstar._BaseGame;

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

namespace Rockstar._BaseWindow
{
    public class RSBaseWindow : IFrameworkView, IFrameworkViewSource
    {
        // ********************************************************************************************
        // Responsible for setting up main window and running the game

        // ****************************************
        // Constructors

        public static RSBaseWindow Create()
        {
            return new RSBaseWindow();
        }

        private RSBaseWindow()
        {
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        //  Properties

        // ****************************************
        // Internal Data

        private readonly string CAPTION = "Rockstar Clock";
        private readonly Color CAPTION_BACKGOUND_COLOR = Color.FromArgb(255, 128, 150, 235);

        private int DEFAULT_WIDTH = 800;
        private int DEFAULT_HEIGHT = 600;

        private CoreWindow _window;
        private ApplicationView _view;
        private RSBaseGame _game;

        // ****************************************
        // IFrameworkViewSource implementation

        public IFrameworkView CreateView()
        {
            return this;
        }

        // ****************************************
        // IFrameworkView implementation

        public void Initialize(CoreApplicationView applicationView)
        {
            applicationView.Activated += OnActivated;
        }

        public void Uninitialize()
        {
        }

        public void SetWindow(CoreWindow window)
        {
            _window = window;
            _window.SizeChanged += OnSizeChanged;

            _view = ApplicationView.GetForCurrentView();

            // Customize the title bar appearance
            _view.TitleBar.BackgroundColor = CAPTION_BACKGOUND_COLOR;
            _view.TitleBar.ButtonBackgroundColor = CAPTION_BACKGOUND_COLOR;
            _view.Title = CAPTION;
        }

        public void Load(string entryPoint)
        {
            Size size = new Size(DEFAULT_WIDTH, DEFAULT_HEIGHT);

            _view.TryResizeView(size);

            _game = RSBaseGame.CreateWithWindowAndSize(_window, size);
        }

        public void Run()
        {
            while (true)
            {
                _window.Dispatcher.ProcessEvents(CoreProcessEventsOption.ProcessAllIfPresent);

                _game.Run();
            }
        }

        // ****************************************
        // Event handlers

        private void OnActivated(CoreApplicationView sender, IActivatedEventArgs args)
        {
            _window.Activate();

            //var picker = new Windows.Storage.Pickers.FileOpenPicker
            //{
            //    ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
            //    // SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
            //};
            //picker.FileTypeFilter.Add(".json");

            //Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            //if (file != null)
            //{
            //    // Do something with the selected file
            //}
        }

        private void OnSizeChanged(CoreWindow sender, WindowSizeChangedEventArgs args)
        {
            _game.Resize(args.Size);
        }

        // ****************************************




    }
}
