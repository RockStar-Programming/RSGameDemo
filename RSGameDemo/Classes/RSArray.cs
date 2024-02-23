using Rockstar._Dictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

namespace Rockstar._Array
{
    public class RSArray : IEnumerable<object>
    {
        // ********************************************************************************************
        // RSArray wraps an List<object>
        //
        // Main functionality is to:
        // - Predictable operation and no crashes
        // - Implement meaningful constructors
        // - Implement convenience getters

        // ********************************************************************************************
        // Constructors

        public static RSArray Create() 
        { 
            return new RSArray();
        }

        public static RSArray CreateWithList(List<object> list)
        { 
            return new RSArray(list);
        }

        public static RSArray CreateWithArray(RSArray array)
        {
            return new RSArray(array.Content);
        }

        public static RSArray CreateWithObject(object value)
        {
            if (value is RSArray result) return CreateWithArray(result);
            return RSArray.Create();
        }

        public RSArray(List<object> list = null)
        {
            _content = new List<object>();
            if (list != null)
            {
                foreach (object item in list)
                {
                    _content.Add(item);
                }
            }
        }

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        public List<object> Content { get { return _content; } }
        public int Count { get { return _content.Count; } }

        // ********************************************************************************************
        // Internal Data

        private List<object> _content;

        // ********************************************************************************************
        // Methods 

        // ********************************************************************************************
        // Methods Getters (convenience wrappers)

        public object GetObject(int index, object fallback = null)
        {
            if (GetEntry(index) is object result) return result;
            return fallback;
        }

        public bool GetBool(int index, bool fallback)
        {
            if (GetEntry(index) is bool result) return result;
            return fallback;
        }


        public long GetLong(int index, long fallback)
        {
            if (GetEntry(index) is long result) return result;
            return fallback;
        }

        public double GetDouble(int index, double fallback)
        {
            if (GetEntry(index) is double result) return result;
            return fallback;
        }

        public string GetString(int index, string fallback)
        {
            if (GetEntry(index) is string result) return result;
            return fallback;
        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        // GetEntry is the only methods used to retrieve data from the RSArray
        //
        private object GetEntry(int index)
        {
            try
            {
                return _content[index];
            }
            catch (Exception)
            {
            }

            // if application gets here, an exception was thrown
            return null;
        }

        // ********************************************************************************************
        // IEnumerator Implementation

        public IEnumerator<object> GetEnumerator()
        {
            return ((IEnumerable<object>)_content).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_content).GetEnumerator();
        }

        // ********************************************************************************************
    }
}
