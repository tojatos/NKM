using System.Collections.Generic;
using Hex;
using MyGameObject_templates;
public static class Active : MonoBehaviour
{
	public static Turn Turn { get; set; }
	public static Phase Phase { get; set; }
	public static Player Player { get; set; }
	public static Action Action { get; set; }
	public static MyGameObject MyGameObject { get; set; }
	public static Character CharacterOnMap { get; set; }
	public static List<HexCell> HexCells { get; set; }
}