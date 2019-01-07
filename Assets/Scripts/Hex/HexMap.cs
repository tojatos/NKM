using System.Collections.Generic;

namespace Hex
{
    public class HexMap
    {
	    public readonly List<BetterHexCell> Cells;
	    
		public HexMap (List<BetterHexCell> cells)
		{
			Cells = cells;
		}
    }
}