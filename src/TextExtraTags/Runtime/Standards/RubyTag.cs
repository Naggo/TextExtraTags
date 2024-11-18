using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextExtraTags.Standards {
    public class RubyTag : ExtraTag<RubyTag> {
        public int BaseLength { get; internal set; }
        public int RubyLength { get; internal set; }

        [System.Obsolete("Use BaseLength.")]
        public int baseLength => BaseLength;

        [System.Obsolete("Use RubyLength.")]
        public int rubyLength => RubyLength;
    }
}
