

namespace TextExtraTags {
    public interface IExtraTagCollection : IReadOnlyExtraTagCollection {
        public void Add(ExtraTag tag);
        public void Clear();
    }
}
