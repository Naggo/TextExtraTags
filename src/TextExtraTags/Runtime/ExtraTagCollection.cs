using System;
using System.Collections;
using System.Collections.Generic;


namespace TextExtraTags {
    public class ExtraTagCollection : IExtraTagCollection, IReadOnlyExtraTagCollection, IDisposable {
        List<ExtraTagBase> m_tags;

        public int Count => m_tags.Count;


        public ExtraTagCollection() {
            m_tags = new();
        }

        public ExtraTagCollection(int capacity) {
            m_tags = new(capacity);
        }


        public void Add(ExtraTagBase tag) {
            m_tags.Add(tag);
        }

        public void AddRange(IEnumerable<ExtraTagBase> tags) {
            m_tags.AddRange(tags);
        }

        public T GetExtraTagOrDefault<T>(int index) where T: ExtraTagBase {
            foreach (var item in m_tags) {
                if (item.Index == index && item is T result) {
                    return result;
                }
            }
            return default;
        }

        public bool TryGetExtraTag<T>(int index, out T tag) where T: ExtraTagBase {
            foreach (var item in m_tags) {
                if (item.Index == index && item is T result) {
                    tag = result;
                    return true;
                }
            }
            tag = default;
            return false;
        }

        public IEnumerable<T> GetExtraTags<T>(int index) where T: ExtraTagBase {
            foreach (var item in m_tags) {
                if (item.Index == index && item is T result) {
                    yield return result;
                }
            }
        }

        public void Clear() {
            foreach (var tag in m_tags) {
                tag.Return();
            }
            m_tags.Clear();
        }


        public IEnumerator<ExtraTagBase> GetEnumerator() {
            return m_tags.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return m_tags.GetEnumerator();
        }

        void IDisposable.Dispose() {
            Clear();
        }
    }
}
