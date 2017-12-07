using System.Collections.Generic;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Hecate
{
	public class ItadakiNoKura : Ability
	{
		private const int HealthPercent = 40;
		public List<Character> CollectedEnergyCharacters { get; }
		public int CollectedEnergy { get {
			float energy = 0;
			CollectedEnergyCharacters.ForEach(c => energy += c.HealthPoints.BaseValue * ((float)HealthPercent /100));
			return (int)energy;
		} }
		public ItadakiNoKura()
		{
			Name = "Itadaki no Kura";
			Type = AbilityType.Passive;
			OverridesEnemyAttack = true;
			CollectedEnergyCharacters = new List<Character>();
		}
		public override string GetDescription()
		{
			return string.Format(
@"{0} gromadzi Energię Życiową każdym podstawowym atakiem.
Jeden ładunek Energii Życiowej przechowuje {1}% maksymalnego HP celu.
Energia Życiowa tej samej postaci może zostać zgromadzona tylko raz.
Aktualna wartość zebranej energii: {2}",
ParentCharacter.Name, HealthPercent, CollectedEnergy
);
		}

		public override void AttackEnemy(Character attackedCharacter)
		{
			ParentCharacter.Attack(attackedCharacter, AttackType.Physical, ParentCharacter.AttackPoints.Value);
			TryCollectingEnergy(attackedCharacter);
		}

		public void TryCollectingEnergy(Character targetCharacter)
		{
			if (!CollectedEnergyCharacters.Contains(targetCharacter))
			{
				CollectedEnergyCharacters.Add(targetCharacter);
				Animations.Instance.ItadakiNoKura(ParentCharacter.CharacterObject.transform, targetCharacter.CharacterObject.transform);
			}
		}
	}
}
