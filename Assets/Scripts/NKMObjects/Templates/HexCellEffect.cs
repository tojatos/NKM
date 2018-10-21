using Hex;
using Managers;

namespace NKMObjects.Templates
{
	public abstract class HexCellEffect : NKMObject
	{
		protected static Game Game => GameStarter.Instance.Game;
		protected static Active Active => Game.Active;
		protected static Console Console => Console.Instance;
		protected HexCellEffect(int cooldown, HexCell parentCell, string name = null)
		{
			CurrentCooldown = cooldown >= 0 ? cooldown : int.MaxValue; //effect is infinite
			ParentCell = parentCell;
			if (name != null) Name = name;
			if(Active.SelectedCell==ParentCell) UI.HexCellUI.Effects.Instance.UpdateButtons();
			Active.Phase.PhaseFinished += () =>//TODO
			{
				if (CurrentCooldown > 0) --CurrentCooldown;
				if (CurrentCooldown != 0) return;
				Remove();
                if(Active.SelectedCell==ParentCell) UI.HexCellUI.Effects.Instance.UpdateButtons();
			};
		}
		
		public event Character.VoidDelegate OnRemove;

		public void Remove()
		{
            ParentCell.Effects.Remove(this);
            OnRemove?.Invoke();
		}
		protected int CurrentCooldown { get; private set; }
		protected HexCell ParentCell { get; }

		public abstract string GetDescription();
		
	}

}
