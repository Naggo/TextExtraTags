

namespace TextExtraTags {
    public abstract class ExtraTagFilter {
        public virtual void ProcessTagData(int index, ref ParserFilterContext context) {}
        public virtual void ProcessBufferedTagData(ref ParserFilterContext context) {}
        public virtual void Setup() {}
        public virtual void Reset() {}
    }
}
