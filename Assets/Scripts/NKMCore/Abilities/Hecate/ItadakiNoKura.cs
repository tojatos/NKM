using System.Collections.Generic;
using NKMCore.Templates;

namespace NKMCore.Abilities.Hecate
{
	public class ItadakiNoKura : Ability
	{
		private const int HealthPercent = 40;
		public List<Character> CollectedEnergyCharacters { get; } = new List<Character>();
		public int CollectedEnergy { get {
			float energy = 0;
			CollectedEnergyCharacters.ForEach(c => energy += c.HealthPoints.BaseValue * ((float)HealthPercent /100));
			return (int)energy;
		} }
		public ItadakiNoKura(Game game) : base(game, AbilityType.Passive, "Itadaki no Kura")
		{
			OnAwake += () => ParentCharacter.AfterAttack += (character, damage) => TryCollectingEnergy(character);
		}
		
		public override string GetDescription() => 
$@"{ParentCharacter.Name} gromadzi Energię Życiową każdym podstawowym atakiem lub z Astera.
Jeden ładunek Energii Życiowej przechowuje {HealthPercent}% maksymalnego HP celu.
Energia Życiowa tej samej postaci może zostać zgromadzona tylko raz.
Aktualna wartość zebranej energii: {CollectedEnergy}";

		public void TryCollectingEnergy(Character targetCharacter)
		{
			if (CollectedEnergyCharacters.Contains(targetCharacter)) return;
			
			CollectedEnergyCharacters.Add(targetCharacter);
//			AnimationPlayer.Add(new Animations.ItadakiNoKura(ParentCharacter.CharacterObject.transform, targetCharacter.CharacterObject.transform));TODO
		}
	}
}
