using NKMCore.Templates;

namespace NKMCore.Extensions
{
    public static class CharacterExtension
    {
		public static bool IsEnemyFor(this Character character, GamePlayer player) => character.Owner != player;
		public static bool IsEnemyFor(this Character character, Character other) => character.IsEnemyFor(other.Owner);
    }
}