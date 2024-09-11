using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextExtraTags {
    public abstract class ExtraTagBase {
        public abstract int Index { get; }

        internal abstract void Return();
    }
}
