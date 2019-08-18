using System.Collections.Generic;
using NKMCore;

namespace Unity.Managers
{
    public class SpriteSelectSelectable : ISelectable
    {
        private readonly SelectableManager _manager;
        private SelectableAction _action;

        public SpriteSelectSelectable(SelectableManager manager)
        {
            _manager = manager;
        }

        public void Init(SelectableAction action)
        {
            _action = action;
        }

        public void OpenSelectable(int selectableId)
        {
            SelectableProperties props = _manager.Get(selectableId);
            SpriteSelect.Instance.Open(new SpriteSelectProperties
            {
                WhatIsSelected = props.WhatIsSelected,
                IdsToSelect = props.IdsToSelect,
                SelectionTitle = props.SelectionTitle,
                FinishSelectingButtonText = "Zakończ wybieranie",
                OnSelectFinish = selectedIds => _action.CloseSelectable(selectableId, selectedIds),
                Instant = props.Instant,
            });
        }

        public void CloseSelectable(int selectableId, List<int> selectedIds)
        {
            SelectableProperties props = _manager.Get(selectableId);
            if(!props.ConstraintOfSelection(selectedIds)) return;
            props.OnSelectFinish(selectedIds);
            SpriteSelect.Instance.Close();
        }
    }
}