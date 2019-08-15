﻿using System;
using System.Collections.Generic;
using System.Linq;
using NKMCore;
using NKMCore.Templates;

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
            switch (props.WhatIsSelected)
            {
                case SelectableProperties.Type.Character:
                    List<Character> toPickFrom = Game.GetMockCharacters();
                    if(GameStarter._game != null) toPickFrom.AddRange(GameStarter._game.Characters);
                    SpriteSelect.Instance.Open(toPickFrom.FindAll(c => props.IdsToSelect.Contains(c.ID)),
                        () =>
                        {
                            List<int> selectedIds = SpriteSelect.Instance.SelectedObjects.Select(c => c.ID).ToList();
                            _action.CloseSelectable(selectableId, selectedIds);
                        }, props.SelectionTitle, "Zakończ wybieranie");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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