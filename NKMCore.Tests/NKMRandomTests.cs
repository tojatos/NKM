using Xunit;

namespace NKMCore.Tests
{
    public class NKMRandomTests
    {
        [Fact]
        public void Get_WithoutSetting_ValueIsNull()
        {
            var nkmRandom = new NKMRandom();
            int? value = nkmRandom.Get("test");
            Assert.Null(value);
        }

        [Fact]
        public void Get_SingleValueSet_ValueIsEqual()
        {
            var nkmRandom = new NKMRandom();
            nkmRandom.Set("test", 3);

            int? value = nkmRandom.Get("test");
            Assert.Equal(3, value);
        }

        [Fact]
        public void Get_SecondTimeAfterSingleSet_ValueIsNull()
        {
            var nkmRandom = new NKMRandom();
            nkmRandom.Set("test", 3);
            nkmRandom.Get("test");

            int? secondValue = nkmRandom.Get("test");
            Assert.Null(secondValue);
        }

        [Fact]
        public void Get_SeveralTimesAfterSeveralValueSet_ValuesGetProperly()
        {
            var nkmRandom = new NKMRandom();
            nkmRandom.Set("test", 3);
            nkmRandom.Set("other", 6);
            nkmRandom.Set("test", 133);

            int? value1 = nkmRandom.Get("test");
            int? value2 = nkmRandom.Get("test");
            int? value3 = nkmRandom.Get("other");
            int? value4 = nkmRandom.Get("other");

            Assert.Equal(133, value1);
            Assert.Null(value2);
            Assert.Equal(6, value3);
            Assert.Null(value4);
        }
    }
}