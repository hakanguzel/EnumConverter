using EnumConverter.Nuget;
using Xunit;

namespace EnumConverter.Tests
{
    public class EnumConverterTests
    {
        [Fact]
        public void ToInt_Should_Be_Successful()
        {
            var actual = 1;
            var expected = TestEnum.NONE.ToInt();

            Assert.Equal(expected, actual);
        }
        [Fact]
        public void ToEnum_Should_Be_Successful()
        {
            var value = 1;
            var actual = TestEnum.NONE;
            var expected = value.ToEnum<TestEnum>();

            Assert.Equal(expected, actual);
        }
        private enum TestEnum
        {
            NONE = 1,
        }
    }
}