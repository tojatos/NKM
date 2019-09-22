using System;
using System.Collections.Generic;
using System.Linq;
using NKMCore.Extensions;
using Xunit;

namespace NKMCore.Tests
{
    public class SystemGenericsTests
    {
        [Theory]
        [InlineData(9, 10, 234, 3, 342, 234)]
        [InlineData("random string", "", "\tanother", "Lorem", "Ipsum")]
        [InlineData(null)] // empty list case
        public void RandomElement(params object[] e)
        {
            if (e == null) e = new object[] { };
            List<object> elements = e.ToList();
            for (int i = 0; i < 100; i++)
            {
                object randomElement = elements.GetRandom();
                if(elements.Count>0) Assert.Contains(randomElement, elements);
                else Assert.Null(randomElement);
            }
        }

        [Theory]
        [InlineData(2, 1)]
        [InlineData(9, 10, 234, 3, 342, 234)]
        [InlineData("random string", "", "\tanother", "Lorem", "Ipsum")]
        [InlineData(null)] // empty list case
        public void AddOne(object toAdd, params object[] e)
        {
            if (e == null) e = new object[] { };
            List<object> elements = e.ToList();

            List<object> result = new List<object>(elements).AddOne(toAdd);
            List<object> expected = new List<object>(elements) {toAdd};
            Assert.Equal(expected, result);
        }

        [Fact]
        public void SecondLast_NoElements_ExceptionThrown()
        {
            var list = new List<object>();
            Assert.Throws<ArgumentException>(() => list.SecondLast());
        }
        [Fact]
        public void SecondLast_SingleElement_ExceptionThrown()
        {
            var list = new List<object>{ "foo" };
            Assert.Throws<ArgumentException>(() => list.SecondLast());
        }

        [Theory]
        [InlineData(2, 1)]
        [InlineData(9, 10, 234, 3, 342, 234)]
        [InlineData("random string", "", "\tanother", "Lorem", "Ipsum")]
        public void SecondLast_MultipleElements_CorrectReturned(params object[] e)
        {
            Assert.Equal(e[e.Length-2], new List<object>(e).SecondLast());
        }

        private enum TestEnum
        {
            One,
            Two,
            Three,
        }
        [Fact]
        public void ToEnum()
        {
            Assert.Equal(TestEnum.One, "One".ToEnum<TestEnum>());
            Assert.Equal(TestEnum.Two, "Two".ToEnum<TestEnum>());
            Assert.Equal(TestEnum.Three, "Three".ToEnum<TestEnum>());
            Assert.Equal(TestEnum.Three, "three".ToEnum<TestEnum>());
            Assert.Equal(TestEnum.Three, "three ".ToEnum<TestEnum>());
            Assert.NotEqual(TestEnum.Three, "3".ToEnum<TestEnum>());
        }

        [Fact]
        public void GetValueOrDefault()
        {
            var dict = new Dictionary<string, string>
            {
                {"a", "c"},
                {"c", "b"},
            };
            Assert.Equal("c", dict.GetValueOrDefault("a"));
            Assert.Equal("b", dict.GetValueOrDefault("c"));
            Assert.Equal(default(string), dict.GetValueOrDefault("d"));
        }
    }
}