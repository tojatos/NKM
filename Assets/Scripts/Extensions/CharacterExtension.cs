using NKMObjects.Templates;

namespace Extensions
{
    public static class CharacterExtension
    {
//        private static Game Game => GameStarter.Instance.Game;
//        private static Active Active => Game.Active;
////		public static string FirstName(this Character character) => character.Name.Split(' ').Last();
//        public static bool CanTakeAction (this Character character)
//	        => !(character.TookActionInPhaseBefore || !character.IsAlive ||
//                Active.Turn.CharacterThatTookActionInTurn != null &&
//                Active.Turn.CharacterThatTookActionInTurn != character || character.IsStunned);
//        public static bool CanWait (this Character character) 
//	        => !(character.Owner != Active.GamePlayer || character.TookActionInPhaseBefore ||
//                Active.Turn.CharacterThatTookActionInTurn != null);
		public static bool IsEnemyFor(this Character character, GamePlayer player) => character.Owner != player;
		public static bool IsEnemyFor(this Character character, Character other) => character.IsEnemyFor(other.Owner);
        
    }
}