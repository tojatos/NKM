using System.Collections.Generic;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore
{
	public static class Delegates
	{
		public delegate void Void();
		public delegate void String(string str);
		public delegate void CellList(List<HexCell> cellList);
		public delegate void Cell(HexCell cell);
		public delegate void AbilityD(Ability ability);
		public delegate void DamageD(Damage damage);
		public delegate void EffectCharacterDamage(Effect effect, Character character, Damage damage);
		public delegate void AbilityCharacterDamage(Ability ability, Character character, Damage damage);
		public delegate void CharacterD(Character character);
		public delegate void CharacterCell(Character character, HexCell cell);
		public delegate void CharacterDamage(Character character, Damage damage);
		public delegate void CharacterInt(Character targetCharacter, int value);
		public delegate void CharacterRefInt(Character targetCharacter, ref int value);
	}
}