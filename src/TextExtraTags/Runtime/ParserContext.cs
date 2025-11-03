using System;
using System.Collections.Generic;



namespace TextExtraTags {
    public ref struct ParserContext {
        public readonly ParserTagData TagData;

        public bool ExcludeFromText;
        public bool ExcludeFromCount;
        public bool SkipOtherFilters;

        ParserTextBuffer TextBuffer;
        ICollection<ExtraTag> ResultTags;


        internal ParserContext(ParserTagData tagData, ParserTextBuffer textBuffer, ICollection<ExtraTag> resultTags) {
            this.TagData = tagData;
            this.TextBuffer = textBuffer;
            this.ResultTags = resultTags;
            this.ExcludeFromText = false;
            this.ExcludeFromCount = false;
            this.SkipOtherFilters = false;
        }


        public void AddText(ReadOnlySpan<char> text) {
            TextBuffer.AddText(text);
        }

        public Span<char> GetTextSpan(int length) {
            return TextBuffer.GetSpan(length);
        }

        public void AddTextLength(int length) {
            TextBuffer.AddLength(length);
        }

        public void AddExtraTag(ExtraTag tag) {
            ResultTags.Add(tag);
        }
    }
}
