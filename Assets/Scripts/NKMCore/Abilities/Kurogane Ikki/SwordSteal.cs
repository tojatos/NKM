using System;
using System.Collections.Generic;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Kurogane_Ikki
{
    public class SwordSteal : Ability, IClickable 
    {
        private Ability _copiedAbility;
        
        public SwordSteal(Game game) : base(game, AbilityType.Normal, "Sword Steal", 3)
        {
            OnAwake += () =>
            {
                Validator.ToCheck.Add(()=>_copiedAbility!=null && _copiedAbility.CanBeUsed);
                ParentCharacter.AfterBeingHitByAbility += ability =>
                {
                    if(!(ability is IClickable)) return; 
                    var a = Activator.CreateInstance(ability.GetType()) as Ability;
                    if(a==null) return;
//                    a.ParentCharacter = ParentCharacter;
                    a.Awake();
                    a.AfterUseFinish += Finish;
                    
                    _copiedAbility = a;
                };
            };
        }

        public override List<HexCell> GetRangeCells() => _copiedAbility==null ? new List<HexCell>() : _copiedAbility.GetRangeCells();
        public override List<HexCell> GetTargetsInRange() => _copiedAbility==null ? new List<HexCell>() : _copiedAbility.GetTargetsInRange();

        public override string GetDescription()
        {
            string desc = 
$@"{ParentCharacter.Name} kopiuje wrogą technikę, używając ostatnią umiejętność, która zadała mu obrażenia.
Czas odnowienia: {Cooldown}";
            if (_copiedAbility != null) desc += $"\n<i>Ostatnia skradziona technika: \n<b>{_copiedAbility.Name}</b></i>\n" + _copiedAbility.GetDescription();
            return desc;
        }

        public void Click() => ((IClickable) _copiedAbility)?.Click();
    }
}