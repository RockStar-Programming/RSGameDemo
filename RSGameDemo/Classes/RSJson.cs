using Json;
using System;
using System.IO;

namespace JSon
{

    public delegate void DSJsonLogException(object sender, Exception exception);

    public class RSJson
    {
        // ********************************************************************
        // Constants

        public const string Extension = ".json";

        // ********************************************************************
        // Properties

        // ********************************************************************
        // Internal Data

        private DSJsonLogException _LogException = null;
        private RSJsonString _JsonString;

        // ********************************************************************
        // Constructors

        public RSJson(string fullPath) 
        {
            try 
            {
                string[] lines = File.ReadAllLines(fullPath);
                _JsonString = new RSJsonString(lines);
            }
            catch (Exception exception) 
            {
                if (_LogException != null) _LogException(this, exception);
            }
        }

        // ********************************************************************
        // Methods



        // ********************************************************************
        // Internals
    }
}
