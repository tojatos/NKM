using System.Linq;

namespace Multiplayer.Network
{
	public static class MessageComposer
	{
		public static string Compose(char delimiter, string header, params string[] contents)
		{
			string msg = header;
			contents.ToList().ForEach(c => msg += $"{delimiter}{c}");
			return msg;
		}
		public static string Compose(string header, params string[] contents)
		{
			return Compose('%', header, contents);
		}
	}
}
