﻿using NKMObjects.Templates;

namespace NKMObjects.Effects
{
	public class Empty : Effect
	{
		private readonly string _description;
		public Empty(int cooldown, Character parentCharacter, string name, string description) : base(cooldown, parentCharacter, name)
		{
			Name = name ?? "Empty";
			Type = EffectType.Neutral;
			_description = description;
		}

		public override string GetDescription() => _description;
	}
}
