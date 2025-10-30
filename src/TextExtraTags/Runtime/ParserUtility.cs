using System;
using System.Collections.Generic;


namespace TextExtraTags {
    public static class ParserUtility {
        public static Parser GetParser(string name) {
            return TextExtraTagsSettings.Instance.GetParser(name);
        }

        public static IEnumerable<string> GetParserNames() {
            return TextExtraTagsSettings.Instance.GetNames();
        }
    }
}
