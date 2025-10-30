using System;
using System.Collections.Generic;


namespace TextExtraTags {
    public class Parser {
        ParserPreset preset;
        ParserFilters filters;

        ParserTextBuffer textBuffer;
        ParserTextBuffer filterTextBuffer;

        public string Name => preset.Name;


        public Parser(ParserPreset preset) {
            this.preset = preset;
            this.filters = preset.CreateFilters();
            this.textBuffer = new ParserTextBuffer(preset.GetParserTextCapacity());
            this.filterTextBuffer = new ParserTextBuffer(preset.GetParserTextCapacity());
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
            return Parse(source, ref results);
        }

        public Parser Parse<T>(ReadOnlySpan<char> source, ref T results) where T : ICollection<ExtraTag> {
            textBuffer.Clear();
            filterTextBuffer.Clear();
            results.Clear();

            filters.Setup();

            int sourceIndex = 0;
            int parsedIndex = 0;
            while (TryParseTag(source, sourceIndex, out ParserTagData tagData)) {
                int textLength = tagData.Index - sourceIndex;
                int textAndTagLength = textLength + tagData.Length;
                parsedIndex += textLength;

                // フィルターにタグを渡す
                var context = new ParserContext(tagData, filterTextBuffer, results);
                filters.ProcessTagData(parsedIndex, ref context);

                if (context.ExcludeFromText) {
                    // タグを除去する
                    textBuffer.AddText(source.Slice(sourceIndex, textLength));
                    sourceIndex += textAndTagLength;
                } else if (context.ExcludeFromCount) {
                    // タグを追加するが、タグの分のインデックスは加算しない
                    textBuffer.AddText(source.Slice(sourceIndex, textAndTagLength));
                    sourceIndex += textAndTagLength;
                } else {
                    // タグを含めた文章の追加
                    textBuffer.AddText(source.Slice(sourceIndex, textAndTagLength));
                    parsedIndex += tagData.Length;
                    sourceIndex += textAndTagLength;
                }

                // バッファデータの処理
                if (filterTextBuffer.HasText) {
                    ReadOnlySpan<char> bufferSpan = filterTextBuffer.AsSpan();
                    int bufferIndex = 0;
                    while (TryParseTag(bufferSpan, bufferIndex, out ParserTagData bufferedTagData)) {
                        int bufferTextLength = bufferedTagData.Index - bufferIndex;
                        int bufferTextAndTagLength = bufferTextLength + bufferedTagData.Length;
                        // フィルターにタグを渡す、追加のバッファ操作は認めない
                        context = new ParserContext(bufferedTagData, filterTextBuffer, results);
                        filters.ProcessBufferedTagData(ref context);
                        if (context.ExcludeFromText) {
                            // タグを除去する
                            textBuffer.AddText(bufferSpan.Slice(bufferIndex, bufferTextLength));
                            bufferIndex += bufferTextAndTagLength;
                        } else if (context.ExcludeFromCount) {
                            // タグを追加するが、タグの分のインデックスは加算しない
                            textBuffer.AddText(bufferSpan.Slice(bufferIndex, bufferTextAndTagLength));
                            parsedIndex += bufferTextLength;
                            bufferIndex += bufferTextAndTagLength;
                        } else {
                            // タグを含めた文章の追加
                            textBuffer.AddText(bufferSpan.Slice(bufferIndex, bufferTextAndTagLength));
                            parsedIndex += bufferTextAndTagLength;
                            bufferIndex += bufferTextAndTagLength;
                        }
                    }

                    // 残りのバッファテキストの加算
                    int remainBufferTextLength = bufferSpan.Length - bufferIndex;
                    parsedIndex += remainBufferTextLength;
                    textBuffer.AddText(bufferSpan.Slice(bufferIndex, remainBufferTextLength));
                    filterTextBuffer.Clear();
                }
            }

            // 残りのソーステキストの加算
            int remainTextLength = source.Length - sourceIndex;
            textBuffer.AddText(source.Slice(sourceIndex, remainTextLength));

            filters.Reset();

            return this;
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
