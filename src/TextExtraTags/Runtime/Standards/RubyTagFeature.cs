using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextExtraTags.Standards {
    public class RubyTagFeature : ExtraTagFeature {
        public override void Register(ParserFilters filters) {
            filters.AddFilter(new RubyTagFilter());
        }
    }
}
