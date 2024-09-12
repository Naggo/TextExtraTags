#define TEXTEXTRATAGS_ZSTRING_SUPPORT

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

#if TEXTEXTRATAGS_ZSTRING_SUPPORT
using Cysharp.Text;
#else
using System.Text;
#endif


namespace TextExtraTags {
    public class Parser {
        ParserPreset preset;
        ParserFilters filters;
        ParserBuffer buffer;

        public string Name => preset.Name;


        public Parser(ParserPreset preset) {
            this.preset = preset;
            this.filters = preset.CreateFilters();
            this.buffer = new ParserBuffer();
        }


        public ReadOnlySpan<char> Parse(ReadOnlySpan<char> source, List<ExtraTagBase> resultTags) {
            #if TEXTEXTRATAGS_ZSTRING_SUPPORT
                using (var builder = ZString.CreateStringBuilder())
            #else
                var builder = new StringBuilder();
            #endif
            {
                buffer.ClearAll();

                bool allowParsing = true;
                int sourceIndex = 0;
                int parsedIndex = 0;
                ParserTagData tagData;
                while (ParserUtility.TryParseTag(source, sourceIndex, out tagData)) {
                    bool isInvalidTag = false;
                    bool isRichTextTag = false;
                    bool isExtraTag = false;

                    do {
                        // noparse の判定
                        if (allowParsing) {
                            if (tagData.IsName("noparse")) {
                                allowParsing = false;
                                isRichTextTag = true;
                            } else if (tagData.IsName("/noparse")) {
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
                        bool isParsed = false;
                        foreach (var filter in filters) {
                            isParsed |= filter.ProcessTagData(parsedIndex, buffer, tagData);
                        }

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
                        int textAndTagLength = tagData.Index + tagData.Length - sourceIndex;
                        builder.Append(source.Slice(sourceIndex, textAndTagLength));
                        parsedIndex += textAndTagLength;
                        sourceIndex += textAndTagLength;
                    } else if (isRichTextTag) {
                        // タグを追加するが、タグの分のインデックスは加算しない
                        int textLength = tagData.Index - sourceIndex;
                        int textAndTagLength = textLength + tagData.Length;
                        builder.Append(source.Slice(sourceIndex, textAndTagLength));
                        parsedIndex += textLength;
                        sourceIndex += textAndTagLength;
                    } else if (isExtraTag) {
                        // タグを除去する
                        int textLength = tagData.Index - sourceIndex;
                        int textAndTagLength = textLength + tagData.Length;
                        builder.Append(source.Slice(sourceIndex, textLength));
                        parsedIndex += textLength;
                        sourceIndex += textAndTagLength;
                    }

                    // バッファデータの処理
                    if (buffer.HasText) {
                        ReadOnlySpan<char> bufferSpan = buffer.Text;
                        int bufferIndex = 0;
                        ParserTagData givenTagData;
                        while (ParserUtility.TryParseTag(bufferSpan, bufferIndex, out givenTagData)) {
                            // エクストラタグの処理は行わない
                            if (ParserUtility.IsRichTextTag(givenTagData)) {
                                // タグを追加するが、タグの分のインデックスは加算しない
                                int textLength = givenTagData.Index - bufferIndex;
                                int textAndTagLength = textLength + givenTagData.Length;
                                builder.Append(bufferSpan.Slice(bufferIndex, textAndTagLength));
                                parsedIndex += textLength;
                                bufferIndex += textAndTagLength;
                            } else {
                                // タグを含めた文章の追加
                                int textAndTagLength = givenTagData.Index + givenTagData.Length - bufferIndex;
                                builder.Append(bufferSpan.Slice(bufferIndex, textAndTagLength));
                                parsedIndex += textAndTagLength;
                                bufferIndex += textAndTagLength;
                            }
                        }

                        // 残りのバッファテキストの加算
                        int remainBufferLength = bufferSpan.Length - bufferIndex;
                        builder.Append(bufferSpan.Slice(bufferIndex, remainBufferLength));
                    }

                    if (buffer.Modified) {
                        buffer.ClearAll();
                    }
                }

                // 残りのソーステキストの加算
                int remainTextLength = source.Length - sourceIndex;
                builder.Append(source.Slice(sourceIndex, remainTextLength));

                #if TEXTEXTRATAGS_ZSTRING_SUPPORT
                    return builder.AsSpan();
                #else
                    return builder.ToString().AsSpan();
                #endif
            }
        }
    }
}
