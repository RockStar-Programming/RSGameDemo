﻿
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

using Rockstar._BaseFile;
using Rockstar._Dictionary;
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

namespace Rockstar._CodecJson
{
    public static class RSCodecJson
    {
        // ********************************************************************************************
        // RSCodecJson supports encoding and decoding JSON files into RSDictionary and RSarray classes
        // It supports any level of nesting
        //
        // IMPORTANT:
        // As RSCodeJson must be aware of RSDictionary and RSArray, it is clear that:
        // - RSCodecJson must be above those classes in the Lepo Principle Pyramid
        // - Because of that, RSDictionary and RSArray CAN NOT be aware of RSCodecJson

        // ********************************************************************************************
        // Constructors

        // top level object is a Dictionary<string, object>
        public static RSDictionary CreateDictionaryWithFilePath(string filePath)
        { 
            try
            {
                string jsonString = RSBaseFile.ReadAsString(filePath);
                object jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonString);

                if (Deserialize(jsonObject) is RSDictionary result) return result;
            }
            catch 
            { 
            }
            return RSDictionary.Create();
        }

        // top level object is a List<object>
        public static RSArray CreateArrayWithFilePath(string filePath)
        {
            try
            {
                string jsonString = RSBaseFile.ReadAsString(filePath);
                object jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonString);

                if (Deserialize(jsonObject) is RSArray result) return result;
            }
            catch
            {
            }
            return RSArray.Create();
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        // ********************************************************************************************
        // Internal Data

        // ********************************************************************************************
        // Methods

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // Recursively deserialise an object
        // object can either be value types (no conversion), or two Json object returned from JsonConvert
        // JObject must hold a dictionary in the format <string, object>
        // JArray most hold a list in the format <object>
        private static object Deserialize(object value)
        {
            object result = value;

            if (value is JObject)
            {
                // Object is a dictionary, so deserialize it into such
                Dictionary<string, object> dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(value.ToString());

                // make key copies, as the key list might be changed on the fly
                List<string> keyList = dictionary.Keys.ToList();

                // iterate keys and deserialize them
                foreach (string newKey in keyList)
                {
                    // Deserialize the object of the dictionary
                    dictionary[newKey] = Deserialize(dictionary[newKey]);
                }

                // return a new RSDictionary
                result = RSDictionary.CreateWithDictionary(dictionary);
            }
            else if (value is JArray)
            {
                // Object is an array, so deserialize it into such
                List<object> array = JsonConvert.DeserializeObject<List<object>>(value.ToString());

                // iterate array and deserialize it
                for (int index = 0; index < array.Count; index++)
                {
                    // deserialize the object in the array
                    array[index] = Deserialize(array[index]);
                }

                // return a new RSArray
                result = new RSArray(array);
            }
            else
            {
                // the object was neither a dictionary nor an array, so no deserialization is required

            }

            return result;
        }


        // ********************************************************************************************
    }
}
