using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextExtraTags {
    public class ParserBuffer : IDisposable {
        const int DefaultBufferSize = 128;


        int textSize;
        char[] textBuffer;
        List<ExtraTagBase> tags;

        public bool HasText => textSize > 0;
        public bool HasTags => tags.Count > 0;
        public bool Modified => HasText || HasTags;

        public ReadOnlySpan<char> Text => textBuffer.AsSpan(0, textSize);
        public IEnumerable<ExtraTagBase> Tags => tags;


        public ParserBuffer() {
            this.textSize = 0;
            this.textBuffer = ArrayPool<char>.Shared.Rent(DefaultBufferSize);
            this.tags = new();
        }

        public ParserBuffer(int textCapacity, int tagsCapacity) {
            this.textSize = 0;
            this.textBuffer = ArrayPool<char>.Shared.Rent(textCapacity);
            this.tags = new(tagsCapacity);
        }


        public void ClearAll() {
            textSize = 0;
            tags.Clear();
        }

        public void AddText(ReadOnlySpan<char> text) {
            Span<char> span = textBuffer.AsSpan(textSize);
            while (text.Length > span.Length) {
                int newBufferSize = textBuffer.Length * 2;
                var newBuffer = ArrayPool<char>.Shared.Rent(newBufferSize);
                textBuffer.CopyTo(newBuffer, 0);
                ArrayPool<char>.Shared.Return(textBuffer);

                textBuffer = newBuffer;
                span = textBuffer.AsSpan(textSize);
            }
            text.CopyTo(span);
            textSize += text.Length;
        }

        public void AddExtraTag(ExtraTagBase tag) {
            tags.Add(tag);
        }


        public void Dispose() {
            if (textBuffer is null) return;
            ArrayPool<char>.Shared.Return(textBuffer);
            textBuffer = null;
        }
    }
}
