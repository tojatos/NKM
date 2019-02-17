﻿using NKMCore.Hex;

namespace NKMCore.Templates
{
	public abstract class HexCellEffect
	{
		public readonly string Name;
		protected readonly Game Game;
		private Active Active => Game.Active;
		
		protected int CurrentCooldown { get; private set; }
		public HexCell ParentCell { get; }
		
		public event Delegates.Void OnRemove;

		public abstract string GetDescription();
		protected HexCellEffect(Game game, int cooldown, HexCell parentCell, string name = null)
		{
			Game = game;
			CurrentCooldown = cooldown >= 0 ? cooldown : int.MaxValue; //effect is infinite
			ParentCell = parentCell;
			if (name != null) Name = name;
			Game.HexMap.InvokeAfterCellEffectCreate(this);
			Active.Phase.PhaseFinished += () =>//TODO
			{
				if (CurrentCooldown > 0) --CurrentCooldown;
				if (CurrentCooldown != 0) return;
				Remove();
			};
		}

		public void Remove()
		{
            ParentCell.Effects.Remove(this);
            OnRemove?.Invoke();
			Game.HexMap.InvokeAfterCellEffectRemove(this);
		}
		
	}

}
