using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextExtraTags {
    public abstract class ExtraTagFilter {
        public abstract bool ProcessTagData(int index, ParserBuffer buffer, in ParserTagData tagData);
        public virtual void Setup() {}
        public virtual void Reset() {}
    }
}
