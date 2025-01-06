

namespace TextExtraTags {
    public ref struct ParserFilterContext {
        public readonly ParserBuffer Buffer;
        public readonly ParserTagData TagData;

        public bool ExcludeFromSourceText;
        public bool ExcludeFromParsedText;
        public bool SkipOtherFilters;


        public ParserFilterContext(ParserBuffer buffer, ParserTagData tagData) {
            this.Buffer = buffer;
            this.TagData = tagData;
            this.ExcludeFromSourceText = false;
            this.ExcludeFromParsedText = false;
            this.SkipOtherFilters = false;
        }
    }
}
