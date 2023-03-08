using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Infrastructure.Utilities;

[TypeConverter(typeof(ConfigurationSecretTypeConverter))]
public sealed class Secret
{
    // Do not use System.String as some serializers can serialize fields.
    // At the moment, System.Text.Json does not support ReadOnlyMemory<char>, so it cannot be serialized.
    private readonly ReadOnlyMemory<char> _data;

    public Secret(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("Value cannot be null or empty.", nameof(value));
        }

        _data = value.ToCharArray();

        // If you want to prevent the string to from being moved in memory, and so, copied multiple times in memory,
        // you can use the Pinned Heap: https://github.com/dotnet/runtime/blob/main/docs/design/features/PinnedHeap.md
        // var data = GC.AllocateUninitializedArray<char>(value.Length, pinned: true);
        // value.AsSpan().CopyTo(data);
        // _data = data;

        // Also, if you want something more secure, you can look at the Microsoft.AspNetCore.DataProtection.Secret. But it may be harder
        // to use with the Microsoft.Extensions.Configuration package. Indeed, Secret is a disposable object, but using IOptions<T> will
        // not dispose the object, so you need to take care of disposing it yourself.
        // https://github.com/dotnet/aspnetcore/blob/ea683686bfac765690cb6e40f6ba7198cae26e65/src/DataProtection/DataProtection/src/Secret.cs
    }

    public string Reveal() => string.Create(_data.Length, _data, (span, data) => data.Span.CopyTo(span));

    [Obsolete($"Use {nameof(Reveal)} instead", error: true)]
    public override string? ToString() => base.ToString();

    /// <summary>
    /// Allow Microsoft.Extensions.Configuration to instantiate the <seealso cref="Secret" />
    /// from a string.
    /// </summary>
    private sealed class ConfigurationSecretTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
            => sourceType == typeof(string);

        public override bool CanConvertTo(ITypeDescriptorContext? context, [NotNullWhen(true)] Type? destinationType)
            => false;

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
            => throw new InvalidOperationException();

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
            => value is string { Length: > 0 } str ? new Secret(str) : null;
    }
}
