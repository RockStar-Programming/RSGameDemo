using Newtonsoft.Json.Linq;
using Rockstar._Array;
using Rockstar._Dictionary;
using Rockstar._Nodes;
using Rockstar._Types;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Audio;

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

namespace Rockstar._NodeFactory
{
    public static class RSNodeFactory
    {
        // ********************************************************************************************
        // The node factory creates nodes based on dictionaries
        //

        // ********************************************************************************************
        // Constructors

        public static RSNode Create(RSDictionary setup, string key)
        {
            return CreateNodeTree(setup, key);
        }

        // ********************************************************************************************

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

        private static RSNode CreateNodeTree(RSDictionary setup, string key) 
        {
            // try to get a fallback dictionary
            RSDictionary fallback = setup.GetDictionary(RSKeys.FALLBACK);

            // get the setup dictionary
            RSArray nodeList = setup.GetArray(key, RSArray.Create());
            foreach (RSDictionary nodeSetup in nodeList)
            {
                RSNode result = CreateNode(nodeSetup.GetArray(RSKeys.CLASS, RSArray.Create()));
                result.InitializeNode(nodeSetup);
                return result;
            }



            return RSNode.Create();
        }

        // Creates a node or one of its descendants
        // The array contains the node class name, and any parameters
        //
        private static RSNode CreateNode(RSArray parameters)
        {
            if (parameters.Count > 0)
            {
                switch (parameters.GetString(0, ""))    
                {
                    case "RSNodeSprite":
                        return RSNodeSprite.CreateWithParams(parameters.GetObject(1), parameters.GetObject(2));

                    case "RSNodeString":
                        return RSNodeString.CreateWithParams(parameters.GetObject(1), parameters.GetObject(2));

                }
            }
            // no node was returned yet
            return RSNode.Create();
        }

        // Initializes a class with properties found in the dictionary
        // Following words have special meaning
        // "Class"      is ignored, as class has already been created based on this
        // "Children"   will initiate recursive class creation
        // 
        public static void InitializeNode<T>(this T target, RSDictionary setup) where T : RSNode 
        {
            foreach (string key in setup.Content.Keys)
            {
                switch (key)
                {
                    case RSKeys.CLASS:
                        // ignore, already handled
                        break;
                    case RSKeys.CHILDREN:
                        // add the children array
                        RSArray childList = setup.GetArray(key, RSArray.Create());
                        foreach (RSDictionary childSetup in childList)
                        { 
                            RSNode child = CreateNode(childSetup.GetArray(RSKeys.CLASS, RSArray.Create()));
                            child.InitializeNode(childSetup);
                            target.AddChild(child); 
                        }
                        break;
                    default:
                        // set any other property
                        // remember, the property must be writeable
                        target.SetProperty(key, setup.GetObject(key));
                        break;
                }
            }
        }

        public static void SetProperty<T>(this T target, string key, object value) where T : class
        {

            List<string> propertyList = new List<string>(key.Split('.'));
            object? property = target;
            PropertyInfo? info = null;
            while ((propertyList.Count > 0) && (property != null))
            {
                info = property.GetType().GetProperty(propertyList[0]);
                if ((propertyList.Count > 1) && (info != null))
                {
                    property = info.GetValue(property);
                }
                propertyList.RemoveAt(0);
            }

            if ((property == null) || (info == null))
            {
                Debug.WriteLine("Unknown Property : " + key);
                return;
            }

            if (info.PropertyType == typeof(SKPoint))
            {
                if (value is RSArray array)
                {
                    info.SetValue(property, array.ToPoint());
                }
            }
            else if (info.PropertyType == typeof(SKSize))
            {
                if (value is RSArray array)
                {
                    info.SetValue(property, array.ToSize());
                }
            }
            else if (info.PropertyType == typeof(SKColor))
            {
                if (value is RSArray array)
                {
                    info.SetValue(property, array.ToColor());
                }
            }
            else
            {
                info.SetValue(property, value);
            }
        }

        // ********************************************************************************************
    }
}
