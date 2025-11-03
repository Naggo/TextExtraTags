using System.Collections;
using System.Collections.Generic;


namespace TextExtraTags.Standards {
    public class PoolableExtraTagCollection : ICollection<ExtraTag> {
        List<ExtraTag> list = new();


        public int Count => list.Count;

        bool ICollection<ExtraTag>.IsReadOnly => false;


        public void Add(ExtraTag item) {
            list.Add(item);
        }

        public void Clear() {
            foreach (var tag in list) {
                if (tag is IPoolableExtraTag poolable) {
                    poolable.ReturnToPool();
                }
            }
            list.Clear();
        }

        public bool Contains(ExtraTag item) {
            return list.Contains(item);
        }

        public void CopyTo(ExtraTag[] array, int arrayIndex) {
            list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ExtraTag> GetEnumerator() {
            return list.GetEnumerator();
        }

        public bool Remove(ExtraTag item) {
            bool result = list.Remove(item);
            if (result && item is IPoolableExtraTag poolable) {
                poolable.ReturnToPool();
            }
            return result;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
