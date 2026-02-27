using ScreenTimeTracker.BuildingBlocks.Common.Types;
using ScreenTimeTracker.BuildingBlocks.Infrastructure.Serialization;
using System.Text.Json.Serialization;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.PatchApp;

public record PatchAppRequest(
    Guid Id,
    [property: JsonConverter(typeof(OptionalJsonConverter<string>))] Optional<string> Name,
    [property: JsonConverter(typeof(OptionalJsonConverter<bool>))] Optional<bool> IsAutoUpdateEnabled,
    [property: JsonConverter(typeof(OptionalJsonConverter<Guid>))] Optional<Guid> AppCategoryId,
    [property: JsonConverter(typeof(OptionalJsonConverter<string?>))] Optional<string?> IconPath
);