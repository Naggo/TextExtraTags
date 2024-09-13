using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextExtraTags {
    public interface IReadOnlyExtraTagCollection : IEnumerable<ExtraTagBase> {
        public T GetExtraTagOrDefault<T>(int index) where T: ExtraTagBase;
        public bool TryGetExtraTag<T>(int index, out T tag) where T: ExtraTagBase;
    }
}
