using System.Collections.Generic;


namespace TextExtraTags {
    public interface IReadOnlyExtraTagCollection : IEnumerable<ExtraTag> {
        public int Count { get; }
        public bool TryGetExtraTag<T>(int index, out T tag) where T: ExtraTag;
        public IEnumerable<T> GetExtraTags<T>(int index) where T : ExtraTag;
    }
}
