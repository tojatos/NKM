using System.Collections.Generic;
using System.Linq;
using NKMCore.Templates;

namespace NKMCore
{
	public class GamePlayer
	{
		public string Name { get; set; }
		public readonly List<Character> Characters = new List<Character>();
		public bool IsEliminated => Characters.All(c => !c.IsAlive);
	}
}