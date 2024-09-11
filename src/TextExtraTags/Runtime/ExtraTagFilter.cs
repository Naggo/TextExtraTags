using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextExtraTags {
    public abstract class ExtraTagFilter {
        public abstract void ProcessTagData(ParserBuffer buffer, in ParserTagData tagData);
    }
}
