using System;
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

	    public static Ability CreateAndInit(Type type, Game game)
	    {
		    var a = Instantiator.Create<Ability>(type, game);
		    game?.InvokeAfterAbilityCreation(a);
		    return a;
	    }

	    private static List<Ability> SpawnAbilities(string name, IEnumerable<string> abilityClassNames, Game game)
	    {
		    string abilityNamespaceName = "Abilities." + name.Replace(' ', '_');
		    List<Ability> abilities = Instantiator.Create<Ability>(abilityNamespaceName, abilityClassNames, game).ToList();
		    return abilities;
	    }
    }
}