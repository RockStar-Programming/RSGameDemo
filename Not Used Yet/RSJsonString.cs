using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Json
{

    public class RSJsonString
    {
        // ********************************************************************
        // Constants

        // ********************************************************************
        // Properties

        public string Data { get { return _Data; } }

        // ********************************************************************
        // Internal Data

        private string _Data = "";
        private List<int> _LineNumberList = new List<int>();
        private List<int> _CharacterIndexList = new List<int>();

        private enum CommentType
        { 
            None,           // no comment
            Single,         // comment started with //
            Multi           // comment started with /*
        }

        // ********************************************************************
        // Constructors

        public RSJsonString(string[] stringList)
        {
            int lineIndex = 0;
            foreach (string entry in stringList) 
            {
                _Data += entry;
                for (int charIndex = 0; charIndex < entry.Length; charIndex++)
                {
                    _LineNumberList.Add(lineIndex);
                    _CharacterIndexList.Add(charIndex);
                }
                lineIndex++;
            }

            RemoveWhiteSpace();
            RemoveComments();

            RemoveInvalidCharacters();
            RemoveExtraCommas();
            AddMissingCommas();

            Debug.WriteLine(_Data);
        }

        // ********************************************************************
        // Methods

        // ********************************************************************
        // Internals

        private void RemoveWhiteSpace()
        {
            string result = "";
            bool insideQuotationMarks = false;

            for (int index = 0; index < _Data.Length; index++) 
            {
                if (_Data[index] == '\"') 
                { 
                    insideQuotationMarks = !insideQuotationMarks;
                }

                if ((insideQuotationMarks == false) && (char.IsWhiteSpace(_Data[index]) == true))
                {
                    _LineNumberList[index] = -1;
                    _CharacterIndexList[index] = -1;
                }
                else
                {
                    result += _Data[index];   
                }
            }

            // remove all invalid indices
            _LineNumberList.RemoveAll(entry => entry < 0);
            _CharacterIndexList.RemoveAll(entry => entry < 0);

            // assign 
            _Data = result;
        }

        private void RemoveComments()
        {
            string result = "";
            CommentType comment = CommentType.None;
            int commentStartLine = -1;

            for (int index = 0; index < _Data.Length; index++)
            {
                switch (comment)
                {
                    case CommentType.None:
                        comment = CommentAtIndex(index);
                        commentStartLine = _LineNumberList[index];
                        break;
                    case CommentType.Single:
                        // if char is in next line, single commet ends
                        // check for new comment starting immediately
                        if (_LineNumberList[index] == commentStartLine + 1)
                        {
                            comment = CommentAtIndex(index);
                            commentStartLine = _LineNumberList[index];
                        }
                        break;
                    case CommentType.Multi:
                        // Comments can not end on first two characters
                        if (index > 1)
                        {
                            // if multi comment active, wait for */
                            if ((_Data[index - 2] == '*') && (_Data[index -1] == '/'))
                            {
                                comment = CommentAtIndex(index);
                                commentStartLine = _LineNumberList[index];
                            }
                        }
                        break;
                }

                if (comment != CommentType.None)
                {
                    _LineNumberList[index] = -1;
                    _CharacterIndexList[index] = -1;
                }
                else 
                {
                    result += _Data[index];
                }
            }

            // remove all invalid indices
            _LineNumberList.RemoveAll(entry => entry < 0);
            _CharacterIndexList.RemoveAll(entry => entry < 0);

            // assign 
            _Data = result;
        }

        private void RemoveInvalidCharacters()
        {
            string result = "";
            bool insideQuotationMarks = false;

            for (int index = 0; index < _Data.Length; index++)
            {
                if (_Data[index] == '\"')
                {
                    insideQuotationMarks = !insideQuotationMarks;
                }

                if ((insideQuotationMarks == false) && (IsJsonCharacter(_Data[index]) == false) && (IsPartOfAReservedWord(index) == false))
                {
                    Debug.WriteLine("Invalid character \"{0}\" removed at Line:{1} Pos:{2}", _Data[index], _LineNumberList[index] + 1, _CharacterIndexList[index] + 1);
                    _LineNumberList[index] = -1;
                    _CharacterIndexList[index] = -1;
                }
                else
                {
                    result += _Data[index];
                }
            }

            // remove all invalid indices
            _LineNumberList.RemoveAll(entry => entry < 0);
            _CharacterIndexList.RemoveAll(entry => entry < 0);

            // assign 
            _Data = result;
        }

        private void RemoveExtraCommas()
        {
            string result = "";
            bool insideQuotationMarks = false;

            for (int index = 0; index < _Data.Length; index++)
            {
                if (_Data[index] == '\"')
                {
                    insideQuotationMarks = !insideQuotationMarks;
                }

                if ((insideQuotationMarks == false) && 
                    (index < _Data.Length - 2) && 
                    (_Data[index] == ',') && 
                    ((_Data[index + 1] == ',') || (_Data[index + 1] == '}') || (_Data[index + 1] == ']')))
                {
                    Debug.WriteLine("Extra comma removed at Line:{0} Pos:{1}", _LineNumberList[index] + 1, _CharacterIndexList[index] + 1);
                    _LineNumberList[index] = -1;
                    _CharacterIndexList[index] = -1;
                }
                else
                {
                    result += _Data[index];
                }
            }

            // remove all invalid indices
            _LineNumberList.RemoveAll(entry => entry < 0);
            _CharacterIndexList.RemoveAll(entry => entry < 0);

            // assign 
            _Data = result;
        }

        private void AddMissingCommas()
        {
            int index;
            int foundIndex = 0;
            List<int> insertPositions = new List<int>();

            do
            {
                index = _Data.IndexOf("}{", foundIndex);
                if (index < 0) index = _Data.IndexOf("][", foundIndex);
                if (index >= 0)
                {
                    foundIndex = index + 1;
                    // add an extra to char index, as it is AFTER the found character
                    Debug.WriteLine("Missing comma inserted at Line:{0} Pos:{1}", _LineNumberList[index] + 1, _CharacterIndexList[index] + 2);
                    insertPositions.Add(index + 1);
                }
            }
            while (index >= 0);

            for (index = insertPositions.Count - 1; index >= 0; index--) 
            {
                _Data = _Data.Insert(insertPositions[index], ",");
            }
        }

        // ********************************************************************

        private CommentType CommentAtIndex(int index)
        {
            if (index >= _Data.Length - 1) return CommentType.None;
            if ((_Data[index] == '/') && (_Data[index + 1] == '/')) return CommentType.Single;
            if ((_Data[index] == '/') && (_Data[index + 1] == '*')) return CommentType.Multi;
            return CommentType.None;
        }

        private string WordAtPosition(int index)
        {
            string delimiters = "{}[]:,";

            int startIndex = index;
            int endIndex = index;
            while ((startIndex > 0) && (delimiters.Contains(_Data[startIndex - 1]) == false)) startIndex--;
            while ((endIndex < _Data.Length - 1) && (delimiters.Contains(_Data[endIndex]) == false)) endIndex++;

            return _Data.Substring(startIndex, endIndex - startIndex);
        }


        private bool IsJsonCharacter(char character)
        {
            string validCharacters = " \t\n\":,{}}][-.";

            return validCharacters.Contains(character) || Char.IsDigit(character) || character == '-';
        }

        private bool IsPartOfAReservedWord(int index)
        {
            string[] reservedWords = { "true", "false", "null" };

            string word = WordAtPosition(index);
            foreach (string entry in reservedWords)
            {
                if (word == entry) return true;
            }
            return false;
        }

        // ********************************************************************
        // EOF

    }
}
