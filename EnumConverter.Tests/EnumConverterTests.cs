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
        public void ToEnum_Generic_From_Long_Returns_Enum()
        {
            const long value = 4_000_000_000L;

            var result = value.ToEnum<LongBackedEnum, long>();

            Assert.Equal(LongBackedEnum.Large, result);
        }

        [Fact]
        public void ToEnum_With_Validation_Throws_When_Undefined()
        {
            const int invalidValue = 999;

            Assert.Throws<ArgumentOutOfRangeException>(() => invalidValue.ToEnum<TestEnum>(validateDefinition: true));
        }

        [Fact]
        public void ToEnum_With_Validation_Allows_Flags_Combinations()
        {
            const int value = (int)(AccessRights.Read | AccessRights.Write);

            var result = value.ToEnum<AccessRights>(validateDefinition: true);

            Assert.Equal(AccessRights.Read | AccessRights.Write, result);
        }

        [Fact]
        public void ToEnum_With_Validation_Throws_For_Flags_When_Undefined_Bit()
        {
            const int invalidValue = 8;

            Assert.Throws<ArgumentOutOfRangeException>(() => invalidValue.ToEnum<AccessRights>(validateDefinition: true));
        }

        [Fact]
        public void ToEnum_From_Nullable_Int_Returns_Null_When_No_Value()
        {
            int? value = null;

            var result = value.ToEnum<TestEnum>();

            Assert.Null(result);
        }

        [Fact]
        public void TryToEnum_From_Nullable_Int_Returns_True_When_Value_Defined()
        {
            int? value = 3;

            var success = value.TryToEnum(out TestEnum result);

            Assert.True(success);
            Assert.Equal(TestEnum.Completed, result);
        }

        [Fact]
        public void TryToEnum_From_Nullable_Int_Returns_False_When_Value_Missing()
        {
            int? value = null;

            var success = value.TryToEnum(out TestEnum result);

            Assert.False(success);
            Assert.Equal(default, result);
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
        public void TryToEnum_Generic_From_Long_Returns_False_For_Undefined_Value()
        {
            const long invalidValue = 99L;

            var success = invalidValue.TryToEnum<LongBackedEnum, long>(out _);

            Assert.False(success);
        }

        [Fact]
        public void TryToEnum_For_Flags_Returns_False_When_Contains_Undefined_Bit()
        {
            const int invalidValue = 16;

            var success = invalidValue.TryToEnum<AccessRights>(out _);

            Assert.False(success);
        }

        [Fact]
        public void ToEnum_From_String_Ignores_Case()
        {
            var result = "pending".ToEnum<TestEnum>();

            Assert.Equal(TestEnum.Pending, result);
        }

        [Fact]
        public void ToEnum_From_String_Throws_When_Null()
        {
            Assert.Throws<ArgumentNullException>(() => EnumConverter.ToEnum<TestEnum>((string)null!));
        }

        [Fact]
        public void TryToEnum_From_String_With_Comparison_Allows_Case_Difference()
        {
            var success = "pending".TryToEnum<TestEnum>(out var result, StringComparison.CurrentCultureIgnoreCase);

            Assert.True(success);
            Assert.Equal(TestEnum.Pending, result);
        }

        [Fact]
        public void TryToEnum_From_String_With_Ordinal_Comparison_Is_Case_Sensitive()
        {
            var success = "pending".TryToEnum<TestEnum>(out _, StringComparison.Ordinal);

            Assert.False(success);
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

        [Fact]
        public void ToNullableEnum_From_String_With_Comparison_Returns_Value()
        {
            var result = "completed".ToNullableEnum<TestEnum>(StringComparison.CurrentCultureIgnoreCase);

            Assert.Equal(TestEnum.Completed, result);
        }

        [Fact]
        public void ToEnum_From_ReadOnlySpan_Returns_Enum()
        {
            var span = "Completed".AsSpan();

            var result = span.ToEnum<TestEnum>();

            Assert.Equal(TestEnum.Completed, result);
        }

        [Fact]
        public void TryToEnum_From_ReadOnlySpan_Returns_False_For_Undefined_Value()
        {
            var span = "42".AsSpan();

            var success = span.TryToEnum<TestEnum>(out var result);

            Assert.False(success);
            Assert.Equal(default, result);
        }

        [Fact]
        public void ToValue_Generic_Returns_Byte_Underlying()
        {
            var underlying = ByteBackedEnum.First.ToValue<ByteBackedEnum, byte>();

            Assert.Equal((byte)1, underlying);
        }

        [Fact]
        public void ToValue_Generic_Throws_When_Mismatched_Underlying()
        {
            Assert.Throws<InvalidOperationException>(() => ((Enum)ByteBackedEnum.First).ToValue<int>());
        }
    }

    public enum TestEnum
    {
        None = 1,
        Pending = 2,
        Completed = 3
    }

    public enum ByteBackedEnum : byte
    {
        None = 0,
        First = 1
    }

    public enum LongBackedEnum : long
    {
        Small = 0,
        Large = 4_000_000_000L
    }

    [Flags]
    public enum AccessRights : byte
    {
        None = 0,
        Read = 1,
        Write = 2,
        Execute = 4
    }
}
