﻿using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Yoshino
{
    public class TheHermit : Ability, IRunnable
    {
	    private const int Range = 2;
	    private const int EffectDuration = 2;
	    private const int AfterLosingHP = 15;

	    private int _lastTreshold;
	    public TheHermit() : base(AbilityType.Passive, "The Hermit")
		{
			OnAwake += () =>
			{
				Stat hp = ParentCharacter.GetStat(StatType.HealthPoints);
                _lastTreshold = hp.BaseValue;
				hp.StatChanged += () =>
				{
					while (hp.Value < _lastTreshold - 15)
					{
						_lastTreshold -= 15;
						StunEnemiesInRange();
					}
				};
			};
		}

	    public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Range);
	    public override string GetDescription() => 
$@"{ParentCharacter.Name} ogłusza wrogów w promieniu {Range} pól na {EffectDuration} tury co każde {AfterLosingHP} HP, które straci.
Następna aktywacja umiejętności po przekroczeniu <color=red>{_lastTreshold - 15}</color> HP";

	    private void StunEnemiesInRange()
	    {
		    List<NKMCharacter> enemiesInRange = GetRangeCells().GetCharacters().FindAll(c => c.IsEnemyFor(Owner));
		    enemiesInRange.ForEach(e => e.Effects.Add(new Stun(EffectDuration, e, Name)));
	    }

	    public void Run() => StunEnemiesInRange();
    }
}