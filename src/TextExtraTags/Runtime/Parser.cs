using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextExtraTags {
    public class Parser : IDisposable {
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

        public Parser Parse(ReadOnlySpan<char> source, ExtraTagCollection resultTags) {
            buffer.ClearAll();
            resultTags.Clear();
            textSize = 0;

            filters.Setup();

            bool allowParsing = true;
            int sourceIndex = 0;
            int parsedIndex = 0;
            ParserTagData tagData;
            while (ParserUtility.TryParseTag(source, sourceIndex, out tagData)) {
                bool isInvalidTag = false;
                bool isRichTextTag = false;
                bool isExtraTag = false;

                int textLength = tagData.Index - sourceIndex;
                int textAndTagLength = textLength + tagData.Length;
                parsedIndex += textLength;

                do {
                    // noparse の判定
                    if (allowParsing) {
                        if (tagData.IsName("noparse")) {
                            allowParsing = false;
                            isRichTextTag = true;
                        }
                    } else {
                        if (tagData.IsName("/noparse")) {
                            allowParsing = true;
                            isRichTextTag = true;
                        } else {
                            isInvalidTag = true;
                            break;
                        }
                    }

                    // フィルターにタグを渡す
                    bool isParsed = filters.ProcessTagData(parsedIndex, buffer, tagData);

                    if (isRichTextTag || ParserUtility.IsRichTextTag(tagData)) {
                        // リッチテキスト
                        isRichTextTag = true;
                    } else if (isParsed) {
                        // エクストラ
                        isExtraTag = true;
                    } else {
                        // 該当なし
                        isInvalidTag = true;
                    }
                } while (false);

                if (isInvalidTag) {
                    // タグを含めた文章の追加
                    AppendText(source.Slice(sourceIndex, textAndTagLength));
                    parsedIndex += tagData.Length;
                    sourceIndex += textAndTagLength;
                } else if (isRichTextTag) {
                    // タグを追加するが、タグの分のインデックスは加算しない
                    AppendText(source.Slice(sourceIndex, textAndTagLength));
                    sourceIndex += textAndTagLength;
                } else if (isExtraTag) {
                    // タグを除去する
                    AppendText(source.Slice(sourceIndex, textLength));
                    sourceIndex += textAndTagLength;
                }

                // バッファデータの処理
                if (buffer.HasText) {
                    ReadOnlySpan<char> bufferSpan = buffer.Text;
                    int bufferIndex = 0;
                    ParserTagData givenTagData;
                    while (ParserUtility.TryParseTag(bufferSpan, bufferIndex, out givenTagData)) {
                        int bufferTextLength = givenTagData.Index - bufferIndex;
                        int bufferTextAndTagLength = bufferTextLength + givenTagData.Length;
                        // エクストラタグの処理は行わない
                        if (ParserUtility.IsRichTextTag(givenTagData)) {
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
                    resultTags.AddExtraTags(buffer.Tags);
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


        public void Dispose() {
            buffer.Dispose();
            textBuffer = null;
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
