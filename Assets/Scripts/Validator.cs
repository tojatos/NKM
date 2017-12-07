using System;
using System.Collections.Generic;
using System.Linq;

public static class Validator
{
	public static void ValidateInitializationSelect(List<string> selectedNames)
	{
		var g = selectedNames.GroupBy(i => i);
		foreach (var grp in g)
		{
			if (grp.Count() > 1)
			{
				throw new Exception("Wybory muszą być różne!");
			}
		}
	}
}
