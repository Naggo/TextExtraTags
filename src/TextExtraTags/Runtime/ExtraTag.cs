using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace TextExtraTags {
    public abstract class ExtraTag<T> : ExtraTagBase where T : ExtraTag<T> {
        public static T Create(int index, Func<T> ctor) {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), "Invalid index");
            T tag = ExtraTagPool<T>.GetItem(ctor);
            tag._index = index;
            tag.Initialize();
            return tag;
        }

        static void Return(T tag) {
            if (tag.Index < 0) return;
            tag.Reset();
            tag._index = -1;
            ExtraTagPool<T>.ReturnItem(tag);
        }


        int _index = -1;

        public sealed override int Index => _index;

        public sealed override void Return() {
            Assert.IsTrue(this is T, "Invalid inheritance");
            Return(this as T);
        }


        protected virtual void Initialize() {}

        protected virtual void Reset() {}
    }
}
