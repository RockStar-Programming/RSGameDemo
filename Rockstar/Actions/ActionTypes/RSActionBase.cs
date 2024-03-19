using Rockstar._Lerp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

namespace Rockstar._ActionTypes
{
    public enum RSActionMode
    {
        Absolute,
        Relative
    }

    public class RSActionBase
    {
        // ********************************************************************************************
        // Brief Class Description
        //
        //

        // ********************************************************************************************
        // Constructors

        // ********************************************************************************************
        // Class Properties

        // ********************************************************************************************
        // Properties

        public bool Completed { get { return GetCompleted(); } }

        // ********************************************************************************************
        // Internal Data

        // ********************************************************************************************
        // Methods

        public virtual void Start(object target)
        {

        }

        public virtual void Update(float interval)
        {

        }

        public virtual void Stop()
        {

        }

        // ********************************************************************************************
        // Event Handlers

        // ********************************************************************************************
        // Internal Methods

        protected virtual bool GetCompleted()
        {
            return true;
        }

        // ********************************************************************************************
    }
}
