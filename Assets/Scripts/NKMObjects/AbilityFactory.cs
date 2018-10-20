using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using NKMObjects.Abilities;
using NKMObjects.Templates;

namespace NKMObjects
{
    public class AbilityFactory
    {
	    private readonly Character _targetCharacter;
	    public AbilityFactory(Character targetCharacter)
	    {
		    _targetCharacter = targetCharacter;
	    }
		public List<Ability> CreateAndInitiateAbilitiesFromDatabase()
		{
			IEnumerable<string> abilityClassNames = GameData.Conn.GetAbilityClassNames(_targetCharacter.Name);
			List<Ability> abilities = SpawnAbilities(_targetCharacter.Name, abilityClassNames);
			InitiateAbilities(ref abilities);
			return abilities;
		}

	    private static List<Ability> SpawnAbilities(string name, IEnumerable<string> abilityClassNames)
	    {
		    string abilityNamespaceName = "Abilities." + name.Replace(' ', '_');
		    List<Ability> abilities =
			    Spawner.Create(abilityNamespaceName, abilityClassNames).ToList().ConvertAll(x => x as Ability);
		    return abilities;
	    }

	    public void InitiateAbilities(ref List<Ability> abilities)
		{
			List<Ability> initiatedAbilities = new List<Ability>();
			foreach (AbilityType type in Enum.GetValues(typeof(AbilityType)))
			{
				int abilitiesOfType = abilities.Count(a => a.Type == type);
				switch (abilitiesOfType)
				{
					case 0:
						initiatedAbilities.Add(new Empty(type));
						break;
					case 1:
						initiatedAbilities.Add(abilities.First(a=>a.Type == type));
						break;
					default:
						initiatedAbilities.AddRange(abilities.FindAll(a=>a.Type==type));
						break;
				}
			}

			initiatedAbilities.ForEach(a => a.ParentCharacter = _targetCharacter);
			abilities = initiatedAbilities;
		}
    }
}