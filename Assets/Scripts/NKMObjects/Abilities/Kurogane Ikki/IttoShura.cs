using NKMObjects.Templates;

namespace NKMObjects.Abilities.Kurogane_Ikki
{
    public class IttoShura : Ability, IClickable, IEnableable
    {
        public IttoShura() : base(AbilityType.Ultimatum, "Itto Shura")
        {
            OnAwake += () => Validator.ToCheck.Add(() => !IsEnabled);
        }

        public override string GetDescription() =>
$@"{ParentCharacter.Name} wykorzystuje swoją najsilniejszą technikę, wyłączając wszystkie zmysły:
Podwaja swój Atak, Zasięg i Szybkość kosztem połowy swojego obecnego HP.
{ParentCharacter.Name} zostaje oczyszczony z efektów kontroli tłumu.
Po użyciu tej umiejętności {ParentCharacter.Name} może użyć podstawowego ataku.";

        public void Click()
        {
            ParentCharacter.AttackPoints.Value *= 2;
            ParentCharacter.BasicAttackRange.Value *= 2;
            ParentCharacter.Speed.Value *= 2;
            ParentCharacter.HealthPoints.Value /= 2;
            ParentCharacter.Effects.RemoveAll(e => e.IsCC);
            ParentCharacter.HasFreeAttack = true;
            IsEnabled = true;
            OnUseFinish();
        }

        public bool IsEnabled { get; private set; }
    }
}