#define TEXTEXTRATAGS_ZSTRING_SUPPORT

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

#if TEXTEXTRATAGS_ZSTRING_SUPPORT
using Cysharp.Text;
#else
using System.Text;
#endif


namespace TextExtraTags {
    public class Parser {
        ParserPreset preset;
        ParserFilters filters;
        ParserBuffer buffer;

        public string Name => preset.Name;


        public Parser(ParserPreset preset) {
            this.preset = preset;
            this.filters = preset.CreateFilters();
            this.buffer = new ParserBuffer();
        }


        public string Parse(string text, List<ExtraTagBase> resultTags) {
#if TEXTEXTRATAGS_ZSTRING_SUPPORT
            using (var builder = ZString.CreateStringBuilder())
#else
            var builder = new StringBuilder();
#endif
            {
                bool allowParsing = true;
                int currentIndex = 0;
                ParserTagData tagData;
                while (ParserUtility.TryParseTag(text, currentIndex, out tagData)) {
                    if (tagData.IsName("noparse")) {
                        allowParsing = false;
                    } else if (tagData.IsName("/noparse")) {
                        allowParsing = true;
                    }
                    currentIndex = tagData.Index + 1;

                    if (!allowParsing) continue;

                    buffer.ClearAll();

                    // フィルターにタグを渡す
                    foreach (var filter in filters) {
                        filter.ProcessTagData(buffer, tagData);
                    }

                    if (buffer.Modified || ParserUtility.IsRichTextTag(tagData)) {
                        
                    }
                }
            }
            return null;
        }
    }
}
