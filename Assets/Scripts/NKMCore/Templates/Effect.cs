using System;
using System.Collections.Generic;
using NKMCore.Hex;

namespace NKMCore.Templates
{
	public abstract class Effect
	{
		public string Name;
		public override string ToString() => Name;

		private readonly Game _game;
		private Active Active => _game.Active;
		protected Effect(Game game, int cooldown, Character parentCharacter, string name = null)
		{
			_game = game;
			CurrentCooldown = cooldown >= 0 ? cooldown : int.MaxValue; //effect is infinite
			ParentCharacter = parentCharacter;
			if (name != null) Name = name;
			Active.Turn.TurnFinished += character => {
				if(character != ParentCharacter) return;
				if(CurrentCooldown == int.MaxValue) return;
                if (CurrentCooldown > 0) CurrentCooldown--; 
				if (CurrentCooldown != 0) return;
				RemoveFromParent();
			};

		}
		public delegate void OnRemoveHandler();
		public event OnRemoveHandler OnRemove;

		public int CurrentCooldown { get; set; }
		protected Character ParentCharacter { get; }
		public EffectType Type { get; protected set; }
		public virtual bool IsCC => false;
		public abstract string GetDescription();
		
		protected List<HexCell> GetNeighboursOfOwner(int depth, SearchFlags searchFlags = SearchFlags.None, Predicate<HexCell> stopAt = null) =>
			ParentCharacter.ParentCell.GetNeighbors(ParentCharacter.Owner, depth, searchFlags, stopAt);
		public void RemoveFromParent()
		{
			if(!ParentCharacter.Effects.Contains(this)) return;
			ParentCharacter.Effects.Remove(this);
			OnRemove?.Invoke();
		}

		public string GetEffectTypeName()
		{
			switch (Type)
			{
				case EffectType.Positive:
					return "Positive Effect";
				case EffectType.Negative:
					return "Negative Effect";
				case EffectType.Neutral:
					return "Neutral Effect";
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}

	public enum EffectType
	{
		Positive,
		Negative,
		Neutral
	}

}
