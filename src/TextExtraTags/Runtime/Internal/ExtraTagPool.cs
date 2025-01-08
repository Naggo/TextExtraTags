using System;
using System.Collections.Generic;


namespace TextExtraTags {
    static class ExtraTagPool<T> where T : ExtraTagBase {
        static Stack<T> stack = new();

        public static T GetItem(Func<T> ctor) {
            if (stack.Count == 0) {
                return ctor();
            } else {
                return stack.Pop();
            }
        }

        public static void ReturnItem(T item) {
            stack.Push(item);
        }
    }
}
