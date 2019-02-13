using System.Collections.Generic;
using System.Linq;
using NKMCore.Extensions;
using NKMCore.Templates;

namespace NKMCore
{
    public static class AbilityFactory
    {
		public static List<Ability> CreateAndInitiateAbilitiesFromDatabase(string name, Game game)
		{
			IEnumerable<string> abilityClassNames = GameData.Conn.GetAbilityClassNames(name);
			List<Ability> abilities = SpawnAbilities(name, abilityClassNames, game);
			abilities.ForEach(a => game?.InvokeAfterAbilityCreation(a));
			return abilities;
		}

	    private static List<Ability> SpawnAbilities(string name, IEnumerable<string> abilityClassNames, Game game)
	    {
		    string abilityNamespaceName = "Abilities." + name.Replace(' ', '_');
		    List<Ability> abilities = Instantiator.Create<Ability>(abilityNamespaceName, abilityClassNames, game).ToList();
		    return abilities;
	    }
    }
}