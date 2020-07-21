using System;

namespace Identity.Models
{
    /// <summary>
    /// Strongly-typed superset of standard ASP.NET Identity/Kentico roles.
    /// </summary>
    /// <remarks>To make the <see cref="FlagsAttribute"/> attribute work correctly, 
    /// use the binary shift operator (https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/bitwise-and-shift-operators) 
    /// to define new roles.</remarks>
    [Flags]
    public enum Roles
    {
        None = 1,
        Patient = 1 << 1,
        Doctor = 1 << 2
    }
}
