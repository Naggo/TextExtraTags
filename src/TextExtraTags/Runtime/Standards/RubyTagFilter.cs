using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if TEXTEXTRATAGS_ZSTRING_SUPPORT
    using Cysharp.Text;
#else
    using System.Text;
#endif


namespace TextExtraTags.Standards {
    public class RubyTagFilter : ExtraTagFilter {
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
            if (tagData.IsName("ruby") || tagData.IsName("r")) {
                var ruby = tagData.Value;
                if (ruby.Length > 0) {
                    startIndex = index;
                    AppendText(tagData.Value);
                    return true;
                }
            } else if (tagData.IsName("/ruby") || tagData.IsName("/r")) {
                if (rubySize > 0) {
                    ProcessRuby(index, buffer, tagData);
                    rubySize = 0;
                }
                return true;
            }

            return false;
        }


        void ProcessRuby(int index, ParserBuffer buffer, in ParserTagData tagData) {
            #if TEXTEXTRATAGS_ZSTRING_SUPPORT
                using (var builder = ZString.CreateStringBuilder(true))
            #else
                var builder = new StringBuilder();
            #endif
            {
                ReadOnlySpan<char> ruby = rubyBuffer.AsSpan(0, rubySize);
                int rL = ruby.Length;
                int kL2 = (index - startIndex) * 2;

                // 文字数分だけ左に移動 - 開始タグ - ルビ - 終了タグ
                float space = -(kL2 * 0.25f + rL * 0.25f);
                builder.AppendFormat("<space={0:F2}em><voffset=1em><size=50%>", space);
                builder.Append(ruby);
                builder.Append("</size></voffset>");

                // 後ろに付ける空白
                space = (kL2 - rL) * 0.25f;
                if (space != 0) {
                    builder.AppendFormat("<space={0:F2}em>", space);
                }

                #if TEXTEXTRATAGS_ZSTRING_SUPPORT
                    buffer.AddText(builder.AsSpan());
                #else
                    buffer.AddText(builder.ToString());
                #endif
            }
        }

        void AppendText(ReadOnlySpan<char> text) {
            Span<char> span = rubyBuffer.AsSpan();
            if (text.Length > span.Length) {
                int newBufferSize = rubyBuffer.Length * 2;
                while (text.Length > newBufferSize) {
                    newBufferSize *= 2;
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
