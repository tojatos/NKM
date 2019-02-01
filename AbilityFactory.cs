using System.Collections.Generic;
using System.Linq;
using NKMCore.Extensions;
using NKMCore.Templates;
using Unity;

namespace NKMCore
{
    public static class AbilityFactory
    {
//	    private readonly Character _targetCharacter;
//	    public AbilityFactory(Character targetCharacter)
//	    {
//		    _targetCharacter = targetCharacter;
//	    }
		public static List<Ability> CreateAndInitiateAbilitiesFromDatabase(string name)
		{
			IEnumerable<string> abilityClassNames = GameData.Conn.GetAbilityClassNames(name);
			List<Ability> abilities = SpawnAbilities(name, abilityClassNames);
//			InitiateAbilities(ref abilities);
			return abilities;
		}

	    private static List<Ability> SpawnAbilities(string name, IEnumerable<string> abilityClassNames)
	    {
		    string abilityNamespaceName = "Abilities." + name.Replace(' ', '_');
		    List<Ability> abilities =
			    Spawner.Create<Ability>(abilityNamespaceName, abilityClassNames).ToList();//.ConvertAll(x => x as Ability);
		    return abilities;
	    }

//	    public static void InitiateAbilities(ref List<Ability> abilities)
//		{
//			List<Ability> initiatedAbilities = new List<Ability>();
//			foreach (AbilityType type in Enum.GetValues(typeof(AbilityType)))
//			{
//				int abilitiesOfType = abilities.Count(a => a.Type == type);
//				switch (abilitiesOfType)
//				{
//					case 0:
//						initiatedAbilities.Add(new Empty(type));
//						break;
//					case 1:
//						initiatedAbilities.Add(abilities.First(a=>a.Type == type));
//						break;
//					default:
//						initiatedAbilities.AddRange(abilities.FindAll(a=>a.Type==type));
//						break;
//				}
//			}
//
////			initiatedAbilities.ForEach(a => a.ParentCharacter = _targetCharacter);
//			abilities = initiatedAbilities;
//		}
    }
}