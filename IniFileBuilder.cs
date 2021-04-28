using System;
using System.IO;
using System.Collections.Generic;

namespace MyIniParser.Parser
{
    internal class IniFileBuilder {
        private Dictionary<string, Dictionary<string, string>> data;

        public IniFileBuilder() {
            this.data = new Dictionary<string, Dictionary<string, string>>();
        }

        public void AddSection(string sectionName, Dictionary<string, string> section) {
            data.Add(sectionName, section);
        }

        public ParsedIniFile Build() {
            return new ParsedIniFile(this.data);
        }
    }
}