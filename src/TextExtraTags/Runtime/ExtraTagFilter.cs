

namespace TextExtraTags {
    public abstract class ExtraTagFilter {
        public virtual void ProcessTagData(int index, ref ParserContext context) {}
        public virtual void Setup() {}
        public virtual void Reset() {}
    }
}
