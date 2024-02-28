
using System;
using System.IO;
using Windows.Storage;

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

namespace Rockstar._BaseFile
{
    public static class RSBaseFile
    {
        // ********************************************************************************************
        // File encapsulates normal file handling, and removes exception handling

        // ********************************************************************************************
        // Constructors

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        // ********************************************************************************************
        // Internal Data

        // ********************************************************************************************
        // Methods

        public static string ReadAsString(params string[] pathList)
        {
            try
            {
               return File.ReadAllText(GetStoragePath(pathList));
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
                return File.ReadAllLines(GetStoragePath(pathList));
            }
            catch (Exception)
            {
                return new string[0];
            }
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        private static string GetStoragePath(string[] pathList) 
        {
            string filePath = Path.Combine(pathList);
            // allow for "/" 
            filePath = filePath.Replace("/", "\\");
            StorageFolder InstallationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFile file = InstallationFolder.GetFileAsync(filePath).GetAwaiter().GetResult();
            return file.Path;
        }

        // ********************************************************************************************
    }
}
