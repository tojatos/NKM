using System;
using System.Collections.Generic;
using NKMCore;

namespace Unity
{
    public class SpriteSelectProperties
    {
        public SelectableProperties.Type WhatIsSelected { get; set; }
        public List<int> IdsToSelect { get; set; }
        public Action<List<int>> OnSelectFinish { get; set; }
        public string SelectionTitle { get; set; }
        public string FinishSelectingButtonText { get; set; }
        public bool Instant { get; set; }
    }
}