using NKMCore.Hex;

namespace NKMCore.Templates
{
	public abstract class HexCellEffect
	{
		public readonly string Name;
		protected readonly Game Game;
		private Active Active => Game.Active;
		protected HexCellEffect(Game game, int cooldown, HexCell parentCell, string name = null)
		{
			Game = game;
			CurrentCooldown = cooldown >= 0 ? cooldown : int.MaxValue; //effect is infinite
			ParentCell = parentCell;
			if (name != null) Name = name;
			if(Active.SelectedCell==ParentCell) Unity.UI.HexCellUI.Effects.Instance.UpdateButtons();
			Active.Phase.PhaseFinished += () =>//TODO
			{
				if (CurrentCooldown > 0) --CurrentCooldown;
				if (CurrentCooldown != 0) return;
				Remove();
                if(Active.SelectedCell==ParentCell) Unity.UI.HexCellUI.Effects.Instance.UpdateButtons();
			};
		}
		
		public event Delegates.Void OnRemove;

		public void Remove()
		{
            ParentCell.Effects.Remove(this);
            OnRemove?.Invoke();
		}
		protected int CurrentCooldown { get; private set; }
		private HexCell ParentCell { get; }

		public abstract string GetDescription();
		
	}

}
