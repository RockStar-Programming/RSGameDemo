#define LOG_AVAILABLE

using JSon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Json
{
    /*
     * DSArray wraps JSON saved files into an array of any value
     * 
     * Supported values are
     *   boolean, integer, double, string, DSArray, DSDictionary
     */

    public class RSArray : List<object>
    {
        // ********************************************************************
        // Constants

        // ********************************************************************
        // Properties

        // ********************************************************************
        // Internal Data

        private DSJsonLogException _LogException = null;

        // ********************************************************************
        // Constructors

        public RSArray()
        {

        }

        public RSArray(object value)
        {
            SetJsonString(value.ToString());
        }

        public RSArray(params string[] valuelist)
        {
            foreach (string value in valuelist)
            {
                AppendItem(value);
            }
        }

        public RSArray(string fullPath)
        {
            LoadFromFile(fullPath);
        }

        // ********************************************************************
        // Methods

        public bool ValidIndex(int index)
        {
            return (index >= 0) && (index < Count);
        }

        public void SetItem(int index, object item)
        {
            try
            {
                this[index] = item;
            }
            catch (Exception exception)
            {
                if (_LogException != null) _LogException(this, exception);
            }
        }

        public object GetItem(int index)
        {
            try
            {
                return this[index];
            }
            catch (Exception exception)
            {
                if (_LogException != null) _LogException(this, exception);
            }
            return null;
        }

        public object GetFirst()
        {
            return GetItem(0);
        }

        public object GetLast()
        {
            return GetItem(Count - 1);
        }

        public void DeleteFirst()
        {
            DeleteItem(0);
        }

        public void DeleteLast()
        {
            DeleteItem(Count - 1);
        }

        public void DeleteItem(int index)
        {
            try
            {
                RemoveAt(index);
            }
            catch (Exception exception)
            {
                if (_LogException != null) _LogException(this, exception);
            }
        }

        public void InsertItem(int index, object item)
        {
            if (index == Count)
            {
                AppendItem(item);
            }
            else
            {
                try
                {
                    Insert(index, item);
                }
                catch (Exception exception)
                {
                    if (_LogException != null) _LogException(this, exception);
                }
            }
        }

        public void AppendItem(object item)
        {
            try
            {
                Add(item);
            }
            catch (Exception exception)
            {
                if (_LogException != null) _LogException(this, exception);
            }
        }

        public void MoveItem(int fromIndex, int toIndex)
        {

        }

        public void SwapItems(int indexA, int indexB)
        {

        }

        public new int IndexOf(object item)
        {
            for (int index = 0; index < Count; index++)
            {
                object entry = GetItem(index);
                if (entry.ToString().ToLower() == item.ToString().ToLower()) return index;
            }
            return -1;
        }

        public new bool Contains(object item)
        {
            foreach (object entry in this)
            {
                if (entry.ToString().ToLower() == item.ToString().ToLower()) return true;
            }
            return false;
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

                List<object> entryList = JsonSerializer.Deserialize<List<object>>(value, options);

                Clear();
                foreach (object entry in entryList)
                {
                    AppendItem(entry);
                }
            }
            catch (Exception exception)
            {
                if (_LogException != null) _LogException(this, exception);
            }
        }

        public void LoadFromFile(string fullPath)
        {
            try
            {
                StreamReader reader = new StreamReader(fullPath);
                SetJsonString(reader.ReadToEnd());
                reader.Close();
            }
            catch (Exception exception)
            {
                if (_LogException != null) _LogException(this, exception);
            }
        }

        public void SaveToFile(string fullPath)
        {
            try
            {
                StreamWriter writer = new StreamWriter(fullPath);
                writer.WriteLine(GetJsonString());
                writer.Close();
            }
            catch (Exception exception)
            {
                if (_LogException != null) _LogException(this, exception);
            }
        }

        // ********************************************************************
        // EOF
    }
}
