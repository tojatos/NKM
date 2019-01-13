using System.Linq;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Ryuko_Matoi
{
    public class KamuiSenketsu : Ability, IClickable, IEnableable
    {
        private const int InitialADBonus = 5;
        private const int BonusADPerTurn = 2;
        private const int Damage = 5;
        public KamuiSenketsu(Game game) : base(game, AbilityType.Ultimatum, "Kamui Senketsu", 5)
        {
	        OnAwake += () =>
	        {
		        Validator.ToCheck.Remove(Validator.IsNotOnCooldown);
		        Validator.ToCheck.Remove(Validator.IsCharacterNotSilenced);
		        Validator.ToCheck.Add(() => Validator.IsNotOnCooldown() || IsEnabled);
		        Validator.ToCheck.Add(() => Validator.IsCharacterNotSilenced() || IsEnabled);
		        Active.Turn.TurnFinished += (character) =>
		        {
			        if(character!=ParentCharacter) return;
			        if(!IsEnabled) return;
			        ParentCharacter.Attack(this, ParentCharacter, new Damage(Damage, DamageType.True));
			        var effect = ParentCharacter.Effects.SingleOrDefault(e => e.Name == Name && e.GetType() == typeof(StatModifier)) as StatModifier;
			        if(effect==null) return;
			        effect.Modifier.Value += 2;
		        };
	        };
        }

        public override string GetDescription() =>
$@"{ParentCharacter.Name} zakłada boskie wdzianko, zyskując umiejętność latania i {InitialADBonus} AD.
Dodatkowo, na końcu każdej jej tury, gdy ta umiejętność jest aktywna, 
bonus AD zwiększa się o {BonusADPerTurn}, a {ParentCharacter.Name} otrzymuje {Damage} obrażeń nieuchronnych.
Po użyciu tej umiejętności {ParentCharacter.Name} może się poruszyć.";

        public void Click()
        {
	        if (!IsEnabled)
	        {
		        //Active.MakeAction();
			ParentCharacter.TryToTakeTurn();
		        IsEnabled = true;
                ParentCharacter.Effects.Add(new Flying(Game, -1, ParentCharacter, Name));
		        ParentCharacter.Effects.Add(new StatModifier(Game, -1, InitialADBonus, ParentCharacter, StatType.AttackPoints, Name));
		        ParentCharacter.HasFreeMoveUntilEndOfTheTurn = true;
		        Finish();
	        }
	        else
	        {
		        IsEnabled = false;
		        ParentCharacter.Effects.RemoveAll(e => e.Name == Name);
//		        ParentCharacter.Select();
		        Active.Select(ParentCharacter);
	        }
	        
        }

        public bool IsEnabled { get; private set; }
    }
}