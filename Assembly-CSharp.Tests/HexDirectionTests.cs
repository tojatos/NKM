using Hex;
using Xunit;

namespace Assembly_CSharp.Tests
{
    public class HexDirectionTests
    {
        [Theory]
        [InlineData(HexDirection.Ne, HexDirection.Nw, HexDirection.E)]
        [InlineData(HexDirection.Nw, HexDirection.W, HexDirection.Ne)]
        [InlineData(HexDirection.Se, HexDirection.E, HexDirection.Sw)]
        public void NearbyDirectionsAreCorrect(HexDirection direction, HexDirection expectedDirection1, HexDirection expectedDirection2)
        {
            HexDirection[] directions = direction.NearbyDirections();
            HexDirection[] expectedDirections = {expectedDirection1, expectedDirection2};
            
            Assert.Equal(expectedDirections, directions);
        }
    }
}