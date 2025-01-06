using System;


namespace TextExtraTags {
    public readonly ref struct ParserTagData {
        readonly ReadOnlySpan<char> source;
        readonly int start;
        readonly int end;
        readonly int separator;

        public int Index => start;
        public int Length => (end - start) + 1;
        public bool HasValue => (start < separator);

        public ReadOnlySpan<char> Name => GetSpan(start+1, (HasValue ? separator : end));
        public ReadOnlySpan<char> Value => GetSpan((HasValue ? separator+1 : end), end);
        public ReadOnlySpan<char> FullText => GetSpan(start, end+1);


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
