using System;
using System.Buffers;
using UnityEngine;

#if TEXTEXTRATAGS_ZSTRING_SUPPORT
    using Cysharp.Text;
#else
    using System.Text;
#endif


/*

RubyTagFilter based on TextMeshProRuby
https://github.com/ina-amagami/TextMeshProRuby

MIT License

Copyright (c) 2019 ina-amagami (ina@amagamina.jp)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/


namespace TextExtraTags.Standards {
    public class RubyTag : ExtraTag<RubyTag> {
        public int BaseLength { get; internal set; }
        public int RubyLength { get; internal set; }

        public int EndTagIndex => Index + BaseLength;
    }


    public class RubyTagFeature : ExtraTagFeature {
        [Min(0)]
        public float rubySize = 0.5f;

        public override void Register(ParserFilters filters) {
            filters.AddFilter(new RubyTagFilter() { rubySize = rubySize });
        }
    }


    public class RubyTagFilter : ExtraTagFilter {
        public float rubySize;

        int startIndex;
        int rubyLength;
        char[] rubyBuffer;


        public override void Setup() {
            startIndex = 0;
            rubyLength = 0;
            rubyBuffer = ArrayPool<char>.Shared.Rent(16);
        }

        public override void Reset() {
            ArrayPool<char>.Shared.Return(rubyBuffer);
            rubyBuffer = null;
        }

        public override bool ProcessTagData(
                int index, ParserBuffer buffer, in ParserTagData tagData) {
            if (tagData.IsName("ruby") || tagData.IsName("r")) {
                var ruby = tagData.Value;
                if (ruby.Length > 0) {
                    startIndex = index;
                    AppendText(tagData.Value);
                    return true;
                }
            } else if (tagData.IsName("/ruby") || tagData.IsName("/r")) {
                if (rubyLength > 0) {
                    ProcessRuby(index, buffer, tagData);
                    rubyLength = 0;
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
                ReadOnlySpan<char> ruby = rubyBuffer.AsSpan(0, rubyLength);
                int rL = ruby.Length;
                int kL = index - startIndex;
                float rHalf = rL * rubySize * 0.5f;
                float kHalf = kL * 0.5f;

                // 文字数分だけ左に移動 - 開始タグ - ルビ - 終了タグ
                float space = -(kHalf + rHalf);
                builder.AppendFormat("<space={0:F2}em><voffset=1em><size={1:0.#%}>", space, rubySize);
                builder.Append(ruby);
                builder.Append("</size></voffset>");

                // 後ろに付ける空白
                space = kHalf - rHalf;
                if (space != 0) {
                    builder.AppendFormat("<space={0:F2}em>", space);
                }

                var tag = RubyTag.Create(startIndex, () => new RubyTag());
                tag.BaseLength = kL;
                tag.RubyLength = rL;
                buffer.AddExtraTag(tag);

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
            rubyLength = text.Length;
        }
    }
}
