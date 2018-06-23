using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Yoshino
{
    public class TheHermit : Ability, IRunable
    {
	    private const int Range = 2;
	    private const int EffectDuration = 2;
	    private const int AfterLosingHP = 15;

	    public TheHermit() : base(AbilityType.Passive, "The Hermit")
		{
			int lastTreshold = ParentCharacter.GetStat(StatType.HealthPoints).BaseValue;
			OnAwake += () =>
			{
				Stat hp = ParentCharacter.GetStat(StatType.HealthPoints);
				hp.StatChanged += () =>
				{
					while (hp.Value < lastTreshold - 15)
					{
						lastTreshold -= 15;
						StunEnemiesInRange();
					}
				};
			};
		}

	    public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Range);
	    public override string GetDescription() => $"{ParentCharacter.Name} ogłusza wrogów w promieniu {Range} pól na {EffectDuration} tury co każde {AfterLosingHP} HP, które straci.";

	    private void StunEnemiesInRange()
	    {
		    List<Character> enemiesInRange = GetRangeCells().GetCharacters().FindAll(c => c.IsEnemy);
		    enemiesInRange.ForEach(e => e.Effects.Add(new Stun(EffectDuration, e, Name)));
	    }

	    public void Run() => StunEnemiesInRange();
    }
}