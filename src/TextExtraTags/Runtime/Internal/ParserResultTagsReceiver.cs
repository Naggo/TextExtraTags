

namespace TextExtraTags {
    readonly struct ParserResultTagsReceiver : IParserResultReceiver {
        readonly IExtraTagCollection tags;


        public ParserResultTagsReceiver(IExtraTagCollection tags) {
            this.tags = tags;
        }


        public void AddExtraTag(ExtraTag tag) {
            tags.Add(tag);
        }

        public void Clear() {
            tags.Clear();
        }
    }
}
