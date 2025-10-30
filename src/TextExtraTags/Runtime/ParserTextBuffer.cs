using System;
using System.Collections.Generic;



namespace TextExtraTags {
    class ParserTextBuffer {
        int textSize;
        char[] textBuffer;

        public bool HasText => textSize > 0;


        public ParserTextBuffer(int capacity) {
            this.textSize = 0;
            this.textBuffer = new char[capacity];
        }


        public ReadOnlySpan<char> AsSpan() {
            return textBuffer.AsSpan(0, textSize);
        }

        public ArraySegment<char> AsArraySegment() {
            return new ArraySegment<char>(textBuffer, 0, textSize);
        }

        public override string ToString() {
            if (textSize == 0)
                return string.Empty;

            return new string(textBuffer, 0, textSize);
        }

        public void Clear() {
            textSize = 0;
        }

        public void EnsureCapacity(int capacity) {
            if (capacity > textBuffer.Length) {
                int newBufferSize = textBuffer.Length * 2;
                while (capacity > newBufferSize) {
                    newBufferSize *= 2;
                }
                var newBuffer = new char[newBufferSize];
                textBuffer.CopyTo(newBuffer, 0);
                textBuffer = newBuffer;
            }
        }

        public void AddText(ReadOnlySpan<char> text) {
            EnsureCapacity(textSize + text.Length);
            Span<char> span = textBuffer.AsSpan(textSize);
            text.CopyTo(span);
            textSize += text.Length;
        }

        public Span<char> GetSpan(int length) {
            EnsureCapacity(textSize + length);
            Span<char> span = textBuffer.AsSpan(textSize, length);
            return span;
        }

        public void AddLength(int length) {
            EnsureCapacity(textSize + length);
            textSize += length;
        }
    }
}
