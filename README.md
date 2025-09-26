# EnumConverter

Simple extension methods for converting between enum values, numeric identifiers, and string representations.

## Features
- Convert enums to their numeric underlying values with null checking.
- Safely map integers or nullable integers back to strongly typed enums.
- Parse enum values from strings with optional case-insensitive handling.
- Try-style helpers that avoid exceptions and report success.
- Multi-targeted library: `netstandard2.0` for broad compatibility and `net8.0` for modern apps.

## Installation
Install from NuGet:

```bash
 dotnet add package EnumConverter --version 1.1.0
```

## Usage
```csharp
using EnumConverter.Nuget;

public enum OrderStatus
{
    Pending = 1,
    Shipped = 2,
    Delivered = 3
}

var status = OrderStatus.Shipped;

// Numeric conversions
int value = status.ToInt();          // 2
OrderStatus parsed = value.ToEnum<OrderStatus>();

// Validation
int unknown = 999;
// Throws ArgumentOutOfRangeException when the value is not defined
unknown.ToEnum<OrderStatus>(validateDefinition: true);

// Nullable values
int? maybeValue = null;
OrderStatus? nullableStatus = maybeValue.ToEnum<OrderStatus>();

// String helpers
var fromString = "delivered".ToEnum<OrderStatus>();
if (" shipped ".TryToEnum<OrderStatus>(out var statusFromString))
{
    // use parsed value
}

// Graceful fallbacks
var optionalStatus = "unknown".ToNullableEnum<OrderStatus>(); // null
```

## Building & Testing
1. Install the .NET SDK 8.0 or later.
2. Restore and test the solution:

```bash
 dotnet restore
 dotnet test
```

Targets older than .NET 8 can still consume the `netstandard2.0` build of the package.

## Contributing
Issues and pull requests are welcome. Please open an issue to discuss significant changes before submitting a PR.

## License
Released under the [MIT License](LICENSE).
