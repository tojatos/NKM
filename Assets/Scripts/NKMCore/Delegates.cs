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
		public delegate void CellHashSet(HashSet<HexCell> set);
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
		public delegate void CharacterCharacter(Character character1, Character character2);
		public delegate void CharacterCharacterList(Character character, List<Character> characters);
		public delegate void CellListCellList(List<HexCell> cellList1, List<HexCell> cellList2);
		public delegate void CharacterCharacterCell(Character character1, Character character2, HexCell cell);
		public delegate void CharacterCellHashSet(Character character, HashSet<HexCell> list);
		public delegate void AbilityHashSet(Ability ability, HashSet<HexCell> list);
	}
}