using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextExtraTags {
    public class ExtraTagCollection : IEnumerable<ExtraTagBase>, IDisposable {
        List<ExtraTagBase> m_tags;


        public ExtraTagCollection() {
            m_tags = new();
        }

        public ExtraTagCollection(int capacity) {
            m_tags = new(capacity);
        }


        public void AddExtraTag(ExtraTagBase tag) {
            m_tags.Add(tag);
        }

        public void AddExtraTags(IEnumerable<ExtraTagBase> tags) {
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

        public void Dispose() {
            Clear();
        }
    }
}