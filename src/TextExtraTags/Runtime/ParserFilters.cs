using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextExtraTags {
    public class ParserFilters : IEnumerable<ExtraTagFilter> {
        List<ExtraTagFilter> filters = new();


        public void AddFilter(ExtraTagFilter filter) {
            filters.Add(filter);
        }


        public IEnumerator<ExtraTagFilter> GetEnumerator() {
            return filters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return filters.GetEnumerator();
        }
    }
}
