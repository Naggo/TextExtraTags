using System;
using System.Buffers;
using System.Globalization;
using UnityEngine;


/*

RubyTagFilter is based on TextMeshProRuby
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
    public class RubyTag : ExtraTag {
        internal int index;

        public int BaseLength { get; internal set; }
        public int RubyLength { get; internal set; }

        public override int Index => index;
        public int EndTagIndex => Index + BaseLength;
    }


    public class RubyTagFeature : ExtraTagFeature {
        public bool convertTag = true;
        [Min(0)]
        public float rubySize = 0.5f;

        public override void Register(ParserFilters filters) {
            filters.AddFilter(new RubyTagFilter() { rubySize = rubySize, convertTag = convertTag });
        }
    }


    public class RubyTagFilter : ExtraTagFilter {
        public float rubySize;
        public bool convertTag;

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

        public override void ProcessTagData(int index, ref ParserContext context) {
            var tagData = context.TagData;
            if (tagData.IsName("ruby") || tagData.IsName("r")) {
                var ruby = tagData.Value;
                if (ruby.Length > 0) {
                    startIndex = index;
                    SetRubyText(tagData.Value);
                    context.ExcludeFromText = true;
                }
            } else if (tagData.IsName("/ruby") || tagData.IsName("/r")) {
                if (rubyLength > 0) {
                    ProcessRuby(index, ref context);
                    rubyLength = 0;
                }
                context.ExcludeFromText = true;
            }
        }


        void ProcessRuby(int index, ref ParserContext context) {
            ReadOnlySpan<char> ruby = rubyBuffer.AsSpan(0, rubyLength);
            int rL = ruby.Length;
            int kL = index - startIndex;

            var tag = new RubyTag();
            tag.index = startIndex;
            tag.BaseLength = kL;
            tag.RubyLength = rL;
            context.AddExtraTag(tag);

            if (!convertTag) return;

            // 文字数分だけ左に移動 - 開始タグ - ルビ - 終了タグ
            int count;
            float rHalf = rL * rubySize * 0.5f;
            float kHalf = kL * 0.5f;
            float space = -(kHalf + rHalf);
            context.AddText("<space=");
            space.TryFormat(context.GetTextSpan(32), out count, "F2", CultureInfo.InvariantCulture);
            context.AddTextLength(count);
            context.AddText("em><voffset=1em><size=");
            rubySize.TryFormat(context.GetTextSpan(32), out count, "0.#%", CultureInfo.InvariantCulture);
            context.AddTextLength(count);
            context.AddText(">");
            context.AddText(ruby);
            context.AddText("</size></voffset>");

            // 後ろに付ける空白
            space = kHalf - rHalf;
            if (space != 0) {
                context.AddText("<space=");
                space.TryFormat(context.GetTextSpan(32), out count, "F2", CultureInfo.InvariantCulture);
                context.AddTextLength(count);
                context.AddText("em>");
            }
        }

        void SetRubyText(ReadOnlySpan<char> text) {
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
