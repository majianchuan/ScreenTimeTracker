using ScreenTimeTracker.BuildingBlocks.Common.Types;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScreenTimeTracker.BuildingBlocks.Infrastructure.Serialization;

/// <summary>
/// 只用于 JSON 到 Optional<T> 的反序列化，不支持 Optional<T> 到 JSON 的序列化。
/// </summary>
public class OptionalJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Optional<>);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var valueType = typeToConvert.GetGenericArguments()[0];
        return (JsonConverter)Activator.CreateInstance(
            typeof(OptionalJsonConverter<>).MakeGenericType(valueType))!;
    }
}

public class OptionalJsonConverter<T> : JsonConverter<Optional<T>>
{
    public override Optional<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // 只要 Read 被调用，说明 Key 存在
        // 如果 JSON 值是 null，Deserialize 会返回 default(T)
        var value = JsonSerializer.Deserialize<T>(ref reader, options);
        return new Optional<T>(value!);
    }

    public override void Write(Utf8JsonWriter writer, Optional<T> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Value, options);
    }
}