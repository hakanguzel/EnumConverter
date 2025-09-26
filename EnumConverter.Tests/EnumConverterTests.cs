using System;
using EnumConverter.Nuget;
using Xunit;

namespace EnumConverter.Tests
{
    public class EnumConverterTests
    {
        [Fact]
        public void ToInt_Returns_Underlying_Value()
        {
            var actual = TestEnum.None.ToInt();

            Assert.Equal(1, actual);
        }

        [Fact]
        public void ToEnum_From_Int_Returns_Enum()
        {
            const int value = 2;

            var result = value.ToEnum<TestEnum>();

            Assert.Equal(TestEnum.Pending, result);
        }

        [Fact]
        public void ToEnum_With_Validation_Throws_When_Undefined()
        {
            const int invalidValue = 999;

            Assert.Throws<ArgumentOutOfRangeException>(() => invalidValue.ToEnum<TestEnum>(validateDefinition: true));
        }

        [Fact]
        public void ToEnum_From_Nullable_Int_Returns_Null_When_No_Value()
        {
            int? value = null;

            var result = value.ToEnum<TestEnum>();

            Assert.Null(result);
        }

        [Fact]
        public void TryToEnum_From_Int_Returns_False_For_Undefined_Value()
        {
            const int invalidValue = -1;

            var success = invalidValue.TryToEnum<TestEnum>(out var result);

            Assert.False(success);
            Assert.Equal(default, result);
        }

        [Fact]
        public void ToEnum_From_String_Ignores_Case()
        {
            var result = "pending".ToEnum<TestEnum>();

            Assert.Equal(TestEnum.Pending, result);
        }

        [Fact]
        public void TryToEnum_From_String_Returns_False_For_Whitespace()
        {
            var success = "   ".TryToEnum<TestEnum>(out var result);

            Assert.False(success);
            Assert.Equal(default, result);
        }

        [Fact]
        public void ToNullableEnum_From_String_Returns_Null_When_Invalid()
        {
            var result = "unknown".ToNullableEnum<TestEnum>();

            Assert.Null(result);
        }
    }

    private enum TestEnum
    {
        None = 1,
        Pending = 2,
        Completed = 3
    }
}
