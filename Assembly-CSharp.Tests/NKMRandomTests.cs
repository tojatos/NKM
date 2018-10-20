using Xunit;

namespace Assembly_CSharp.Tests
{
    public class NKMRandomTests
    {
        [Fact]
        public void Get_WithoutSetting_ValueIsNull()
        {
            int? value = NKMRandom.Get("test");
            Assert.Null(value);
        }

        [Fact]
        public void Get_SingleValueSet_ValueIsEqual()
        {
            NKMRandom.Set("test", 3);
            
            int? value = NKMRandom.Get("test");
            Assert.Equal(3, value);
        }
        
        [Fact]
        public void Get_SecondTimeAfterSingleSet_ValueIsNull()
        {
            NKMRandom.Set("test", 3);
            NKMRandom.Get("test");
            
            int? secondValue = NKMRandom.Get("test");
            Assert.Null(secondValue);
        }
        
        [Fact]
        public void Get_SeveralTimesAfterSeveralValueSet_ValuesGetProperly()
        {
            NKMRandom.Set("test", 3);
            NKMRandom.Set("other", 6);
            NKMRandom.Set("test", 133);
            
            int? value1 = NKMRandom.Get("test");
            int? value2 = NKMRandom.Get("test");
            int? value3 = NKMRandom.Get("other");
            int? value4 = NKMRandom.Get("other");
            
            Assert.Equal(133, value1);
            Assert.Null(value2);
            Assert.Equal(6, value3);
            Assert.Null(value4);
        }
    }
}