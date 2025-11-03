using System;
using System.Collections.Generic;
using UnityEngine.Assertions;


namespace TextExtraTags.Standards {
    public static class ExtraTagPool<T> where T: IPoolableExtraTag {
        static Stack<T> stack = new();

        public static T Get(Func<T> ctor) {
            if (stack.Count == 0) {
                return ctor();
            } else {
                return stack.Pop();
            }
        }

        public static void Return(T item) {
            Assert.IsFalse(stack.Contains(item));
            stack.Push(item);
        }
    }
}
