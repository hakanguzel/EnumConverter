# EnumConverter

Simple extension methods for converting between enum values, numeric identifiers, and string representations.

## Features
- Convert enums to their numeric underlying values with null checking.
- Generic helpers cover every enum backing type (byte, long, etc.).
- Definition validation understands `[Flags]` combinations while rejecting unknown bits.
- Parse from strings using `StringComparison` or allocation-free `ReadOnlySpan<char>` APIs.
- Nullable and try-style helpers cover numeric, string, and span inputs.
- Built for modern .NET 8 apps.

## Installation
Install from NuGet:

```bash
 dotnet add package EnumConverter --version 2.2.0
```

## Usage
```csharp
using System;
using EnumConverter;

[Flags]
public enum AccessRights : byte
{
    None = 0,
    Read = 1,
    Write = 2
}

public enum OrderStatus
{
    Pending = 1,
    Shipped = 2,
    Delivered = 3
}

// Numeric conversions
int value = OrderStatus.Shipped.ToInt();              // 2
OrderStatus parsed = value.ToEnum<OrderStatus>();     // OrderStatus.Shipped
byte access = AccessRights.Read.ToValue<AccessRights, byte>(); // 1

// Validation across all backing types (supports [Flags] combinations)
int unknown = 999;
unknown.ToEnum<OrderStatus>(validateDefinition: true); // throws ArgumentOutOfRangeException
var readWrite = ((byte)3).ToEnum<AccessRights, byte>(validateDefinition: true); // Read | Write

// Nullable numbers
int? maybeValue = null;
OrderStatus? nullableStatus = maybeValue.ToEnum<OrderStatus>();
if (maybeValue.TryToEnum(out OrderStatus fallback))
{
    // handled
}

// String helpers with custom comparisons
var fromString = "delivered".ToEnum<OrderStatus>();
if (" shipped ".TryToEnum<OrderStatus>(out var statusFromString, StringComparison.OrdinalIgnoreCase))
{
    // use parsed value
}

// Allocation-free span parsing
ReadOnlySpan<char> span = "Pending".AsSpan();
var spanParsed = span.ToEnum<OrderStatus>();

// Graceful fallbacks
var optionalStatus = "unknown".ToNullableEnum<OrderStatus>();          // null
var optionalFlags = "read, execute".ToNullableEnum<AccessRights>();    // AccessRights.Read | AccessRights.Execute
```

## Requirements
- .NET SDK 8.0 or later

## Building & Testing
1. Restore and test the solution:

```bash
 dotnet restore
 dotnet test
```

## Contributing
Issues and pull requests are welcome. Please open an issue to discuss significant changes before submitting a PR.

## License
Released under the [MIT License](LICENSE).
