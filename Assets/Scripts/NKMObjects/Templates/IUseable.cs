using System.Collections.Generic;
using Hex;

namespace NKMObjects.Templates
{
    public interface IUseable
    {
        void Use(List<HexCell> cells);
    }
}