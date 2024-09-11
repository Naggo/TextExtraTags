using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextExtraTags {
    public struct ParserTagData {
        string sourceText;
        int start;
        int end;
        int separator;

        public int Index => start;
        public int Length => (end - start) + 1;
        public bool HasValue => (start < separator);

        public ReadOnlySpan<char> Name => GetSpan(start+1, (HasValue ? separator : end));
        public ReadOnlySpan<char> Value => GetSpan((HasValue ? separator+1 : end), end);


        public ParserTagData(string sourceText, int start, int end, int separator) {
            this.sourceText = sourceText;
            this.start = start;
            this.end = end;
            this.separator = separator;
        }


        public bool IsName(ReadOnlySpan<char> value) {
            return Name.SequenceEqual(value);
        }


        ReadOnlySpan<char> GetSpan(int sliceStart, int sliceEnd) {
            if (sourceText is null) {
                return ReadOnlySpan<char>.Empty;
            }
            return sourceText.AsSpan().Slice(sliceStart, sliceEnd-sliceStart);
        }
    }
}
