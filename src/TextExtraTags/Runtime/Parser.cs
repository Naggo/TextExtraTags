using System;


namespace TextExtraTags {
    public class Parser {
        ParserPreset preset;
        ParserFilters filters;
        ParserBuffer buffer;

        int textSize;
        char[] textBuffer;

        public string Name => preset.Name;
        public bool IsDefault => preset.IsDefault;


        public Parser(ParserPreset preset) {
            this.preset = preset;
            this.filters = preset.CreateFilters();
            this.buffer = new ParserBuffer(
                preset.GetParserBufferTextCapacity(),
                preset.GetParserBufferTagsCapacity()
            );
            this.textSize = 0;
            this.textBuffer = new char[preset.GetParserTextCapacity()];
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

        public Parser Parse(ReadOnlySpan<char> source, IExtraTagCollection resultTags) {
            var wrapper = new ParserResultTagsReceiver(resultTags);
            return Parse(source, ref wrapper);
        }

        public Parser Parse<T>(ReadOnlySpan<char> source, ref T result) where T : IParserResultReceiver {
            buffer.ClearAll();
            result.Clear();
            textSize = 0;

            filters.Setup();

            int sourceIndex = 0;
            int parsedIndex = 0;
            while (ParserUtility.TryParseTag(source, sourceIndex, out ParserTagData tagData)) {
                int textLength = tagData.Index - sourceIndex;
                int textAndTagLength = textLength + tagData.Length;
                parsedIndex += textLength;

                // フィルターにタグを渡す
                var context = new ParserFilterContext(buffer, tagData);
                filters.ProcessTagData(parsedIndex, ref context);

                if (context.ExcludeFromSourceText) {
                    // タグを除去する
                    AppendText(source.Slice(sourceIndex, textLength));
                    sourceIndex += textAndTagLength;
                } else if (context.ExcludeFromParsedText) {
                    // タグを追加するが、タグの分のインデックスは加算しない
                    AppendText(source.Slice(sourceIndex, textAndTagLength));
                    sourceIndex += textAndTagLength;
                } else {
                    // タグを含めた文章の追加
                    AppendText(source.Slice(sourceIndex, textAndTagLength));
                    parsedIndex += tagData.Length;
                    sourceIndex += textAndTagLength;
                }

                // バッファデータの処理
                if (buffer.HasText) {
                    ReadOnlySpan<char> bufferSpan = buffer.Text;
                    int bufferIndex = 0;
                    while (ParserUtility.TryParseTag(bufferSpan, bufferIndex, out ParserTagData bufferedTagData)) {
                        int bufferTextLength = bufferedTagData.Index - bufferIndex;
                        int bufferTextAndTagLength = bufferTextLength + bufferedTagData.Length;
                        // フィルターにタグを渡す、追加のバッファ操作は認めない
                        context = new ParserFilterContext(null, bufferedTagData);
                        filters.ProcessBufferedTagData(ref context);
                        if (context.ExcludeFromSourceText) {
                            // タグを除去する
                            AppendText(bufferSpan.Slice(bufferIndex, bufferTextLength));
                            bufferIndex += bufferTextAndTagLength;
                        } else if (context.ExcludeFromParsedText) {
                            // タグを追加するが、タグの分のインデックスは加算しない
                            AppendText(bufferSpan.Slice(bufferIndex, bufferTextAndTagLength));
                            parsedIndex += bufferTextLength;
                            bufferIndex += bufferTextAndTagLength;
                        } else {
                            // タグを含めた文章の追加
                            AppendText(bufferSpan.Slice(bufferIndex, bufferTextAndTagLength));
                            parsedIndex += bufferTextAndTagLength;
                            bufferIndex += bufferTextAndTagLength;
                        }
                    }

                    // 残りのバッファテキストの加算
                    int remainBufferTextLength = bufferSpan.Length - bufferIndex;
                    parsedIndex += remainBufferTextLength;
                    AppendText(bufferSpan.Slice(bufferIndex, remainBufferTextLength));
                }

                if (buffer.HasTags) {
                    foreach (ExtraTag tag in buffer.Tags) {
                        result.AddExtraTag(tag);
                    }
                }

                if (buffer.Modified) {
                    buffer.ClearAll();
                }
            }

            // 残りのソーステキストの加算
            int remainTextLength = source.Length - sourceIndex;
            AppendText(source.Slice(sourceIndex, remainTextLength));

            filters.Reset();

            return this;
        }


        void AppendText(ReadOnlySpan<char> text) {
            Span<char> span = textBuffer.AsSpan(textSize);
            while (text.Length > span.Length) {
                int newBufferSize = textBuffer.Length * 2;
                var newBuffer = new char[newBufferSize];
                textBuffer.CopyTo(newBuffer, 0);

                textBuffer = newBuffer;
                span = textBuffer.AsSpan(textSize);
            }
            text.CopyTo(span);
            textSize += text.Length;
        }
    }
}
