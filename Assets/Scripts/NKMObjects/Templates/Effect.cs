﻿using System;
using System.Collections.Generic;
using Hex;
using Managers;

namespace NKMObjects.Templates
{
	public abstract class Effect// : NKMObject
	{
		private Game Game;
		public string Name;
		protected Active Active => Game.Active;
		protected Console Console => Game.Console;
		public Character Owner => ParentCharacter;
		protected Effect(Game game, int cooldown, Character parentCharacter, string name = null)
		{
			Game = game;
			CurrentCooldown = cooldown >= 0 ? cooldown : int.MaxValue; //effect is infinite
			ParentCharacter = parentCharacter;
			if (name != null) Name = name;
			if (Active.CharacterOnMap == ParentCharacter) UI.CharacterUI.Effects.Instance.UpdateButtons();
			Active.Turn.TurnFinished += character => {
				if(character != ParentCharacter) return;
				if(CurrentCooldown == int.MaxValue) return;
                if (CurrentCooldown > 0) CurrentCooldown--; 
				if (CurrentCooldown != 0) return;
				RemoveFromParent();
				
				if (Active.CharacterOnMap == ParentCharacter) UI.CharacterUI.Effects.Instance.UpdateButtons();
			};

		}
		public delegate void OnRemoveHandler();
		public event OnRemoveHandler OnRemove;

		public int CurrentCooldown { get; set; }
		public Character ParentCharacter { get; }
		public EffectType Type { get; protected set; }
		public virtual bool IsCC => false;

		public abstract string GetDescription();
		
		protected List<HexCell> GetNeighboursOfOwner(int depth, SearchFlags searchFlags = SearchFlags.None, Predicate<HexCell> stopAt = null) =>
			ParentCharacter.ParentCell.GetNeighbors(Owner.Owner, depth, searchFlags, stopAt);
//		public virtual int Modifier(StatType statType)
//		{
//			return 0;
//		}

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
