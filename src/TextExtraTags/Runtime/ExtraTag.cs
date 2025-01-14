using System;


namespace TextExtraTags {
    public abstract class ExtraTag {
        public abstract int Index { get; }
    }


    public abstract class ExtraTag<T> : ExtraTag, IExtraTagInternal where T : ExtraTag<T> {
        public static T Create(int index, Func<T> ctor) {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), index, "Invalid index");
            T tag = ExtraTagPool<T>.GetItem(ctor);
            tag._index = index;
            tag.Initialize();
            return tag;
        }

        static void Return(T tag) {
            if (tag.Index < 0) throw new InvalidOperationException("Already returned");
            tag.Reset();
            tag._index = -1;
            ExtraTagPool<T>.ReturnItem(tag);
        }


        int _index = -1;

        public sealed override int Index => _index;

        bool IExtraTagInternal.IsActive => _index >= 0;

        void IExtraTagInternal.ReturnToPool() {
            if (this is T tag) {
                Return(tag);
            } else {
                throw new InvalidOperationException("Invalid inheritance");
            }
        }


        protected virtual void Initialize() {}

        protected virtual void Reset() {}
    }
}
