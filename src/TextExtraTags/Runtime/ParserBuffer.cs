using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextExtraTags {
    public class ParserBuffer {
        bool modified = false;
        List<string> texts = new();
        List<ExtraTagBase> tags = new();

        public bool Modified => modified;
        public IEnumerable<string> Texts => texts;
        public IEnumerable<ExtraTagBase> Tags => tags;


        public void ClearAll() {
            modified = false;
            texts.Clear();
            tags.Clear();
        }

        public void AddText(string text) {
            modified = true;
            texts.Add(text);
        }

        public void AddExtraTag(ExtraTagBase tag) {
            modified = true;
            tags.Add(tag);
        }
    }
}
