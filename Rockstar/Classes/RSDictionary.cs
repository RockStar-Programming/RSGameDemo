
using Rockstar._Array;

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

namespace Rockstar._Dictionary
{
    // ********************************************************************************************
    // DSDictionary wraps a Dictionary<string, RSNode>
    // Keys are NOT case sensitive and always stored as lower case
    // 
    // Main functionality is to:
    // - Predictable operation and no crashes
    // - Implement meaningful constructors
    // - Implement convenience getters
    //
    // Adds support for folder approach in keys, and for fallbacks
    // - GetString("data\\setup\\name", fallback);
    // - Here "data" and "setup" are sub dictionaries

    public partial class RSDictionary
    {

        // ********************************************************************************************
        // Constructors

        public static RSDictionary Create()
        {
            return new RSDictionary();
        }

        public static RSDictionary CreateWithDictionary(Dictionary<string, object>? dictionary)
        {
            return new RSDictionary(dictionary);
        }

        public static RSDictionary CreateWithDictionary(RSDictionary dictionary)
        {
            return new RSDictionary(dictionary.Content);
        }

        public static RSDictionary CreateWithObject(object? value)
        {
            if (value is RSDictionary result) return CreateWithDictionary(result);
            return RSDictionary.Create(); ;
        }

        private RSDictionary(Dictionary<string, object>? value = null)
        {
            _content = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            // the Dictionary to create from, is manually copied into _content
            // This preserves StringComparer.OrdinalIgnoreCase
            //
            if (value != null ) 
            {
                foreach (string key in value.Keys) 
                {
                    _content[key] = value[key];
                }
            }
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        public Dictionary<string, object> Content { get { return _content; } }

        // ********************************************************************************************
        // Internal Data

        private Dictionary<string, object> _content;

        // ********************************************************************************************
        // Methods

        public bool ValidKey(string key)
        {
            return (GetEntry(key) != null);
        }

        // Adds an RSDictionary to the content, overwriting existing keys
        public void Merge(RSDictionary dictionary)
        {
            foreach (string key in dictionary.Content.Keys)
            {
                _content[key] = dictionary.Content[key];
            }
        }

        // Adds a dictionary the the content, but leaves existing keys
        public void Supplement(RSDictionary dictionary)
        {
            foreach (string key in dictionary.Content.Keys)
            {
                if (ValidKey(key) == false)
                {
                    _content[key] = dictionary.Content[key];
                }
            }
        }

        // ********************************************************************************************
        // Methods Getters (convenience wrappers)

        public object GetObject(string key, object? fallback = null)
        {
            if (GetEntry(key) is object result) return result;
            if (fallback != null) return fallback;
            return new object();
        }

        public bool GetBool(string key, bool fallback)
        {
            if (GetEntry(key) is bool result) return result;
            return fallback;
        }


        public long GetLong(string key, long fallback)
        {
            object? result = GetEntry(key);
            if (result is long) return (long)result;
            return fallback;
        }

        public float GetFloat(string key, float fallback)
        {
            return (float)GetDouble(key, fallback);
        }

        public double GetDouble(string key, double fallback)
        {
            object? result = GetEntry(key);
            if (result is long) return (long)result;
            if (result is double) return (double)result;
            return fallback;
        }

        public string GetString(string key, string fallback)
        {
            if (GetEntry(key) is string result) return result;
            return fallback;
        }

        public RSArray GetArray(string key, RSArray? fallback = null)
        {
            if (GetEntry(key) is RSArray result) return result;
            if (fallback == null) return RSArray.Create();
            return fallback;
        }

        public RSDictionary GetDictionary(string key, RSDictionary? fallback = null)
        {
            if (GetEntry(key) is RSDictionary result) return result;
            if (fallback == null) return RSDictionary.Create();
            return fallback;
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // GetEntry is the only methods used to retrieve data from the underlying _content
        // If nested keys are supported here, all getters will automatically support it
        // Nested keys MUST be RSDictionaries, otherwise it does not make sense
        //
        // Ex key = "clock/hands/width"
        // Will load the RSDictionary "clock", then the RSDictionary "hands", then return the RSNode for "width"
        private object? GetEntry(string key)
        {
            if (key == null) return null;

            try
            {
                // set source dictionary
                Dictionary<string, object> source = _content;

                // allow for "/"
                key = key.Replace("/", "\\");
                string[] keyList = key.Split('\\');

                // as long as there are path components in the keyList
                while (keyList.Length > 1)
                {
                    // get next source dictionary
                    source = ((RSDictionary)source[keyList[0]]).Content;
                    // remove first entry
                    keyList = keyList.Skip(1).ToArray();
                }

                // fetch the final value
                return source[keyList[0]];
            }
            catch (Exception)
            {
            }

            // if application gets here, an exception was thrown
            return null;
        }

        // ********************************************************************************************
    }
}
