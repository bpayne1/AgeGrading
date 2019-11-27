using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace AgeGrading
{
    class AgeGradeTextReader
    {
        public AgeGradeTextReader()
        {
        }

        private string mText;
        public string Text
        {
            get { return mText; }
            set { mText = value; }
        }

        private List<string> mColumnHeadings;
        public List<string> ColumnHeadings
        {
            get { return mColumnHeadings; }
        }

        private List<int> mColumnWidths;
        public List<int> ColumnWidths
        {
            get { return mColumnWidths; }
        }

        public List<List<string>> GetDelimitedLines(string fileName, int columnsLineNumber, int startingLine, string delimiters)
        {
            if (String.IsNullOrEmpty(fileName)) return null;
#if !USE_STANDARD_PARSE
            TextFieldParser parser = FileSystem.OpenTextFieldParser(fileName, delimiters);
            parser.HasFieldsEnclosedInQuotes = true;
            List<List<string>> lineList = new List<List<string>>();
            int lineNumber = 0;
            while (!parser.EndOfData)
            {
                string[] temp = parser.ReadFields();
                if (temp == null) continue;
                List<string> fields = new List<string>(temp);
                if (fields == null) continue;
                bool isColumnHeader = false;
                if (lineNumber == columnsLineNumber)
                {
                    isColumnHeader = true;
                    mColumnHeadings = fields;
                }
                lineNumber++;
                if (isColumnHeader) continue;
                lineList.Add(fields);
            }
            return lineList;
#else
            string[] lines = File.ReadAllLines(fileName);
            if (lines != null && lines.Length > 0) return GetDelimitedLines(lines, columnsLineNumber, startingLine, delimiters);
            return null;
#endif
        }

        public List<List<string>> GetDelimitedLines(string[] lines, int columnsLineNumber, int startingLine, string delimiters)
        {
            if (lines == null || lines.Length <= 0) return null;
            if (columnsLineNumber < 0 || lines.Length <= columnsLineNumber) columnsLineNumber = 0;
            if (startingLine < 0 || lines.Length <= startingLine) startingLine = 1;
            if (lines.Length <= startingLine) return null;
            if (delimiters == null || delimiters.Length <= 0) delimiters = "\t ";
            string columnHeader = lines[columnsLineNumber].Trim();
            mColumnHeadings = GetFreeFormColumns(columnHeader, delimiters);
            List<List<string>> lineList = new List<List<string>>();
            StringBuilder str = new StringBuilder();
            for (int ii = startingLine; ii < lines.Length; ii++)
            {
                string line = lines[ii];
                if (String.IsNullOrEmpty(line)) break;
                str.AppendLine(line.Trim());
            }
            bool useReader = false;
            if (useReader)
            {
                StringReader reader = new StringReader(str.ToString());
                TextFieldParser parser = new TextFieldParser(reader);
                parser.HasFieldsEnclosedInQuotes = true;
                parser.Delimiters = ToStringArray(delimiters);
                parser.TrimWhiteSpace = true;
                parser.TextFieldType = FieldType.Delimited;
                while (!parser.EndOfData)
                {
                    string[] temp = parser.ReadFields();
                    if (temp == null) continue;
                    List<string> fields = new List<string>(temp);
                    if (fields == null) continue;
                    lineList.Add(fields);
                }
            }
            else
            {
                for (int ii = startingLine; ii < lines.Length; ii++)
                {
                    List<string> fields = GetFreeFormColumns(lines[ii], delimiters);
                    if (fields != null && fields.Count > 0) lineList.Add(fields);
                }
            }
            return lineList;
        }

        private string[] ToStringArray(string str)
        {
            if (String.IsNullOrEmpty(str)) return null;
            string[] array = new string[str.Length];
            for (int ii = 0; ii < str.Length; ii++)
            {
                array.SetValue(str[ii].ToString(), ii);
            }
            return array;
        }

        private List<string> GetFreeFormColumns(string columns, string delimiters)
        {
            if (String.IsNullOrEmpty(columns)) return null;
            List<string> list = new List<string>();
            string[] columnList = columns.Split(delimiters.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (columnList == null || columnList.Length <= 0) return null;
            foreach (string column in columnList)
            {
                string str = column.Trim();
                if (String.IsNullOrEmpty(str)) continue;
                list.Add(str);
            }
            return list;
        }

        private List<string> GetTabSeparatedColumns(string columns)
        {
            if (String.IsNullOrEmpty(columns)) return null;
            List<string> list = new List<string>();
            string[] columnList = columns.Split("\t".ToCharArray());
            if (columnList == null || columnList.Length <= 0) return null;
            foreach (string column in columnList)
            {
                string str = column.Trim();
                if (String.IsNullOrEmpty(str)) continue;
                list.Add(str);
            }
            return list;
        }

        private string[] TrimLines(string[] lines)
        {
            int longestLength = 0;
            List<string> trimList = new List<string>();
            foreach (var item in lines)
            {
                string line = item.TrimEnd();
                if (line.Length > longestLength)
                    longestLength = line.Length;
                trimList.Add(line);
            }
            List<string> list = new List<string>();
            foreach (var item in trimList)
            {
                string line = item;
                if (line.Length < longestLength)
                {
                    // Add spaces to the end.
                    line = line.PadRight(longestLength);
                }
                list.Add(line);
            }
            return list.ToArray();
        }

        public List<List<string>> GetFixedColumnLines(string[] lines, int columnsLineNumber, int startingLine)
        {
            if (lines == null || lines.Length <= 0) return null;
            if (columnsLineNumber < 0 || lines.Length <= columnsLineNumber) columnsLineNumber = 0;
            if (startingLine < 0 || lines.Length <= startingLine) startingLine = 1;
            if (lines.Length <= startingLine) return null;
            lines = TrimLines(lines);
            string columnHeader = lines[columnsLineNumber];
            string widthColumns = columnHeader;
            int widthColumn = columnsLineNumber + 1;
            if (startingLine > 0 && widthColumn < lines.Length)
            {
                if (lines[widthColumn].Contains('=')) widthColumns = lines[widthColumn];
            }
            mColumnWidths = GetFixedColumnWidths(columnHeader, widthColumns, out mColumnHeadings);
            if (mColumnWidths == null || mColumnWidths.Count <= 0) return null;
            if (mColumnHeadings == null || mColumnHeadings.Count <= 0) return null;
            List<List<string>> lineList = new List<List<string>>();
            StringBuilder str = new StringBuilder();
            for (int ii = startingLine; ii < lines.Length; ii++)
            {
                string line = lines[ii];
                if (String.IsNullOrEmpty(line)) break;
                str.AppendLine(line);
            }
            StringReader reader = new StringReader(str.ToString());
            TextFieldParser parser = new TextFieldParser(reader);
            parser.FieldWidths = mColumnWidths.ToArray<int>();
            parser.TextFieldType = FieldType.FixedWidth;
            try
            {
                while (!parser.EndOfData)
                {
                    string[] temp = parser.ReadFields();
                    if (temp == null) continue;
                    List<string> fields = new List<string>(temp);
                    if (fields == null) continue;
                    lineList.Add(fields);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Parsing");
            }
            return lineList;
        }

        private List<int> GetFixedColumnWidths(string headingColumns, string widthColumns, out List<string> columnHeadings)
        {
            columnHeadings = new List<string>();
            if (String.IsNullOrEmpty(headingColumns)) return null;
            if (String.IsNullOrWhiteSpace(widthColumns)) widthColumns = headingColumns;
            List<int> widths = new List<int>();
            bool spaceSeen = false;
            int count = 0;
            for (int ii = 0; ii < widthColumns.Length; ii++)
            {
                char character = widthColumns[ii];
                bool isWhiteSpace = char.IsWhiteSpace(character);
                if (spaceSeen && !isWhiteSpace)
                {
                    widths.Add(count);
                    count = 1;
                    spaceSeen = false;
                    continue;
                }
                if (!spaceSeen && isWhiteSpace)
                {
                    spaceSeen = true;
                    count++;
                    continue;
                }
                count++;
            }
            widths.Add(count);
            StringBuilder columnHeading = new StringBuilder();
            int startingIndex = 0;
            for (int ii = 0; ii < widths.Count; ii++)
            {
                int width = widths[ii];
                width = Math.Min(width, headingColumns.Length - 1);
                int lastIndex = (startingIndex + width);
                string temp = String.Empty;
                if (lastIndex < headingColumns.Length)
                    temp = headingColumns.Substring(startingIndex, width);
                else
                    temp = headingColumns.Substring(startingIndex);
                temp = temp.Trim();
                temp = temp.Replace(" ", String.Empty);
                columnHeadings.Add(temp.Trim());
                startingIndex = lastIndex;
            }
#if SHOW_COLUMN_WIDTHS
            for (int ii = 0; ii < columnHeadings.Count; ii++)
            {
                Debug.WriteLine(String.Format("Column {0}::coordinates = {1}",
                    columnHeadings[ii], columnHeadings[ii]));
            }
#endif
            return widths;
        }

    }
}
