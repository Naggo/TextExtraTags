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
        [Range(0, 4)]
        int capacityLevel;

        [SerializeField]
        [SerializeReference]
        List<ExtraTagFeature> features;

        public string Name => name;
        public bool IsDefault => this == TextExtraTagsSettings.Instance.GetDefaultPreset();


        public ParserPreset(string name, int capacityLevel) {
            this.name = name;
            this.capacityLevel = capacityLevel;
            this.features = new List<ExtraTagFeature>();
        }

        public ParserPreset(string name, int capacityLevel, IEnumerable<ExtraTagFeature> features) {
            this.name = name;
            this.capacityLevel = capacityLevel;
            this.features = new List<ExtraTagFeature>(features);
        }


        public ParserFilters CreateFilters() {
            var filters = new ParserFilters();
            foreach (var feature in features) {
                if (feature is not null && feature.enabled) {
                    feature.Register(filters);
                }
            }
            return filters;
        }

        public int GetParserTextCapacity() {
            return capacityLevel switch {
                0 => 64,
                1 => 128,
                2 => 256,
                3 => 512,
                4 => 1024,
                _ => 64,
            };
        }

        public int GetParserBufferTextCapacity() {
            return capacityLevel switch {
                0 => 32,
                1 => 64,
                2 => 128,
                3 => 256,
                4 => 512,
                _ => 32,
            };
        }

        public int GetParserBufferTagsCapacity() {
            return capacityLevel switch {
                0 => 1,
                1 => 2,
                2 => 4,
                3 => 8,
                4 => 16,
                _ => 1,
            };
        }
    }
}
