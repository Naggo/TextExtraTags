using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextExtraTags.Standards {
    public class RubyTagFilter : RubyTagFilter {
        int startIndex;
        int rubySize;
        char[] rubyBuffer;


        public override void Setup() {
            startIndex = 0;
            rubySize = 0;
            rubyBuffer = ArrayPool<char>.Shared.Rent(8);
        }

        public override void Reset() {
            ArrayPool<char>.Shared.Return(rubyBuffer);
            rubyBuffer = null;
        }

        public override bool ProcessTagData(
                int index, ParserBuffer buffer, in ParserTagData tagData)
        {
            if ((tagData.IsName("ruby") || tagData.IsName("r") && tagData.HasValue)) {
                startIndex = index;
                AppendText(tagData.Value);
                return true;
            }

            if (tagData.IsName("/ruby") || tagData.IsName("/r")) {
                if (rubySize > 0) {
                    
                }
                return true;
            }

            return false;
        }


        void AppendText(ReadOnlySpan<char> text) {
            Span<char> span = rubyBuffer.AsSpan();
            if (text.Length > span.Length) {
                int newBufferSize = rubyBuffer.Length * 2;
                while (text.Length > newBufferSize) {
                    newBufferSize = rubyBuffer.Length * 2;
                }

                ArrayPool<char>.Shared.Return(rubyBuffer);
                rubyBuffer = ArrayPool<char>.Shared.Rent(newBufferSize);
                span = rubyBuffer.AsSpan();
            }
            text.CopyTo(span);
            rubySize = text.Length;
        }
    }
}
