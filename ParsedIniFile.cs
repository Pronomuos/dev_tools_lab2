using System;
using System.Collections.Generic;
using System.Linq;
using MyIniParser.Exceptions;

namespace MyIniParser.Parser
{
    public class ParsedIniFile
    {
        private Dictionary<string, Dictionary<string, string>> data;

        public ParsedIniFile(Dictionary<string, Dictionary<string, string>> data)
        {
            this.data = data;
        }

        public void ClearParsedFile() => data.Clear();

        public void AddSection(string sectionName, Dictionary<string, string> section)
        {
            if (data.ContainsKey(sectionName))
                data[sectionName] = section;
        }

        public void AddValue(string sectionName, string key, string value) =>
            data[sectionName][key] = value;

        public void RemoveSection(string sectionName) => data.Remove(sectionName);
        public void RemoveKey(string sectionName, string key) => data[sectionName].Remove(key);

        public void PrintInfo()
        {
            foreach (var section in data)
            {
                Console.WriteLine($"[{section.Key}]");
                foreach (var property in section.Value)
                    Console.WriteLine($"{property.Key} = {property.Value}");
            }
        }

        private string TryGetValue(string sectionName, string key)
        {
            if (!data.ContainsKey(sectionName))
                throw new NotFoundSectionException();

            if (!data[sectionName].TryGetValue(key, out var value))
                throw new NotFoundFieldException();

            return value;
        }

        public string TryGetString(string sectionName, string key)
        {
            return TryGetValue(sectionName, key);
        }

        public int TryGetInt(string sectionName, string key)
        {
            var value = TryGetValue(sectionName, key);
            if (int.TryParse(value, out var parsedValue))
                return parsedValue;

            throw new InvalidTypeConversion(AvailableTypes.String, AvailableTypes.Int);
        }

        public float TryGetFloat(string sectionName, string key)
        {
            var value = TryGetValue(sectionName, key);
            if (float.TryParse(value, out var parsedValue))
                return parsedValue;

            throw new InvalidTypeConversion(AvailableTypes.String, AvailableTypes.Float);
        }
    }
}