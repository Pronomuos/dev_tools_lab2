using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using MyIniParser.Exceptions;
 

namespace MyIniParser.Parser
{
    internal static class IniParser
    {
        private static readonly string sectionNamePattern = @"\[(?<name>\w+)]";
        private static readonly string valueFieldPattern = @"(?<key>\w+)\s*=\s*(?<value>[\w./]+)";
        private static readonly string commentPattern = @";.+";

        private static readonly Regex SectionNameField = 
            new Regex($@"^\s*{sectionNamePattern}\s*({commentPattern})?$");
        private static readonly Regex ValueField = 
            new Regex($@"^\s*{valueFieldPattern}\s*({commentPattern})?$");
        private static readonly Regex CommentField = 
            new Regex($@"^\s*({commentPattern})?$");

        private static readonly string formatFilePattern = $"[\\w/]+.ini";
        private static readonly Regex FormatFile =
            new Regex($"{formatFilePattern}");

        public static ParsedIniFile Parse(string filePath)
        {
            try
            {
                if (!FormatFile.IsMatch(filePath))
                    throw new InvalidFileFormatException();
                
                if (!File.Exists(filePath))
                    throw new FileNotFoundException();
                
                var builder = new IniFileBuilder();
                var lines = File.ReadAllLines(filePath);
                string curSectionName = null;
                var curSection = new Dictionary<string, string>();
                for (var i = 0; i < lines.Length; ++i)
                {
                    var sectionMatch =  SectionNameField.Match(lines[i]);
                    var propertyMatch = ValueField.Match(lines[i]);
                    var commentMatch = CommentField.Match(lines[i]);
                    if (sectionMatch.Success)
                    {
                        if (curSectionName != null)
                            builder.AddSection(curSectionName, curSection);
                        curSectionName = sectionMatch.Groups["name"].Value;
                    }
                    else if (propertyMatch.Success)
                    {
                        var key = propertyMatch.Groups["key"].Value;
                        var value = propertyMatch.Groups["value"].Value;
                        curSection[key] = value;
                    }
                    else if (!commentMatch.Success)
                    {
                        throw new InvalidFieldException($"parser is not able to read {i + 1} line");
                    }
                }
                return builder.Build();
            }
            catch (Exception e)
            {
                throw new FileParsingException(e);
            }
        }

    }
}