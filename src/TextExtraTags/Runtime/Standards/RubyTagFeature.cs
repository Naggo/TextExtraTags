using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextExtraTags.Standards {
    public class RubyTagFeature : ExtraTagFeature {
        [Min(0)]
        public float rubySize = 0.5f;

        public override void Register(ParserFilters filters) {
            filters.AddFilter(new RubyTagFilter() {rubySize = rubySize});
        }
    }
}
