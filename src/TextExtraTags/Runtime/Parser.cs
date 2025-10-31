using System;
using System.Collections.Generic;


namespace TextExtraTags {
    public class Parser {
        ParserPreset preset;
        ParserFilters filters;

        ParserTextBuffer textBuffer;
        ParserTextBuffer filterTextBuffer;
        int iterationLimit;

        public string Name => preset.Name;


        public Parser(ParserPreset preset) {
            this.preset = preset;
            this.filters = preset.CreateFilters();
            this.textBuffer = new ParserTextBuffer(preset.GetParserTextCapacity());
            this.filterTextBuffer = new ParserTextBuffer(preset.GetParserTextCapacity());
            this.iterationLimit = preset.GetIterationLimit();
        }


        public ReadOnlySpan<char> AsSpan() {
            return textBuffer.AsSpan();
        }

        public ArraySegment<char> AsArraySegment() {
            return textBuffer.AsArraySegment();
        }

        public override string ToString() {
            return textBuffer.ToString();
        }

        public Parser Parse(ReadOnlySpan<char> source, ICollection<ExtraTag> results) {
            textBuffer.Clear();
            filterTextBuffer.Clear();
            results.Clear();

            filters.Setup();
            ProcessText(source, results, 1, 0);
            filters.Reset();

            return this;
        }


        int ProcessText(ReadOnlySpan<char> source, ICollection<ExtraTag> results, int iterationCount, int textCount) {
            int sourceIndex = 0;
            int nextBufferStart = filterTextBuffer.Length;
            while (TryParseTag(source, sourceIndex, out ParserTagData tagData)) {
                int textLength = tagData.Index - sourceIndex;
                int textAndTagLength = textLength + tagData.Length;
                textCount += textLength;

                // フィルターにタグを渡す
                var context = new ParserContext(tagData, filterTextBuffer, results);
                filters.ProcessTagData(textCount, ref context);

                if (context.ExcludeFromText) {
                    // タグを除去する
                    textBuffer.AddText(source.Slice(sourceIndex, textLength));
                    sourceIndex += textAndTagLength;
                } else if (context.ExcludeFromCount) {
                    // タグを追加するが、タグの分のカウントは加算しない
                    textBuffer.AddText(source.Slice(sourceIndex, textAndTagLength));
                    sourceIndex += textAndTagLength;
                } else {
                    // タグを含めた文章の追加
                    textBuffer.AddText(source.Slice(sourceIndex, textAndTagLength));
                    textCount += tagData.Length;
                    sourceIndex += textAndTagLength;
                }

                if (filterTextBuffer.Length > nextBufferStart) {
                    ReadOnlySpan<char> bufferSpan = filterTextBuffer.AsSpan().Slice(nextBufferStart);
                    if (iterationCount < iterationLimit) {
                        // 追加テキストを解析する
                        textCount = ProcessText(bufferSpan, results, iterationCount + 1, textCount);
                    } else {
                        // 追加テキストをそのまま加算する
                        textBuffer.AddText(bufferSpan);
                        textCount += bufferSpan.Length;
                    }
                    filterTextBuffer.SetLength(nextBufferStart);
                }
            }

            // 残りのテキストの加算
            int remainTextLength = source.Length - sourceIndex;
            textBuffer.AddText(source.Slice(sourceIndex, remainTextLength));
            textCount += remainTextLength;
            return textCount;
        }

        bool TryParseTag(ReadOnlySpan<char> source, int startIndex, out ParserTagData result) {
            int lbIndex = -1;
            int sepIndex = -1;

            for (int i = startIndex; i < source.Length; i++) {
                char c = source[i];

                if (c == '<') {
                    lbIndex = i;
                } else if (c == '=' && sepIndex < lbIndex) {
                    sepIndex = i;
                } else if (c == '>' && 0 <= lbIndex){
                    result = new ParserTagData(source, lbIndex, i, sepIndex);
                    return true;
                }
            }

            result = default;
            return false;
        }
    }
}
