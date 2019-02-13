using System.Collections.Generic;
using NKMCore;
using NKMCore.Templates;

namespace Unity.Managers
{
    internal class SpriteSelectSelectable : ISelectable
    {
        public void Select<T>(SelectableProperties<T> props)
        {
            if(typeof(T) == typeof(Character)) SpriteSelect.Instance.Open(props.ToSelect as List<Character>, () =>
            {
                List<Character> selectedObj = SpriteSelect.Instance.SelectedObjects;
                if (!props.ConstraintOfSelection(selectedObj as List<T>)) return;
                props.OnSelectFinish(selectedObj as List<T>);

                SpriteSelect.Instance.Close();
            }, props.SelectionTitle, "Zakończ wybieranie" );
        }
    }
}