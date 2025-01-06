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

        public static bool TryParseTag(
            ReadOnlySpan<char> source, int startIndex, out ParserTagData result
        ) {
            int lbIndex = -1;
            int sepIndex = -1;

            for (int i = startIndex; i < source.Length; i++) {
                char c = source[i];

                if (c == '<') {
                    lbIndex = i;
                } else if (c == '=' && sepIndex < lbIndex) {
                    sepIndex = i;
                } else if (c == '>' && 0 <= lbIndex){
                    result = new ParserTagData(source, lbIndex, i, sepIndex);
                    return true;
                }
            }

            result = default;
            return false;
        }
    }
}
