using System.Collections.Generic;
using Hex;

namespace NKMObjects.Templates
{
    public interface IUseableCellList
    {
        void Use(List<HexCell> cells);
    }
}