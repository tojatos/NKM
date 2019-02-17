using System.Collections.Generic;
using NKMCore.Hex;

namespace NKMCore.Templates
{
    public interface IUseableCellList
    {
        void Use(List<HexCell> cells);
    }
}