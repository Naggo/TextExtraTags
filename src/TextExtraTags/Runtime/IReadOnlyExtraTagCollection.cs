using System.Collections.Generic;


namespace TextExtraTags {
    public interface IReadOnlyExtraTagCollection : IEnumerable<ExtraTagBase> {
        public int Count { get; }
        public bool TryGetExtraTag<T>(int index, out T tag) where T: ExtraTagBase;
        public IEnumerable<T> GetExtraTags<T>(int index) where T : ExtraTagBase;
    }
}
