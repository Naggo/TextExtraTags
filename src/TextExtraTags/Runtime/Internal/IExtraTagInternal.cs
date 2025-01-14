

namespace TextExtraTags {
    interface IExtraTagInternal {
        public bool IsActive { get; }
        public abstract void ReturnToPool();
    }
}
