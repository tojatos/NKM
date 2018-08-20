using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Sabrac
{
    public class Fuiuchi : Ability, IClickable, IRunnable
    {
        private const int Range = 3;
        private const int Damage = 15;
        private const int SlowDuration = 2;
        private const int SlowTo = 3;
        public Fuiuchi() : base(AbilityType.Normal, "Fuiuchi", 3)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }

        public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Range);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyEnemiesOf(Owner);

        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} przywołuje słup płomieni, zadając {Damage} obrażeń magicznych
i spowalniając trafionych wrogów do {SlowTo}.

Zasięg: {Range}
Czas trwania spowolnienia: {SlowDuration}
Czas odnowienia: {Cooldown}";

        public void Click() => Run();

        public void Run()
        {
            GetTargetsInRange().GetCharacters().ForEach(c =>
            {
                ParentCharacter.Attack(this, c, new Damage(Damage, DamageType.Magical));
                c.Effects.Add(new StatModifier(SlowDuration, -c.Speed.Value+SlowTo, c, StatType.Speed, Name));
            });
            Finish();
        }
    }
}