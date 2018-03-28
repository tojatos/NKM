using System;
using System.Threading.Tasks;
using MyGameObjects.MyGameObject_templates;

namespace Helpers
{
	public static class SynchronizableExtensions
	{
		public static string SynchronizableSerialize<T>(this T value, string name)
		{
			string serializedValue;
            if (value == null) serializedValue = null;
            else
            {
                switch (name)
                {
                    case ActivePropertyName.GamePlayer:
                        serializedValue = (value as GamePlayer).Name;
                        break;
	                case ActivePropertyName.Ability:
		                serializedValue = (value as Ability).Guid.ToString();
		                break;
	                case ActivePropertyName.CharacterOnMap:
		                serializedValue = (value as Character).Guid.ToString();
		                break;
	                case ActivePropertyName.MyGameObject:
		                serializedValue = (value as MyGameObject).Guid.ToString();
		                break;
	                case ActivePropertyName.Action:
		                serializedValue = (value).ToString();
		                break;
                    default:
                        throw new ArgumentOutOfRangeException();

                }
            }

            return serializedValue;
        }
		
		
	}
}