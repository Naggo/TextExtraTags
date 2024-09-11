using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextExtraTags {
    [Serializable]
    public class ParserPreset {
        [SerializeField]
        string name;

        [SerializeField]
        [SerializeReference]
        List<ExtraTagFeature> features;

        public string Name => name;


        public ParserPreset(string name) {
            this.name = name;
            features = new();
        }


        public ParserFilters CreateFilters() {
            var filters = new ParserFilters();
            foreach (var feature in features) {
                feature.Register(filters);
            }
            return filters;
        }
    }
}
