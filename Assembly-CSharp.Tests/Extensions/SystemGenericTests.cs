using System.Collections.Generic;
using System.Linq;
using Extensions;
using Xunit;

namespace Assembly_CSharp.Tests
{
    public class SystemGenericsTests
    {
        [Theory]
        [InlineData(9, 10, 234, 3, 342, 234)]
        [InlineData("random string", "", "\tanother", "Lorem", "Ipsum")]
        [InlineData(null)] // empty list case
        public void RandomElementTest(params object[] e)
        {
            if (e == null) e = new object[] { };
            List<object> elements = e.ToList();
            for (int i = 0; i < 100; i++)
            {
                object randomElement = elements.GetRandom();
                if(elements.Count>0) Assert.Contains(randomElement, elements);
                else Assert.Equal(null, randomElement);
            }
        }

        [Theory]
        [InlineData(2, 1)]
        [InlineData(9, 10, 234, 3, 342, 234)]
        [InlineData("random string", "", "\tanother", "Lorem", "Ipsum")]
        [InlineData(null)] // empty list case
        public void AddOneTest(object toAdd, params object[] e)
        {
            if (e == null) e = new object[] { };
            List<object> elements = e.ToList();

            List<object> result = new List<object>(elements).AddOne(toAdd);
            List<object> expected = new List<object>(elements) {toAdd};
            Assert.Equal(expected, result);
        }
    }
}