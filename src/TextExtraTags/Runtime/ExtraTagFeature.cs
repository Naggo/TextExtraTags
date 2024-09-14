using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextExtraTags {
    [Serializable]
    public abstract class ExtraTagFeature {
        public bool enabled = true;

        public abstract void Register(ParserFilters filters);
    }
}
