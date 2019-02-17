using System;
//using System.Drawing;
using System.Linq;
using NKMCore.Templates;

namespace NKMCore.Extensions
{
    public static class CharacterExtension
    {
		public static bool IsEnemyFor(this Character character, GamePlayer player) => character.Owner != player;
		public static bool IsEnemyFor(this Character character, Character other) => character.IsEnemyFor(other.Owner);
		public static string FormattedFirstName(this Character character) => string.Format("<color={0}><</color><b>{1}</b><color={0}>></color>", character.Owner.GetColor(character._game), character.Name.Split(' ').Last());
	    
		public static string GetColor(this GamePlayer gamePlayer, Game game)
		{
			switch (gamePlayer.GetIndex(game))
			{
				case 0:
					return "#FF0000";
				case 1:
					return "#00FF00";
				case 2:
					return "#0000FF";
				case 3:
					return "#00FFFF";
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
    }
}