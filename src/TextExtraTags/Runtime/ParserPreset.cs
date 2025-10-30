using System;
using System.Collections.Generic;
using UnityEngine;


namespace TextExtraTags {
    [Serializable]
    public class ParserPreset {
        [SerializeField]
        string name;

        [SerializeField]
        [Range(1, 5)]
        int capacityLevel;

        [SerializeField]
        [Range(1, 5)]
        int iterationLimit;

        [SerializeField]
        [SerializeReference]
        List<ExtraTagFeature> features;

        public string Name => name;


        public ParserPreset(string name, int capacityLevel = 1, int iterationLimit = 2, IEnumerable<ExtraTagFeature> features = null) {
            this.name = name;
            this.capacityLevel = capacityLevel;
            this.iterationLimit = iterationLimit;
            this.features = (features is null) ? new() : new(features);
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
                <=1 => 256,
                2 => 512,
                3 => 1024,
                4 => 2048,
                >=5 => 4096
            };
        }

        public int GetIterationLimit() {
            return iterationLimit;
        }
    }
}
