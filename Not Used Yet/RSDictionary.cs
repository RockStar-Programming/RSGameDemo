using System;
using System.Collections.Generic;

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

namespace Rockstar.Dictionary
{
    public class RSDictionary
    {
        // ********************************************************************************************
        // Brief Class Description
        //
        //

        // ********************************************************************************************
        // Constructors

        public static RSDictionary CreateWithJSON(string filePath = null)
        {
            RSDictionary result = new RSDictionary();
            // read the JSON from file into _dictionary

            return result;
        }

        // ********************************************************************************************
        // Properties
        public bool Empty { get { return _dictionary.Count == 0; } }

        // ********************************************************************************************
        // Internal Data

        private Dictionary<string, object> _dictionary;

        // ********************************************************************************************
        // Methods (Read)

        public bool ReadAsBool(string key, bool fallback = false)
        {
            return Convert.ToBoolean(ReadAsObject(key, fallback));
        }

        public int ReadAsInt(string key, int fallback = default(int))
        {
            return Convert.ToInt32(ReadAsObject(key, fallback));
        }

        public double ReadAsDouble(string key, double fallback = default(double))
        {
            return Convert.ToDouble(ReadAsObject(key, fallback));
        }

        public string ReadAsString(string key, string fallback = null)
        {
            object result = ReadAsObject(key, fallback);
            if (result is string) return (string)result;

            if (fallback != null) return fallback;
            return string.Empty;
        }

        public RSDictionary ReadAsDictionary(string key, RSDictionary fallback = null)
        {
            object result = ReadAsObject(key, fallback);
            if (result is RSDictionary) return (RSDictionary)result;

            if (fallback != null) return fallback;
            return new RSDictionary();
        }

        // ********************************************************************************************
        // Methods (Write)

        public void WriteAsBool(string key, bool value)
        {
            WriteAsObject(key, value);
        }

        public void WriteAsInt(string key, int value)
        {
            WriteAsObject(key, value);
        }

        public void WriteAsDouble(string key, double value)
        {
            WriteAsObject(key, value);
        }

        public void WriteAsString(string key, string value)
        {
            if (value is string)
            {
                WriteAsObject(key, value);
            }
        }

        public void WriteAsDictionary(string key, RSDictionary value)
        {
            if (value is RSDictionary)
            {
                WriteAsObject(key, value);
            }
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        private RSDictionary()
        {
            _dictionary = new Dictionary<string, object>();
        }

        private object ReadAsObject(string key, object fallback)
        {
            object result = null;

            try
            {
                // no exceptions for missing key
                if (_dictionary.ContainsKey(key) == true)
                {
                    result = _dictionary[key];
                }
            }
            catch
            {
            }

            if (result == null) result = fallback;
            return result;
        }

        private void WriteAsObject(string key, object value)
        {
            try
            {
                _dictionary[key] = value;
            }
            catch
            {
            }
        }

        // ********************************************************************************************
    }
}

