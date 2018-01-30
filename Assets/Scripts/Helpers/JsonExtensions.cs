using Newtonsoft.Json;

namespace Helpers
{
	public static class JsonExtensions
	{
		public static string ToJson(this object o)
		{
			return JsonConvert.SerializeObject(o, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto,
				PreserveReferencesHandling = PreserveReferencesHandling.Objects,
				Formatting = Formatting.Indented,
			});
		}
		public static T JsonToObject<T>(this string json) where T : new()
		{
			var player = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto,
				ObjectCreationHandling = ObjectCreationHandling.Replace,
			});
			return player;

		}
	}
}
