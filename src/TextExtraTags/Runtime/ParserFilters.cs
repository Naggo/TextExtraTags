using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextExtraTags {
    public class ParserFilters : IEnumerable<ExtraTagFilter> {
        List<ExtraTagFilter> filters = new();


        public void AddFilter(ExtraTagFilter filter) {
            filters.Add(filter);
        }

        public bool ProcessTagData(int index, ParserBuffer buffer, in ParserTagData tagData) {
            bool isParsed = false;
            foreach (var filter in filters) {
                isParsed |= filter.ProcessTagData(index, buffer, tagData);
            }
            return isParsed;
        }

        public void Setup() {
            foreach (var filter in filters) {
                filter.Setup();
            }
        }

        public void Reset() {
            foreach (var filter in filters) {
                filter.Reset();
            }
        }


        public IEnumerator<ExtraTagFilter> GetEnumerator() {
            return filters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return filters.GetEnumerator();
        }
    }
}
