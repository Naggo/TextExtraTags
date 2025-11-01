using System;


namespace TextExtraTags {
    public readonly ref struct ParserTagData {
        public static bool TryParse(ReadOnlySpan<char> source, int startIndex, out ParserTagData result) {
            int lbIndex = -1;
            int sepIndex = -1;

            for (int i = startIndex; i < source.Length; i++) {
                char c = source[i];

                switch (c) {
                    case '<':
                        lbIndex = i;
                        break;
                    case ' ':
                    case '=':
                        if (sepIndex < lbIndex) {
                            sepIndex = i;
                        }
                        break;
                    case '>':
                        if (lbIndex < 0) break;
                        if (sepIndex < lbIndex) {
                            result = new ParserTagData(source, lbIndex, i);
                        } else {
                            result = new ParserTagData(source, lbIndex, i, sepIndex);
                        }
                        return true;
                }
            }

            result = default;
            return false;
        }


        readonly ReadOnlySpan<char> source;
        readonly int start;
        readonly int end;
        readonly int separator;

        public int Index => start;
        public int Length => end - start + 1;
        public bool HasValue => source[separator] == '=';

        public ReadOnlySpan<char> Name => GetSpan(start+1, separator);
        public ReadOnlySpan<char> Value => HasValue ? GetSpan(separator+1, end) : ReadOnlySpan<char>.Empty;
        public ReadOnlySpan<char> FullText => GetSpan(start, end+1);


        public ParserTagData(ReadOnlySpan<char> source, int start, int end)
            : this(source, start, end, end) {}

        public ParserTagData(ReadOnlySpan<char> source, int start, int end, int separator) {
            this.source = source;
            this.start = start;
            this.end = end;
            this.separator = separator;
        }


        public bool IsName(ReadOnlySpan<char> name) {
            return Name.SequenceEqual(name);
        }


        ReadOnlySpan<char> GetSpan(int sliceStart, int sliceEnd) {
            if (source.IsEmpty) {
                return source;
            }
            return source.Slice(sliceStart, sliceEnd-sliceStart);
        }
    }
}
