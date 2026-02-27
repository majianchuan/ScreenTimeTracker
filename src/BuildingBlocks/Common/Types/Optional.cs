namespace ScreenTimeTracker.BuildingBlocks.Common.Types;

/// <summary>
/// 用于表示一个值可能存在，也可能不存在（未定义）。
/// 主要用于区分 "null" (显式空值) 和 "undefined" (未传值)。
/// </summary>
public readonly record struct Optional<T>
{
    private readonly T _value = default!;
    public T Value => HasValue
        ? _value
        : throw new InvalidOperationException("Property 'Value' cannot be accessed when 'HasValue' is false. Check 'HasValue' before accessing the value.");
    public bool HasValue { get; init; }

    public Optional(T value)
    {
        _value = value;
        HasValue = true;
    }

    // 隐式转换
    public static implicit operator Optional<T>(T value) => new(value);
}