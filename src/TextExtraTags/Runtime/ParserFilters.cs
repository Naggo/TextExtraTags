using System.Collections;
using System.Collections.Generic;


namespace TextExtraTags {
    public class ParserFilters : IEnumerable<ExtraTagFilter> {
        List<ExtraTagFilter> filters = new();


        public void AddFilter(ExtraTagFilter filter) {
            filters.Add(filter);
        }

        public void ProcessTagData(int index, ref ParserFilterContext context) {
            foreach (var filter in filters) {
                filter.ProcessTagData(index, ref context);
                if (context.SkipOtherFilters) {
                    break;
                }
            }
        }

        public void ProcessBufferedTagData(ref ParserFilterContext context) {
            foreach (var filter in filters) {
                filter.ProcessBufferedTagData(ref context);
                if (context.SkipOtherFilters) {
                    break;
                }
            }
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
