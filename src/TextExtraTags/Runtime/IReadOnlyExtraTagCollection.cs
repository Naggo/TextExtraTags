using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextExtraTags {
    public interface IReadOnlyExtraTagCollection : IEnumerable<ExtraTagBase> {
        public int Count { get; }
        public bool TryGetExtraTag<T>(int index, out T tag) where T: ExtraTagBase;
    }
}
