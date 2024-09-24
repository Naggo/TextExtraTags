using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextExtraTags {
    public interface IExtraTagCollection : IReadOnlyExtraTagCollection {
        public void Add(ExtraTagBase tag);
        public void Clear();
    }
}
