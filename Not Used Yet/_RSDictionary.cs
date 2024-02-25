using JSon;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;

namespace Json
{
    /*
     * DSDictionary wraps JSON saved files into a dictionary with keys and values
     * Keys are NOT case sensitive and always stored as lower case
     *
     * Keys must be string based
     * Supported values are
     *   boolean, integer, double, string, DSArray, DSDictionary
     *   
     * TODO:
     *   Add support for folder approach
     *   GetString("data/setup/name", fallback);
     *   Here data and setup are sub dictionaries
     *   
     *   Add support for fallback sections
     *   If data/setup/name does not exist, fallbacks are checked
     *     data/fallback/name
     *     fallback/setup/name
     *     fallback/name
     */

    public partial class _RSDictionary : Dictionary<string, object>
    {
        // ********************************************************************
        // Constants

        // ********************************************************************
        // Properties

        public string FilePath { get { return _filePath; } }

        // ********************************************************************
        // Internal Data

        private string _filePath;
        private DSJsonLogException _LogException = null;

        // ********************************************************************
        // Constructors

        public _RSDictionary() : base(StringComparer.OrdinalIgnoreCase)
        {
            
        }

        public _RSDictionary(string fullpath) : base(StringComparer.OrdinalIgnoreCase)
        {
            if (fullpath != null)
            {
                _filePath = Path.GetDirectoryName(fullpath);
                LoadFromFile(fullpath);
            }
        }

        public _RSDictionary(object value) : base(StringComparer.OrdinalIgnoreCase)
        {
            try
            {
                SetJsonString(value.ToString());
            }
            catch (Exception exception)
            {
                if (_LogException != null) _LogException(this, exception);
            }
        }

        // ********************************************************************
        // Methods

        public bool ValidKey(string key)
        {
            try
            {
                return ContainsKey(key);
            }
            catch (Exception exception)
            {
                if (_LogException != null) _LogException(this, exception);
            }
            return false;
        }

        public void VerifyKey(string key, object value)
        {
            if (ValidKey(key) == false)
            {
                SetEntry(key, value);
            }
        }

        public void SetEntry(string key, object entry)
        {
            if (ValidKey(key) == true)
            {
                DeleteKey(key);
            }

            try
            {
                if (entry is RSArray array)
                {
                    Add(key, array.GetJsonString());
                }
                else if (entry is _RSDictionary dictionary)
                {
                    Add(key, dictionary.GetJsonString());
                }
                else if (entry is Size size) 
                {
                    Add(key, "[ " + size.Width.ToString() + ", " + size.Height.ToString() + " ]"); 
                }
                else if (entry is SizeF sizeF)
                {
                    Add(key, "[ " + sizeF.Width.ToString() + ", " + sizeF.Height.ToString() + " ]");
                }
                else if (entry is Point point)
                {
                    Add(key, "[ " + point.X.ToString() + ", " + point.Y.ToString() + " ]");
                }
                else if (entry is PointF pointF)
                {
                    Add(key, "[ " + pointF.X.ToString() + ", " + pointF.Y.ToString() + " ]");
                }
                else
                {
                    Add(key, entry);
                }
            }
            catch (Exception exception)
            {
                if (_LogException != null) _LogException(this, exception);
            }
        }

        public object GetEntry(string key)
        {
            try
            {
                if (ValidKey(key) == true)
                {
                    return this[key];
                }
            }
            catch (Exception exception)
            {
                if (_LogException != null) _LogException(this, exception);
            }
            return null;
        }

        public object GetObject(string key, object fallback)
        {
            object result = GetEntry(key);
            if (result == null) result = fallback;
            return result;
        }

        public void DeleteKey(string key)
        {
            try
            {
                Remove(key);
            }
            catch (Exception exception)
            {
                if (_LogException != null) _LogException(this, exception);
            }
        }

        // merges a merge dictionary into the current dictionary
        // any key from merge will override any current key
        public void Merge(_RSDictionary merge)
        {
            foreach (string key in merge.Keys)
            {
                SetEntry(key, merge[key]);
            }
        }

        public void Set(_RSDictionary set)
        {
            Clear();
            Merge(set);
        }

        // ********************************************************************
        // Various methods

        public string[] GetKeysStartingWith(string search)
        {
            List<string> result = new List<string>();

            foreach (string key in Keys)
            {
                if (key.StartsWith(search) == true)
                {
                    result.Add(key);
                }
            }

            result.Sort();
            return result.ToArray();
        }


        // ********************************************************************
        // Methods serializing

        public string GetJsonString()
        {
            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions();
                options.WriteIndented = true;
                return JsonSerializer.Serialize(this, options);
            }
            catch (Exception exception)
            {
                if (_LogException != null) _LogException(this, exception);
            }
            return "";
        }

        public void SetJsonString(string value)
        {
            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions();
                options.ReadCommentHandling = JsonCommentHandling.Skip;
                options.AllowTrailingCommas = true;

                Dictionary<string, object> entryList = JsonSerializer.Deserialize<Dictionary<string, object>>(value, options);

                foreach (KeyValuePair<string, object> entry in entryList)
                {
                    if (entry.Value != null)
                    {
                        SetEntry(entry.Key, entry.Value);
                    }
                }
            }
            catch (Exception exception)
            {
                if (_LogException != null) _LogException(this, exception);
            }
        }

        public void LoadFromFile(string fullpath)
        {
            try
            {
                _filePath = Path.GetDirectoryName(fullpath);
                StreamReader reader = new StreamReader(fullpath);
                SetJsonString(reader.ReadToEnd());
                reader.Close();
            }
            catch (Exception exception)
            {
                if (_LogException != null) _LogException(this, exception);
            }
        }

        public void SaveToFile(string fullpath)
        {
            try
            {
                _filePath = Path.GetDirectoryName(fullpath);
                StreamWriter writer = new StreamWriter(fullpath);
                writer.WriteLine(GetJsonString());
                writer.Close();
            }
            catch (Exception exception)
            {
                if (_LogException != null) _LogException(this, exception);
            }
        }

        // ********************************************************************
        // Internal stuff

        // ********************************************************************
        // EOF
    }
}
