using Hex;

namespace NKMObjects.Templates
{
	public abstract class HexCellEffect : NKMObject
	{
		protected HexCellEffect(int cooldown, HexCell parentCell, string name = null)
		{
			CurrentCooldown = cooldown >= 0 ? cooldown : int.MaxValue; //effect is infinite
			ParentCell = parentCell;
			if (name != null) Name = name;
			Active.Phase.PhaseFinished += () =>//TODO
			{
				if (CurrentCooldown > 0) --CurrentCooldown;
				if (CurrentCooldown != 0) return;
				ParentCell.Effects.Remove(this);
				OnRemove?.Invoke();
			};
		}
		
		public delegate void OnRemoveHandler();
		public event OnRemoveHandler OnRemove;
		
		protected int CurrentCooldown { get; private set; }
		protected HexCell ParentCell { get; }

		public abstract string GetDescription();
		
	}

}
