using System;


namespace TextExtraTags {
    [Serializable]
    public abstract class ExtraTagFeature {
        public bool enabled = true;

        public abstract void Register(ParserFilters filters);
    }
}
