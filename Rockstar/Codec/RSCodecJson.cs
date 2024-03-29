
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using Rockstar._File;
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
        // RSCodecJson supports decoding JSON files into RSDictionary and RSarray classes
        // It supports any level of nesting
        //
        // IMPORTANT:
        // As RSCodecJson must be aware of RSDictionary and RSArray, it is clear that:
        // - RSCodecJson must be above those classes in the Lepo Principle Pyramid
        // - Because of that, RSDictionary and RSArray CAN NOT be aware of RSCodecJson

        // ********************************************************************************************
        // Constructors

        // top level RSNode is a Dictionary<string, RSNode>
        public static RSDictionary CreateDictionaryWithFilePath(string filePath)
        { 
            try
            {
                string jsonString = RSFile.ReadAsString(filePath);
                object? jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonString);

                if (Deserialize(jsonObject) is RSDictionary result) return result;
            }
            catch 
            { 
            }
            return RSDictionary.Create();
        }

        // top level RSNode is a List<RSNode>
        public static RSArray CreateArrayWithFilePath(string filePath)
        {
            try
            {
                string jsonString = RSFile.ReadAsString(filePath);
                object? jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonString);

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

        // Recursively deserialise an RSNode
        // RSNode can either be value types (no conversion), or two Json RSNode returned from JsonConvert
        // JObject must hold a dictionary in the format <string, RSNode>
        // JArray most hold a list in the format <RSNode>
        private static object Deserialize(object? value)
        {
            object? result = value;

            if (value is JObject dictionaryObject)
            {
                // RANT
                // See, this is why we hate all this null checking shit exception crap ...
                // Since value came from JsonConvert.DeserializeObject, and since it just passed as being a JObject
                // it MUST be a ¤#%&!? string, and it obfuscates the code unnessasary to check for this "never going to happen" exception
                // 
                // So pretty pretty please allow methods to take null parameters, and return meaningful fallback values
                // /RANT

                // Object is a dictionary, so deserialize it into such
                Dictionary<string, object>? dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(dictionaryObject.ToString());

                if (dictionary != null)
                {
                    // make key copies, as the key list might be changed on the fly
                    List<string> keyList = dictionary.Keys.ToList();

                    // iterate keys and deserialize them
                    foreach (string newKey in keyList)
                    {
                        // Deserialize the RSNode of the dictionary
                        dictionary[newKey] = Deserialize(dictionary[newKey]);
                    }
                }
                // return a new RSDictionary
                result = RSDictionary.CreateWithDictionary(dictionary);
            }
            else if (value is JArray arrayObject)
            {
                // Object is an array, so deserialize it into such
                List<object>? array = JsonConvert.DeserializeObject<List<object>>(arrayObject.ToString());

                if (array != null)
                {
                    // iterate array and deserialize it
                    for (int index = 0; index < array.Count; index++)
                    {
                        // deserialize the RSNode in the array
                        array[index] = Deserialize(array[index]);
                    }
                }

                // return a new RSArray
                result = RSArray.CreateWithList(array);
            }
            else
            {
                // the RSNode was neither a dictionary nor an array, so no deserialization is required
                if (result == null) result = 0;

            }

            return result;
        }


        // ********************************************************************************************
    }
}
