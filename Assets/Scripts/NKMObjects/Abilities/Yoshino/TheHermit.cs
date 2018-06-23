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

	    private int _lastTreshold;
        
		public TheHermit() : base(AbilityType.Passive, "The Hermit")
		{
//			Name = "The Hermit";
//			Type = AbilityType.Passive;
			_lastTreshold = ParentCharacter.GetStat(StatType.HealthPoints).BaseValue;
		}

	    public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Range);
	    public override string GetDescription() => $"{ParentCharacter.Name} ogłusza wrogów w promieniu {Range} pól na {EffectDuration} tury co każde {AfterLosingHP} HP, które straci.";
	    public override void Awake()
	    {
		    Stat hp = ParentCharacter.GetStat(StatType.HealthPoints);
		    hp.StatChanged += () =>
		    {
			    while (hp.Value < _lastTreshold - 15)
			    {
				    _lastTreshold -= 15;
				    StunEnemiesInRange();
			    }
		    };
	    }

	    private void StunEnemiesInRange()
	    {
		    List<Character> enemiesInRange = GetRangeCells().GetCharacters().FindAll(c => c.IsEnemy);
		    enemiesInRange.ForEach(e => e.Effects.Add(new Stun(EffectDuration, e, Name)));
	    }

	    public void Run() => StunEnemiesInRange();
    }
}