using System;


namespace TextExtraTags.Standards {
    public interface IPoolableExtraTag {
        void ReturnToPool();
    }


    public interface IPoolableExtraTag<T> : IPoolableExtraTag where T: IPoolableExtraTag<T> {
        void Reset() {}

        void IPoolableExtraTag.ReturnToPool() {
            if (this is T tag) {
                Reset();
                ExtraTagPool<T>.Return(tag);
            }
        }
    }
}
