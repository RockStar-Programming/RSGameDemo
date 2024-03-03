
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

using SkiaSharp;

namespace Rockstar._CoreFile
{
    public static class RSCoreFile
    {
        // ********************************************************************************************
        // File encapsulates normal file handling, and removes exception handling

        // ********************************************************************************************
        // Constructors

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        public static string ApplicationPath { get { return GetApplicationPath(); } }

        // ********************************************************************************************
        // Internal Data

        private static string APPLICATION_ASSETS_FOLDER = "Assets";
        private static string? _applicationPath = null;

        // ********************************************************************************************
        // Methods

        public static string ReadAsString(params string[] pathList)
        {
            try
            {
               return File.ReadAllText(GetAbsolutePath(pathList));
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string[] ReadAsLines(params string[] pathList)
        {
            try
            {
                return File.ReadAllLines(GetAbsolutePath(pathList));
            }
            catch (Exception)
            {
                return Array.Empty<string>();
            }
        }

        public static SKBitmap ReadAsBitmap(params string[] pathList)
        {
            try
            {
                return SKBitmap.Decode(GetAbsolutePath(pathList));
            }
            catch (Exception)
            {
                return SKBitmap.Decode(GetAbsolutePath(@"Assets/default.png"));
            }
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        private static string GetAbsolutePath(params string[] pathList) 
        {
            string filePath = Path.Combine(pathList);
            filePath = Path.Combine(ApplicationPath, filePath);
            // allow for "/" 
            filePath = filePath.Replace("/", "\\");
            return filePath;
        }

        private static string GetApplicationPath()
        {
            if (_applicationPath != null) return _applicationPath;

            _applicationPath = AppDomain.CurrentDomain.BaseDirectory;
            bool applicationPathFound = false;
            while ((applicationPathFound == false) && (_applicationPath != null))
            {
                if (Directory.Exists(Path.Combine(_applicationPath, APPLICATION_ASSETS_FOLDER)) == true)
                {
                    applicationPathFound = true;
                }
                else
                {
                    _applicationPath = Path.GetDirectoryName(_applicationPath);
                }
            }

            if (_applicationPath == null) return "";
            return _applicationPath;
        }

        // ********************************************************************************************
    }
}
