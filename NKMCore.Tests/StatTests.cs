using Xunit;

namespace NKMCore.Tests
{
    public class StatTests
    {
        [Fact]
        public void IsCreated()
        {
            var stat = new Stat(StatType.AttackPoints, 8);
            Assert.Equal(8, stat.BaseValue);
            Assert.Equal(8, stat.RealValue);
            Assert.Equal(8, stat.Value);
        }

        [Fact]
        public void ValueChanged()
        {
            var stat = new Stat(StatType.AttackPoints, 8);
            stat.Value = 5;
            Assert.Equal(8, stat.BaseValue);
            Assert.Equal(5, stat.RealValue);
            Assert.Equal(5, stat.Value);
        }

        [Fact]
        public void ModifiersWork()
        {
            var stat = new Stat(StatType.AttackPoints, 8);

            stat.Modifiers.Add(new Modifier(7));
            Assert.Equal(8, stat.BaseValue);
            Assert.Equal(8, stat.RealValue);
            Assert.Equal(15, stat.Value);

            stat.Modifiers.Add(new Modifier(-5));
            Assert.Equal(8, stat.BaseValue);
            Assert.Equal(8, stat.RealValue);
            Assert.Equal(10, stat.Value);
        }

        [Fact]
        public void OnChangedEventWorks()
        {
            var stat = new Stat(StatType.AttackPoints, 8);
            int counter = 0;

            stat.StatChanged += (int1, int2) => ++counter;

            // Adding a modifier does not count as change
            stat.Modifiers.Add(new Modifier(7));
            Assert.Equal(0, counter);

            stat.Value = 5;
            Assert.Equal(1, counter);

            stat.Value -= 3;
            Assert.Equal(2, counter);
        }
    }
}