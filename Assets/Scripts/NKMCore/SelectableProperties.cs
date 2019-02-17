using System;
using System.Collections.Generic;

namespace NKMCore
{
    public class SelectableProperties<T>
    {
        public List<T> ToSelect { get; set; }
        public Func<List<T>, bool> ConstraintOfSelection { get; set; }
        public Action<List<T>> OnSelectFinish { get; set; }
        public string SelectionTitle { get; set; }
    }
}